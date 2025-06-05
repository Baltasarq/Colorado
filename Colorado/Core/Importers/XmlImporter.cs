// Colorado (c) 2015/19 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Importers
{
    using System.IO;
    using System.Xml;
    using System.Collections.Generic;

    public class XmlImporter : Importer
    {
        public const string Name = "XML";
        const string Extension = "xml";

        /// <summary>Import data from XML.</summary>
        public override CsvDocument Load()
        {
            var rows = new List<List<string>>();
            var headers = new Dictionary<string, int>();
            var docXml = new XmlDocument();

            using (var f = new StreamReader(
                new FileStream(this.Options.Path, FileMode.Open, FileAccess.Read)))
            {
                docXml.Load( f );
            }

            XmlNodeList? nodeList = docXml.DocumentElement?.ChildNodes;

            // For each "row"
            if ( nodeList is not null ) {
                foreach(XmlNode node in nodeList) {
                    rows.Add( new List<string>( headers.Count ) );

                    // For each "column"
                    var childs = node.ChildNodes;
                    for(int j = 0; j < childs.Count; ++j) {
                        var subNode = childs[ j ];
                        string header = subNode?.Name ?? "col";


                        // Take header position
                        if ( !headers.TryGetValue( header, out int col ) ) {
                            headers.Add( header, j );
                            col = j;
                        }

                        // Store value
                        var row = rows[ ^1 ];

                        if ( row.Count <= col ) {
                            row.AddRange( new string[ ( col - row.Count ) + 1 ] );
                        }

                        row[ col ] = subNode?.InnerText ?? "#ERR!";
                    }
                }
            }

            return Dump( headers, rows );
        }

        /// <summary>Gets the name of the importer.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this importer's output.</summary>
        public override string FileExtension => Extension;
    }
}
