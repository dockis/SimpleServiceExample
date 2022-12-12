using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleService.Models;

[XmlRoot("document")]
public class Document : IXmlSerializable
{
    [JsonProperty("id")]
    public string Id { get; init; }
    
    [JsonProperty("tags")]
    public string[] Tags { get; init; }
    
    [JsonProperty("data")]
    public dynamic? Data { get; init; }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString("id", Id);

        writer.WriteStartElement("tags");
        foreach (string tag in this.Tags)
        {
            writer.WriteElementString("tag", tag);
        }
        writer.WriteEndElement();
    
        JObject jData = new JObject();
        jData["data"] = Data;
        XmlDocument xmlData = JsonConvert.DeserializeXmlNode(jData.ToString());
        writer.WriteRaw(xmlData.OuterXml);
    }
}