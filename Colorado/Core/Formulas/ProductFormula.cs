using System;
using Colorado.Core;

namespace Colorado
{
	public class ProductFormula: Formula
	{
		public ProductFormula(CsvDocument doc, Position pos, Position.Direction dir)
			: base( doc, pos, dir )
		{
		}

		public override double DoIt()
		{
			double toret = 0;
			double[] data = this.RetrieveData();

			if ( data.Length > 0 ) {
				toret = 1;

				foreach(var x in data) {
					toret *= x;
				}
			}

			return toret;
		}

		public override string Name => Id;

		public const string Id = "Product";
	}
}
