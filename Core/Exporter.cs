﻿// Colorado (c) 2015/18 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System;
    using System.Reflection;
    using System.Collections.Generic;


    /// <summary>Base class for all exporters.</summary>
    public abstract class Exporter {
        /// <summary>Save this instance.</summary>
        public abstract void Save();

        /// <summary>
        /// Gets the export options.
        /// </summary>
        /// <value>The <see cref="ExportOptions"/>.</value>
        public ExportOptions Options {
            get; set;
        }

        /// <summary>Gets the name of the exporter.</summary>
        public abstract string Id {
            get;
        }

        /// <summary>Gets the extension for this exporter's output.</summary>
        public abstract string FileExtension {
            get;
        }

        static void InitDictionary()
        {
            if ( exporters == null ) {
                Type exporterType = typeof( Exporter );
                exporters = new Dictionary<string, Exporter>();

                IEnumerable<Type> types = Assembly.GetAssembly( exporterType ).GetTypes();

                foreach (Type type in types) {
                    if ( type.IsClass
                        && !type.IsAbstract
                        && type.IsSubclassOf( exporterType ) )
                    {
                        var name = (string) type.GetField( "Name" ).GetValue( null );
                        exporters.Add( name, (Exporter) Activator.CreateInstance( type ) );
                    }
                }
            }

            return;
        }

        /// <summary>
        /// Gets the exporter, given its id.
        /// </summary>
        /// <returns>The corresponding <see cref="Exporter"/>.</returns>
        /// <param name="id">The string identifier.</param>
        public static Exporter GetExporter(string id)
        {
            InitDictionary();
            return exporters[ id ];
        }

        /// <summary>
        /// Gets all exporters.
        /// </summary>
        /// <returns>All the available <see cref="Exporter"/> objects.</returns>
        public static Exporter[] GetAllExporters()
        {
            if ( allExporters == null ) {
                InitDictionary();

                allExporters = new Exporter[ exporters.Count ];
                exporters.Values.CopyTo( allExporters, 0 );
            }

            return allExporters;
        }

        /// <summary>
        /// Saves or exports the document honoring
        /// the Format attribute in the <see cref="ExportOptions"/> object.
        /// </summary>
        /// <param name="options">The <see cref="ExportOptions"/> settings.</param>
        public static void Save(ExportOptions options)
        {
            Exporter exporter = Exporter.GetExporter( options.ExporterId );

            exporter.Options = options;
            exporter.Save();

            return;
        }


        static Dictionary<string, Exporter> exporters;
        static Exporter[] allExporters;
    }
}
