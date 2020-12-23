using System;
using System.Collections.Generic;
using System.Text;

namespace RafaelEstevam.Simple.Spider.Storage
{
    public class InMemoryStorage : Interfaces.IStorage
    {
        List<dynamic> items;

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="link">Link where the object was found. Its is not stored</param>
        /// <param name="item">Item to be collected</param>
        /// <returns>Returns true aways</returns>
        public bool AddItem(Link link, dynamic item)
        {
            items.Add(item);
            return true;
        }
        /// <summary>
        /// Counts the number of items collected
        /// </summary>
        /// <returns>Returns the number of items collected</returns>
        public int Count()
        {
            return items.Count;
        }

        /// <summary>
        /// Initializes the storager
        /// </summary>
        public void Initialize(Configuration Config)
        {
            items = new List<dynamic>();
        }
        /// <summary>
        /// InMemory data does not save data!
        /// </summary>
        public void LoadData()
        { return; }
        /// <summary>
        /// InMemory data does not save data!
        /// </summary>
        public void SaveData(bool IsAutoSave)
        { return; }

        /// <summary>
        /// Retrieve all stored items
        /// </summary>
        public IEnumerable<dynamic> RetrieveAllItems()
        {
            return items;
        }
    }
}
