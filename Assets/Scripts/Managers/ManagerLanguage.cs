using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ManagerLanguage
{
    Dictionary<EnumLanguageWords, JsonValue.DataLanguageWordsItem> dicWord;
    Dictionary<EnumLanguageStatement, JsonValue.DataLanguageStatementItem> dicStatement;
    Dictionary<EnumLanguageStory, JsonValue.DataLanguageStoryItem> dicStory;
    static ManagerLanguage _instance;
    public static ManagerLanguage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerLanguage();
                string strData = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableLanguageWords);
                JsonValue.DataLanguageWordsBase language = JsonUtility.FromJson<JsonValue.DataLanguageWordsBase>(strData);

                _instance.dicWord = new Dictionary<EnumLanguageWords, JsonValue.DataLanguageWordsItem>();
                for (int i = 0; i < language.listItem.Count; i++)
                {
                    _instance.dicWord.Add(language.listItem[i].enumWord, language.listItem[i]);
                }

                string strStatement = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableLanguageStatement);
                JsonValue.DataLanguageStatementBase languageStatement = JsonUtility.FromJson<JsonValue.DataLanguageStatementBase>(strStatement);
                _instance.dicStatement = new Dictionary<EnumLanguageStatement, JsonValue.DataLanguageStatementItem>();
                for (int i = 0; i < languageStatement.listItem.Count; i++)
                {
                    _instance.dicStatement.Add(languageStatement.listItem[i].enumStatment, languageStatement.listItem[i]);
                }

                string strStory = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableLanguageStory);
                JsonValue.DataLanguageStoryBase languageStory = JsonUtility.FromJson<JsonValue.DataLanguageStoryBase>(strStory);
                _instance.dicStory = new Dictionary<EnumLanguageStory, JsonValue.DataLanguageStoryItem>();
                for (int i = 0; i < languageStory.listItem.Count; i++)
                {
                    _instance.dicStory.Add(languageStory.listItem[i].enumStory, languageStory.listItem[i]);
                }
            }
            return _instance;
        }
    }

    public string GetWord(EnumLanguageWords key)
    {
        if (!dicWord.ContainsKey(key))
        {
            return "(?)??";
        }
        return dicWord[key].strNames[(int)ManagerValue.enumLanguage];
    }

    public string GetStatement(EnumLanguageStatement key, string[] strContent)
    {
        if (!dicStatement.ContainsKey(key))
        {
            Debug.Log(key);
            return "?(?)";
        }
        string strTemp = dicStatement[key].listName[(int)ManagerValue.enumLanguage].strName;
        if (strContent != null)
        {
            List<int> listIndex = new List<int>();
            for (int i = 0; i < strTemp.Length; i++)
            {
                if (strTemp[i] == '_')
                {
                    listIndex.Add(i);
                }
            }
            string str = null;
            int intIndex = 0;
            for (int i = 0; i < strTemp.Length; i++)
            {
                if (listIndex.Contains(i))
                {
                    str += strContent[dicStatement[key].listName[(int)ManagerValue.enumLanguage].intNameSizes[intIndex++]];
                }
                else
                {
                    str += strTemp[i];
                }
            }

            strTemp = str;
        }
        return strTemp;
    }

    public string GetStory(EnumLanguageStory key)
    {
        if (!dicStory.ContainsKey(key))
        {
            return "?-?";
        }
        return dicStory[key].strNames[(int)ManagerValue.enumLanguage];
    }
}
