using System.Collections.Generic;
using System.Linq;

namespace RafaelEstevam.Simple.Spider.Storage
{
    public class InMemoryStorage<T> : Interfaces.IStorage
    {
        List<T> items;

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="link">Link where the object was found. Its is not stored</param>
        /// <param name="item">Item to be collected</param>
        /// <returns>Returns true aways</returns>
        public bool AddItem(Link link, T item)
        {
            items.Add(item);
            return true;
        }

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
            items = new List<T>();
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
            return items.Cast<dynamic>();
        }
        /// <summary>
        /// Retrieve all stored items
        /// </summary>
        public IEnumerable<T> ReadAll()
        {
            return items;
        }
    }
}
