using NUnit.Framework;
using System;

using Colorado.Core;

namespace ColoradoTests {
	[TestFixture]
	public class TestDecimalMark {
		[Test]
		public void TestDecimalSeparator() {
			for (int i = 0; i < DecimalMark.DecimalSeparatorChar.Count; ++i) {
				Assert.AreEqual(
					DecimalMark.DecimalSeparatorChar[ i ],
					DecimalMark.AsChar( (DecimalMark.DecimalSeparator) i )
				);
			}
		}

		[Test]
		public void TestIsDecimalMark() {
			Assert.AreEqual( false, DecimalMark.IsDecimalMark( 'a' ) );
			Assert.AreEqual( true, DecimalMark.IsDecimalMark( '.' ) );
			Assert.AreEqual( true, DecimalMark.IsDecimalMark( ',' ) );
		}

		[Test]
		public void TestWhichDecimalMark() {
			Assert.AreEqual( DecimalMark.DecimalSeparator.Point, DecimalMark.WhichDecimalMark( "4" ) );
			Assert.AreEqual( DecimalMark.DecimalSeparator.Point, DecimalMark.WhichDecimalMark( "4.5" ) );
			Assert.AreEqual( DecimalMark.DecimalSeparator.Comma, DecimalMark.WhichDecimalMark( "4,5" ) );

			Assert.AreEqual( 0, DecimalMark.WhichDecimalMark( '.' ) );
			Assert.AreEqual( 1, DecimalMark.WhichDecimalMark( ',' ) );
			Assert.AreEqual( -1, DecimalMark.WhichDecimalMark( 'a' ) );
		}

		[Test]
		public void TestIsNumber()
		{
			string[] testNumbers = {
				"", " ", ".0",
				"-.1", "-.", "0.4e-5",
				"5.4", "0", "+",
				"+.0", "0,6e-6", "5,6",
				"hello4", "4d", "3",
				"0.4e5", "0,1E45", "0,0001",
				",1", "1,", "1.",
			};

			bool[] testResults = {
				false, false, true,
				true, false, true,
				true, true, false,
				true, true,	true,
				false, false, true,
				true, true, true,
				true, true, true,
			};

			for(int i = 0; i < testResults.Length; ++i) {
				double num;
				bool actualResult = double.TryParse( testNumbers[ i ], out num );
				bool result = DecimalMark.IsNumber( testNumbers[ i ] );

				Console.WriteLine( "[\"{0}\"] -> {1} == {2} == {3} == {4}?",
					testNumbers[ i ], testResults[ i ], result, actualResult, num );
				
				Assert.AreEqual( testResults[ i ], result );
				Assert.AreEqual( testResults[ i ], actualResult );
			}
		}

		[Test]
		public void TestIsRealNumber()
		{
			string[] testNumbers = {
				"", " ", ".0",
				"4e+5", "-.", "4",
				"5.4", "0", "+",
				"+.0", "0,6e-6", "5,6",
				"hello4", "4d", "0,",
				"0.4e5", "0,1E45", "0,0001",
			};

			bool[] testResults = {
				false, false, true,
				false, false, false,
				true, false, false,
				true, true, true,
				false, false, true,
				true, true, true,
			};

			for(int i = 0; i < testResults.Length; ++i) {
				bool result = DecimalMark.IsRealNumber( testNumbers[ i ] );

				Console.WriteLine( "[\"{0}\"] -> {1} == {2}?",
					testNumbers[ i ], testResults[ i ], result );
				
				Assert.AreEqual( testResults[ i ], result );
			}
		}
	}
}

