using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCompound
{
    Dictionary<int, JsonValue.DataTableCompoundItem> dicCompound;
    Dictionary<int, int> dicProductPrice = new Dictionary<int, int>();
    static ManagerCompound _instance;
    public static ManagerCompound Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerCompound();
                _instance.dicCompound = new Dictionary<int, JsonValue.DataTableCompoundItem>();
                JsonValue.DataTableCompoundBase temp = JsonUtility.FromJson<JsonValue.DataTableCompoundBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableProductCompound));
                for (int i = 0; i < temp.listCompound.Count; i++)
                {
                    JsonValue.DataTableCompoundItem item = temp.listCompound[i];
                    _instance.dicCompound.Add(item.intCompoundID, item);
                    if (!_instance.dicProductPrice.ContainsKey(item.intProductID))
                    {
                        _instance.dicProductPrice.Add(item.intProductID, item.intPrice);
                    }
                }
            }
            return _instance;
        }
    }

    public JsonValue.DataTableCompoundItem GetValue(int intProductCompoundID)
    {
        if (dicCompound.ContainsKey(intProductCompoundID))
        {
            return dicCompound[intProductCompoundID];
        }
        return null;
    }

    public int GetProductPrice(int intProductID)
    {
        if (dicProductPrice.ContainsKey(intProductID))
        {
            return dicProductPrice[intProductID];
        }
        return -1;
    }
}
