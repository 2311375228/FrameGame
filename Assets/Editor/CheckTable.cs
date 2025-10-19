using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CheckTable
{
    public Dictionary<int, JsonValue.DataTableBackPackItem> dicProduct;
    public Dictionary<EnumTableName, string> dicStr;
    List<int> listProductCompound;
    List<int> listProduct;
    /// <summary>
    /// 检查产品生产合成链
    /// </summary>
    public void CheckProductCompound(string strData)
    {
        listProduct = new List<int>();
        listProductCompound = new List<int>();
        JsonValue.DataTableCompoundBase table = JsonUtility.FromJson<JsonValue.DataTableCompoundBase>(strData);
        //检查合成产品ID链条
        for (int i = 0; i < table.listCompound.Count; i++)
        {
            if (!dicProduct.ContainsKey(table.listCompound[i].intProductID))
            {
                Debug.LogError("产品ID:" + table.listCompound[i].intCompoundID);
            }
            for (int j = 0; j < table.listCompound[i].intPorductIDStuff.Length; j++)
            {
                if (!dicProduct.ContainsKey(table.listCompound[i].intPorductIDStuff[j]))
                {
                    Debug.LogError("产品ID配料:" + table.listCompound[i].intCompoundID + ":" + table.listCompound[i].intPorductIDStuff[j]);
                }
            }
            listProduct.Add(table.listCompound[i].intProductID);
            listProductCompound.Add(table.listCompound[i].intCompoundID);
        }
    }
    public void CheckBuildCompound(string strData)
    {
        JsonValue.DataTableBuildCompoundBase table = JsonUtility.FromJson<JsonValue.DataTableBuildCompoundBase>(strData);
        //检查建筑拥有的产品合成链条
        for (int i = 0; i < table.listItem.Count; i++)
        {
            if (!listProductCompound.Contains(table.listItem[i].intCompundID))
            {
                Debug.LogError("建筑ID:" + table.listItem[i].intbuildID + ":" + table.listItem[i].intCompundID);
            }
        }
    }
    /// <summary>
    /// 检查任务奖励的产品ID，并且该ID存在于生产产品链
    /// </summary>
    public void CheckTaskProduct(string strData)
    {
        JsonValue.DataTaskProductBase table = JsonUtility.FromJson<JsonValue.DataTaskProductBase>(strData);
        for (int i = 0; i < table.listItem.Count; i++)
        {
            for (int j = 0; j < table.listItem[i].awards.Length; j++)
            {
                if (!listProduct.Contains(table.listItem[i].awards[j].intID))
                {
                    //Debug.LogError("产品任务ID 奖励" + table.listItem[i].intID + ":" + table.listItem[i].awards[j].intID);
                }
            }
            for (int j = 0; j < table.listItem[i].goods.Length; j++)
            {
                if (!listProduct.Contains(table.listItem[i].goods[j].intID))
                {
                    Debug.LogError("产物任务ID 物品" + table.listItem[i].intID + ":" + table.listItem[i].goods[j].intID);
                }
            }
        }
    }
    /// <summary>
    /// 副本任务ID
    /// </summary>
    /// <param name="strData"></param>
    public void CheckTaskDungeon(string strData)
    {
        JsonValue.DataTaskDungeonBase table = JsonUtility.FromJson<JsonValue.DataTaskDungeonBase>(strData);
        for (int i = 0; i < table.listItem.Count; i++)
        {
            for (int j = 0; j < table.listItem[i].awards.Length; j++)
            {
                if (!listProduct.Contains(table.listItem[i].awards[j].intID))
                {
                    Debug.LogError("副本任务奖励" + table.listItem[i].intID + ":" + table.listItem[i].awards[j].intID);
                }
            }
        }
    }
    /// <summary>
    /// 副本通关奖励
    /// </summary>
    public void CheckDungeon(string strData)
    {
        JsonValue.DataGameDungeonBase table = JsonUtility.FromJson<JsonValue.DataGameDungeonBase>(strData);
        for (int i = 0; i < table.listItem.Count; i++)
        {
            for (int j = 0; j < table.listItem[i].taskPoints.Length; j++)
            {
                JsonValue.DataGameDungeonItemTaskPoint temp = table.listItem[i].taskPoints[j];
                for (int t = 0; t < temp.intAwardIDs.Length; t++)
                {
                    if (!listProduct.Contains(temp.intAwardIDs[t]))
                    {
                        Debug.LogError("副本" + table.listItem[i].intGameDungeonID + ":" + j + ":" + temp.intAwardIDs[t]);
                    }
                }
            }
        }
    }
}
