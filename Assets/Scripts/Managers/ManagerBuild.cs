using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBuild
{
    Dictionary<int, JsonValue.DataTableBuildingItem> dicBuild;
    Dictionary<int, JsonValue.DataTableBuildNameItem> dicBuildName;
    static ManagerBuild _instance;
    public static ManagerBuild Instance
    {
        get
        {
            if (_instance == null)
            {
                JsonValue.DataTableBuildingBase build = null;
                _instance = new ManagerBuild();
                string strData = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableBuild);
                build = JsonUtility.FromJson<JsonValue.DataTableBuildingBase>(strData);

                _instance.dicBuild = new Dictionary<int, JsonValue.DataTableBuildingItem>();
                for (int i = 0; i < build.listTable.Count; i++)
                {
                    int numBuildID = build.listTable[i].intBuildID;
                    _instance.dicBuild.Add(numBuildID, build.listTable[i]);

                }

                string strBuildName = ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableBuildLanguage);
                JsonValue.DataTableBuildNameBase buildName = JsonUtility.FromJson<JsonValue.DataTableBuildNameBase>(strBuildName);

                _instance.dicBuildName = new Dictionary<int, JsonValue.DataTableBuildNameItem>();
                for (int i = 0; i < buildName.listTable.Count; i++)
                {
                    int numBuildID = buildName.listTable[i].intBuildID;
                    _instance.dicBuildName.Add(numBuildID, buildName.listTable[i]);
                }

            }
            return _instance;
        }
    }

    public string GetBuildName(int intBuildID)
    {
        if (dicBuild.ContainsKey(intBuildID))
        {
            return dicBuildName[intBuildID].GetName;
        }
        return "";
    }
    public JsonValue.DataTableBuildingItem GetBuildItem(int intBuildID)
    {
        if (dicBuild.ContainsKey(intBuildID))
        {
            return dicBuild[intBuildID];
        }
        return null;
    }
    public Dictionary<int, JsonValue.DataTableBuildingItem> GetBuildAll()
    {
        return dicBuild;
    }

}
