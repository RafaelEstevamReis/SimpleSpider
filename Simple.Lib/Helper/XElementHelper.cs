using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RafaelEstevam.Simple.Spider.Helper
{
    /// <summary>
    /// Helper to do stuff with pages parsed as XElement 
    /// </summary>
    public static class XElementHelper
    {
        /// <summary>
        /// Enumerates all XElements named 'table'
        /// </summary>
        /// <param name="Root">Root element to be enumerated</param>
        /// <returns>Enumeration of XElement tables</returns>
        public static IEnumerable<XElement> GetTables(XElement Root)
        {
            return Root.XPathSelectElements("table");
        }
        /// <summary>
        /// Enumerates all XElements named 'table' as DataTable collection
        /// </summary>
        /// <param name="Root">Root element to be enumerated</param>
        /// <returns>Enumeration of DataTable tables</returns>
        public static IEnumerable<DataTable> GetAllTables(this XElement Root)
        {
            return GetTables(Root).Select(t => GetTable(t));
        }
        /// <summary>
        /// Converts a XElement table to a DataTable
        /// </summary>
        /// <param name="table">Table element to be converted</param>
        /// <returns>DataTable converted</returns>
        public static DataTable GetTable(XElement table)
        {
            if (table.Name.LocalName != "table") throw new ArgumentException("table is not a <table> element");
            string tableName = "";
            var att = table.Attributes("id").ToArray();
            if (att.Length > 0) tableName = att[0].Value;

            // search for <thead> e <tbody>
            // TODO, separate into two functions
            // if has HEAD e BODY call two times and set header
            // else, call directly

            DataTable dt = new DataTable(tableName);

            var caption = table.GetChilds("caption").ToArray();
            var tBody = table.GetChilds("tbody").ToArray();
            var tHead = table.GetChilds("thead").ToArray();
            if (tBody != null && tBody.Length > 0) table = tBody[0];

            if (caption != null && caption.Length > 0)
            {
                dt.ExtendedProperties["Caption"] = caption[0].Value;
            }

            // get all rows
            foreach (var row in table.GetChilds("tr", "th"))
            {
                var columns = row.GetChilds("td", "th").ToArray();
                var columnsHeader = columns;
                // inicializa as colunas
                if (dt.Columns.Count == 0)
                {
                    bool hasHeader = false;
                    if (tHead != null && tHead.Length > 0)
                    {
                        hasHeader = true;
                        columnsHeader = tHead[0].GetChilds("tr", "th").ToArray()[0].GetChilds("td", "th").ToArray();
                    }

                    if (row.Name.LocalName == "th") hasHeader = true;
                    if (columnsHeader.All(o => o.Name.LocalName == "th")) hasHeader = true;
                    foreach (var c in columnsHeader)
                    {
                        int colspan = 1;
                        var atts = c.Attributes("colspan").ToArray();
                        if (atts != null && atts.Length > 0) colspan = Convert.ToInt32(atts[0].Value);

                        string cName = "Cln" + (dt.Columns.Count + 1);
                        cName = c.Value.Trim();
                        dt.Columns.Add(cName, typeof(XElement));
                        for (int span = 1; span < colspan; span++) dt.Columns.Add("", typeof(XElement));
                    }
                    // não cria uma linha com o cabeçalho
                    if (hasHeader && tHead == null) continue;
                }
                // get all cells (cellspan)
                var cells = new List<XElement>();
                foreach (var v in columns)
                {
                    if (cells.Count >= dt.Columns.Count) continue; // evita erros
                    cells.Add(v);

                    int colspan = 1;
                    var atts = v.Attributes("colspan").ToArray();
                    if (atts != null && atts.Length > 0) colspan = Convert.ToInt32(atts[0].Value);
                    for (int span = 1; span < colspan; span++)
                    {
                        if (cells.Count >= dt.Columns.Count) continue;
                        cells.Add(null);
                    }
                }
                try
                {
                    dt.Rows.Add(cells.ToArray());
                }
                catch { }
            }
            return dt;
        }
        /// <summary>
        /// Enumerates all nodes with Name.LocalName equals to Name
        /// </summary>
        /// <param name="Root">Root object to start searching</param>
        /// <param name="Name">Name to be searched</param>
        /// <returns>Enumeration of XElements named Name</returns>
        public static IEnumerable<XElement> GetChilds(this XElement Root, string Name)
        {
            foreach (var x in Root.Elements())
            {
                if (x.Name.LocalName.Equals(Name, StringComparison.InvariantCultureIgnoreCase)) yield return x;
            }
        }
        /// <summary>
        /// Enumerates all nodes with any Name.LocalName equals to Names
        /// </summary>
        /// <param name="Root">Root object to start searching</param>
        /// <param name="Names">Names to be searched</param>
        /// <returns>Enumeration of XElements named any of Names</returns>
        public static IEnumerable<XElement> GetChilds(this XElement Root, params string[] Names)
        {
            foreach (var x in Root.Elements())
            {
                if (Names.Length == 0)
                    yield return x; // get all
                else if (Names.Contains(x.Name.LocalName))
                    yield return x;
            }
        }
    }
}
