using System.IO;
using System.Xml.Serialization;

namespace Net.RafaelEstevam.Spider.Helper
{
    public static class XmlSerializerHelper
    {
        public static void SerializeToFile<T>(T obj, string file) where T : new()
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            using TextWriter writer = new StreamWriter(file);
            x.Serialize(writer, obj);
        }
        public static T DeserializeFromFile<T>(string file) where T : new()
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            using var reader = new StreamReader(file);
            return (T)x.Deserialize(reader);
        }
    }
}
