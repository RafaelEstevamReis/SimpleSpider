using System.Text;
using System.Xml.Linq;
using Net.RafaelEstevam.Spider.Helper;
using Net.RafaelEstevam.Spider.Wrapers;

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

        // Lazy loaded xElement
        XElement xElement;
        /// <summary>
        /// Get the XElement representation of the Html property
        /// </summary>
        public XElement GetXElement()
        {
            if (xElement == null)
            {
                xElement = HtmlToXElement.Parse(Html);
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
    }
}