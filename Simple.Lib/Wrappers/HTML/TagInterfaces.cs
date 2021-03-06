﻿namespace RafaelEstevam.Simple.Spider.Wrappers.HTML
{
    /// <summary>
    /// Interface for Html Tag, define Global Attributes
    /// </summary>
    public interface ITag
    {
        // Not all tags are implemented,
        // see here: https://www.w3schools.com/tags/ref_standardattributes.asp
        // a complete list

        /// <summary>
        /// Name attribute
        /// </summary>
        public string TagName { get; }
        /// <summary>
        /// Id attribute
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Class attribute
        /// </summary>
        public string Class { get; }
        /// <summary>
        /// Style attribute
        /// </summary>
        public string Style { get; }
        /// <summary>
        /// Itemprop attribute
        /// </summary>
        public string ItemProp { get; }
        /// <summary>
        /// Casts Tag to specific Html Tag type
        /// </summary>
        public T Cast<T>() where T : ITag;
    }
    /// <summary>
    /// Represents tags with Value attribute
    /// </summary>
    public interface ITagValue : ITag
    {
        // Not all tags are implemented,
        // see here: https://www.w3schools.com/tags/att_value.asp
        // a complete list

        /// <summary>
        /// Value attribute
        /// </summary>
        public string Value { get; }
    }
    /// <summary>
    /// Represents tags with Type attribute
    /// </summary>
    public interface ITagType : ITag
    {
        // Not all tags are implemented,
        // see here: https://www.w3schools.com/tags/att_type.asp
        // a complete list

        /// <summary>
        /// Type attribute
        /// </summary>
        public string Type { get; }
    }
    /// <summary>
    /// Represents tags with Src attribute
    /// </summary>
    public interface ITagSrc : ITag
    {
        // Not all tags are implemented,
        // see here: https://www.w3schools.com/tags/att_src.asp
        // a complete list

        /// <summary>
        /// Src attribute
        /// </summary>
        public string Src { get; }
    }
    /// <summary>
    /// Represents tags with Name attribute
    /// </summary>
    public interface ITagName : ITag
    {
        // Not all tags are implemented,
        // see here: https://www.w3schools.com/tags/att_name.asp
        // a complete list

        /// <summary>
        /// Name attribute
        /// </summary>
        public string Name { get; }
    }
    /// <summary>
    /// Represents tags associated with Label element
    /// </summary>
    public interface ILabelable : ITag
    {
    }
}
