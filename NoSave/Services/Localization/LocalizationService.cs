using NoSave.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NoSave.Services.Localization
{
    public class LocalizationService
    {
        private const AppLanguage DefaultLanguage = AppLanguage.En;

        private static readonly Dictionary<string, AppLanguage> LanguageArgs =
            new Dictionary<string, AppLanguage>(StringComparer.OrdinalIgnoreCase)
            {
                { "-en", AppLanguage.En },
                { "-uk", AppLanguage.Uk },
                { "-ru", AppLanguage.Ru }
            };

        private static readonly Dictionary<string, AppLanguage> SystemLanguages =
            new Dictionary<string, AppLanguage>(StringComparer.OrdinalIgnoreCase)
            {
                { "uk", AppLanguage.Uk },
                { "ru", AppLanguage.Ru },
                { "kk", AppLanguage.Ru }
            };

        public AppLanguage Language { get; private set; }

        public LocalizationService(string[] args)
        {
            Language = GetStartupLanguage(args);
        }

        private AppLanguage GetStartupLanguage(string[] args)
        {
            foreach (string arg in args)
            {
                if (LanguageArgs.TryGetValue(arg, out AppLanguage language))
                    return language;
            }

            return GetLanguageFromSystem();
        }

        private AppLanguage GetLanguageFromSystem()
        {
            string languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            if (SystemLanguages.TryGetValue(languageCode, out AppLanguage language))
                return language;

            return DefaultLanguage;
        }

        public string GetString(string key)
        {
            string localizedKey = $"{key}_{Language}";
            string defaultKey = $"{key}_{DefaultLanguage}";

            return Strings.ResourceManager.GetString(localizedKey)
                ?? Strings.ResourceManager.GetString(defaultKey)
                ?? key;
        }
    }
}
