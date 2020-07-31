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
            ClassEquals,
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
                if (Type == SearchType.ElementName)
                {
                    var x = xElements
                        .SelectMany(el => el.DescendantsAndSelf()
                                            .Where(d => d.Name.LocalName == Name))
                        .ToArray();
                    return new HObject(x);
                }
                else if (Type == SearchType.IdEquals)
                {
                    var x = xElements
                        .SelectMany(el => el.DescendantsAndSelf()
                                            .Where(d => d.Attributes()
                                                         .Any(a => a.Name.LocalName == "id" && a.Value == Name)))
                        .ToArray();

                    return new HObject(x);
                }
                else if (Type == SearchType.ClassEquals)
                {
                    var x = xElements
                      .SelectMany(el => el.DescendantsAndSelf()
                                          .Where(d => d.Attributes()
                                                       .Any(a => a.Name.LocalName == "class" && a.Value == Name)))
                      .ToArray();

                    return new HObject(x);
                }
                else if (Type == SearchType.ClassContaining)
                {
                    var x = xElements
                      .SelectMany(el => el.DescendantsAndSelf()
                                          .Where(d => d.Attributes()
                                                       .Any(a => a.Name.LocalName == "class" && a.Value.Split(' ').Contains(Name))))
                      .ToArray();

                    return new HObject(x);
                }

                throw new ArgumentException("SearchType dows not exists");
            }
        }
        public HObject this[string TagName]
        {
            get
            {
                return this[TagName, SearchType.ElementName];
            }
        }

        public static implicit operator XElement(HObject h)
        {
            return h.xElements.FirstOrDefault();
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