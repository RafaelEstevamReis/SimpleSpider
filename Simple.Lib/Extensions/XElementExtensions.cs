using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace Net.RafaelEstevam.Spider.Extensions
{
    public static class XElementExtensions
    {
        public static IEnumerable<XElement> GetAllTablesX(this XElement Root)
        {
            foreach (var e in Root.DescendantsAndSelf())
            {
                if (e.Name.LocalName == "table") yield return e;
            }
        }
        public static IEnumerable<DataTable> GetAllTables(this XElement Root)
        {
            return GetAllTablesX(Root).Select(t => GetDataTable(t));
        }
        public static DataTable GetDataTable(this XElement TableElement)
        {
            if (TableElement.Name.LocalName != "table") throw new ArgumentException("The element is not a table");

            // Extract headers from <head>
            // Extract data from <body>

            DataTable dt = new DataTable();
            foreach (var row in TableElement.Descendants("tr"))
            {
                var cols = row.Elements().ToArray();
                if (dt.Columns.Count == 0)
                {
                    for (int i = 0; i < cols.Length; i++)
                    {
                        dt.Columns.Add();
                    }
                }
                dt.Rows.Add(cols.Select(o => o.Value).ToArray());
            }
            return dt;
        }
    }
}
