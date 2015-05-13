// Nombre: Count.cs
// Fecha:  2011-01-17
// Autor:  baltasarq@gmail.com

using System;

using Colorado.Core;

namespace Colorado.Core {
	
	public class CountNonBlankFormula : Formula  {
		public const string Id = "CountNonBlank";
		
		public CountNonBlankFormula(CsvDocument doc, Position pos, Position.Direction dir) : base( doc, pos, dir )
		{
		}
		
		public CountNonBlankFormula()
		{
		}
		
		public override double DoIt()
		{
			return this.CountNonBlankData();
		}
		
		protected int CountNonBlankData()
		{
			int toret = 0;
			var pos = this.Position.Inc( this.Direction );
			
			// Retrieve all data
			while( pos.IsValid() ) {
				if ( this.Document.Data[ pos.Row ][ pos.Column ] != "" ) {
					++toret;
				}

				pos = pos.Inc( this.Direction );
			}
			
			return toret;
		}
		
		public override string Name {
			get { return Id; }
		}

	}

}

