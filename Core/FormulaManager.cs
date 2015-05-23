using System;
using System.Collections.Generic;

namespace Colorado.Core {
	public class FormulaManager {
		public FormulaManager(CsvDocument doc)
		{
			this.document = doc;
		}

		private void PrepareFormulas()
		{
			this.formulas = new List<Formula>();
			this.AllowFormulaUpdating = true;
		}

		/// <summary>
		/// Adds a formula.
		/// </summary>
		/// <param name="f">
		/// A <see cref="Formula"/> that will be added to the document.
		/// </param>
		public void AddFormula(Formula f)
		{
			int oldPos = -1;
			int newPos = -1;

			// Check the place for the new formula
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position == f.Position ) {
					oldPos = i;
					break;
				}
				else
					if ( f.Position < this.formulas[ i ].Position ) {
						newPos = i;
						break;
					}
			}

			// Add formula
			if ( oldPos < 0 ) {
				if ( newPos < 0 ) {
					this.formulas.Add( f );
				} else {
					this.formulas.Insert( newPos, f );
				}
			} else {
				this.formulas[ oldPos ] = f;
			}

			Document.Changed = true;
		}

		/// <summary>
		/// Recalculates the results of all existing formulas.
		/// </summary>
		public void UpdateFormulas()
		{
			if ( this.AllowFormulaUpdating ) {
				foreach(var f in this.formulas) {
					// Apply formula & store result
					string result = f.DoIt().ToString();
					Document.Data[ f.Position.Row, f.Position.Column ] = result;
					Document.UpdateClients( f.Position.Row, f.Position.Column, result );
				}
			}

			return;
		}

		/// <summary>
		/// Moves the formula f to the new position
		/// </summary>
		/// <param name="f">
		/// A <see cref="Formula"/> holding the formula to move.
		/// </param>
		/// <param name="newPos">
		/// A <see cref="Position"/> holding the position to move the formula to.
		/// </param>
		public void MoveFormula(Formula f, Position newPos)
		{
			f.Position = newPos;
		}

		/// <summary>
		/// Eliminates/fixes all formulas deleted with columns
		/// </summary>
		/// <param name="numCol">
		/// A <see cref="System.Int32"/> holding the column deleted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of columns deleted.
		/// </param>
		public void FixFormulasColumnsRemoved(int numCol, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Column >= numCol
					&& this.formulas[ i ].Position.Column < ( numCol + num ) )
				{
					this.formulas.RemoveAt( i );
					--i;
				}
				else
					if ( this.formulas[ i ].Position.Column > numCol ) {
						MoveFormula( this.formulas[ i ],
							new Position( Document,
								this.formulas[ i ].Position.Row,
								this.formulas[ i ].Position.Column - num )
						);
					}
			}

			UpdateFormulas();
		}

		/// <summary>
		/// Fixes all formulas when columns inserted.
		/// </summary>
		/// <param name="numCol">
		/// A <see cref="System.Int32"/> holding the position in which the column inserted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of columns inserted.
		/// </param>
		public void FixFormulasColumnsInserted(int numCol, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Column >= numCol ) {
					MoveFormula( this.formulas[ i ],
						new Position( Document,
							this.formulas[ i ].Position.Row,
							this.formulas[ i ].Position.Column + num )
					);
				}
			}

			UpdateFormulas();
		}

		/// <summary>
		/// Eliminates/fixes all formulas deleted with rows.
		/// </summary>
		/// <param name="numRow">
		/// A <see cref="System.Int32"/> holding the row deleted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of rows deleted.
		/// </param>
		public void FixFormulasRowsRemoved(int numRow, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Row >= numRow
					&& this.formulas[ i ].Position.Row < ( numRow + num ) )
				{
					this.formulas.RemoveAt( i );
					--i;
				}
				else
					if ( this.formulas[ i ].Position.Row > numRow ) {
						MoveFormula( this.formulas[ i ],
							new Position( Document,
								this.formulas[ i ].Position.Row - num,
								this.formulas[ i ].Position.Column )
						);
					}
			}

			UpdateFormulas();
		}

		/// <summary>
		/// Fixes all formulas when rows inserted.
		/// </summary>
		/// <param name="numRow">
		/// A <see cref="System.Int32"/> holding the position in which the rows inserted.
		/// </param>
		/// <param name="num">
		/// A <see cref="System.Int32"/> holding the number of rows inserted.
		/// </param>
		public void FixFormulasRowsInserted(int numRow, int num)
		{
			for(int i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position.Row >= numRow ) {
					MoveFormula( this.formulas[ i ],
						new Position( Document,
							this.formulas[ i ].Position.Row + num,
							this.formulas[ i ].Position.Column )
					);
				}
			}

			UpdateFormulas();
		}

		/// <summary>
		/// Eliminates a formula in a given position.
		/// </summary>
		/// <param name="row">
		/// A <see cref="System.Int32"/> holding the row in which the formula lives.
		/// </param>
		/// <param name="col">
		/// A <see cref="System.Int32"/> holding the column in which the formula lives.
		/// </param>
		public void RemoveFormula(int row, int col)
		{
			int i = 0;
			var pos = new Position( Document, row, col );

			for(i = 0; i < this.formulas.Count; ++i) {
				if ( this.formulas[ i ].Position == pos ) {
					this.formulas.RemoveAt( i );
					break;
				}
			}

			return;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Colorado.Core.FormulaManager"/> allow formula updating.
		/// This is needed because sometimes formulas are moved from their position;
		/// we do not want to report to the client gui at that time.
		/// </summary>
		/// <value><c>true</c> if allow formula updating; otherwise, <c>false</c>.</value>
		public bool AllowFormulaUpdating {
			get; set;
		}

		/// <summary>
		/// Gets the document.
		/// </summary>
		/// <value>The document, as a CsvDocument object.</value>
		private CsvDocument Document {
			get {
				return this.document;
			}
		}

		private CsvDocument document;
		private List<Formula> formulas;
	}
}

