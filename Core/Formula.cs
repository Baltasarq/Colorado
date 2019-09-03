// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace Colorado.Core {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

	
	public abstract class Formula {
		private CsvDocument csvDocument = null;
		private Position.Direction direction;

        protected Formula()
        {
            this.csvDocument = null;
            this.position = null;
            this.direction = Position.Direction.Left;
        }

        protected Formula(CsvDocument doc, Position pos, Position.Direction dir)
        {
            this.csvDocument = doc;
            this.position = pos;
            this.direction = dir;
        }

		/// <summary>
		/// Returns the document this formula will be applied to 
		/// </summary>
		public CsvDocument Document {
			get { return this.csvDocument; }
			set { this.csvDocument = value; }
		}
		
		public Position Position {
			get { return this.position; }
			set { this.position = value; }
		}
		
		public Position.Direction Direction {
			get { return this.direction; }
			set { this.direction = value; }
		}
				
		public abstract double DoIt();
		
		protected double[] RetrieveData()
		{
			double data = 0;
			var toret = new List<Double>();
			var pos = this.Position.Inc( this.Direction );
			
			// Retrieve all data
			while( pos.IsValid() ) {
				if ( Double.TryParse( this.Document.Data[ pos.Row, pos.Column ], out data ) )
				{
					toret.Add( data );
				}

				pos = pos.Inc( this.Direction );
			}
			
			return toret.ToArray();
		}
		
		public static ReadOnlyCollection<Type> GetAllFormulas()
		{
			if ( allFormulas == null ) {
				var toret = new List<Type>();
				var asm = System.Reflection.Assembly.GetExecutingAssembly();
				
				foreach(var type in asm.GetTypes()) {
					if ( type.IsSubclassOf( typeof( Formula ) ) ) {
						toret.Add( type );
					}
				}
				
				allFormulas = toret.AsReadOnly();
			}
			
			return allFormulas;
		}
		
		public static string GetFormulaName(Type t)
		{
			return (string) t.GetField( "Id" ).GetValue( null );
		}
		
		public static Formula GetFormula(int i)
		{
			return (Formula) Activator.CreateInstance( GetAllFormulas()[ i ] );
		}
		
		public abstract string Name {
			get;
		}

        private Position position = null;
        private static ReadOnlyCollection<Type> allFormulas = null;
	}
	
}
