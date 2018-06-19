// Colorado (c) 2015 MIT License <baltasarq@gmail.com>

namespace Colorado.Core.Cfg {
    using System;
    using System.Globalization;
    using System.Threading;

    public static class Locale {
        public const string EsLocale = "es-ES";
        public const string UsLocale = "en-US";

        public static void SetLocale(string locale)
        {
            CultureInfo cultureInfo;

            locale = locale.Trim();
            if ( locale.Length == 0
              || locale[ 0 ] == '<' )
            {
                cultureInfo = SystemLocale;
            } else {
                cultureInfo = new CultureInfo( locale );
            }

            try {
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
            catch(Exception) {
                CurrentLocale = SystemLocale;
            }

            return;
        }

        public static void SetLocaleFromDescription(string strLocale)
        {
            strLocale = strLocale.Trim();

            if ( strLocale[ 0 ] != '<' ) {
                string[] strLocales = strLocale.Split( ':' );

                if ( strLocales.Length > 1 ) {
                    strLocale = strLocales[ 1 ];
                }
            }

            SetLocale( strLocale );
        }

        public static string GetCurrentLocaleCode()
        {
            return GetLocaleCode( CurrentLocale );
        }

        public static string GetLocaleCode(CultureInfo locale)
        {
            return locale.ToString();
        }

        public static string LocaleToDescription(CultureInfo locale)
        {
            return locale.NativeName + ": " + locale.ToString();
        }

        public static string CurrentLocaleToDescription()
        {
            return LocaleToDescription( CurrentLocale );
        }

        public static CultureInfo CurrentLocale {
            get {
                return Thread.CurrentThread.CurrentCulture;
            }
            set {
                Thread.CurrentThread.CurrentCulture = value;
                Thread.CurrentThread.CurrentUICulture = value;
            }
        }

        public static CultureInfo SystemLocale {
            get {
                return CultureInfo.InstalledUICulture;
            }
        }
    }
}
