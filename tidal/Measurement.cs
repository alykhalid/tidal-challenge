using System;
using System.Globalization;
using System.Net;
using NodaTime;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

public class Measurement {
    public int Id { get; set; }
    public LocalDate dateOfMeasurment { get; set; }
    public LocalTime timeOfMeasurement { get; set; }
    public float latitudeOfMeasurement { get; set; }
    public float longitudeOfMeasurement { get; set; }
    public float depthOfMeasurement { get; set; }
    public float chartdatumWaterLevel { get; set; }
    public float chartdatumDepth => this.chartdatumWaterLevel + this.depthOfMeasurement;
}

public class csvOutput {
    public string Id { get; set; }
    public string DateaAndTime { get; set; }
    public string WaterLevel { get; set; }
    public string Depth { get; set; }


}


