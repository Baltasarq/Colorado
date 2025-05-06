// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System.Globalization;
    using System.Collections.ObjectModel;


    public class DecimalMark {
        public enum DecimalSeparator { Point, Comma };
        public static readonly ReadOnlyCollection<char> DecimalSeparatorChar =
            new ReadOnlyCollection<char>( new char[] { '.', ',' } );
        
        public DecimalMark() {
            this.Value = GetFromSystem();
        }

        public char AsChar()
        {
            return AsChar( this.Value );
        }

        public DecimalSeparator Value {
            get; set;
        }

        /// <summary>
        /// Gets the decimal mark from the system settings.
        /// </summary>
        /// <value>The decimal mark, as a char.</value>
        public static DecimalSeparator GetFromSystem() {
            DecimalSeparator toret = DecimalSeparator.Point;

            char separator = CultureInfo.CurrentUICulture.
                NumberFormat.NumberDecimalSeparator[ 0 ];

            if ( separator == ',' ) {
                toret = DecimalSeparator.Comma;
            }

            return toret;
        }

        /// <summary>
        /// Determines if the parameter is a decimal mark.
        /// </summary>
        /// <returns><c>true</c> if the parameter is a decimal mark; otherwise, <c>false</c>.</returns>
        /// <param name="ch">A char possibly containing . or ,</param>
		public static bool IsDecimalMark(char ch) {
			return ( WhichDecimalMark( ch ) >= 0 );
		}

		public static int WhichDecimalMark(char ch) {
			int toret = -1;

			for (int i = 0; i < DecimalSeparatorChar.Count; ++i) {
				if ( DecimalSeparatorChar[ i ] == ch ) {
					toret = i;
					break;
				}
			}

			return toret;
        }

		public static DecimalSeparator WhichDecimalMark(string s) {
			DecimalSeparator toret = DecimalSeparator.Point;

			s = s.Trim();
			for (int i = 0; i < DecimalSeparatorChar.Count; ++i) {
				if ( s.IndexOf( DecimalSeparatorChar[ i ] ) >= 0 ) {
					toret = (DecimalSeparator) i;
					break;
				}
			}

			return toret;
		}

        /// <summary>
        /// Determines if parameter is a number.
        /// </summary>
        /// <returns><c>true</c> if parameter is a number; otherwise, <c>false</c>.</returns>
        /// <param name="s">A string possibly containing a number.</param>
        public static bool IsNumber(string s) {
			bool isReal;

			return ParseNumber( s, out isReal );
		}

		private static bool ParseNumber(string s, out bool isReal) {
            bool toret = false;
            int pos = 0;
            int numMarks = 1;
            int numEs = 1;
			int numDecs = 0;

			isReal = false;
			if ( !string.IsNullOrWhiteSpace( s ) ) {
				toret = true;
				s = s.Trim();

				// Maybe there is a sign before the number
				if ( s[ pos ] == '+'
			      || s[ pos ] == '-' )
				{
					++pos;
				}

				// Maybe the decimal mark is at the beginning
				if ( pos < s.Length
				  && IsDecimalMark( s[ pos ] ) )
				{
					++pos;
					--numMarks;
					isReal = true;
				}

				// Check the remaining positions
				while ( pos < s.Length ) {
					if ( IsDecimalMark( s[ pos ] ) ) {
						--numMarks;
						isReal = true;
					}
					else
					if ( char.ToUpper( s[ pos ] ) == 'E' ) {
						--numEs;

						// Maybe there is a sign after the number
						if ( pos < ( s.Length - 1 ) ) {
							if ( s[ pos + 1 ] == '+'
				  		      || s[ pos + 1 ] == '-' )
							{
								++pos;
							}
						}
					}
					else
					if ( char.IsDigit( s[ pos ] ) ) {
						++numDecs;
					}
					else {
						toret = false;
						break;
					}

					++pos;
				}

				// Do we have correct parts of the number?
				if ( numMarks < 0
	              || numEs < 0
			  	  || numDecs < 1 )
				{
					toret = false;
				}
			}

			// Can't be a real number if it is not a number
			if ( !toret ) {
				isReal = false;
			}

            return toret;
        }

		public static bool IsRealNumber(string s) {
			bool isReal;
			bool isNumber = ParseNumber( s, out isReal );

			return ( isReal && isNumber );
		}

        public static char AsChar(DecimalSeparator value)
        {
            return DecimalSeparatorChar[ (int) value ];
        }
    }
}

