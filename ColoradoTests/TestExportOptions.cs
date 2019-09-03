// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace ColoradoTests {
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Colorado.Core;

    [TestFixture]
    public class TestExportOptions {
        [SetUp]
        public void Init()
        {
            this.fileName = Process.GetCurrentProcess().MainModule.FileName;
            this.doc = new CsvDocument( 3, 2 ) { FileName = this.fileName };
            GetAllExporters();
        }

        [Test]
        public void TestDefaultExportCreation()
        {
            var opts = new ExportOptions( this.fileName, this.doc );

            Assert.AreEqual( this.fileName, opts.Path );
            Assert.AreEqual( this.doc.DelimiterValue, opts.Delimiter.Raw.ToString() );
            Assert.AreEqual( Colorado.Core.Importers.CsvImporter.Name, opts.ExporterId );
            Assert.AreEqual( false, opts.IncludeRowNumbers );
            Assert.AreEqual( false, opts.IncludeTableBorder );
            Assert.AreEqual( Enumerable.Range( 0, this.doc.Data.NumColumns ).ToArray(),
                             opts.ColumnsIncluded );
            Assert.AreEqual( this.doc.SurroundText, opts.QuotedText );
        }

        [Test]
        public void TestExportersId()
        {
            foreach(Type cl in GetAllExporters()) {
                var opts = new ExportOptions( this.fileName, this.doc );

                opts.ExporterId = (string) cl.GetField( "Name" ).GetValue( null );
                Assert.AreEqual( cl.Name, opts.ExporterId );
            }

            return;
        }

        static Type[] GetAllExporters()
        {
            if ( allExporters == null ) {
                var toret = new List<Type>();
                var asm = System.Reflection.Assembly.GetExecutingAssembly();

                foreach(var type in asm.GetTypes()) {
                    if ( type.IsSubclassOf( typeof( Exporter ) ) ) {
                        toret.Add( type );
                    }
                }

                allExporters = toret.ToArray();
            }

            return allExporters;
        }

        string fileName;
        CsvDocument doc;
        static Type[] allExporters;
    }
}
