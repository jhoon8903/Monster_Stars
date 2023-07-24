using UnityEngine;
using System.Collections.Generic;

public class Language : MonoBehaviour
{
    public enum LanguageType
    {
        Korean,
        English
    }

    public LanguageType selectedLanguage = LanguageType.Korean; // 기본 언어를 한국어로 설정

    private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();

    public void Awake()
    {
        // Load the saved language setting from PlayerPrefs
        if (PlayerPrefs.HasKey("Language"))
        {
            string savedLanguage = PlayerPrefs.GetString("Language");
            if (savedLanguage == "KOR")
            {
                selectedLanguage = LanguageType.Korean;
            }
            else if (savedLanguage == "ENG")
            {
                selectedLanguage = LanguageType.English;
            }
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
            var Kor = data[1];
            var Eng = data[2];

            if (!translations.ContainsKey(type))
            {
                translations[type] = new Dictionary<string, string>();
            }

            translations[type]["Kor"] = Kor;
            translations[type]["Eng"] = Eng;
        }
    }

    public string GetTranslation(string type)
    {
        // Get the selected language based on SelectedLanguage property
        string selectedLanguage = this.selectedLanguage == LanguageType.Korean ? "Kor" : "Eng";

        if (translations.TryGetValue(type, out var typeTranslations))
        {
            if (typeTranslations.TryGetValue(selectedLanguage, out var translation))
            {
                return translation;
            }
        }

        Debug.LogWarning($"Type: {type}와 선택된 언어: {selectedLanguage}에 해당하는 번역을 찾을 수 없습니다.");
        return string.Empty;
    }
}
