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
            using var sw = new StreamWriter(file);
            Serialize(obj, sw);
        }
        /// <summary>
        /// Serialize a generic {T} object to a stream
        /// </summary>
        /// <typeparam name="T">Type of the objects to be serialized</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <param name="stream">Stream to save object to</param>
        public static void Serialize<T>(T obj, TextWriter stream) where T : new()
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            x.Serialize(stream, obj);
        }
        /// <summary>
        /// Serialize a generic {T} object to a string
        /// </summary>
        /// <typeparam name="T">Type of the objects to be serialized</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>String containing the object</returns>
        public static string Serialize<T>(T obj) where T : new()
        {
            using StringWriter sw = new StringWriter();
            Serialize(obj, sw);
            return sw.ToString();
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
