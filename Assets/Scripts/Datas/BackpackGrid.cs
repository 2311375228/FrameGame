using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackGrid
{
    public int intIndex;
    public bool booSelect;
    /// <summary>
    /// 物品ID与装备ID是不一样的,不会重复
    /// </summary>
    public int intID;
    public int intRank = 1;
    public int intCount;
    public int intPrice;
    public int intLimitCount;
    public string icon;
    public string strName;
    public EnumKnapsackStockType enumStockType;

    public Product product;
    public Equipment equipment;

    public BackpackGrid()
    {
        product = new Product();
        equipment = new Equipment();
    }

    public class Product
    {
        public string strInfo;
    }
    public class Equipment
    {
        public int intEnd;//耐久
        public string strContent;

        public Dictionary<EnumEquipmentCombat, int> dicEquipment = new Dictionary<EnumEquipmentCombat, int>();

    }

    /// <summary>
    /// 装备作用
    /// </summary>
    public enum EnumEquipmentCombat
    {
        None,
        HP,
        ATK,
        MP,
        Speed,
        Critical,//暴击
        End,//耐久
    }
}
