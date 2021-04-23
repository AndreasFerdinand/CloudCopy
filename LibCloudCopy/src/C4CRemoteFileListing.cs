namespace CloudCopy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;

    public class C4CRemoteFileListing : IRemoteFileListing<C4CRemoteFileMetadata>
    {
        private List<C4CRemoteFileMetadata> remoteFileMetadata = new List<C4CRemoteFileMetadata>();

        public C4CRemoteFileListing(string sourceXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXML);

            XmlNamespaceManager mgr = C4CRemoteFileMetadata.GetDefaultXmlNamespaceManager();

            XmlNodeList xmlNodes = xmlDoc.SelectNodes("//default:entry", mgr);

            foreach (XmlNode elementNode in xmlNodes)
            {
                C4CRemoteFileMetadata currentFile = new C4CRemoteFileMetadata(elementNode);

                this.remoteFileMetadata.Add(currentFile);
            }
        }

        public IEnumerator<C4CRemoteFileMetadata> GetEnumerator()
        {
            for (int i = 0; i < this.remoteFileMetadata.Count; i++)
            {
                yield return this.remoteFileMetadata[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void ListFiles()
        {
            this.ListFiles(new OutputOptions());
        }

        public void SortFiles(IOutputOptions outputOptions)
        {
            if (this.remoteFileMetadata.Count == 0)
            {
                return;
            }

            if (outputOptions.SortAttribute == "UUID")
            {
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => x.UUID.CompareTo(y.UUID));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => y.UUID.CompareTo(x.UUID));
                }
            }
            else if (outputOptions.SortAttribute == "MimeType")
            {
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => x.MimeType.CompareTo(y.MimeType));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => y.MimeType.CompareTo(x.MimeType));
                }
            }
            else if (outputOptions.SortAttribute == "FilenameExtension")
            {
                #pragma warning disable S1449
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => Path.GetExtension(x.Filename).CompareTo(Path.GetExtension(y.Filename)));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => Path.GetExtension(y.Filename).CompareTo(Path.GetExtension(x.Filename)));
                }
                #pragma warning restore S1449
            }
            else
            {
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => x.Filename.CompareTo(y.Filename));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => y.Filename.CompareTo(x.Filename));
                }
            }
        }

        public void ListFiles(IOutputOptions outputOptions)
        {
            if (this.remoteFileMetadata.Count == 0)
            {
                return;
            }

            if (outputOptions.SortAttribute == "UUID")
            {
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => x.UUID.CompareTo(y.UUID));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => y.UUID.CompareTo(x.UUID));
                }
            }
            else if (outputOptions.SortAttribute == "MimeType")
            {
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => x.MimeType.CompareTo(y.MimeType));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => y.MimeType.CompareTo(x.MimeType));
                }
            }
            else if (outputOptions.SortAttribute == "FilenameExtension")
            {
                #pragma warning disable S1449
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => Path.GetExtension(x.Filename).CompareTo(Path.GetExtension(y.Filename)));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => Path.GetExtension(y.Filename).CompareTo(Path.GetExtension(x.Filename)));
                }
                #pragma warning restore S1449
            }
            else
            {
                if (outputOptions.SortDirection == ListSortDirection.Ascending)
                {
                    this.remoteFileMetadata.Sort((x, y) => x.Filename.CompareTo(y.Filename));
                }
                else
                {
                    this.remoteFileMetadata.Sort((x, y) => y.Filename.CompareTo(x.Filename));
                }
            }

            int longestMimeType = this.remoteFileMetadata.Max(s => s.MimeType.Length);

            foreach (var file in this.remoteFileMetadata)
            {
                string categoryCodeID = "O";
                string filename2Display = file.Filename;

                if (file.CategoryCode == "3")
                {
                    categoryCodeID = "H";
                }
                else if (file.CategoryCode == "2")
                {
                    categoryCodeID = "F";
                }

                if (filename2Display.Contains(' '))
                {
                    filename2Display = "'" + filename2Display + "'";
                }

                Console.WriteLine("{0} {1} {2} {3}", categoryCodeID, file.UUID, file.MimeType.PadRight(longestMimeType), filename2Display);
            }
        }

        public void RemoveNotMatchingRegex(string pattern)
        {
            this.remoteFileMetadata = this.remoteFileMetadata.Where(x => Regex.IsMatch(x.Filename, pattern)).ToList();
        }

        public void RemoveNotMatchingWildcard(string pattern)
        {
            // see https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard
            string regexpattern;

            regexpattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";

            this.RemoveNotMatchingRegex(regexpattern);
        }

        public void RemoveEmptyURIs()
        {
            this.remoteFileMetadata = this.remoteFileMetadata.Where(x => x.DownloadURI != null).ToList();
        }
    }
}