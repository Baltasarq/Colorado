using System;

namespace Colorado.Core.Fillers {
    public class FixedFiller : Filler {
        public FixedFiller(CsvDocument doc, Position pos, int n, string initValue, FillType ft, FillDirection fd)
            :base( doc, pos, n, initValue, ft, fd )
        {
        }

        /// <summary>
        /// Calculate the next value for fixed.
        /// </summary>
        /// <returns>Just the this.Value itself, as a string.</returns>
        protected override string NextValue()
        {
            return this.Value;
        }
    }
}

