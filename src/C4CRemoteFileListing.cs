using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections;

namespace CloudCopy
{
    class C4CRemoteFileListing : IRemoteFileListing<C4CRemoteFileMetadata>
    {
        List<C4CRemoteFileMetadata> _RemoteFileMetadata = new List<C4CRemoteFileMetadata>();

        public C4CRemoteFileListing(string sourceXML)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXML);

            XmlNamespaceManager mgr = new XmlNamespaceManager(xmlDoc.NameTable);
            mgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            mgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            mgr.AddNamespace(String.Empty,"http://www.w3.org/2005/Atom");
            mgr.AddNamespace("default","http://www.w3.org/2005/Atom");

            XmlElement root = xmlDoc.DocumentElement;

            XmlNodeList xmlNodes = xmlDoc.SelectNodes("//default:entry",mgr);

            foreach( XmlNode ElementNode in xmlNodes)
            {
                C4CRemoteFileMetadata CurrentFile = new C4CRemoteFileMetadata();

                XmlNode ObjectIDNode = ElementNode.SelectSingleNode(".//m:properties/d:Name",mgr);
                CurrentFile.Filename = ObjectIDNode.InnerText;

                ObjectIDNode = ElementNode.SelectSingleNode(".//m:properties/d:UUID",mgr);
                CurrentFile.UUID = ObjectIDNode.InnerText;

                ObjectIDNode = ElementNode.SelectSingleNode(".//m:properties/d:MimeType",mgr);
                CurrentFile.MimeType = ObjectIDNode.InnerText;

                ObjectIDNode = ElementNode.SelectSingleNode(".//m:properties/d:CategoryCode",mgr);
                CurrentFile.CategoryCode = ObjectIDNode.InnerText;

                if ( CurrentFile.CategoryCode == "2")
                {
                    ObjectIDNode = ElementNode.SelectSingleNode(".//m:properties/d:DocumentLink",mgr);
                    CurrentFile.DownloadURI = new Uri( ObjectIDNode.InnerText );
                }

                ObjectIDNode = ElementNode.SelectSingleNode(".//default:id",mgr);
                CurrentFile.MetadataURI = new Uri( ObjectIDNode.InnerText );

                _RemoteFileMetadata.Add(CurrentFile);
            }
        }

        public IEnumerator<C4CRemoteFileMetadata> GetEnumerator()
        {
            for ( int i = 0; i < _RemoteFileMetadata.Count; i++ )
            {
                yield return _RemoteFileMetadata[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void listFiles()
        {
            listFiles(new OutputOptions());
        }

        public void listFiles(IOutputOptions outputOptions)
        {
            if ( _RemoteFileMetadata.Count == 0 )
            {
                return;
            }

            if ( outputOptions.sortAttribute == "UUID" )
            {
                if ( outputOptions.sortDirection == ListSortDirection.Ascending )
                {
                    _RemoteFileMetadata.Sort((x,y) => x.UUID.CompareTo(y.UUID));
                }
                else
                {
                    _RemoteFileMetadata.Sort((x,y) => y.UUID.CompareTo(x.UUID));
                }
            }
            else if ( outputOptions.sortAttribute == "MimeType" )
            {
                if ( outputOptions.sortDirection == ListSortDirection.Ascending )
                {
                    _RemoteFileMetadata.Sort((x,y) => x.MimeType.CompareTo(y.MimeType));
                }
                else
                {
                    _RemoteFileMetadata.Sort((x,y) => y.MimeType.CompareTo(x.MimeType));
                }
            }
            else if ( outputOptions.sortAttribute == "FilenameExtension" )
            {
                if ( outputOptions.sortDirection == ListSortDirection.Ascending )
                {
                    _RemoteFileMetadata.Sort((x,y) => Path.GetExtension(x.Filename).CompareTo(Path.GetExtension(y.Filename)));
                }
                else
                {
                    _RemoteFileMetadata.Sort((x,y) => Path.GetExtension(y.Filename).CompareTo(Path.GetExtension(x.Filename)));
                }
            }
            else
            {
                if ( outputOptions.sortDirection == ListSortDirection.Ascending )
                {
                    _RemoteFileMetadata.Sort((x,y) => x.Filename.CompareTo(y.Filename));
                }
                else
                {
                    _RemoteFileMetadata.Sort((x,y) => y.Filename.CompareTo(x.Filename));
                }
            }

            int longestMimeType = _RemoteFileMetadata.Max( s => s.MimeType.Length );

            foreach(var File in _RemoteFileMetadata)
            {
                string CategoryCodeID = "O";
                string Filename2Display = File.Filename;

                if ( File.CategoryCode == "3" )
                {
                    CategoryCodeID = "H";
                }
                else if ( File.CategoryCode == "2" )
                {
                    CategoryCodeID = "F";
                }

                if ( Filename2Display.Contains(' ') )
                {
                    Filename2Display = "'" + Filename2Display + "'";
                }

                Console.WriteLine("{0} {1} {2} {3}",CategoryCodeID,File.UUID,File.MimeType.PadRight(longestMimeType),Filename2Display);
            }


        }

        public void removeNotMatchingRegex(string pattern)
        {
            _RemoteFileMetadata = _RemoteFileMetadata.Where( x => Regex.IsMatch(x.Filename,pattern) ).ToList();
        }

        public void removeNotMatchingWildcard(string pattern)
        {
            //see https://stackoverflow.com/questions/30299671/matching-strings-with-wildcard

            string regexpattern;

            regexpattern = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$"; 

            removeNotMatchingRegex(regexpattern);
        }


    }
}