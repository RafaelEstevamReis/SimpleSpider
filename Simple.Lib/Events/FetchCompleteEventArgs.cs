using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrappers;

namespace Net.RafaelEstevam.Spider
{
    /// <summary>
    /// Represents a method that passes fetch completed data
    /// </summary>
    /// <param name="Sender">The source of the event</param>
    /// <param name="args">Object containing fetch data</param>
    public delegate void FetchComplete(object Sender, FetchCompleteEventArgs args);

    /// <summary>
    /// Arguments to de FetchComplete event
    /// </summary>
    public class FetchCompleteEventArgs : FetchEventArgs
    {
        static readonly string saveBoundary = "--------------------";

        /// <summary>
        /// Byte array with the data fetched
        /// </summary>
        public byte[] Result { get; }
        /// <summary>
        /// The response headers returned 
        /// </summary>
        public HeaderCollection ResponseHeaders { get; }

        // Lazy loaded html string
        string htmlCache;
        /// <summary>
        /// LazyLoaded Text (Html?) content parsed from byte[] Result encoded with UTF8
        /// </summary>
        public string Html
        {
            get
            {
                if (htmlCache == null)
                {
                    var enc = Encoding.UTF8;
                    // check ResponseHeaders for encoding
                    return HtmlContent(enc);
                }
                return htmlCache;
            }
        }

        HtmlDocument document;
        public HtmlDocument GetDocument()
        {
            if (document == null)
            {
                document = HtmlToXElement.ParseHtmlDocument(Html);
            }
            return document;
        }

        // Lazy loaded xElement
        XElement xElement;
        /// <summary>
        /// Get the XElement representation of the Html property. 
        /// You can use Helper.HtmlToXElement properties to control XElement parsing
        /// </summary>
        public XElement GetXElement()
        {
            if (xElement == null)
            {
                xElement = HtmlToXElement.Parse(GetDocument(), new HtmlToXElement.ParseOptions()
                {
                    XElementParserMode = HtmlToXElement.XElementParser.LoadFromXmlReader,
                    SearchAndRemoveStyleElements = true,
                    SearchAndRemoveComments = true,
                    SearchAndRemoveScripts = true,
                    SearchForInvalidNames = true,
                });
            }
            return xElement;
        }
        /// <summary>
        /// Get the HObject representation of the Html property
        /// </summary>
        /// <returns>A HObject</returns>
        public HObject GetHObject()
        {
            return new HObject(GetXElement());
        }

        /// <summary>
        /// Parses  byte[] Result using and specific Encoding. The 'Html' property will be updated with this value
        /// </summary>
        /// <param name="enc">Encoding to be used</param>
        public string HtmlContent(Encoding enc)
        {
            xElement = null; // discards Lazy properties
            return htmlCache = enc.GetString(Result);

        }
        /// <summary>
        /// Constructs a new FetchCompleteEventArgs
        /// </summary>
        public FetchCompleteEventArgs(Link current, byte[] result, HeaderCollection requestHeaders, HeaderCollection responseHeaders)
        {
            this.Link = current;
            this.Result = result;
            this.RequestHeaders = requestHeaders;
            this.ResponseHeaders = responseHeaders;
        }
        /// <summary>
        /// Load a FetchCompleteEventArgs from a stream
        /// </summary>
        /// <param name="stream">Stream to load from</param>
        /// <returns>FetchCompleteEventArgs loaded</returns>
        public static FetchCompleteEventArgs LoadFetchResult(Stream stream)
        {
            // The streamreader internal cache will consume the binary payload
            BinaryReader br = new BinaryReader(stream);
            // read link
            var link = readLinesBlock("LINK", br);
            // read link
            var req = readLinesBlock("REQ-HDR", br);
            // read link
            var resp = readLinesBlock("RES-HDR", br);
            
            byte[] bytes = readToEnd(br);

            return new FetchCompleteEventArgs(Link.LoadLink(link), bytes, HeaderCollection.LoadHeader(req), HeaderCollection.LoadHeader(resp));
        }

        private static byte[] readToEnd(BinaryReader br)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[1024];
            int len;

            while ((len = br.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, len);
            }
            return ms.ToArray();
        }
        private static string[] readLinesBlock(string blockName, BinaryReader br)
        {
            List<string> lines = new List<string>();

            string line = readLine(br);
            if (line != $"{saveBoundary}{blockName}") throw new InvalidOperationException($"Invalid {blockName} boundary");
            while (!string.IsNullOrEmpty(line = readLine(br)))
            {
                lines.Add(line);
            }
            return lines.ToArray();
        }
        private static string readLine(BinaryReader br)
        {
            StringBuilder sb = new StringBuilder();
            // stream reader has a giant buffer
            while (true)
            {
                var iB = br.ReadByte();
                if (iB < 0) break; // EOS

                if (iB == '\r') continue;
                if (iB == '\n') break;
                sb.Append((char)(byte)iB); // ASCII only
            }
            return sb.ToString();
        }

        /// <summary>
        /// Saves a FetchCompleteEventArgs to a stream
        /// </summary>
        /// <param name="fetchComplete">Object to be saved</param>
        /// <param name="stream">Stream to save to</param>
        public static void SaveFetchResult(FetchCompleteEventArgs fetchComplete, Stream stream)
        {
            string[] lines;
            StreamWriter sw = new StreamWriter(stream, Encoding.ASCII); // headers are ASCII

            lines = Link.SaveLink(fetchComplete.Link).ToArray();
            writeLinesBlock("LINK", sw, lines);

            lines = HeaderCollection.SaveHeader(fetchComplete.RequestHeaders).ToArray();
            writeLinesBlock("REQ-HDR", sw, lines);

            lines = HeaderCollection.SaveHeader(fetchComplete.ResponseHeaders).ToArray();
            writeLinesBlock("RES-HDR", sw, lines);
            sw.Flush();

            if (fetchComplete.Result != null && fetchComplete.Result.Length > 0)
            {
                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write(fetchComplete.Result);
                bw.Flush();
            }
        }
        private static void writeLinesBlock(string blockName, StreamWriter sw, string[] lines)
        {
            sw.Write(saveBoundary);
            sw.WriteLine(blockName);

            foreach (var l in lines) sw.WriteLine(l);

            sw.WriteLine(); // Empty Line
        }
    }
}