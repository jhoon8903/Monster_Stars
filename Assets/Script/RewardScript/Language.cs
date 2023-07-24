using System.Collections.Generic;
using UnityEngine;

namespace Script.RewardScript
{
    public class Language : MonoBehaviour
    {
        public enum LanguageType
        {
            Korean,
            English
        }

        public LanguageType selectedLanguage = LanguageType.Korean; // 기본 언어를 한국어로 설정

        private readonly Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();

        public void Awake()
        {
            // Load the saved language setting from PlayerPrefs
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
            if (csvFile == null)
            {
                Debug.LogError("리소스 폴더에 파일을 찾을 수 없습니다.");
                return;
            }

            var csvData = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);

            if (csvData.Length <= 1)
            {
                Debug.LogError("CSV 파일이 비어 있거나 잘못된 형식입니다.");
                return;
            }

            // 헤더 행을 가져와 언어를 확인합니다.
            var header = csvData[0].Split(',');
            if (header.Length < 3)
            {
                Debug.LogError("잘못된 CSV 형식입니다. 최소 세 개의 열이 필요합니다: Type, Kor, Eng");
                return;
            }

            // 각 번역을 사전에 추가합니다.
            for (var i = 1; i < csvData.Length; i++)
            {
                var data = csvData[i].Split(',');
                if (data.Length < 3)
                {
                    Debug.LogError($"CSV 형식이 잘못되었습니다. {i + 1}번째 줄에는 세 개의 열이 필요합니다: Type, Kor, Eng");
                    continue;
                }
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
            if (_translations.TryGetValue(type, out var typeTranslations))
            {
                if (typeTranslations.TryGetValue(selectedLang, out var translation))
                {
                    return translation;
                }
            }
            Debug.LogWarning($"Type: {type}와 선택된 언어: {selectedLang}에 해당하는 번역을 찾을 수 없습니다.");
            return string.Empty;
        }
    }
}
