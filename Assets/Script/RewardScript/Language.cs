using System.Collections.Generic;
using UnityEngine;

namespace Script.RewardScript
{
    public class Language : MonoBehaviour
    {
        public enum LanguageType { Korean, English }
        public LanguageType selectedLanguage = LanguageType.Korean;
        private readonly Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();
        public void Awake()
        {
            if (PlayerPrefs.HasKey("Language"))
            {
                var savedLanguage = PlayerPrefs.GetString("Language");
                selectedLanguage = savedLanguage switch
                {
                    "KOR" => LanguageType.Korean,
                    "ENG" => LanguageType.English,
                    _ => selectedLanguage
                };
            }
            var csvFile = Resources.Load<TextAsset>("languageData");
            var csvData = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            for (var i = 1; i < csvData.Length; i++)
            {
                var data = csvData[i].Split(',');
                var type = data[0];
                var kor = data[1];
                var eng = data[2];

                if (!_translations.ContainsKey(type))
                {
                    _translations[type] = new Dictionary<string, string>();
                }
                _translations[type]["Kor"] = kor;
                _translations[type]["Eng"] = eng;
            }
        }

        public string GetTranslation(string type)
        {
            var selectedLang = selectedLanguage == LanguageType.Korean ? "Kor" : "Eng";
            if (!_translations.TryGetValue(type, out var typeTranslations)) return string.Empty;
            return typeTranslations.TryGetValue(selectedLang, out var translation) ? translation : string.Empty;
        }
    }
}
