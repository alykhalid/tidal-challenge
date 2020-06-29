using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NodaTime;
using NodaTime.Text;

public class NorTide
{
    private string apiURL => "http://api.sehavniva.no/tideapi.php?";

    private static readonly HttpClient client = new HttpClient();
    private string getUrlEncodedTime(LocalDateTime baseTime, int minutes) => WebUtility.UrlEncode(baseTime.PlusMinutes(minutes).ToString("s", CultureInfo.InvariantCulture));
    private int RoundDown(int value) => 10 * (value / 10);

    public async Task getWaterLevel(Measurement m)
    {
        LocalDateTime timeStamp = new LocalDateTime(m.dateOfMeasurment.Year, m.dateOfMeasurment.Month, m.dateOfMeasurment.Day, m.timeOfMeasurement.Hour, m.timeOfMeasurement.Minute);
        Instant instantTimeStamp = Instant.FromDateTimeOffset(timeStamp.WithOffset(Offset.FromHours(1)).ToDateTimeOffset());

        string requestURL = apiURL + string.Format("lat={0}&lon={1}&fromtime={2}&totime={3}&datatype=all&refcode=cd&place=&file=&lang=en&interval=10&dst=1&tzone=1&tide_request=locationdata",
         m.latitudeOfMeasurement, m.longitudeOfMeasurement, getUrlEncodedTime(timeStamp, -30), getUrlEncodedTime(timeStamp, 30));

        client.DefaultRequestHeaders.Accept.Clear();
        var stringTask = client.GetStringAsync(requestURL);

        var msg = await stringTask;
        Tide entity = FromXml<Tide>(msg);

        //TODO: Needs proper error handling for cases where connections times out or no response is returned.

        List<Waterlevel> waterlevel = entity.Locationdata.Data.Find(e => e.Type == "observation").Waterlevel;

        if(waterlevel.Count == 7) {
            float tmp;
            if(float.TryParse(waterlevel[3].Value, out tmp)) {
                m.chartdatumWaterLevel = tmp;
            }
        } else {
            OffsetDateTimePattern offset = OffsetDateTimePattern.ExtendedIso;
            long x = instantTimeStamp.ToUnixTimeSeconds();
            long x1 = Instant.FromDateTimeOffset(offset.Parse(waterlevel[2].Time).Value.ToDateTimeOffset()).ToUnixTimeSeconds();
            long x2 = Instant.FromDateTimeOffset(offset.Parse(waterlevel[3].Time).Value.ToDateTimeOffset()).ToUnixTimeSeconds();
            
            float y1 = float.Parse(waterlevel[2].Value);
            float y2 = float.Parse(waterlevel[3].Value);

            m.chartdatumWaterLevel = y1 + (((float)(x - x1) / (float)(x2 - x1)) * (y2 - y1));
        }

    }

    private T FromXml<T>(String xml)
    {
        T returnedXmlClass = default(T);
        try
        {
            using (TextReader reader = new StringReader(xml))
            {
                try
                {
                    returnedXmlClass = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                }
                catch (InvalidOperationException)
                {
                    // String passed is not XML, simply return defaultXmlClass
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("XML serialization exception" + ex.Message);
        }

        return returnedXmlClass;
    }

}