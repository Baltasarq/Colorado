using System;

using Colorado.Core;

namespace Colorado
{
	public class MaxFormula: Formula
	{
		public MaxFormula(CsvDocument doc, Position pos, Position.Direction dir): base( doc, pos, dir )
		{
		}
		
		public MaxFormula()
		{
		}
		
		public override double DoIt()
		{
			double toret = 0.0;
			double[] data = this.RetrieveData();
			
			if ( data.Length > 0 ) {
				toret = data[ 0 ];
				
				for(int i = 1; i < data.Length; ++i) {
					if ( toret < data[ i ] ) {
						toret = data[ i ];
					}
				}
			}
			
			return toret;
		}
		
		public const string Id = "Max";
		public override string Name {
			get { return Id; }
		}
	}
}

