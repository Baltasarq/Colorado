using System;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Colorado.Core {
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
            return ( ch == ',' || ch == '.' );
        }

        /// <summary>
        /// Determines if parameter is a number.
        /// </summary>
        /// <returns><c>true</c> if parameter is a number; otherwise, <c>false</c>.</returns>
        /// <param name="s">A string possibly containing a number.</param>
        public static bool IsNumber(string s) {
            bool toret = true;
            int pos = 0;
            int numMarks = 1;
            int numEs = 1;

            // Maybe there is a sign before the number
            if ( s[ pos ] == '+'
                || s[ pos ] == '-' )
            {
                ++pos;
            }

            // Maybe the decimal mark is at the beginning
            if ( IsDecimalMark( s[ pos ] ) ) {
                ++pos;
                --numMarks;
            }

            // Check the remaining positions
            while( pos < s.Length ) {
                if ( IsDecimalMark( s[ pos ] ) ) {
                    --numMarks;
                }
                else
                    if ( char.ToUpper( s[ pos ] ) == 'E' ) {
                        --numEs;
                    }
                    else
                        if ( !char.IsDigit( s[ pos ] ) ) {
                            toret = false;
                            break;
                        }

                ++pos;
            }

            // More than one separator oe 'e'?
            if ( numMarks < 0
                || numEs < 0 )
            {
                toret = false;
            }

            return toret;
        }

        public static char AsChar(DecimalSeparator value)
        {
            return DecimalSeparatorChar[ (int) value ];
        }
    }
}

