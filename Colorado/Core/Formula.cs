// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>


namespace Colorado.Core {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;


	public abstract class Formula {
		public Formula(CsvDocument doc, Position pos, Position.Direction dir)
		{
			this.Document = doc;
			this.Position = pos;
			this.Direction = dir;
		}

		/// <summary>
		/// Returns the document this formula will be applied to
		/// </summary>
		public CsvDocument Document {
			get;
		}

		public Position Position {
			get; set;
		}

		public Position.Direction Direction {
			get;
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

		/// <summary>Returns the name of a <see cref="Formula"/>Formula</see>.</summary>
		/// <param name="t">The class of a given <see cref="Formula"/>Formula</see>.</param>
		/// <returns>The name of the formula, or an empty string if it cannot be retrieved.</returns>
		public static string GetFormulaName(Type t)
		{
			string toret = "";

			if ( t is not null ) {
				var idField = t.GetField( "Id" );

				if ( idField is not null ) {
					toret = (string) ( idField.GetValue( null ) ?? "" );
				}
			}

			return toret;
		}

		/// <summary>Gets a <see cref="Formula"/> from its type.</summary>
		/// <seealso cref="Formula.GetAllFormulas"/>
		/// <param name="i">The position in the collection of formulas.</param>
		/// <param name="doc">The <see cref="CsvDocument"/>.</param>
		/// <param name="pos">The <see cref="Position"> in the document.</param>
		/// <param name="dir">The <see cref="Position.Direction" in the document.</param>
		/// <returns>The corresponding formula, already created and configured.</returns>
		public static Formula? GetFormula(int i, CsvDocument doc, Position pos, Position.Direction dir)
		{
			Formula? toret = null;

			try {
				toret = (Formula?) Activator.CreateInstance(
												GetAllFormulas()[ i ], [ doc, pos, dir] );
			} catch(Exception) {
				toret = null;
			}

			return toret;
		}

		/// <summary>The id of the formula, <see cref="Id"/>.</summary>
		public abstract string Name {
			get;
		}

        private static ReadOnlyCollection<Type>? allFormulas = null;
	}
}
