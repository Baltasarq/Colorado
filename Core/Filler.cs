using System;

namespace Colorado.Core {
    using Fillers;

    public abstract class Filler {
        public enum FillType { Fixed, Increment, Decrement };
        public enum FillDirection { Column, Row };

        public static Filler CreateFiller(CsvDocument doc, Position pos, int n, string initValue, FillType ft, FillDirection fd)
        {
            // Set init value
            initValue = initValue.Trim();

            if ( initValue.Length == 0 ) {
                initValue = "1";
            }

            // Fixed filler by default
            Filler toret = new FixedFiller( doc, pos, n, initValue, FillType.Fixed, fd );

            if ( ft != FillType.Fixed ) {
                // Is there a digit at the end?
                if ( char.IsDigit( initValue[ initValue.Length - 1 ] ) ) {
                    toret = new NumberFiller( doc, pos, n, initValue, ft, fd, NumberFiller.NumberPosition.Postfix );
                }
                else
                // Is there a digit at the beginning?
                if ( char.IsDigit( initValue[ 0 ] ) ) {
                    toret = new NumberFiller( doc, pos, n, initValue, ft, fd, NumberFiller.NumberPosition.Prefix );
                }

                // No digit at all: default fixed filler
            }

            return toret;
        }

        /// <summary>
        /// A new <see cref="Colorado.Core.Filler"/> class.
        /// This is used in order to fill a row or a column from a start position.
        /// </summary>
        /// <param name="doc">The document in which to fill.</param>
        /// <param name="r">Starting row</param>
        /// <param name="c">Starting column</param>
        /// <param name="c">Number of cells to affect.</param>
        /// <param name="ft">The type of filling (fixed, increment, decrement...).</param>
        /// <param name="fd">Direction of filling (row, column...).</param>
        protected Filler(CsvDocument doc, Position pos, int n, string initValue, FillType ft, FillDirection fd)
        {
            this.doc = doc;
            this.value = initValue.Trim();
            this.type = ft;
            this.direction = fd;
            this.position = pos;
            this.numCells = n;

            // Empty value?
            if ( this.value.Length == 0 ) {
                this.value = "1";
            }

            // The number of cells to affect
            if ( numCells < 0 ) {
                if ( this.Direction == FillDirection.Column ) {
                    this.numCells = ( this.Document.Data.NumRows - this.Row ) - 1;
                }
                else {
                    this.numCells = ( this.Document.Data.NumColumns - this.Column ) - 1;
                }
            }

            // Prepare and set the first value
            this.CalculateInternalDirection();
            this.Document.Data[ this.CellPosition.Row, this.CellPosition.Column ] = this.Value;
            return;
        }

        public void DoIt()
        {
            if ( this.numCells > 0 ) {
                this.numCells -= 1;
                this.NextValue();
                this.position = this.position.Inc( this.internalDir );
                this.Document.Data[ this.CellPosition.Row, this.CellPosition.Column ] = this.value;
            }
        }

        /// <summary>
        /// Calculates the internal direction, fromm FillDirection
        /// </summary>
        /// <returns>The new internal direction, as a Position.Direction object.</returns>
        private Position.Direction CalculateInternalDirection()
        {
            Position.Direction toret = Position.Direction.Down;

            if ( this.Direction == FillDirection.Row ) {
                toret = Position.Direction.Right;
            }

            this.internalDir = toret;
            return toret;
        }

        /// <summary>
        /// Calculates the next value.
        /// </summary>
        /// <returns>The calculated value, as a string.</returns>
        protected abstract string NextValue();

        public FillType Type {
            get { return this.type; }
        }

        public FillDirection Direction {
            get { return this.direction; }
        }

        public Position CellPosition {
            get { return this.position; }
        }

        public int Row {
            get { return this.CellPosition.Row; }
        }

        public int Column {
            get { return this.CellPosition.Column; }
        }

        public int NumCells {
            get { return this.numCells; }
        }

        public string Value {
            get { return this.value; }
        }

        public CsvDocument Document {
            get { return this.doc; }
        }

        protected string value;
        private CsvDocument doc;
        private FillDirection direction;
        private FillType type;
        private Position position;
        private int numCells;
        private Position.Direction internalDir;
    }
}

