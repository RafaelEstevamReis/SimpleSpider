using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Net.RafaelEstevam.Spider.Wrappers
{
    /// <summary>
    /// Represents a Html Tags Collection
    /// </summary>
    public class HObject: IEnumerable<HObject>
    {
        /// <summary>
        /// Types of items to search
        /// </summary>
        public enum SearchType
        {
            /// <summary>
            /// Search for Tag Name
            /// </summary>
            AnyElement,
            /// <summary>
            /// Search for Tag Name only on child elements
            /// </summary>
            ChildElement,
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
        /// Checks if the filtering results in an empty object
        /// </summary>
        /// <returns>True if there are no elements left</returns>
        public bool IsEmpty()
        {
            return xElements.Count() == 0;
        }

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
                    SearchType.AnyElement => Tags(Name),
                    SearchType.ChildElement => Children(Name),
                    SearchType.AnyIdEquals => IDs(Name),
                    SearchType.AnyClassContaining => Classes(Name),
                    SearchType.FilterId => OfID(Name),
                    SearchType.FilterClass => OfClass(Name),
                    _ => throw new ArgumentException("SearchType dows not exists"),
                };
            }
        }
        /// <summary>
        /// Gets a HObject with specified Tag. Supports css notation of '#' for ID and '.' for classes
        /// </summary>
        /// <param name="SearchName">Name of the Tag to search for or the '#{IdName}' or the '.{ClassName}'</param>
        /// <returns>A HObject filtered</returns>
        public HObject this[string SearchName]
        {
            get
            {
                if (SearchName.Contains(">"))
                {
                    return this[SearchName.Split('>')];
                }

                if (SearchName.StartsWith('#'))
                {
                    return this[SearchType.FilterId, SearchName.Substring(1)];
                }
                if (SearchName.StartsWith('.'))
                {
                    return this[SearchType.FilterClass, SearchName.Substring(1)];
                }
                return this[SearchType.AnyElement, SearchName];
            }
        }
        /// <summary>
        /// Gets a HObject searched in chain with an array of Names. Supports css notation of '#' for ID and '.' for classes
        /// </summary>
        /// <param name="SearchNames">Array of names of the Tag to search for or the '#{IdName}' or the '.{ClassName}'</param>
        /// <returns>A HObject filtered</returns>
        public HObject this[params string[] SearchNames]
        {
            get
            {
                var result = this;
                foreach (var n in SearchNames)
                {
                    result = result[n.Trim()];
                }
                return result;
            }
        }
        /// <summary>
        /// Gets the n-th HObject
        /// </summary>
        /// <param name="Position">Index of the HObject to be returned</param>
        /// <returns>HObject at specified position</returns>
        public HObject this[int Position]
        {
            get { return new HObject(xElements.ElementAt(Position)); }
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
        /// <summary>
        /// Returns all children tags named TagName
        /// </summary>
        /// <param name="TagName">The name of the tag</param>
        /// <returns>A HObject filtered</returns>
        public HObject Children(string TagName)
        {
            var x = xElements
                         .SelectMany(el => el.Elements()
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
                .Where(x => x.Attribute("class").Value == ClassName
                            || x.Attribute("class").Value.Split(' ').Contains(ClassName)).ToArray());
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
        /// Executes a CSS Select query with '&gt;' separator, '.Class' and '#Id'
        /// </summary>
        /// <param name="Query">CSS Query string</param>
        /// <returns>HObject selected</returns>
        public HObject CssSelect(string Query)
        {
            return this[Query.Split('>')];   
        }
        /// <summary>
        /// Executes a XPath Select query in each element and returns all results
        /// </summary>
        /// <param name="Query">XPath Query string</param>
        /// <returns>HObject elements with all results</returns>
        public HObject XPathSelect(string Query)
        {
            return new HObject(xElements.SelectMany(x => x.XPathSelectElements(Query)));
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

        /// <summary>
        /// Gets the value of attribute named AttributeName of the first item
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <returns>Value of the attribute</returns>
        public string GetAttributeValue(string AttributeName)
        {
            return GetXElement()?.Attribute(AttributeName)?.Value;
        }
        /// <summary>
        /// Gets all the values of the attribute named AttributeName of all items
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <returns>String array of Values</returns>
        public string[] GetAttributeValues(string AttributeName)
        {
            return GetXElements().Select(e => e.Attribute(AttributeName)?.Value).ToArray();
        }

        /// <summary>
        /// Checks if the first element has the specified attribute
        /// </summary>
        /// <param name="AttributeName">Name of the attribute</param>
        /// <returns>True if the first element has the Attribute</returns>
        public bool HasAttribute(string AttributeName)
        {
            return GetXElement().Attribute(AttributeName) != null;
        }

        /// <summary>
        /// Gets the value of attribute 'href' of the first item
        /// </summary>
        /// <returns>String containing the value</returns>
        public string GetHrefValue()
        {
            return GetAttributeValue("href");
        }
        /// <summary>
        /// Gets all the values of the attribute 'href' of all items
        /// </summary>
        /// <returns>String array with values</returns>
        public string[] GetHrefValues()
        {
            return GetAttributeValues("href");
        }
        /// <summary>
        /// Gets the value of the Class attribute of the first item
        /// </summary>
        /// <returns>String containing the Class</returns>
        public string GetClassValue()
        {
            return GetAttributeValue("class");
        }
        /// <summary>
        /// Gets the value of the Id attribute of the first item
        /// </summary>
        /// <returns>String containing the Id</returns>
        public string GetIdValue()
        {
            return GetAttributeValue("id");
        }
        /// <summary>
        /// Gets the value of all the Id attribute of all items
        /// </summary>
        /// <returns>String array containing all the Ids</returns>
        public string[] GetIdsValues()
        {
            return GetAttributeValues("id");
        }

        /// <summary>
        /// Gets the value of the Style attribute of the first item
        /// </summary>
        /// <returns>String containing the Style</returns>
        public string GetStyleValue()
        {
            return GetAttributeValue("style");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the HObject collection.
        /// </summary>
        /// <returns>An HObject enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<HObject> GetEnumerator()
        {
            return xElements.Select(x => new HObject(x)).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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