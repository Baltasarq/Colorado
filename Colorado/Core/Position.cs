// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Core {
    using System;

	public class Position : IComparable<Position> {
		public enum Direction { Left, Up, Right, Down };

        /// <summary>
        /// Creates a new position at 0,0
        /// <param name="doc">
        /// A <see cref="CsvDocument"/> holding the document to be used as reference.
        /// </param>
        /// </summary>
        public Position(CsvDocument doc)
        {
            this.doc = doc;
            this.Row = this.Column = 0;
        }

        /// <summary>
        /// Creates a new position equals to the value of the given position
        /// </summary>
        /// <param name="p">
        /// A <see cref="Position"/> holding the reference value.
        /// </param>
        public Position(Position p)
        {
            this.doc = p.Document;
            this.Row = p.Row;
            this.Column = p.Column;
        }

        /// <summary>
        /// Creates a new position, given a row and a column reference. 
        /// </summary>
        /// <param name="doc">
        /// A <see cref="CsvDocument"/> holding the document to be used as reference.
        /// </param>
        /// <param name="row">
        /// A <see cref="System.Int32"/> holding the reference of the row (0-based).
        /// </param>
        /// <param name="column">
        /// A <see cref="System.Int32"/> holding the reference of the column (0-based).
        /// </param>
        public Position(CsvDocument doc, int row, int column)
        {
            this.doc = doc;
            this.Row = row;
            this.Column = column;
        }

		/// <summary>/// The document this position pertains to.</summary>
		public CsvDocument Document {
			get { return this.doc; }
		}

		/// <summary>The column number</summary>
		public int X {
			get { return this.Column; }
			set { this.Column = value; }
		}

		/// <summary>The column number</summary>
		public int Column {
			get { return this.x; }
			set { this.x = value; }
		}

		/// <summary>The row number</summary>
		public int Y {
			get { return this.Row; }
			set { this.Row = value; }
		}

		/// <summary>The row number</summary>
		public int Row {
			get { return this.y; }
			set { this.y = value; }
		}

		/// <summary>Increment the position represented by this object</summary>
		/// <param name="dir">
		/// A <see cref="Direction"/> holding the direction in which the object should be incremented.
		/// </param>
		/// <returns>
		/// A <see cref="Position"/> object holding the new position (this object is not modified)
		/// </returns>
		public Position Inc(Direction dir)
		{
			return this.Inc( dir, 1 );
		}

		/// <summary>
		/// Increment the position represented by this object
		/// </summary>
		/// <param name="dir">
		/// A <see cref="Direction"/> holding the direction in which the object should be incremented.
		/// </param>
		/// <param name="delta">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="Position"/> object holding the new position (this object is not modified)
		/// </returns>
		public Position Inc(Direction dir, int delta)
		{
			var toret = new Position( this );

			if ( dir == Direction.Left ) {
				toret.Column -= delta;
			}
			else
			if ( dir == Direction.Right ) {
				toret.Column += delta;
			}
			else
			if ( dir == Direction.Up ) {
				toret.Row -= delta;
			}
			else
			if ( dir == Direction.Down ) {
				toret.Row += delta;
			}

			return toret;
		}

		/// <summary>
		/// Decides whether the position represented by this object
		/// is valid for the document.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/> holding true if this position is valid; false otherwise.
		/// </returns>
		public bool IsValid()
		{
			return ( this.Row >= 0
			      && this.Column >= 0
			      && this.Row < doc.Data.NumRows
			      && this.Column < doc.Data.NumColumns );
		}

		/// <summary>Returns the distance to another position, for comparison.</summary>
		/// <param name="p">
		/// A <see cref="Position"/> that is going to be compared with this one.
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/> holding negative number if this is less than p,
		/// 0 if they are equal, or positive number if is greater than p.
		/// </returns>
		public int CompareTo(Position? p)
		{
            int toret = 1;

            if ( p is not null ) {
                toret = 0;

                if ( this.Row == p.Row )
                {
                    toret = this.Column - p.Column;
                }
                else {
                    toret = ( ( this.Row - p.Row ) * this.Document.Data.NumColumns )
                            + ( this.Column - p.Column );
                }
            }

			return toret;
		}

		public override int GetHashCode()
		{
			return ( this.Row  * 11 ) + this.Column;
		}

		public override bool Equals(object? obj)
		{
			bool toret = false;

			if ( obj is Position pos) {
				toret = ( this.CompareTo( pos ) == 0 );
			}

			return toret;
		}

		public static bool operator < (Position x, Position y)
		{
			return ( x.CompareTo( y ) < 0 );
		}

		public static bool operator > (Position x, Position y)
		{
			return ( x.CompareTo( y ) > 0 );
		}

		public static bool operator == (Position x, Position y)
		{
			return ( x.CompareTo( y ) == 0 );
		}

		public static bool operator != (Position x, Position y)
		{
			return ( x.CompareTo( y ) != 0 );
		}


        private int x;
        private int y;
        private readonly CsvDocument doc;
	}

}

