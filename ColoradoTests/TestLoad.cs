// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace ColoradoTests {
    using System.IO;

    using NUnit.Framework;
    using Colorado.Core;

    [TestFixture]
    public class TestLoad {
        [Test]
        public void TestIntMatrixSettingDelimiter()
        {
            string documentContents = "1,2,3\n4,5,6\n7,8,9";
            string[][] rawContents = this.ConvertToRaw( documentContents, ',' );

            var loader = new CsvDocumentPersistence();
            loader.Load( new StringReader( documentContents ), ',', false );

            Assert.True( this.CompareCsv( loader.Document.Data, rawContents ) );
        }

        [Test]
        public void TestIntMatrixInferringDelimiter()
        {
            string documentContents = "1,2,3\n4,5,6\n7,8,9";
            string[][] rawContents = this.ConvertToRaw( documentContents, ',' );

            var loader = new CsvDocumentPersistence();
            loader.Load( new StringReader( documentContents ), firstRowForHeaders: false );

            this.Show( loader.Document.Data );
            this.Show( rawContents );

            Assert.True( this.CompareCsv( loader.Document.Data, rawContents ) );
        }

        [Test]
        public void TestOneColumnDocSettingDelimiter()
        {
            string documentContents = "a\nb\nc\nd\ne\nf\ng";
            string[][] rawContents = this.ConvertToRaw( documentContents, '\t' );

            var loader = new CsvDocumentPersistence();
            loader.Load( new StringReader( documentContents ), '\t', false );

            Assert.True( this.CompareCsv( loader.Document.Data, rawContents ) );
        }

        [Test]
        public void TestOneColumnDocInferringDelimiter()
        {
            string documentContents = "a\nb\nc\nd\ne\nf\ng";
            string[][] rawContents = this.ConvertToRaw( documentContents, '\t' );

            var loader = new CsvDocumentPersistence();
            loader.Load( new StringReader( documentContents ), firstRowForHeaders: false );

            Assert.True( this.CompareCsv( loader.Document.Data, rawContents ) );
        }

        bool CompareCsv(Data data, string[][] lines)
        {
            bool toret = false;

            if ( lines.Length == data.NumRows ) {
                if ( lines.Length > 0 ) {
                    for(int i = 0; i < lines.Length; ++i) {
                        string[] cols = lines[ i ];

                        if ( cols.Length == data.NumColumns ) {
                            toret = true;

                            for(int j = 0; j < cols.Length; ++j) {
                                if ( cols[ j ] != data[ i, j ] ) {
                                    toret = false;
                                    break;
                                }
                            }
                        } else {
                            System.Console.WriteLine( "Col " + i + ": " + cols.Length + " != " + data.NumColumns );
                            this.Show( data );
                        }
                    }
                } else {
                    toret = true;
                }
            } else {
                System.Console.WriteLine( "Rows: " + lines.Length + " != " + data.NumRows );
                this.Show( data );
            }

            return toret;
        }

        void Show(Data data)
        {
            for(int i = 0; i < data.NumRows; ++i) {
                System.Console.Write( i + ": " );
                System.Console.WriteLine( string.Join( ", ", data[ i ] ) );
            }
        }

        void Show(string[][] raw)
        {
            for(int i = 0; i < raw.Length; ++i) {
                System.Console.Write( i + ": " );

                for(int j = 0; j < raw[ i ].Length; ++j) {
                    System.Console.Write( raw[ i ][ j ] + " " );
                }   

                System.Console.WriteLine();
            }
        }

        string[][] ConvertToRaw(string contents, char delim)
        {
            string[] lines = contents.Split( '\n' );
            string[][] toret = new string[ lines.Length ][];

            for(int i = 0; i < lines.Length; ++i) {
                toret[ i ] = lines[ i ].Split( delim );
            }

            return toret;
        }
    }
}
