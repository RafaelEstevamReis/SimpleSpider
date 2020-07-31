using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Net.RafaelEstevam.Spider.Wrapers
{
    /// <summary>
    /// Represents a Html Tags Collection
    /// </summary>
    public class HObject
    {
        /// <summary>
        /// Types of items to search
        /// </summary>
        public enum SearchType
        {
            /// <summary>
            /// Search for Tag Name
            /// </summary>
            ElementName,
            /// <summary>
            /// Search for ID attribute
            /// </summary>
            AnyIdEquals,
            /// <summary>
            /// Search for Class attribute
            /// </summary>
            AnyClassContaining,

            /// <summary>
            /// Filter current by ID, similar to this.OfID(Name)
            /// </summary>
            FilterId,
            /// <summary>
            /// Filter current by Class, similar to this.OfClass(Name)
            /// </summary>
            FilterClass,
        }

        private IEnumerable<XElement> xElements;
        /// <summary>
        /// Initializes a new instance of the HObject class
        /// </summary>
        /// <param name="x">A root XElement</param>
        public HObject(XElement x)
        {
            this.xElements = new XElement[] { x };
        }
        /// <summary>
        /// Initializes a new instance of the HObject class
        /// </summary>
        /// <param name="xs">A collection of XElements</param>
        public HObject(IEnumerable<XElement> xs)
        {
            this.xElements = xs;
        }
        /// <summary>
        /// Gets a HObject with specified Name matching Type
        /// </summary>
        /// <param name="Name">Name of the element to search for</param>
        /// <param name="Type">Type of search</param>
        /// <returns>A HObject filtered</returns>
        public HObject this[SearchType Type, string Name]
        {
            get
            {
                return Type switch
                {
                    SearchType.ElementName => Tags(Name),
                    SearchType.AnyIdEquals => IDs(Name),
                    SearchType.AnyClassContaining => Classes(Name),
                    SearchType.FilterId => OfID(Name),
                    SearchType.FilterClass => OfClass(Name),
                    _ => throw new ArgumentException("SearchType dows not exists"),
                };
            }
        }
        /// <summary>
        /// Gets a HObject with specified Tag
        /// </summary>
        /// <param name="TagName">Name of the Tag to search for</param>
        /// <returns>A HObject filtered</returns>
        public HObject this[string TagName]
        {
            get
            {
                return this[SearchType.ElementName, TagName];
            }
        }

        #region Element Name
        /// <summary>
        /// Returns all Tags named TagName
        /// </summary>
        /// <param name="TagName">The name of the tag</param>
        /// <returns>A HObject filtered</returns>
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
        /// <summary>
        /// Returns all Tags with ID equals to specified parameter
        /// </summary>
        /// <param name="IDsEquals">Id value to search for</param>
        /// <returns>A HObject</returns>
        public HObject IDs(string IDsEquals)
        {
            return Having("id", IDsEquals);
        }
        /// <summary>
        /// Filters current tags selecting only the ones with specified Id value
        /// </summary>
        /// <param name="IDsEquals">Id value to search for</param>
        /// <returns>A HObject</returns>
        public HObject OfID(string IDsEquals)
        {
            return OfWhich("id", IDsEquals);
        }

        #endregion

        #region Classes
        /// <summary>
        /// Returns all Tags with Class containing specified parameter
        /// Note: In HTML classes are an array of strings
        /// </summary>
        /// <param name="ClassName">The class name to search for</param>
        /// <returns>A HObject</returns>
        public HObject Classes(string ClassName)
        {
            var x = xElements
                      .SelectMany(el => el.DescendantsAndSelf()
                                          .Where(d => d.Attributes()
                                                       .Any(a => a.Name.LocalName == "class" && a.Value.Split(' ').Contains(ClassName))))
                      .ToArray();

            return new HObject(x);
        }
        /// <summary>
        /// Filters current tags selecting only the ones with specified Class value
        /// Note: In HTML classes are an array of strings
        /// </summary>
        /// <param name="ClassName">The class name to search for</param>
        /// <returns>A HObject</returns>
        public HObject OfClass(string ClassName)
        {
            return new HObject(xElements
                .Where(x => x.Attribute("class")?.Value != null)
                .Where(x => x.Attribute("class").Value.Split(' ').Contains(ClassName)).ToArray());
        }

        #endregion

        #region Choose Attribute
        /// <summary>
        /// Returns all Tags with Named Attribute equals to specified parameter. Search in all descendants
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <param name="AttributeValue">Value to search for</param>
        /// <returns>A HObject</returns>
        public HObject Having(string AttributeName, string AttributeValue)
        {
            return new HObject(xElements.DescendantsAndSelf().Where(x => x.Attribute(AttributeName)?.Value == AttributeValue).ToArray());
        }
        /// <summary>
        /// Filters current tags selecting only the ones with Named Attribute equals to specified parameter. Search only in root elements
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <param name="AttributeValue">Value to search for</param>
        /// <returns>A HObject</returns>
        public HObject OfWhich(string AttributeName, string AttributeValue)
        {
            return new HObject(xElements.Where(x => x.Attribute(AttributeName)?.Value == AttributeValue).ToArray());
        }

        /// <summary>
        /// Returns first XElement of the collection
        /// </summary>
        public XElement GetXElement()
        {
            return xElements.FirstOrDefault();
        }
        /// <summary>
        /// Returns all XElements of the collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XElement> GetXElements()
        {
            return xElements;
        }

        /// <summary>
        /// Returns first element Value
        /// </summary>
        /// <returns>The string Value of the element</returns>
        public string GetValue()
        {
            return GetXElement()?.Value;
        }
        /// <summary>
        /// Returns an array with all Elements values
        /// </summary>
        /// <returns>String array of the values</returns>
        public string[] GetValues()
        {
            return xElements.Select(x => x.Value).ToArray();
        }

        #endregion
        /// <summary>
        /// Returns first XElement of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator XElement(HObject h)
        {
            return h.GetXElement();
        }
        /// <summary>
        /// Returns all XElements of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator XElement[](HObject h)
        {
            return h.GetXElements().ToArray();
        }
        /// <summary>
        /// Returns the Value of the first element of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator string(HObject h)
        {
            return h.GetValue();
        }
        /// <summary>
        /// Returns the Values of all elements of the collection
        /// </summary>
        /// <param name="h">A HObject to be converted</param>
        public static implicit operator string[](HObject h)
        {
            return h.GetValues();
        }

    }
}