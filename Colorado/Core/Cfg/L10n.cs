// Colorado (c) 2015 MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Cfg {
    using System.Globalization;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The localization 
    /// </summary>
    public static class L10n {
        /// <summary>An identifier for localized strings.</summary>
        public enum Id {
            MnFile,
            MnEdit,
            MnHelp,
        };

        ///<summary>The collection of localized Spanish strings.</summary>
        public static readonly ReadOnlyCollection<string> StringsEN =
            new ReadOnlyCollection<string>( new string[] {
            "File",
            "Edit",
            "Help",
        });

        ///<summary>The collection of localized Spanish strings.</summary>
        public static readonly ReadOnlyCollection<string> StringsES =
            new ReadOnlyCollection<string>( new string[] {
            "Archivo",
            "Editar",
            "Ayuda",
        });

        private static ReadOnlyCollection<string> strings = StringsEN;

        /// <summary>Sets the language for this app.</summary>
        /// <param name="locale">A <see cref="CultureInfo"/> locale.</param>
        public static void SetLanguage(CultureInfo locale)
        {

            if ( locale.TwoLetterISOLanguageName.ToUpper() == "ES" ) {
                strings = StringsES;
            }
            else
            if ( locale.TwoLetterISOLanguageName.ToUpper() == "EN" ) {
                strings = StringsEN;
            }

            return;
        }

        /// <summary>Gets the localized string for the given id.</summary>
        /// <returns>The localized string.</returns>
        /// <param name="id">An identifier, as an <see cref="T:Id"/>.</param>
        public static string Get(Id id)
        {
            string toret = null;
            int numId = (int) id;

            if ( numId < strings.Count ) {
                toret = strings[ numId ];
            }

            return toret;
        }
    }
}
