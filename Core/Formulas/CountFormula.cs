// Nombre: Count.cs
// Fecha:  2011-01-17
// Autor:  baltasarq@gmail.com


using System;

namespace Colorado.Core {
	
	public class CountFormula : Formula  {
		public const string Id = "Count";
		
		public CountFormula(CsvDocument doc, Position pos, Position.Direction dir) : base( doc, pos, dir )
		{
		}
		
		public CountFormula()
		{
		}
		
		public override double DoIt()
		{
			return this.RetrieveData().Length;
		}
		
		public override string Name {
			get { return Id; }
		}

	}

}
