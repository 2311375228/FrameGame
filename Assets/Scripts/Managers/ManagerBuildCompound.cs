using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBuildCompound
{
    JsonValue.DataTableBuildCompoundItem[] items;
    static ManagerBuildCompound _instance;
    public static ManagerBuildCompound Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerBuildCompound();

                JsonValue.DataTableBuildCompoundBase temp = JsonUtility.FromJson<JsonValue.DataTableBuildCompoundBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableBuildCompound));
                _instance.items = new JsonValue.DataTableBuildCompoundItem[temp.listItem.Count];
                for (int i = 0; i < temp.listItem.Count; i++)
                {
                    JsonValue.DataTableBuildCompoundItem item = temp.listItem[i];
                    _instance.items[i] = new JsonValue.DataTableBuildCompoundItem();
                    _instance.items[i].intbuildID = temp.listItem[i].intbuildID;
                    _instance.items[i].intCompundID = temp.listItem[i].intCompundID;
                }
            }
            return _instance;
        }
    }

    public int[] GetBuildCompoundProduct(int buildID)
    {
        List<int> listTemp = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].intbuildID == buildID)
            {
                listTemp.Add(items[i].intCompundID);
            }
        }
        return listTemp.ToArray();
    }
}
