using System;
using System.Collections.Generic;

namespace RafaelEstevam.Simple.Spider.Interfaces
{
    /// <summary>
    /// Represents a Interface to store collected data
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Initialize the Storage with spider config
        /// </summary>
        void Initialize(Configuration Config);
        /// <summary>
        /// Instructs the storage to load saved data (optional)
        /// </summary>
        void LoadData();
        /// <summary>
        /// Instructs the storage to save data
        /// </summary>
        /// <param name="IsAutoSave">Indicates if was initiated by the auto-save. If false the spider finished its queue</param>
        void SaveData(bool IsAutoSave);

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="link">Link where the object was found</param>
        /// <param name="item">Item to be collected</param>
        /// <returns>Returns true if the item was added to the collection</returns>
        bool AddItem(Link link, dynamic item);

        /// <summary>
        /// Counts the number of items collected
        /// </summary>
        /// <returns>Returns the number of items collected</returns>
        int Count();

        /// <summary>
        /// Retrieve all stored items
        /// </summary>
        IEnumerable<dynamic> RetrieveAllItems() 
        {
            throw new NotImplementedException();
        }
    }
}
