namespace Colorado.Core.Cfg {
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Collections.Generic;

    public class Config {
        /// <summary>The maximum number of recent files.</summary>
        private int MaxNumRecentFiles = 10;
        /// <summary>Tag in the configuration file for the recent files entry.</summary>
        private const string EtqRecent = "recent";
        /// <summary>Tag in the configuration file for the locale of the app.</summary>
        private const string EtqLocale = "locale";
        /// <summary>The name of the confif file.</summary>
        public const string CfgFileName = ".colorado.cfg";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Colorado.Core.Config"/> class.
        /// </summary>
        private Config()
        {
            this.recentFileNames = new HashSet<string>();

            try {
                this.ReadConfiguration();
            } catch(IOException exc) {
                Trace.WriteLine( "Reading cfg: " + exc.Message + exc.StackTrace );
                throw;
            }

            return;
        }

        protected void ReadConfiguration()
        {
            string line;
            StreamReader? file = null;

            // Chk whether the file exists or not.
            try {
                file = new StreamReader( CfgFilePath );
            } catch(IOException) {
                var fileCreate = new StreamWriter( CfgFilePath );
                fileCreate.Close();
                file = new StreamReader( CfgFilePath );

                Trace.WriteLine( "Reading cfg, file not found, created." );
            }

            // Now yes, try to read the config file.
            line = file.ReadLine() ?? "";
            while( !file.EndOfStream ) {
                string[] lineParts = line.Split( '=' );

                if ( lineParts.Length == 2 ) {
                    string etq = lineParts[ 0 ].Trim().ToLower();
                    string arg = lineParts[ 1 ].Trim();

                    if ( etq == EtqLocale ) {
                        Locale.SetLocale( arg );
                    }
                    else
                    if ( etq == EtqRecent ) {
                        string[] readFileNames = arg.Split( '|' );
                        string[] fileNames = readFileNames;
                        var numOfReadFileNames = readFileNames.Length;

                        // Cut the number of files
                        if ( numOfReadFileNames > MaxNumRecentFiles ) {
                            Array.Copy( readFileNames,
                                        numOfReadFileNames - MaxNumRecentFiles,
                                        fileNames,
                                        0,
                                        MaxNumRecentFiles );
                        }

                        this.RecentFiles = fileNames;
                    }
                }

                line = file.ReadLine() ?? "";
            }

            file.Close();
        }

        public void Save()
        {
            // Prepare recent files info for configuration
            string recentFiles = string.Join( "|", this.RecentFiles );

            // Write configuration
            using ( var file = new StreamWriter( CfgFilePath ) ) {
                    file.WriteLine( "{0}={1}", EtqRecent, recentFiles );
                    file.WriteLine( "{0}={1}", EtqLocale, Locale.GetCurrentLocaleCode() );
                    file.WriteLine();
                    file.Close();
            }

            return;
        }

        /// <summary>
        /// Gets the list of recent files.
        /// </summary>
        /// <value>The list of recent files, as an array.</value>
        public string[] RecentFiles {
            get {
                string[] fileNames = new string[ this.recentFileNames.Count ];
                this.recentFileNames.CopyTo( fileNames, 0 );
                return fileNames;
            }
            set {
                foreach(string fileName in value) {
                    this.recentFileNames.Add( fileName );    
                }

                return;
            }
        }

        /// <summary>
        /// Returns a new <see cref="Config"/> object, after reading the
        /// configuration file.
        /// </summary>
        /// <returns>The loaded configuration.</returns>
        public static Config Load()
        {
            return new Config();
        }

        /// <summary>
        /// Returns the path to the config file in the user profile's directory.
        /// </summary>
        /// <value>The complete path.</value>
        public static string CfgFilePath
        {
            get {
                if ( string.IsNullOrWhiteSpace( cfgFilePath ) ) {
                    var usrCfgPath = Environment.GetFolderPath(
                                        Environment.SpecialFolder.UserProfile );
                    cfgFilePath = Path.Combine( usrCfgPath, CfgFileName );
                }

                return cfgFilePath;
            }
        }

        private ISet<string> recentFileNames;

        private static string cfgFilePath = "";
    }
}
