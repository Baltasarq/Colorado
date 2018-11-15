// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System.Collections.ObjectModel;

	public class Delimiter {
        public const string TabDelimiterName = "<TAB>";
        public const char CommaDelimiter = ',';
        public const char SemicolonDelimiter = ';';
        public const char ColonDelimiter = ':';
        public const char TabDelimiter = '\t';
        public static readonly ReadOnlyCollection<char> PredefinedDelimiters =
            new ReadOnlyCollection<char>(
                new char[]{ TabDelimiter, ColonDelimiter, SemicolonDelimiter, CommaDelimiter  }
            );
		public static readonly ReadOnlyCollection<string> PredefinedDelimiterNames =
			new ReadOnlyCollection<string>(
                new string[]{ TabDelimiterName,
                              ColonDelimiter.ToString(),
                              SemicolonDelimiter.ToString(),
                              CommaDelimiter.ToString() }
			);

		/// <summary>
		/// Initializes a new instance of the <see cref="Colorado.Delimiter"/> class.
		/// </summary>
		/// <param name="c">The delimiter to use, as char.</param>
		public Delimiter(char c)
			: this( c.ToString() )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Colorado.Delimiter"/> class.
		/// </summary>
		/// <param name="d">The delimiter to use, as a string (can be special).</param>
		public Delimiter(string d)
		{
			this.Name = d;
		}

        /// <summary>
        /// Gets or sets the name of the char used as delimiter.
        /// Most delimiters are used by themselves, except tabs.
        /// </summary>
        /// <value>The name.</value>
		/// <seealso cref="TabDelimiterName"/>
        public string Name {
            get {
                return GetName( this.Raw );
            }
            set {
                if ( !string.IsNullOrEmpty( value ) ) {
                    if ( value == TabDelimiterName ) {
                        value = "\t";
                    }

                    this.Raw = value[ 0 ];
                }

                return;
            }
        }
                    
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current
        /// <see cref="Colorado.Delimiter"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/>
        /// that represents the current <see cref="Colorado.Delimiter"/>.</returns>
		public override string ToString()
		{
			return Raw.ToString();
		}

		/// <summary>
		/// Gets the raw char used as delimiter
		/// </summary>
		/// <value>The raw.</value>
		public char Raw {
            get; private set;
		}

        public static string GetName(string delimiter) {
            var toret = TabDelimiterName;

            if ( !string.IsNullOrWhiteSpace( delimiter ) ) {
                toret = GetName( delimiter[ 0 ] );
            }

            return toret;
        }

        public static string GetName(char delimiter) {
            var toret = delimiter.ToString();

            if ( toret == "\t" ) {
                toret = TabDelimiterName;
            }

            return toret;
        }
	}
}

