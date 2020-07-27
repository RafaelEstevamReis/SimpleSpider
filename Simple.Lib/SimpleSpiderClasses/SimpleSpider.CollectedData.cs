using System;

namespace Net.RafaelEstevam.Spider
{
    public sealed partial class SimpleSpider
    {
        public class CollectedData
        {
            public CollectedData(object Object, string CollectedOn)
            {
                this.Object = Object;
                this.CollectedOn = CollectedOn;
                this.CollectAt = DateTime.Now;
            }

            public object Object { get;  }
            public string CollectedOn { get;  }
            public DateTime CollectAt { get; }
        }
    }
}
