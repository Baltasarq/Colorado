using System;
using System.Collections.ObjectModel;

namespace Colorado.Core {
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
			Name = d;
		}

        /// <summary>
        /// Gets or sets the name of the char used as delimiter.
        /// Most delimiters are used by themselves, except tabs.
        /// </summary>
        /// <value>The name.</value>
		/// <seealso cref="TabDelimiterName"/>
        public string Name {
            get {
                var toret = Raw.ToString();

                if ( toret == "\t" ) {
                    toret = TabDelimiterName;
                }

                return toret;
            }
            set {
                if ( !string.IsNullOrWhiteSpace( value ) ) {
                    if ( value == TabDelimiterName ) {
                        value = "\t";
                    }

                    this.raw = value[ 0 ];
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
			get {
				return this.raw;
			}
		}

		/// <summary>The raw value of the delimiter</summary>
		private char raw;
	}
}

