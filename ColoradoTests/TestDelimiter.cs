// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace ColoradoTests {
    using NUnit.Framework;

    using Colorado.Core;

    [TestFixture]
    public class TestDelimiter {
        [SetUp]
        public void Init()
        {
            int numDelimiters = Delimiter.PredefinedDelimiters.Count;

            this.delimiters = new char[ numDelimiters + 2 ];
            this.delimiterNames = new string[ numDelimiters + 2 ];

            Delimiter.PredefinedDelimiterNames.CopyTo( delimiterNames, 0 );
            Delimiter.PredefinedDelimiters.CopyTo( delimiters, 0 );

            this.delimiters[ numDelimiters - 1 ] = '|';
            this.delimiterNames[ numDelimiters - 1 ] = "|";

            this.delimiters[ numDelimiters - 2 ] = '>';
            this.delimiterNames[ numDelimiters - 2 ] = ">";
        }

        [Test]
        public void TestConvertTabDelimiterName()
        {
            Assert.AreEqual( new Delimiter( Delimiter.TabDelimiterName ).Raw,
                             Delimiter.TabDelimiter );
            
            Assert.AreEqual( new Delimiter( Delimiter.TabDelimiter ).Raw,
                             Delimiter.TabDelimiter );
        }

        [Test]
        public void TestUnderstandDelimiterName()
        {
            for(int i = 0; i < delimiters.Length; ++i) {
                var delimiter = new Delimiter( this.delimiterNames[ i ] );

                Assert.AreEqual( delimiter.Raw,
                                 this.delimiters[ i ] );
            }
        }

        string[] delimiterNames;
        char[] delimiters;
    }
}
