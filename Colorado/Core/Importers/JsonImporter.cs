// Colorado (c) 2015/19 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Core.Importers;


using System.IO;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

public class JsonImporter : Importer
{
    public const string Name = "JSON";
    const string Extension = "json";

    /// <summary>Import data from JSON.</summary>
    public override CsvDocument Load()
    {
        CsvDocument? toret = null;
        JsonDocument jsonDoc = JsonDocument.Parse(
                                        File.ReadAllText(
                                            this.Options.Path ) );

        var array = LocateArray( JsonArray.Create( jsonDoc.RootElement ) );

        if ( array != null ) {
            toret = this.LoadFromJson( array );
        } else {
            throw new FileLoadException( "no array found in json" );
        }

        return toret;
    }

    static JsonArray? LocateArray(JsonNode? jsonNode)
    {
        JsonArray? toret = null;

        if ( jsonNode is not null ) {
            if ( jsonNode is not JsonArray ) {
                if ( jsonNode is JsonObject jobj ) {
                    foreach(KeyValuePair<string, JsonNode?> values in jobj) {
                        if ( values.Value is not null ) {
                            toret = LocateArray( values.Value );

                            if ( toret != null ) {
                                break;
                            }
                        }
                    }
                }
            } else {
                toret = jsonNode.AsArray();
            }
        }

        return toret;
    }

    CsvDocument LoadFromJson(JsonArray array) {
        var rows = new List<List<string>>();
        var headers = new Dictionary<string, int>();
        var headerTitles = new List<string>();
        var values = new List<string>();

        // For each "row"
        foreach(JsonNode? jvalue in array) {
            rows.Add( new List<string>( headers.Count ) );

            if ( jvalue is JsonObject jobj ) {
                // For each "column"
                IList<KeyValuePair<string, JsonNode?>> dictJobj = jobj.ToList();
                headerTitles.AddRange( dictJobj.Select( p => p.Key ) );
                values.AddRange( dictJobj.Select( p => ( p.Value ?? "" ).ToString() ) );

                for(int j = 0; j < headerTitles.Count; ++j) {
                    int col;

                    // Take header position
                    if ( !headers.TryGetValue( headerTitles[ j ], out col ) ) {
                        headers.Add( headerTitles[ j ], j );
                        col = j;
                    }

                    // Store value
                    var row = rows[ rows.Count - 1 ];

                    if ( row.Count <= col ) {
                        row.AddRange( new string[ ( col - row.Count ) + 1 ] );
                    }

                    row[ col ] = RemoveQuotes( values[ j ] );
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
                toret = toret[ 1.. ];
            }


            if ( value[ ^1 ] == '"' ) {
                toret = toret[ ..^1 ];
            }
        }

        return toret;
    }

    /// <summary>Gets the name of the importer.</summary>
    public override string Id => Name;

    /// <summary>Gets the extension for this importer's output.</summary>
    public override string FileExtension => Extension;
}
