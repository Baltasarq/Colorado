// Colorado (c) 2015/19 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System;
    using System.Reflection;
    using System.Collections.Generic;

    /// <summary>Base class for all importers.</summary>
    public abstract class Importer {
        /// <summary>Gets the name of the importer.</summary>
        public abstract string Id {
            get;
        }

        /// <summary>Gets the extension for this importer's output.</summary>
        public abstract string FileExtension {
            get;
        }

        /// <summary>Gets the extension for this importer's output.</summary>
        public ImportOptions Options {
            get; set;
        }

        /// <summary>Loads a file into a CSV document.</summary>
        /// <returns>The <see cref="Core.CsvDocument"/>.</returns>
        public abstract CsvDocument Load();

        /// <summary>
        /// Creates a new document from the given headers and rows.
        /// </summary>
        /// <returns>A new <see cref="Core.CsvDocument"/>.</returns>
        /// <param name="headers">
        ///     The headers, in a dictionary in which the key is the title
        ///     of the header, and the value its position.
        /// </param>
        /// <param name="rows">
        ///     The rows of values, as a list of lists.
        ///     The external list are the rows, while the internal lists are
        ///     the columns.
        /// </param>
        protected CsvDocument Dump(Dictionary<string, int> headers, List<List<string>> rows)
        {
            // Create the CSV document
            var toret = new CsvDocument( rows.Count, headers.Count );

            // Set the headers
            var sortedHeaders = new List<KeyValuePair<string, int>>( headers );
            sortedHeaders.Sort( (pair1, pair2) => pair1.Value.CompareTo( pair2.Value ) );

            toret.Data.Headers = sortedHeaders.ConvertAll<string>( pair => pair.Key ).ToArray();

            // Load the data
            for(int i = 0; i < rows.Count; ++i) {
                var row = rows[ i ];

                for(int j = 0; j < row.Count; ++j) {
                    toret.Data[ i, j ] = row[ j ];
                }
            }

            return toret;
        }

        static void InitDictionary()
        {
            if ( importers == null ) {
                Type importerType = typeof( Importer );
                importers = new Dictionary<string, Importer>();

                IEnumerable<Type> types = Assembly.GetAssembly( importerType ).GetTypes();

                foreach (Type type in types) {
                    if ( type.IsClass
                        && !type.IsAbstract
                        && type.IsSubclassOf( importerType ) )
                    {
                        var name = (string) type.GetField( "Name" ).GetValue( null );
                        importers.Add( name, (Importer) Activator.CreateInstance( type ) );
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Gets the importer, given its id.
        /// </summary>
        /// <returns>The corresponding <see cref="Importer"/>.</returns>
        /// <param name="id">The string identifier.</param>
        public static Importer GetImporter(string id)
        {
            InitDictionary();
            return importers[ id ];
        }

        /// <summary>
        /// Gets all importers.
        /// </summary>
        /// <returns>All the available <see cref="Importer"/> objects.</returns>
        public static Importer[] GetAllImporters()
        {
            if ( allImporters == null ) {
                InitDictionary();

                allImporters = new Importer[ importers.Count ];
                importers.Values.CopyTo( allImporters, 0 );
            }

            return allImporters;
        }

        /// <summary>
        /// Loads the document honoring the given id.
        /// </summary>
        /// <param name="importOptions">A <see cref="ImportOptions"/> with the configuration.</param>
        public static CsvDocument Load(ImportOptions importOptions)
        {
            Importer importer = GetImporter( importOptions.ImportId );

            importer.Options = importOptions;
            return importer.Load();
        }

        static Dictionary<string, Importer> importers;
        static Importer[] allImporters;
    }
}
