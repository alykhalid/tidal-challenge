using System;
using System.Collections.Generic;
using System.Globalization;
using NodaTime;
using NodaTime.Text;

public class DataParser {
    public List<Measurement> Measurements { get; set; }
    public List<string> parsingErrors { get; set; }

    private LocalDate dateFailureValue => new LocalDateTime(1900, 1, 1, 0, 0, 00).Date;
    private LocalTime timeFailureValue => new LocalDateTime(1900, 1, 1, 0, 0, 00).TimeOfDay;
    private LocalDatePattern datePattern => LocalDatePattern.CreateWithInvariantCulture("dd.MM.yyyy");
    private LocalTimePattern timePattern => LocalTimePattern.CreateWithInvariantCulture("HH:mm");
    private CultureInfo cultureInfo => new CultureInfo("no");
    public DataParser()
    {
        this.Measurements = new List<Measurement>();
        this.parsingErrors = new List<string>();
    }

    public void Parse(IEnumerable<dynamic> records)
    {
        if (records is null)
        {
            throw new System.ArgumentNullException(nameof(records));
        }

        //On text applications, the line numbering will start from 1 but we start from 2 due to header. 
        int i = 1;
        foreach (var item in records)
        {
            i++;

            Measurement m = new Measurement();
            m.Id = i;

            var dateParseResult = datePattern.Parse((string)((IDictionary<string, object>)item)["Date"]);
            LocalDate dateOfMeasurement;
            if (!dateParseResult.TryGetValue(dateFailureValue, out dateOfMeasurement))
            {
                parsingErrors.Add("Date parsing error on line: " + i);
                continue;
            }

            var timeParseResult = timePattern.Parse((string)((IDictionary<string, object>)item)["Time"]);
            LocalTime timeOfMeasurement;
            if (!timeParseResult.TryGetValue(timeFailureValue, out timeOfMeasurement))
            {
                parsingErrors.Add("Time parsing error on line: " + i);
                continue;
            }

            m.dateOfMeasurment = dateOfMeasurement;
            m.timeOfMeasurement = timeOfMeasurement;

            float latitudeOfMeasurement;
            if (!float.TryParse((string)((IDictionary<string, object>)item)["GPS Latitude"], NumberStyles.Any, cultureInfo, out latitudeOfMeasurement))
            {
                parsingErrors.Add("Latitude parsing error on line: " + i);
                continue;
            }
            
            m.latitudeOfMeasurement = latitudeOfMeasurement;

            float longitudeOfMeasurement;
            if (!float.TryParse((string)((IDictionary<string, object>)item)["GPS Longitude"], NumberStyles.Any, cultureInfo, out longitudeOfMeasurement))
            {
                parsingErrors.Add("Longitude parsing error on line: " + i);
                continue;
            }

            m.longitudeOfMeasurement = longitudeOfMeasurement;

            float depthOfMeasurement;
            if (!float.TryParse((string)((IDictionary<string, object>)item)["depth"], NumberStyles.Any, cultureInfo, out depthOfMeasurement))
            {
                parsingErrors.Add("Depth parsing error on line: " + i);
                continue;
            }

            m.depthOfMeasurement = depthOfMeasurement;
            Measurements.Add(m);
        }
    }
}
