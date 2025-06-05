using System;

namespace Colorado.Core {
	public class SumFormula : Formula  {

		public SumFormula(CsvDocument doc, Position pos, Position.Direction dir)
			: base( doc, pos, dir )
		{
		}

		public override double DoIt()
		{
			double toret = 0;
			double[] data = this.RetrieveData();

			foreach(var x in data) {
				toret += x;
			}

			return toret;
		}

		public override string Name {
			get { return Id; }
		}

		public const string Id = "Sum";
	}
}
