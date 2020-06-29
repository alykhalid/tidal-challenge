using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Mono.Options;
using NodaTime;
using NodaTime.Text;

namespace tidal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var shouldShowHelp = false;
            var dryRun = false;
            string input = string.Empty;
            string output = string.Empty;

            var options = new OptionSet {
                { "i|input=", "full path of the input file (required)", i => input = i },
                { "o|output=", "full path of the input file", o => output = o },
                { "d|dryrun=", "parse and display parsing errors", d => dryRun = d != null },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null },
            };
            List<string> extra;
            try
            {
                // parse the command line
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                // output some error message
                Console.Write("tidal: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `tidal --help' for more information.");
                return;
            }

            if (shouldShowHelp) {
                ShowHelp(options);
                return;
            }

            if(string.IsNullOrEmpty(input)) {
                ShowHelp(options);
                return;
            }

            Console.WriteLine(input);
 
            List<csvOutput> csvoutput = new List<csvOutput>();

            using (var reader = new StreamReader(input))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                DataParser dp = new DataParser();
                var records = csv.GetRecords<dynamic>();
                dp.Parse(records);

                if(dryRun) {
                    Console.WriteLine("Number of parsing errors: " + dp.parsingErrors.Count);
                    Console.WriteLine(string.Join(Environment.NewLine, dp.parsingErrors));
                    return;
                }

                EventLimiter el = new EventLimiter();
                await el.EnqueueRequest(dp.Measurements);

                foreach (var item in dp.Measurements)
                {
                    csvOutput tmp = new csvOutput();
                    tmp.Id = item.Id.ToString();
                    tmp.DateaAndTime = new LocalDateTime(item.dateOfMeasurment.Year, item.dateOfMeasurment.Month, item.dateOfMeasurment.Day, item.timeOfMeasurement.Hour, item.timeOfMeasurement.Minute).ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                    tmp.WaterLevel = item.chartdatumWaterLevel.ToString("F", CultureInfo.InvariantCulture);
                    tmp.Depth = item.chartdatumDepth.ToString("F", CultureInfo.InvariantCulture);
                    csvoutput.Add(tmp);
                }
            }

            if(string.IsNullOrEmpty(output)) {
                output = Path.Join(Path.GetDirectoryName(input), "output.csv");
            }

            using (var reader = new StreamWriter(output))
            using (var csv = new CsvWriter(reader, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(csvoutput);
            }




        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: tidal [OPTIONS]");
            Console.WriteLine("Process depth information from CSV");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
