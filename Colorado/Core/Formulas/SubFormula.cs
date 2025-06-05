using System;
using System.Data.Common;
using Colorado.Core;

namespace Colorado
{
	class SubFormula: Formula {
		public SubFormula(CsvDocument doc, Position pos, Position.Direction dir) : base( doc, pos, dir )
		{
		}

		public override double DoIt()
		{
			double toret = 0.0;
			double[] data = this.RetrieveData();

			if ( data.Length > 0 ) {
				toret = data[ 0 ];

				for(int i = 1; i < data.Length; ++i) {
					toret -= data[ i ];
				}
			}

			return Math.Abs( toret );
		}

		public override string Name => Id;

		public const string Id = "Substract";
	}
}
