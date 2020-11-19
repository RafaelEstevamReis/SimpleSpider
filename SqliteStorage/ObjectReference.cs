using System;
using System.Collections.Generic;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Storage
{
    public class ObjectReference
    {
        public Uri Uri { get; set; } 
        public DateTime CrawTime { get; set; }
        public long InsertedItem { get; set; }
    }
}
