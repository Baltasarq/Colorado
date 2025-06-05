using System;

namespace Colorado.Core.Fillers {
    public class NumberFiller: Filler {
        public enum NumberPosition { Prefix, Postfix };

        public NumberFiller(CsvDocument doc, Position pos, int n, string initValue, FillType ft, FillDirection fd, NumberPosition npos)
            :base( doc, pos, n, initValue, ft, fd )
        {
            this.NumPosition = npos;
            this.Delta = 1;

            if ( this.Type == FillType.Decrement ) {
                this.Delta = -1;
            }

            ( this.StrValue, this.NumValue ) = this.DivideValue();
        }

        /// <summary>
        /// Gets the string of the value, and divides it in the numeric and
        /// alpha part.
        /// </summary>
        protected (string, int) DivideValue()
        {
            string strNum = "";
            int pos = 0;
            int step = 1;
            int length = this.Value.Length;
            int end = length;
            char ch = '\0';

            // Determine step
            if ( this.NumPosition == NumberPosition.Postfix ) {
                step = -1;
                pos = length - 1;
                end = -1;
            }

            // Determine numeric part
			ch = this.Value[ pos ];
            do {
                if ( this.NumPosition == NumberPosition.Prefix ) {
                    strNum += ch;
                } else {
                    strNum = ch + strNum;
                }

                pos += step;

				if ( pos != end ) {
					ch = this.Value[ pos ];
				}
            } while ( pos != end
                   && char.IsDigit( ch ) );

            // Determine alpha part
            if ( this.NumPosition == NumberPosition.Prefix ) {
				this.StrValue = this.Value.Substring( pos );
            } else {
                this.StrValue = this.Value.Substring( 0, pos + 1 );
            }

            // End
            this.NumValue = int.Parse( strNum );
            return ( this.StrValue, this.NumValue );
        }

        /// <summary>
        /// Builds the new value, calculating the number,
        /// and concatenating StrValue and NumValue, by honoring Position.
        /// </summary>
        /// <returns>The new value, as a string.</returns>
        protected override string NextValue()
        {
            this.NumValue += this.Delta;
            string strNumValue = this.NumValue.ToString();

            // Concat
            if ( this.NumPosition == NumberPosition.Prefix ) {
                this.value = strNumValue + this.StrValue;
            } else {
                this.value = this.StrValue + strNumValue;
            }

            return this.Value;
        }

        /// <summary>
        /// Gets or sets the numeric part of the value.
        /// </summary>
        /// <value>The numeric part of the value, as an int.</value>
        public int NumValue {
            get; set;
        }

        /// <summary>
        /// Gets or sets the alpha part of the value.
        /// </summary>
        /// <value>The alpha part of the value, as a string.</value>
        public string StrValue {
            get; set;
        }

        /// <summary>
        /// Gets the position of the numeric part of the value.
        /// </summary>
        /// <value>The position, as a NumberPosition.</value>
        public NumberPosition NumPosition {
            get; set;
        }

        public int Delta {
            get; set;
        }
    }
}

