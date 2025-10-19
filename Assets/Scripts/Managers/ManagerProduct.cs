using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerProduct
{
    Dictionary<int, JsonValue.DataTableBackPackItem> dicProduct;
    Dictionary<int, JsonValue.DataTableBackPackItemName> dicProductName;
    List<int> listProductID = new List<int>();
    static ManagerProduct _instance;
    public static ManagerProduct Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerProduct();
                JsonValue.DataTableBackPackBase temp = JsonUtility.FromJson<JsonValue.DataTableBackPackBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableProduct));
                _instance.dicProduct = new Dictionary<int, JsonValue.DataTableBackPackItem>();

                for (int i = 0; i < temp.listItem.Count; i++)
                {
                    _instance.dicProduct.Add(temp.listItem[i].intProductID, temp.listItem[i]);
                    _instance.listProductID.Add(temp.listItem[i].intProductID);
                    //UserValue.Instance.StockProductAdd(temp.listItem[i].intProductID);
                }

                JsonValue.DataTableBackPackNameBase productName = JsonUtility.FromJson<JsonValue.DataTableBackPackNameBase>(ManagerResources.Instance.GetTextAssetString("DataTables/", EnumTableName.TableProductLanguage));
                _instance.dicProductName = new Dictionary<int, JsonValue.DataTableBackPackItemName>();

                for (int i = 0; i < productName.listItem.Count; i++)
                {
                    _instance.dicProductName.Add(productName.listItem[i].intID, productName.listItem[i]);
                }
            }
            return _instance;
        }
    }

    public string GetName(int intID, bool boo)
    {
        return dicProductName[intID].GetName(boo);
    }

    public JsonValue.DataTableBackPackItem GetProductTableItem(int intProductID)
    {
        return dicProduct[intProductID];
    }

    public List<int> GetRandomProductID(int intCount)
    {
        List<int> listTemp = new List<int>();
        int intLimit = 0;
        while (true)
        {
            if (intLimit++ > 100)
            {
                Debug.LogError("随机错误");
                break;
            }
            if (listTemp.Count == intCount)
            {
                break;
            }

            int intRandom = Random.Range(0, listProductID.Count);
            if (!listTemp.Contains(listProductID[intRandom]))
            {
                listTemp.Add(listProductID[intRandom]);
            }
        }
        return listTemp;
    }
}
