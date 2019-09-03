// Colorado (c) 2015 Baltasar MIT License <baltasarq@gmail.com>

namespace ColoradoTests {
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;

    using NUnit.Framework;

    using Colorado.Core;

    [TestFixture]
    public class TestImportOptions {
        [SetUp]
        public void Init()
        {
            this.fileName = Process.GetCurrentProcess().MainModule.FileName;
            GetAllImporters();
        }

        [Test]
        public void TestDefaultImportCreation()
        {
            var opts = new ImportOptions( this.fileName );

            Assert.AreEqual( this.fileName, opts.Path);
            Assert.AreEqual( Delimiter.CommaDelimiter, opts.Delimiter.Raw );
            Assert.AreEqual( Colorado.Core.Importers.CsvImporter.Name, opts.ImportId );
        }

        [Test]
        public void TestImportersId()
        {
            foreach(Type cl in GetAllImporters()) {
                var opts = new ImportOptions( this.fileName );

                opts.ImportId = (string) cl.GetField( "Name" ).GetValue( null );
                Assert.AreEqual( cl.Name, opts.ImportId );
            }

            return;
        }

        static Type[] GetAllImporters()
        {
            if ( allImporters == null ) {
                var toret = new List<Type>();
                var asm = System.Reflection.Assembly.GetExecutingAssembly();

                foreach(var type in asm.GetTypes()) {
                    if ( type.IsSubclassOf( typeof( Importer ) ) ) {
                        toret.Add( type );
                    }
                }

                allImporters = toret.ToArray();
            }

            return allImporters;
        }

        string fileName;
        static Type[] allImporters;
    }
}
