// Colorado (c) 2015/19 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Importers
{
    using System.IO;
    using System.Json;
    using System.Collections.Generic;

    public class JsonImporter : Importer
    {
        public const string Name = "JSON";
        const string Extension = "json";

        /// <summary>Import data from JSON.</summary>
        public override CsvDocument Load()
        {
            CsvDocument toret = null;

            using (var f = new StreamReader(
                new FileStream(this.Options.Path, FileMode.Open, FileAccess.Read)))
            {
                var array = this.LocateArray( JsonValue.Load( f ) );

                if ( array != null ) {
                    toret = this.LoadFromJson( array );
                } else {
                    throw new FileLoadException( "no array found in json" );
                }
            }

            return toret;
        }

        JsonArray LocateArray(JsonValue jsonValue)
        {
            JsonArray toret = null;

            if( jsonValue != null
               && !( jsonValue is JsonArray ) )
            {
                if ( jsonValue is JsonObject jobj ) {
                    foreach(KeyValuePair<string, JsonValue> values in jobj) {
                        toret = this.LocateArray( values.Value );

                        if ( toret != null ) {
                            break;
                        }
                    }
                }
            } else {
                toret = (JsonArray) jsonValue;
            }

            return toret;
        }

        CsvDocument LoadFromJson(JsonArray array)
        {
            var rows = new List<List<string>>();
            var headers = new Dictionary<string, int>();
            var headerTitles = new List<string>();
            var values = new List<JsonValue>();

            // For each "row"
            foreach(JsonValue jvalue in array) {
                rows.Add( new List<string>( headers.Count ) );

                if ( jvalue is JsonObject jobj ) {
                    // For each "column"
                    headerTitles.AddRange( jobj.Keys );
                    values.AddRange( jobj.Values );

                    for(int j = 0; j < headerTitles.Count; ++j) {
                        int col;

                        // Take header position
                        if ( !headers.TryGetValue( headerTitles[ j ], out col ) )
                        {
                            headers.Add( headerTitles[ j ], j );
                            col = j;
                        }

                        // Store value
                        var row = rows[ rows.Count - 1 ];

                        if ( row.Count <= col ) {
                            row.AddRange( new string[ ( col - row.Count ) + 1 ] );
                        }

                        row[ col ] = RemoveQuotes( values[ j ].ToString() );
                    }

                    // Clean up for next row
                    headerTitles.Clear();
                    values.Clear();
                }
            }

            return this.Dump( headers, rows );
        }

        /// <summary>Removes the quotes, if existing.</summary>
        /// <param name="value">A string value.</param>
        /// <returns>The given string, without quotes.</returns>
        static string RemoveQuotes(string value)
        {
            string toret = value;

            if ( toret.Length > 0 ) {
                if ( value[ 0 ] == '"' ) {
                    toret = toret.Substring( 1 );
                }


                if ( value[ value.Length - 1 ] == '"' ) {
                    toret = toret.Substring( 0, toret.Length - 1 );
                }
            }

            return toret;
        }

        /// <summary>Gets the name of the importer.</summary>
        public override string Id => Name;

        /// <summary>Gets the extension for this importer's output.</summary>
        public override string FileExtension => Extension;
    }
}
