using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RafaelEstevam.Simple.Spider.Helper
{
    public class ApacheListing
    {
        public Uri Uri { get; }

        Queue<Uri> toVisit;
        HashSet<string> visited;

        public ApacheListing(string url)
            : this(new Uri(url)) { }
        public ApacheListing(Uri uri)
        {
            Uri = uri;
            toVisit = new Queue<Uri>();
            visited = new HashSet<string>();
        }

        public IEnumerable<ListingInfo> GetListing(ListingOptions options)
        {
            toVisit.Enqueue(Uri);

            while (toVisit.TryDequeue(out Uri uri))
            {
                IEnumerable<ListingInfo> result = processPage(uri, options);

                foreach (var l in result)
                {
                    yield return l;

                    if (!l.IsDirectory) continue;
                    enqueueDirectory(l, options);
                }
            }
        }

        private void enqueueDirectory(ListingInfo l, ListingOptions options)
        {
            if (options.ShouldFetch != null)
            {
                var args = new ShouldFetchEventArgs(new Link(l.Uri, l.Uri));
                options.ShouldFetch(this, args);
                if (args.Cancel) return;
            }

            toVisit.Enqueue(l.Uri);
        }
        private IEnumerable<ListingInfo> processPage(Uri uri, ListingOptions options)
        {
            HtmlAgilityPack.HtmlDocument doc;
            var url = uri.ToString();
            if (visited.Contains(url)) return new ListingInfo[0];

            try
            {
                doc = FetchHelper.FetchResourceDocument(uri, enableCaching: options.AllowCaching);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err: " + ex.Message);
                // send to end
                toVisit.Enqueue(Uri);
                Thread.Sleep(5000);
                return new ListingInfo[0];
            }
            // add to 'seen' list
            visited.Add(url);

            var rows = doc.DocumentNode.SelectNodes("//table/tr");
            return processRows(rows, uri, options);
        }

        private IEnumerable<ListingInfo> processRows(HtmlAgilityPack.HtmlNodeCollection rows, Uri uri, ListingOptions options)
        {
            foreach (var row in rows)
            {
                var cells = row.Descendants().ToArray();

                if (cells[0].Name == "th") continue;

                bool dir = cells[0].InnerHtml.Contains("/icons/folder.gif");
                bool isParent = cells[0].InnerHtml.Contains("/icons/back.gif");

                var href = cells[3].GetAttributeValue("href", "");
                string strLastMod = cells[5].InnerText;
                DateTime lastModified;
                DateTime.TryParse(strLastMod, out lastModified);

                string size = cells[7].InnerText.Trim();

                if (isParent && options.NoParent) continue;

                long numSize = 0;
                if (!size.Contains("-"))
                {
                    var numbers = ConversionHelper.ToDouble(ConversionHelper.ExtractNumbers(size), 0);

                    if (size[^1] == 'K') numbers *= 1024;
                    else if (size[^1] == 'M') numbers *= 1024 * 1024;
                    else if (size[^1] == 'G') numbers *= 1024 * 1024 * 1024;

                    numSize = Convert.ToInt64(numbers);
                }

                bool isDir = dir || isParent;
                string fileName = "";
                if (!isDir) fileName = href;

                yield return new ListingInfo()
                {
                    Parent = uri,
                    Uri = new Uri(uri, href),
                    LastModified = lastModified,
                    Size = size,
                    FileSize = numSize,
                    IsDirectory = isDir,
                    FileName = fileName,
                    FileExtension = fileName.Split('.')[^1]
                };
            }
        }

        public class ListingInfo
        {
            public bool IsDirectory { get; set; }
            public Uri Parent { get; set; }
            public Uri Uri { get; set; }
            public DateTime LastModified { get; set; }
            public long FileSize { get; set; }
            public string Size { get; set; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }
        }

        public class ListingOptions
        {
            public bool  NoParent { get; set; }
            public ShouldFetch ShouldFetch { get; set; }
            public bool AllowCaching { get; set; }
        }
    }
}
