using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using RafaelEstevam.Simple.Spider.Helper;
using RafaelEstevam.Simple.Spider.Wrappers;

namespace RafaelEstevam.Simple.Spider
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
        /// <summary>
        /// Defines an default Encoding to be used when no instance Encoding is defined 
        /// </summary>
        public static Encoding DefaultEncoding = Encoding.UTF8;
        /// <summary>
        /// Instance encoding to be used. If null, static DefaultEncoding will be used
        /// </summary>
        public Encoding Encoding = null;

        static readonly string saveBoundary = "--------------------";

        /// <summary>
        /// Byte array with the data fetched
        /// </summary>
        public byte[] Result { get; }
        /// <summary>
        /// The response headers returned 
        /// </summary>
        public HeaderCollection ResponseHeaders { get; }

        #region Process Result

        // Lazy loaded html string
        string htmlCache;
        /// <summary>
        /// LazyLoaded Text (Html?) content parsed from byte[] Result encoded with static DefaultEncoding (UTF-8) or Encoding properties
        /// </summary>
        public string Html
        {
            get
            {
                if (htmlCache == null)
                {
                    // check ResponseHeaders for encoding
                    return HtmlContent(Encoding ?? DefaultEncoding);
                }
                return htmlCache;
            }
        }

        HtmlDocument document;
        /// <summary>
        /// Parses the Result bytes with HtmlAgilityPack
        /// </summary>
        public HtmlDocument GetDocument()
        {
            if (document == null)
            {
                using MemoryStream ms = new MemoryStream(Result);
                document = HtmlParseHelper.ParseHtmlDocument(ms);
            }
            return document;
        }
        /// <summary>
        /// Parses the Html string with HtmlAgilityPack using specified encoding
        /// </summary>
        /// <param name="SpecifyEncoding">Encoding to use, if NULL instance Encoding will be used. If NULL static DefaultEncoding will be used.</param>
        public HtmlDocument GetDocument(Encoding SpecifyEncoding)
        {
            if (SpecifyEncoding == null) SpecifyEncoding = Encoding ?? DefaultEncoding;

            document = HtmlParseHelper.ParseHtmlDocument(HtmlContent(SpecifyEncoding));
            return document;
        }

        /// <summary>
        /// Get the HObject representation of the HtmlDocument using GetDocument()
        /// </summary>
        /// <returns>A HObject</returns>
        public HObject GetHObject()
        {
            return new HObject(GetDocument());
        }

        /// <summary>
        /// Parses  byte[] Result using and specific Encoding. The 'Html' property will be updated with this value
        /// </summary>
        /// <param name="enc">Encoding to be used</param>
        public string HtmlContent(Encoding enc)
        {
            return htmlCache = enc.GetString(Result);
        }

        #endregion

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
