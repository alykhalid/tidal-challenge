using System;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot(ElementName = "location")]
public class Location
{
    [XmlAttribute(AttributeName = "name")]
    public string Name { get; set; }
    [XmlAttribute(AttributeName = "code")]
    public string Code { get; set; }
    [XmlAttribute(AttributeName = "latitude")]
    public string Latitude { get; set; }
    [XmlAttribute(AttributeName = "longitude")]
    public string Longitude { get; set; }
    [XmlAttribute(AttributeName = "delay")]
    public string Delay { get; set; }
    [XmlAttribute(AttributeName = "factor")]
    public string Factor { get; set; }
    [XmlAttribute(AttributeName = "obsname")]
    public string Obsname { get; set; }
    [XmlAttribute(AttributeName = "obscode")]
    public string Obscode { get; set; }
}

[XmlRoot(ElementName = "waterlevel")]
public class Waterlevel
{
    [XmlAttribute(AttributeName = "value")]
    public string Value { get; set; }
    [XmlAttribute(AttributeName = "time")]
    public string Time { get; set; }
    [XmlAttribute(AttributeName = "flag")]
    public string Flag { get; set; }
}

[XmlRoot(ElementName = "data")]
public class Data
{
    [XmlElement(ElementName = "waterlevel")]
    public List<Waterlevel> Waterlevel { get; set; }
    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; }
    [XmlAttribute(AttributeName = "unit")]
    public string Unit { get; set; }
}

[XmlRoot(ElementName = "locationdata")]
public class Locationdata
{
    [XmlElement(ElementName = "location")]
    public Location Location { get; set; }
    [XmlElement(ElementName = "reflevelcode")]
    public string Reflevelcode { get; set; }
    [XmlElement(ElementName = "data")]
    public List<Data> Data { get; set; }
}

[XmlRoot(ElementName = "tide")]
public class Tide
{
    [XmlElement(ElementName = "locationdata")]
    public Locationdata Locationdata { get; set; }
}


