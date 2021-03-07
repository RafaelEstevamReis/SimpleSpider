﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RafaelEstevam.Simple.Spider.Helper;

namespace RafaelEstevam.Simple.Spider.Specialized.Apache
{
    public class Listing
    {
        public Uri Uri { get; }

        Queue<Uri> toVisit;
        HashSet<string> visited;

        public Listing(string url)
            : this(new Uri(url)) { }
        public Listing(Uri uri)
        {
            Uri = uri;
            toVisit = new Queue<Uri>();
            visited = new HashSet<string>();
        }

        public IEnumerable<ListingInfo> GetListing(ListingOptions options)
        {
            toVisit.Enqueue(Uri);
            yield return new ListingInfo()
            {
                IsDirectory = true,
                Parent = Uri,
                Uri = Uri
            };

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
            var pre = doc.DocumentNode.SelectSingleNode("//pre");
            if (rows != null)
            {
                return processRows(rows, uri, options);
            }
            else if (pre != null)
            {
                var nodes = pre.ChildNodes.ToArray(); // index-based
                return processPreLines(nodes, uri, options);
            }
            else
            {
                throw new NotImplementedException("This version of the apache directory listing is not supported yet");
            }
        }
        private IEnumerable<ListingInfo> processPreLines( HtmlAgilityPack.HtmlNode[] nodes, Uri uri, ListingOptions options)
        {
            // [9] is header's <hr>
            for (int i = 10; i < nodes.Length; i += 2)
            {
                if (nodes[i].Name == "hr") break;

                var text = nodes[i + 1];
                var textParts = text.InnerText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                var href = nodes[i].GetAttributeValue("href", "");
                var lnkText = nodes[i].InnerText;
                bool isParent = lnkText == "Parent Directory";
                if(isParent) textParts = new string[] {" ", " ", "-" };

                bool isDir = href.EndsWith("/");

                if (isParent && options.NoParent) continue;

                DateTime.TryParse(textParts[0] + " " + textParts[1], out DateTime lastModified);

                string size = textParts[2];
                long numSize = processTextualSize(size);

                //bool isDir = dir || isParent;
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
        private IEnumerable<ListingInfo> processRows(HtmlAgilityPack.HtmlNodeCollection rows, Uri uri, ListingOptions options)
        {
            foreach (var row in rows)
            {
                var cells = row.Descendants().ToArray();

                if (cells[0].Name == "th") continue;

                bool dir = cells[0].InnerHtml.Contains("/icons/folder.gif");
                bool isParent = cells[0].InnerHtml.Contains("/icons/back.gif");

                if (isParent && options.NoParent) continue;

                var href = cells[3].GetAttributeValue("href", "");
                string strLastMod = cells[5].InnerText;

                DateTime.TryParse(strLastMod, out DateTime lastModified);

                string size = cells[7].InnerText.Trim();

                long numSize = processTextualSize(size);

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
        private long processTextualSize(string size)
        {
            if (size.Contains("-")) return 0;

            var numbers = ConversionHelper.ToDouble(ConversionHelper.ExtractNumbers(size), 0);

            if (size[^1] == 'K') numbers *= 1024;
            else if (size[^1] == 'M') numbers *= 1024 * 1024;
            else if (size[^1] == 'G') numbers *= 1024 * 1024 * 1024;

            return Convert.ToInt64(numbers);
        }

        public static ListingDirectory BuildTree(IEnumerable<ListingInfo> listings)
        {
            var allItems = listings.OrderBy(o => o.Uri.ToString())
                                   .Distinct()
                                   .ToArray(); // keep enumeration "active"

            var root = allItems.First();

            var dicDirs = allItems.Where(i => i.IsDirectory)
                                  .ToDictionary(o => o.Uri, o => new ListingDirectory(o));

            foreach (var i in allItems)
            {
                // remove "Parent dir link"
                if (i.Parent.ToString().Length >= i.Uri.ToString().Length) continue;

                var parent = dicDirs[i.Parent];

                if (i.IsDirectory) parent.Directories.Add(dicDirs[i.Uri]);
                else parent.Files.Add(ListingFile.Create(i));
            }

            return dicDirs[root.Uri];
        }
    }

    public class ListingDirectory
    {
        public ListingInfo Entity { get; }
        public List<ListingDirectory> Directories { get; }
        public List<ListingFile> Files { get; }

        public ListingDirectory(ListingInfo entity)
        {
            Directories = new List<ListingDirectory>();
            Files = new List<ListingFile>();
            Entity = entity;
        }

        public bool HasFiles => Files.Count > 0;
        public bool HasDirectories => Directories.Count > 0;
        public bool IsEmpty => !(HasFiles || HasDirectories);

        public IEnumerable<ListingDirectory> GetAllDescendants()
        {
            return Directories.SelectMany(d => d.GetAllDescendantsAndSelf());
        }
        public IEnumerable<ListingDirectory> GetAllDescendantsAndSelf()
        {
            yield return this;
            foreach (var d in GetAllDescendants()) yield return d;
        }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
    public class ListingFile : ListingInfo
    {
        private ListingFile() { }
        public static ListingFile Create(ListingInfo entity)
        {
            if (entity.IsDirectory) throw new ArgumentException("Must be a file");

            return new ListingFile()
            {
                IsDirectory = false,
                Uri = entity.Uri,
                Parent = entity.Parent,
                FileName = entity.FileName,
                FileExtension = entity.FileExtension,
                FileSize = entity.FileSize,
                Size = entity.Size,
                LastModified = entity.LastModified,

            };
        }
        public override string ToString()
        {
            return $"{FileName} [{Size}]";
        }
    }
}
