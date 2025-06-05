using System;


namespace Colorado.Core {
	public class AverageFormula : Formula {
		public AverageFormula(CsvDocument doc, Position pos, Position.Direction dir)
			:base( doc, pos, dir )
		{
		}

		public override double DoIt()
		{
			double toret = 0.0;
			double[] data = this.RetrieveData();

			foreach(var x in data) {
				toret += x;
			}

			return ( toret / data.Length );
		}

		public const string Id = "Average";
		public override string Name => Id;
	}
}
