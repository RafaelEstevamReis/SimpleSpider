using System.IO;
using System.Xml.Serialization;

namespace Net.RafaelEstevam.Spider.Helper
{
    /// <summary>
    /// Helper class to serialize and deserialize stuff using XML
    /// </summary>
    public static class XmlSerializerHelper
    {
        /// <summary>
        /// Serialize a generic {T} object to a file
        /// </summary>
        /// <typeparam name="T">Type of the objects to be serialized</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="file">File path of the file</param>
        public static void SerializeToFile<T>(T obj, string file) where T : new()
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            using TextWriter writer = new StreamWriter(file);
            x.Serialize(writer, obj);
        }
        /// <summary>
        /// Deserialize a generic {T} object from a file
        /// </summary>
        /// <typeparam name="T">Type of the objects to be deserialized</typeparam>
        /// <param name="file">File path of the file</param>
        /// <returns>An instance of {T} with data from the file</returns>
        public static T DeserializeFromFile<T>(string file) where T : new()
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            using var reader = new StreamReader(file);
            return (T)x.Deserialize(reader);
        }
    }
}
