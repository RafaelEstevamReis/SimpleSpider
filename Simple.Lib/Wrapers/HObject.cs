using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Net.RafaelEstevam.Spider.Wrapers
{
    public class HObject
    {
        public enum SearchType
        {
            ElementName,
            IdEquals,
            ClassContaining,
        }

        private IEnumerable<XElement> xElements;

        public HObject(XElement x)
        {
            this.xElements = new XElement[] { x };
        }
        public HObject(IEnumerable<XElement> xs)
        {
            this.xElements = xs;
        }

        public HObject this[string Name, SearchType Type]
        {
            get
            {
                return Type switch
                {
                    SearchType.ElementName => Tags(Name),
                    SearchType.IdEquals => IDs(Name),
                    SearchType.ClassContaining => Classes(Name),
                    _ => throw new ArgumentException("SearchType dows not exists"),
                };
            }
        }
        public HObject this[string TagName]
        {
            get
            {
                return this[TagName, SearchType.ElementName];
            }
        }

        #region Element Name

        public HObject Tags(string TagName)
        {
            var x = xElements
                         .SelectMany(el => el.DescendantsAndSelf()
                                             .Where(d => d.Name.LocalName == TagName))
                         .ToArray(); // force enumerate
            return new HObject(x);
        }

        #endregion

        #region ID Stuff

        public HObject IDs(string IDsEquals)
        {
            return Having("id", IDsEquals);
        }
        public HObject OfID(string IDsEquals)
        {
            return OfWhich("id", IDsEquals);
        }

        #endregion

        #region Classes

        public HObject Classes(string HasClass)
        {
            var x = xElements
                      .SelectMany(el => el.DescendantsAndSelf()
                                          .Where(d => d.Attributes()
                                                       .Any(a => a.Name.LocalName == "class" && a.Value.Split(' ').Contains(HasClass))))
                      .ToArray();

            return new HObject(x);
        }
        public HObject OfClass(string IDsEquals)
        {
            return new HObject(xElements
                .Where(x => x.Attribute("class")?.Value != null)
                .Where(x => x.Attribute("class").Value.Split(' ').Contains(IDsEquals)).ToArray());
        }

        #endregion

        #region Choose Attribute

        public HObject Having(string AttributeName, string AttibuteValue)
        {
            return new HObject(xElements.DescendantsAndSelf().Where(x => x.Attribute(AttributeName)?.Value == AttibuteValue).ToArray());
        }
        public HObject OfWhich(string AttributeName, string AttibuteValue)
        {
            return new HObject(xElements.Where(x => x.Attribute(AttributeName)?.Value == AttibuteValue).ToArray());
        }

        #endregion

        public static implicit operator XElement(HObject h)
        {
            return h.xElements.FirstOrDefault();
        }
        public static implicit operator XElement[](HObject h)
        {
            return h.xElements.ToArray();
        }

        public static implicit operator string(HObject h)
        {
            return h.xElements.FirstOrDefault()?.Value;
        }
        public static implicit operator string[](HObject h)
        {
            return h.xElements.Select(x => x.Value).ToArray();
        }

    }
}