using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserValue
{
    static UserValue _instance = new UserValue();
    public static UserValue Instance
    {
        get
        {
            return _instance;
        }
    }
    private UserValue()
    { }

    long _longCoinMax;
    long _numCoin = 100000000;
    public long GetCoin
    {
        get
        {
            return _numCoin;
        }
    }
    public long SetCoid
    {
        set
        {
            _numCoin = value;
        }
    }
    public long SetCoinAdd
    {
        set
        {
            _numCoin += value;
            if (_numCoin > _longCoinMax)
            {
                _numCoin = _longCoinMax;
                ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
                // "需要增加钱仓的容量";
                hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.NeedTITCOGC, null);
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
            }
        }
    }

    public bool SetCoinReduce(long intCoin)
    {
        _numCoin -= intCoin;
        if (_numCoin < 0)
        {
            ManagerView.Instance.Show(EnumView.ViewBankrupt);
        }
        return true;
    }


    public long SetCoinMax
    {
        set
        {
            _longCoinMax = value;
        }
    }

    public long GetCoinMax
    {
        get
        {
            return _longCoinMax;
        }
    }
    public void CoinMaxAdd(long longMax)
    {
        _longCoinMax += longMax;
        ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
    }
    public void CoinMaxReduce(long longMax)
    {
        _longCoinMax -= longMax;
        ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
    }

    public string GetGameModeName(EnumGameMode key)
    {
        switch (key)
        {
            case EnumGameMode.GuideMode://教程模式
                return "教程模式";
            case EnumGameMode.NoviceMode://种田模式
                return "种田模式";
            case EnumGameMode.NormalMode://均衡模式
                return "均衡模式";
            case EnumGameMode.DifficultMode://战斗模式
                return "战斗模式";
        }
        return null;
    }

    //用户昵称
    string _strNickname;
    public string SetNickname { set { _strNickname = value; } }
    public string GetNickname { get { return _strNickname; } }

    Dictionary<EnumColorType, string> dicColor;
    public string GetStrColor(EnumColorType key, string strContent)
    {
        if (dicColor == null)
        {
            dicColor = new Dictionary<EnumColorType, string>();
            dicColor.Add(EnumColorType.cyan, "#00FFFF");
            dicColor.Add(EnumColorType.DarkOrange, "#F88017");
            dicColor.Add(EnumColorType.Gray, "#808080");
            dicColor.Add(EnumColorType.Green, "#008000");
            dicColor.Add(EnumColorType.magenta, "#FF00FF");
            dicColor.Add(EnumColorType.Purple, "#800080");
            dicColor.Add(EnumColorType.White, "#FFFFFF");
            dicColor.Add(EnumColorType.Yellow, "#FFFF00");
            dicColor.Add(EnumColorType.Red, "#FF0000");
        }
        if (dicColor.ContainsKey(key))
        {
            strContent = "<color=" + dicColor[key] + ">" + strContent + "</color>";
        }
        return strContent;
    }

    Dictionary<EnumColorType, Color32> dicImageColor;
    public Color32 GetImageColor(EnumColorType key)
    {
        if (dicImageColor == null)
        {
            dicImageColor = new Dictionary<EnumColorType, Color32>();

            dicImageColor.Add(EnumColorType.Green, new Color32(0, 120, 116, 255));
            dicImageColor.Add(EnumColorType.Yellow, new Color32(255, 255, 0, 255));
            dicImageColor.Add(EnumColorType.Red, new Color32(255, 0, 0, 255));
            dicImageColor.Add(EnumColorType.Mint, new Color32(62, 180, 137, 255));
            dicImageColor.Add(EnumColorType.Gray, new Color32(180, 180, 180, 255));
            dicImageColor.Add(EnumColorType.Brown, new Color32(100, 65, 23, 255));
            dicImageColor.Add(EnumColorType.Black, new Color32(40, 40, 40, 255));
        }
        return dicImageColor[key];
    }

    List<ViewBarTop_ItemMail.Mail> _listMail;

    public ViewBarTop_ItemMail.Mail[] GetMailAll()
    {
        return _listMail.ToArray();
    }

    public void MailAdd(ViewBarTop_ItemMail.Mail mail)
    {
        if (_listMail == null)
        {
            _listMail = new List<ViewBarTop_ItemMail.Mail>();
        }

        if (1500 < _listMail.Count)
        {
            _listMail.Add(mail);
            if (1000 < _listMail.Count)
            {
                ViewHint.MessageHint temp = new ViewHint.MessageHint();
                // "已经有大量邮件堆积,请尽快处理！";
                temp.strHint = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.ThereIAALAOEAP, null);
                ManagerView.Instance.Show(EnumView.ViewHint);
                ManagerView.Instance.SetData(EnumView.ViewHint, temp);
            }
        }
        else
        {
            ViewHint.MessageHint temp = new ViewHint.MessageHint();
            //"由于邮箱容量有限,新收邮件已经清除,请整理邮箱!";
            temp.strHint = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.DueTLMCNREH, null);
            ManagerView.Instance.Show(EnumView.ViewHint);
            ManagerView.Instance.SetData(EnumView.ViewHint, temp);
        }
    }

    public List<PropertiesTask> listTask = new List<PropertiesTask>();
    public Dictionary<int, PropertiesDungeon> dicDungeon = new Dictionary<int, PropertiesDungeon>();

    #region 背包

    //当前使用的背包ID
    EnumKnapsackType _enumKnapsackType;
    public EnumKnapsackType GetEnumKnapsackType { get { return _enumKnapsackType; } }
    public EnumKnapsackType SetEnumKnapsackType { set { _enumKnapsackType = value; } }

    // 一个背包最大容纳的产品个数
    int intKnapsackCountMax = 160;
    //产品格子存储限制
    Dictionary<EnumKnapsackType, BackpackGrid[]> _dicProductData;
    void InitKnapsack()
    {
        _dicProductData = new Dictionary<EnumKnapsackType, BackpackGrid[]>
        {
            { EnumKnapsackType.Backpack_1, new BackpackGrid[intKnapsackCountMax] },
            { EnumKnapsackType.Backpack_2, new BackpackGrid[intKnapsackCountMax] }
        };

        foreach (BackpackGrid[] temp in _dicProductData.Values)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = new BackpackGrid();
                temp[i].intIndex = i;
                temp[i].enumStockType = EnumKnapsackStockType.None;
            }
        }
    }

    /// <summary>
    /// 获取指定背包所有格子信息
    /// </summary>
    public BackpackGrid[] GetKnapsackGrids(EnumKnapsackType key)
    {
        List<BackpackGrid> listTemp = new List<BackpackGrid>();
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        for (int i = 0; i < _dicProductData[key].Length; i++)
        {
            listTemp.Add(_dicProductData[key][i]);
        }
        return listTemp.ToArray();
    }

    /// <summary>
    /// 获取所有背包格子信息
    /// </summary>
    public BackpackGrid[] GetKnapsackItems()
    {
        if (_dicProductData == null)
        {
            InitKnapsack();
        }

        List<BackpackGrid> listTemp = new List<BackpackGrid>();
        for (int i = 0; i < _dicProductData[EnumKnapsackType.Backpack_1].Length; i++)
        {
            if (_dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType != EnumKnapsackStockType.None)
            {
                listTemp.Add(_dicProductData[EnumKnapsackType.Backpack_1][i]);
            }
        }
        for (int i = 0; i < _dicProductData[EnumKnapsackType.Backpack_2].Length; i++)
        {
            if (_dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType != EnumKnapsackStockType.None)
            {
                listTemp.Add(_dicProductData[EnumKnapsackType.Backpack_2][i]);
            }
        }
        return listTemp.ToArray();
    }

    /// <summary>
    /// 获取指定背包里的存在物品数据集合
    /// </summary>
    public BackpackGrid[] GetKnapsackItems(EnumKnapsackType key)
    {
        List<BackpackGrid> listTemp = new List<BackpackGrid>();
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        for (int i = 0; i < _dicProductData[key].Length; i++)
        {
            if (_dicProductData[key][i].enumStockType != EnumKnapsackStockType.None)
            {
                listTemp.Add(_dicProductData[key][i]);
            }
        }
        return listTemp.ToArray();
    }

    /// <summary>
    /// 获取指定类型的背包数据
    /// </summary>
    public BackpackGrid[] GetKnapsackTarget(EnumKnapsackStockType key)
    {
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        List<BackpackGrid> listTemp = new List<BackpackGrid>();
        for (int i = 0; i < _dicProductData[EnumKnapsackType.Backpack_1].Length; i++)
        {
            if (_dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType != EnumKnapsackStockType.None)
            {
                if (_dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == key)
                {
                    listTemp.Add(_dicProductData[EnumKnapsackType.Backpack_1][i]);
                }
            }
            if (_dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType != EnumKnapsackStockType.None)
            {
                if (_dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == key)
                {
                    listTemp.Add(_dicProductData[EnumKnapsackType.Backpack_2][i]);
                }
            }
        }
        return listTemp.ToArray();
    }

    public BackpackGrid[] GetKnapsackTarget(EnumKnapsackStockType[] keys)
    {
        List<BackpackGrid> listTemp = new List<BackpackGrid>();
        for (int i = 0; i < keys.Length; i++)
        {
            BackpackGrid[] temp = GetKnapsackTarget(keys[i]);
            for (int j = 0; j < temp.Length; j++)
            {
                listTemp.Add(temp[j]);
            }
        }
        return listTemp.ToArray();
    }

    /// <summary>
    /// 整理背包顺序
    /// </summary>
    public void KnapasckTidy(EnumKnapsackType key)
    {
        //一种是可叠加的,一种是一个就占用一个格子的
        //临时存储已经存在ID
        List<BackpackGrid> listGridEquipment = new List<BackpackGrid>();
        List<BackpackGrid> listGridEquipmentScroll = new List<BackpackGrid>();//卷轴
        List<BackpackGrid> listGridProduct = new List<BackpackGrid>();
        BackpackGrid tempGrid = null;
        for (int i = 0; i < _dicProductData[key].Length; i++)
        {
            if (_dicProductData[key][i].enumStockType != EnumKnapsackStockType.None)
            {
                if (_dicProductData[key][i].enumStockType == EnumKnapsackStockType.Sword
                    || _dicProductData[key][i].enumStockType == EnumKnapsackStockType.Bow
                    || _dicProductData[key][i].enumStockType == EnumKnapsackStockType.Wand
                    || _dicProductData[key][i].enumStockType == EnumKnapsackStockType.Armor
                    || _dicProductData[key][i].enumStockType == EnumKnapsackStockType.Shoes)
                {
                    if (_dicProductData[key][i].intRank < 10)
                    {
                        tempGrid = new BackpackGrid();
                        KnapsackItemValueExchange(tempGrid, _dicProductData[key][i]);
                        listGridEquipment.Add(tempGrid);
                    }
                    else
                    {
                        tempGrid = new BackpackGrid();
                        KnapsackItemValueExchange(tempGrid, _dicProductData[key][i]);
                        listGridEquipmentScroll.Add(tempGrid);
                    }
                }
                else
                {
                    tempGrid = new BackpackGrid();
                    KnapsackItemValueExchange(tempGrid, _dicProductData[key][i]);
                    listGridProduct.Add(tempGrid);
                }
            }
        }

        List<BackpackGrid> listGridOrder = new List<BackpackGrid>();
        //产品排序
        for (int i = 0; i < listGridProduct.Count; i++)
        {
            for (int j = 0; j < listGridProduct.Count; j++)
            {
                if (listGridProduct[j].intID > listGridProduct[i].intID)
                {
                    tempGrid = listGridProduct[j];
                    listGridProduct[j] = listGridProduct[i];
                    listGridProduct[i] = tempGrid;
                }
            }
        }
        if (listGridProduct.Count > 0)
        {
            //堆叠
            int intTemp = listGridProduct[0].intID;
            for (int i = 1; i < listGridProduct.Count; i++)
            {
                if (intTemp == listGridProduct[i].intID)
                {
                    listGridProduct[i - 1].intCount += listGridProduct[i].intCount;
                    listGridProduct.RemoveAt(i);
                    i--;
                }
                else
                {
                    intTemp = listGridProduct[i].intID;
                }
            }
            for (int i = 0; i < listGridProduct.Count; i++)
            {
                do
                {
                    if (listGridProduct[i].intCount > listGridProduct[i].intLimitCount)
                    {
                        listGridProduct[i].intCount -= listGridProduct[i].intLimitCount;
                        tempGrid = new BackpackGrid();
                        KnapsackItemValueExchange(tempGrid, listGridProduct[i]);
                        tempGrid.intCount = listGridProduct[i].intLimitCount;
                        listGridProduct.Insert(i, tempGrid);
                        i++;
                    }
                }
                while (listGridProduct[i].intLimitCount < listGridProduct[i].intCount);

            }
        }

        //装备排序
        for (int i = 0; i < listGridEquipment.Count; i++)
        {
            for (int j = 0; j < listGridEquipment.Count; j++)
            {
                if (listGridEquipment[j].intID > listGridEquipment[i].intID && listGridEquipment[i].intRank < 10)
                {
                    tempGrid = listGridEquipment[j];
                    listGridEquipment[j] = listGridEquipment[i];
                    listGridEquipment[i] = tempGrid;
                }
            }
        }
        //装备卷轴排序
        for (int i = 0; i < listGridEquipment.Count; i++)
        {
            for (int j = 0; j < listGridEquipment.Count; j++)
            {
                if (listGridEquipment[j].intRank > listGridEquipment[i].intRank)
                {
                    tempGrid = listGridEquipment[j];
                    listGridEquipment[j] = listGridEquipment[i];
                    listGridEquipment[i] = tempGrid;
                }
            }
        }

        //重新排序背包
        int intIndexGrid = 0;
        while (listGridEquipment.Count != 0)
        {
            KnapsackItemValueExchange(_dicProductData[key][intIndexGrid], listGridEquipment[0]);
            listGridEquipment.RemoveAt(0);
            intIndexGrid++;
        }
        while (listGridEquipmentScroll.Count != 0)
        {
            KnapsackItemValueExchange(_dicProductData[key][intIndexGrid], listGridEquipmentScroll[0]);
            listGridEquipmentScroll.RemoveAt(0);
            intIndexGrid++;
        }
        while (listGridProduct.Count != 0)
        {
            KnapsackItemValueExchange(_dicProductData[key][intIndexGrid], listGridProduct[0]);
            listGridProduct.RemoveAt(0);
            intIndexGrid++;
        }
        for (int i = intIndexGrid; i < _dicProductData[key].Length; i++)
        {
            _dicProductData[key][i].enumStockType = EnumKnapsackStockType.None;
        }
    }

    void KnapsackItemValueExchange(BackpackGrid item1, BackpackGrid item2)
    {
        item1.intID = item2.intID;
        item1.intPrice = item2.intPrice;
        item1.intRank = item2.intRank;
        item1.intCount = item2.intCount;
        item1.enumStockType = item2.enumStockType;
        item1.icon = item2.icon;
        item1.intLimitCount = item2.intLimitCount;

        item1.product.strInfo = item2.product.strInfo;

        item1.equipment.intEnd = item2.equipment.intEnd;
        item1.equipment.strContent = item2.equipment.strContent;
    }
    /// <summary>
    /// intIndexGrid 如果有指定格子,则同类型填满,没有则加格子
    /// </summary>
    bool KnapsackProductAdd(UserValue.EnumKnapsackType key, int intIndexGrid, BackpackGrid product)
    {
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        if (intIndexGrid != -1)
        {
            for (int i = 0; i < _dicProductData[key].Length; i++)
            {
                if (_dicProductData[key][i].enumStockType != EnumKnapsackStockType.None)
                {
                    _dicProductData[key][i].intCount += product.intCount;
                    return true;
                }
            }
        }

        return false;
    }
    /// <summary>
    /// 背包是否有足够数量的格子
    /// </summary>
    public bool KnapsackCheckGridCount(int intCount)
    {
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        int intTemp = 0;
        foreach (BackpackGrid[] temp in _dicProductData.Values)
        {
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i].enumStockType == EnumKnapsackStockType.None)
                {
                    intTemp++;
                }
                if (intTemp >= intCount)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 查找是否有空格子装物品,有=true,没有=false
    /// </summary>
    public bool KnapsackProductAddGrid(BackpackGrid product, EnumKnapsackType key = EnumKnapsackType.None)
    {
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        UserValue.EnumKnapsackType[] keys = null;
        if (key == EnumKnapsackType.None)
        {
            keys = new UserValue.EnumKnapsackType[]
            {
                UserValue.EnumKnapsackType.Backpack_1,
                UserValue.EnumKnapsackType.Backpack_2,
            };
        }
        else
        {
            keys = new EnumKnapsackType[] { key };
        }

        //先查找足够多的空位
        int intGrid = 0;
        for (int i = 0; i < keys.Length; i++)
        {
            for (int j = 0; j < _dicProductData[keys[i]].Length; j++)
            {
                if (_dicProductData[keys[i]][j].enumStockType == EnumKnapsackStockType.None)
                {
                    intGrid++;
                }
            }
        }
        intGrid = intGrid * product.intLimitCount;
        if (intGrid >= product.intCount)
        {
            while (true)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    for (int j = 0; j < _dicProductData[keys[i]].Length; j++)
                    {
                        if (_dicProductData[keys[i]][j].enumStockType == EnumKnapsackStockType.None)
                        {
                            _dicProductData[keys[i]][j].intCount = 0;
                            if (product.intCount <= product.intLimitCount)
                            {
                                KnapsackItemValueExchange(_dicProductData[keys[i]][j], product);
                                _dicProductData[keys[i]][j].intCount = product.intCount;
                                return true;
                            }
                            product.intCount -= product.intLimitCount;
                            KnapsackItemValueExchange(_dicProductData[keys[i]][j], product);
                            _dicProductData[keys[i]][j].intCount = product.intLimitCount;
                        }
                    }
                }

            }
        }

        return false;
    }

    /// <summary>
    /// 背包 检查产品数量
    /// </summary>
    public bool KnapsackProductChectCount(int intProductID, int intProductCount)
    {
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        int intCount = 0;
        //获取背包的长度
        for (int i = 0; i < _dicProductData[EnumKnapsackType.Backpack_1].Length; i++)
        {
            if (_dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType != EnumKnapsackStockType.None
                && !(_dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Sword
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Bow
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Wand
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Armor
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Shoes))
            {
                if (_dicProductData[EnumKnapsackType.Backpack_1][i].intID == intProductID)
                {
                    intCount += _dicProductData[EnumKnapsackType.Backpack_1][i].intCount;
                }
            }
            if (_dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType != EnumKnapsackStockType.None
                  && !(_dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Sword
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Bow
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Wand
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Armor
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Shoes
                  ))
            {
                if (_dicProductData[EnumKnapsackType.Backpack_2][i].intID == intProductID)
                {
                    intCount += _dicProductData[EnumKnapsackType.Backpack_2][i].intCount;
                }
            }
            if (intCount >= intProductCount)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 获取背包中指定物品数量
    /// </summary>
    public int GetKnapsackProductCount(int intProductID)
    {
        if (_dicProductData == null)
        {
            InitKnapsack();
        }
        int intCount = 0;
        for (int i = 0; i < _dicProductData[EnumKnapsackType.Backpack_1].Length; i++)
        {
            if (_dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType != EnumKnapsackStockType.None
                && !(_dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Sword
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Bow
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Wand
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Armor
                || _dicProductData[EnumKnapsackType.Backpack_1][i].enumStockType == EnumKnapsackStockType.Shoes
                ))
            {
                if (_dicProductData[EnumKnapsackType.Backpack_1][i].intID == intProductID)
                {
                    intCount += _dicProductData[EnumKnapsackType.Backpack_1][i].intCount;
                }
            }
            if (_dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType != EnumKnapsackStockType.None
                  && !(_dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Sword
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Bow
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Wand
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Armor
                  || _dicProductData[EnumKnapsackType.Backpack_2][i].enumStockType == EnumKnapsackStockType.Shoes
                  ))
            {
                if (_dicProductData[EnumKnapsackType.Backpack_2][i].intID == intProductID)
                {
                    intCount += _dicProductData[EnumKnapsackType.Backpack_2][i].intCount;
                }
            }
        }
        return intCount;
    }
    /// <summary>
    /// 减少背包中的物品
    /// </summary>
    public bool KnapsackProductReduce(EnumKnapsackType key, int intGrid, int intProductCount)
    {
        _dicProductData[key][intGrid].intCount -= intProductCount;
        _dicProductData[key][intGrid].enumStockType = _dicProductData[key][intGrid].intCount == 0 ? EnumKnapsackStockType.None : _dicProductData[key][intGrid].enumStockType;
        if (_dicProductData[key][intGrid].enumStockType == EnumKnapsackStockType.None)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// 背包 减少材料
    /// </summary>
    public bool KnapsackProductReduce(int intProductID, int intProductCount)
    {
        if (_dicProductData == null)
        {
            return false;
        }
        //key=格子下标
        List<int>[] dicTemp = new List<int>[] { new List<int>(), new List<int>() };
        int intCount = 0;
        EnumKnapsackType keyTemp = EnumKnapsackType.Backpack_1;
        for (int i = 0; i < _dicProductData[keyTemp].Length; i++)
        {
            if (_dicProductData[keyTemp][i].enumStockType != EnumKnapsackStockType.None
                && _dicProductData[keyTemp][i].intID == intProductID)
            {
                if (_dicProductData[keyTemp][i].intCount >= intProductCount)
                {
                    _dicProductData[keyTemp][i].intCount -= intProductCount;
                    _dicProductData[keyTemp][i].enumStockType = _dicProductData[keyTemp][i].intCount == 0 ? EnumKnapsackStockType.None : _dicProductData[keyTemp][i].enumStockType;
                    return true;
                }
                else
                {
                    intCount += _dicProductData[keyTemp][i].intCount;
                    dicTemp[0].Add(i);
                }
            }
            if (intCount >= intProductCount)
            {
                intCount -= intProductCount;
                for (int j = 0; j < dicTemp[0].Count; j++)
                {
                    if (j == dicTemp[0].Count - 1)
                    {
                        _dicProductData[keyTemp][dicTemp[0][j]].intCount = intCount;
                        _dicProductData[keyTemp][dicTemp[0][j]].enumStockType = intCount == 0 ? EnumKnapsackStockType.None : _dicProductData[keyTemp][dicTemp[0][j]].enumStockType;
                        continue;
                    }
                    _dicProductData[keyTemp][dicTemp[0][j]].enumStockType = EnumKnapsackStockType.None;
                }
                return true;
            }
        }
        keyTemp = EnumKnapsackType.Backpack_2;
        for (int i = 0; i < _dicProductData[keyTemp].Length; i++)
        {
            if (_dicProductData[keyTemp][i].enumStockType != EnumKnapsackStockType.None
                && _dicProductData[keyTemp][i].intID == intProductID)
            {
                if (_dicProductData[keyTemp][i].intID >= intProductCount)
                {
                    _dicProductData[keyTemp][i].intCount -= intProductCount;
                    _dicProductData[keyTemp][i].enumStockType = _dicProductData[keyTemp][i].intCount == 0 ? EnumKnapsackStockType.None : _dicProductData[keyTemp][i].enumStockType;
                    return true;
                }
                else
                {
                    intCount += _dicProductData[keyTemp][i].intCount;
                    dicTemp[1].Add(i);
                }
            }
            if (intCount >= intProductCount)
            {
                intCount -= intProductCount;
                for (int j = 0; j < dicTemp[0].Count; j++)
                {
                    _dicProductData[keyTemp][dicTemp[0][j]].enumStockType = EnumKnapsackStockType.None;
                }
                for (int j = 0; j < dicTemp[1].Count; j++)
                {
                    if (j == dicTemp[1].Count - 1)
                    {
                        _dicProductData[keyTemp][dicTemp[1][j]].intCount = intCount;
                        _dicProductData[keyTemp][dicTemp[1][j]].enumStockType = _dicProductData[keyTemp][i].intCount == 0 ? EnumKnapsackStockType.None : _dicProductData[keyTemp][dicTemp[1][j]].enumStockType;
                        continue;
                    }
                    _dicProductData[keyTemp][dicTemp[1][j]].enumStockType = EnumKnapsackStockType.None;
                }
                return true;
            }
        }

        return false;
    }

    #endregion

    #region 生产仓库

    /// <summary>
    /// 已经存储了多少
    /// </summary>
    Dictionary<int, FarmClass.StockCount> dicStockCount = new Dictionary<int, FarmClass.StockCount>();
    /// <summary>
    /// 消耗量
    /// </summary>
    Dictionary<int, FarmClass.StockExpend> dicStockExpend = new Dictionary<int, FarmClass.StockExpend>();
    /// <summary>
    /// 仓库的容量
    /// </summary>
    Dictionary<int, FarmClass.StockProduction> dicStockProduction = new Dictionary<int, FarmClass.StockProduction>();

    public Dictionary<int, FarmClass.StockExpend> GetStockExpend()
    {
        return dicStockExpend;
    }
    public Dictionary<int, FarmClass.StockCount> GetStockCountAll()
    {
        return dicStockCount;
    }
    public Dictionary<int, FarmClass.StockProduction> GetStockProduction()
    {
        return dicStockProduction;
    }
    public FarmClass.StockProduction[] GetStockProductionOrder()
    {
        List<FarmClass.StockProduction> listStockProduction = dicStockProduction.Values.ToList();
        for (int i = 0; i < listStockProduction.Count; i++)
        {
            for (int j = 0; j < listStockProduction.Count; j++)
            {
                if (listStockProduction[i].intIndex > listStockProduction[j].intIndex)
                {
                    FarmClass.StockProduction item = listStockProduction[i];
                    listStockProduction[i] = listStockProduction[j];
                    listStockProduction[j] = item;
                }
            }
        }
        return listStockProduction.ToArray();
    }

    /// <summary>
    /// 这里设置消耗,生产量
    /// </summary>
    public void UpdateStock()
    {
        dicStockProduction.Clear();
        dicStockExpend.Clear();
        int intIndex = 0;
        //_dicBuildProductionSee key=intBuildID   value=intInIndexGround
        foreach (BuildBase temp in _dicBuild.Values)
        {
            //因为要拿到具体建筑的状态,所以要拿每一块地上建筑的生产值
            PropertiesGround ground = _dicGround[temp.GetIndexGround];
            if (ground.GetState == EnumGroudState.BuildingPruchased)
            {
                BuildBaseFarm farm = ground.buildBase as BuildBaseFarm;
                BuildBasePasture pasture = ground.buildBase as BuildBasePasture;
                BuildSaleShop saleShop = ground.buildBase as BuildSaleShop;

                if (farm != null)
                {
                    if (!dicStockProduction.ContainsKey(farm.intFarmProductID))
                    {
                        dicStockProduction.Add(farm.intFarmProductID, new FarmClass.StockProduction());
                        dicStockProduction[farm.intFarmProductID].intProductID = farm.intFarmProductID;
                        dicStockProduction[farm.intFarmProductID].intIndex = intIndex++;

                        if (!dicStockCount.ContainsKey(farm.intFarmProductID))
                        {
                            dicStockCount.Add(farm.intFarmProductID, new FarmClass.StockCount());
                        }
                    }

                    dicStockProduction[farm.intFarmProductID].intTotalRipeCount += farm.intFarmProductCountsing;
                    dicStockProduction[farm.intFarmProductID].intTotalRipeDay += farm.intFarmRipeDay;
                    JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(farm.intFarmProductID);
                    dicStockProduction[farm.intFarmProductID].intStockMax += itemProduct.intStockMax;
                    dicStockProduction[farm.intFarmProductID].intBuildCount += 1;
                }
                if (pasture != null)
                {
                    JsonValue.DataTableCompoundItem[] intPastureProductIDs = pasture.GetIntProductIDs;
                    for (int i = 0; i < intPastureProductIDs.Length; i++)
                    {
                        if (!dicStockProduction.ContainsKey(intPastureProductIDs[i].intProductID))
                        {
                            dicStockProduction.Add(intPastureProductIDs[i].intProductID, new FarmClass.StockProduction());
                            dicStockProduction[intPastureProductIDs[i].intProductID].intProductID = intPastureProductIDs[i].intProductID;
                            dicStockProduction[intPastureProductIDs[i].intProductID].intIndex = intIndex++;


                            if (!dicStockCount.ContainsKey(intPastureProductIDs[i].intProductID))
                            {
                                dicStockCount.Add(intPastureProductIDs[i].intProductID, new FarmClass.StockCount());
                            }
                        }

                        JsonValue.DataTableBackPackItem itemProduct = ManagerProduct.Instance.GetProductTableItem(intPastureProductIDs[i].intProductID);
                        dicStockProduction[intPastureProductIDs[i].intProductID].intStockMax += itemProduct.intStockMax;
                    }
                    dicStockProduction[pasture.intPastureProductID].intTotalRipeCount += pasture.intPastureProductCount;
                    dicStockProduction[pasture.intPastureProductID].intTotalRipeDay += pasture.intPastureRipeDay;
                    dicStockProduction[pasture.intPastureProductID].intBuildCount += 1;

                    //消耗品
                    for (int i = 0; i < pasture.IntProductExpendIDs.Length; i++)
                    {
                        if (!dicStockProduction.ContainsKey(pasture.IntProductExpendIDs[i]))
                        {
                            dicStockProduction.Add(pasture.IntProductExpendIDs[i], new FarmClass.StockProduction());
                            dicStockProduction[pasture.IntProductExpendIDs[i]].intProductID = pasture.IntProductExpendIDs[i];
                            dicStockProduction[pasture.IntProductExpendIDs[i]].intIndex = intIndex++;

                            if (!dicStockCount.ContainsKey(pasture.IntProductExpendIDs[i]))
                            {
                                dicStockCount.Add(pasture.IntProductExpendIDs[i], new FarmClass.StockCount());
                            }
                        }
                        if (!dicStockExpend.ContainsKey(pasture.IntProductExpendIDs[i]))
                        {
                            dicStockExpend.Add(pasture.IntProductExpendIDs[i], new FarmClass.StockExpend());
                        }
                    }
                    //消耗品种类中,工业部分不包含在牧园中,
                    dicStockExpend[pasture.intPastureExpendProductID].intDayExpend += pasture.intPastureExpendProductCount;
                }
                if (saleShop != null)
                {
                    //消耗品
                    if (!dicStockProduction.ContainsKey(saleShop.intPastureExpendProductID))
                    {
                        dicStockProduction.Add(saleShop.intPastureExpendProductID, new FarmClass.StockProduction());
                        dicStockProduction[saleShop.intPastureExpendProductID].intProductID = saleShop.intPastureExpendProductID;
                        dicStockProduction[saleShop.intPastureExpendProductID].intIndex = intIndex++;

                        if (!dicStockCount.ContainsKey(saleShop.intPastureExpendProductID))
                        {
                            dicStockCount.Add(saleShop.intPastureExpendProductID, new FarmClass.StockCount());
                        }
                    }
                    if (!dicStockExpend.ContainsKey(saleShop.intPastureExpendProductID))
                    {
                        dicStockExpend.Add(saleShop.intPastureExpendProductID, new FarmClass.StockExpend());
                    }
                    if (dicStockExpend.ContainsKey(saleShop.intPastureExpendProductID))
                    {
                        //消耗品种类中,工业部分不包含在牧园中,
                        dicStockExpend[saleShop.intPastureExpendProductID].intDayExpend += saleShop.intPastureExpendProductCount;
                    }
                }
            }
        }

        foreach (KeyValuePair<int, FarmClass.StockCount> temp in dicStockCount)
        {
            if (!dicStockProduction.ContainsKey(temp.Key))
            {
                dicStockProduction.Add(temp.Key, new FarmClass.StockProduction());
                dicStockProduction[temp.Key].intProductID = temp.Key;
                dicStockProduction[temp.Key].intIndex = intIndex++;
            }
        }

        List<int> listProductID = new List<int>();
        foreach (FarmClass.StockCount temp in dicStockCount.Values)
        {
            if (!dicStockProduction.ContainsKey(temp.intProductID))
            {
                listProductID.Add(temp.intProductID);
            }
        }
        for (int i = 0; i < listProductID.Count; i++)
        {
            dicStockCount.Remove(listProductID[i]);
        }
    }

    /// <summary>
    /// 增加仓储容量,可能之前不包含
    /// true : 有容量,且已经存储
    /// false : 没有容量,没有存储
    /// </summary>
    public bool StockCountAdd(int intProductID, int intCount)
    {
        //建筑中不包含该类型,则代表没有仓储容量
        if (!dicStockProduction.ContainsKey(intProductID))
        {
            return false;
        }
        if (!dicStockCount.ContainsKey(intProductID))
        {
            return false;
        }
        if (dicStockProduction[intProductID].intStockMax >= dicStockCount[intProductID].intStockCount + intCount)
        {
            dicStockCount[intProductID].intStockCount += intCount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// true : 仓储足够,并正常减少
    /// false: 仓储不足,不能减少
    /// </summary>
    public bool StockCountReduce(int intProductID, int intCount)
    {
        //仓库中没有该物品,不能减少
        if (!dicStockProduction.ContainsKey(intProductID))
        {
            return false;
        }
        if (!dicStockCount.ContainsKey(intProductID))
        {
            dicStockCount.Add(intProductID, new FarmClass.StockCount());
        }

        if (dicStockCount[intProductID].intStockCount < intCount)
        {
            return false;
        }
        dicStockCount[intProductID].intStockCount -= intCount;
        return true;
    }

    #endregion

    #region 员工

    //员工不能被自动解雇
    int intIndexEmployee = 1;

    Dictionary<int, PropertiesEmployee> dicEmployee = new Dictionary<int, PropertiesEmployee>();
    Dictionary<int, PropertiesEmployee> dicGuest = new Dictionary<int, PropertiesEmployee>();
    //回收ID
    List<int> listEmployeeID = new List<int>();

    public Dictionary<int, PropertiesEmployee> GetEmployeeAll()
    {
        return dicEmployee;
    }

    public Dictionary<int, PropertiesEmployee> GetEmployeeGuestAll()
    {
        return dicGuest;
    }

    public List<int> GetEmployeeIndexAll()
    {
        return listEmployeeID;
    }

    public PropertiesEmployee GetEmployeeValue(int intIndexID)
    {
        if (dicEmployee.ContainsKey(intIndexID))
        {
            return dicEmployee[intIndexID];
        }
        return null;
    }

    public PropertiesEmployee GetEmployeeValueGuest(int intIndexID)
    {
        if (dicGuest.ContainsKey(intIndexID))
        {
            return dicGuest[intIndexID];
        }
        return null;
    }

    public PropertiesEmployee AddEmployee()
    {
        PropertiesEmployee proTemp = AddEmployeeItem();

        if (listEmployeeID.Count > 0)
        {
            if (!dicEmployee.ContainsKey(proTemp.intIndexID))
            {
                proTemp.intIndexID = listEmployeeID[0];
                listEmployeeID.RemoveAt(0);
                dicEmployee.Add(proTemp.intIndexID, proTemp);
                return proTemp;
            }
        }
        dicEmployee.Add(proTemp.intIndexID, proTemp);
        intIndexEmployee++;
        return proTemp;
    }

    public PropertiesEmployee AddGuest()
    {
        PropertiesEmployee proTemp = AddEmployeeItem();
        if (listEmployeeID.Count > 0)
        {
            if (!dicGuest.ContainsKey(proTemp.intIndexID))
            {
                proTemp.intIndexID = listEmployeeID[0];
                listEmployeeID.RemoveAt(0);
                dicGuest.Add(proTemp.intIndexID, proTemp);
                return proTemp;
            }
        }
        dicGuest.Add(proTemp.intIndexID, proTemp);
        intIndexEmployee++;
        return proTemp;
    }

    public PropertiesEmployee AddEmployeeRead()
    {
        return AddEmployeeItem();
    }

    public void SetEmployeeRead(PropertiesEmployee employee)
    {
        if (intIndexEmployee <= employee.intIndexID)
        {
            intIndexEmployee = employee.intIndexID + 1;
        }
        dicEmployee.Add(employee.intIndexID, employee);
    }

    public void SetEmployeeGuestRead(PropertiesEmployee employee)
    {
        if (intIndexEmployee <= employee.intIndexID)
        {
            intIndexEmployee = employee.intIndexID + 1;
        }
        dicGuest.Add(employee.intIndexID, employee);
    }

    /// <summary>
    /// 客人回收站,解除已经在队列里的客人
    /// </summary>
    public void RecyclingStationGuest(int intEmployeeID)
    {
        if (dicGuest.ContainsKey(intEmployeeID))
        {
            listEmployeeID.Add(intEmployeeID);
            dicGuest[intEmployeeID].enumState = EnumEmployeeState.Delete;
            dicGuest.Remove(intEmployeeID);
        }
    }
    /// <summary>
    /// 员工回收站,解除已经在队列里的员工
    /// </summary>
    public void RecyclingStationEmployee(int intEmployeeID)
    {
        if (dicEmployee.ContainsKey(intEmployeeID))
        {
            listEmployeeID.Add(intEmployeeID);
            dicEmployee[intEmployeeID].enumState = EnumEmployeeState.Delete;
            dicEmployee.Remove(intEmployeeID);
        }
    }
    /// <summary>
    /// 员工队列转为客人队列
    /// </summary>
    public void SetEmployeeToGuest(int intEmployeeID)
    {
        if (dicEmployee.ContainsKey(intEmployeeID))
        {
            dicGuest.Add(intEmployeeID, dicEmployee[intEmployeeID]);
            dicEmployee.Remove(intEmployeeID);
        }
    }
    /// <summary>
    /// 客人队列转为员工队列
    /// </summary>
    public void SetGuestToEmployee(int intEmployeeID)
    {
        if (dicGuest.ContainsKey(intEmployeeID))
        {
            dicGuest.Add(intEmployeeID, dicGuest[intEmployeeID]);
            dicGuest.Remove(intEmployeeID);
        }
    }

    PropertiesEmployee AddEmployeeItem()
    {
        //员工ID是临时增量下标
        JsonValue.DataTableEmployeeItem item = ManagerEmployee.Instance.GetEmployeeValue();
        PropertiesEmployee proTemp = new PropertiesEmployee();
        proTemp.intIndexID = intIndexEmployee;
        proTemp.intTableEmployeeID = item.intEmployeeID;
        proTemp.intRank = 1;
        proTemp.intIndexGroundWork = -1;
        proTemp.combatAttackType = (EnumCombatAttackType)item.intCombatType;
        proTemp.intCombatTypeRank = item.intCombatTypeRank;
        proTemp.intHP = item.intHP;
        proTemp.intATK = item.intATK;
        proTemp.intMP = item.intMP;
        proTemp.floSpeed = item.intSpeed;
        //proTemp.enumEmployeeProperties = (EnumEmployeeProperties)item.intProperties;
        proTemp.dicEmployeeProperties = new Dictionary<EnumEmployeeProperties, int>();
        proTemp.dicEmployeeProperties.Add(EnumEmployeeProperties.Strengt, item.intStrengt);
        proTemp.dicEmployeeProperties.Add(EnumEmployeeProperties.Agility, item.intAgility);
        proTemp.dicEmployeeProperties.Add(EnumEmployeeProperties.Intellect, item.intIntellect);
        proTemp.dicEmployeeProperties.Add(EnumEmployeeProperties.Stamina, item.intStamina);
        proTemp.dicEmployeeProperties.Add(EnumEmployeeProperties.Versatility, item.intVersatility);
        proTemp.intHP = item.intHP;
        proTemp.intMP = item.intMP;
        proTemp.strEmployeeName = item.strEmployeeChina;
        ManagerValue.ActionRoleTexture(proTemp);
        return proTemp;
    }

    public Dictionary<int, PropertiesEmployee> GetAllEmployee()
    {
        return dicEmployee;
    }
    public int GetIdleEmployeeNum()
    {
        if (dicEmployee.Count == 0)
        {
            return -1;
        }
        int intNum = 0;
        foreach (PropertiesEmployee temp in dicEmployee.Values)
        {
            if (temp.intIndexGroundWork == -1)
            {
                intNum++;
            }
        }
        return intNum;
    }
    #endregion

    #region 装备


    public int GetRandomEquipmentRank()
    {
        int intRandom = Random.Range(0, 10000);
        if (intRandom < 8000)
        {
            return 1;
        }
        else if (intRandom >= 8000 && intRandom < 9500)
        {
            return 2;
        }
        else if (intRandom >= 9500 && intRandom < 9950)
        {
            return 3;
        }
        else if (intRandom >= 9950 && intRandom < 9998)
        {
            return 4;
        }
        else if (intRandom >= 9998)
        {
            return 5;
        }
        return 1;
    }

    #endregion

    #region 土地

    Dictionary<int, List<int>> _dicBuildProductionSee = new Dictionary<int, List<int>>();
    public Dictionary<int, List<int>> GetAllBuildProduceSee()
    {
        return _dicBuildProductionSee;
    }
    /// <summary>
    /// 查看生产 增加生产建筑的产品
    /// </summary>
    public void BuildProductSeeAdd(int intBuildID, int intIndexGround)
    {
        if (!_dicBuildProductionSee.ContainsKey(intBuildID))
        {
            _dicBuildProductionSee.Add(intBuildID, new List<int>());
        }
        _dicBuildProductionSee[intBuildID].Add(intIndexGround);
    }
    /// <summary>
    /// 查看生产 减少生产建筑的产品
    /// </summary>
    public void BuildProductSeeReduce(int intBuildID, int intIndexGround)
    {
        if (_dicBuildProductionSee.ContainsKey(intBuildID))
        {
            _dicBuildProductionSee[intBuildID].Remove(intIndexGround);
            if (_dicBuildProductionSee[intBuildID].Count == 0)
            {
                _dicBuildProductionSee.Remove(intBuildID);
            }
        }
    }

    public Dictionary<int, List<int>> GetBuildProductSeeAll()
    {
        return _dicBuildProductionSee;
    }

    Dictionary<int, PropertiesGround> _dicGround;
    public Dictionary<int, PropertiesGround> GetDicGround
    {
        get { return _dicGround; }
    }
    public Dictionary<int, PropertiesGround> SetDicGround
    {
        set { _dicGround = value; }
    }
    //记录当前建筑类型,以及建筑所在 IndexGround
    Dictionary<int, List<int>> _dicBuildType = new Dictionary<int, List<int>>();
    public Dictionary<int, List<int>> GetAllBuildType()
    {
        return _dicBuildType;
    }
    public void BuildTypeAdd(int intBuildID, int intIndexGround)
    {
        if (!_dicBuildType.ContainsKey(intBuildID))
        {
            _dicBuildType.Add(intBuildID, new List<int>());
        }
        if (!_dicBuildType[intBuildID].Contains(intIndexGround))
        {
            _dicBuildType[intBuildID].Add(intIndexGround);
        }
    }
    public void BuildTypeRemove(int intBuildID, int intIndexGround)
    {
        if (_dicBuildType.ContainsKey(intBuildID))
        {
            _dicBuildType[intBuildID].Remove(intIndexGround);
        }
    }
    public List<int> GetBuildTypeIntGound(int intBuildID)
    {
        if (_dicBuildType.ContainsKey(intBuildID) && _dicBuildType[intBuildID].Count > 0)
        {
            return _dicBuildType[intBuildID];
        }
        return null;
    }

    Dictionary<int, BuildBase> _dicBuild = new Dictionary<int, BuildBase>();
    public void SetBuild(int intIndexGround, BuildBase build)
    {
        BuildTypeAdd(build.GetBuildID, intIndexGround);
        _dicBuild.Add(intIndexGround, build);
        _dicBuild[intIndexGround].OnStart();
    }
    public BuildBase GetBuildValue(int intGround)
    {
        return _dicBuild[intGround];
    }
    public void RemoveBuild(int intGround)
    {
        BuildBase build = _dicBuild[intGround];
        BuildTypeRemove(build.GetBuildID, intGround);
        _dicBuild.Remove(intGround);
    }
    public Dictionary<int, BuildBase> GetAllBuild()
    {
        return _dicBuild;
    }
    #endregion

    public enum EnumColorType
    {
        None,
        White,
        /// <summary>
        /// 青色,蓝绿色
        /// </summary>
        cyan,
        Red,
        /// <summary>
        /// 品红,洋红色
        /// </summary>
        magenta,
        /// <summary>
        /// 深橙色
        /// </summary>
        DarkOrange,
        /// <summary>
        /// 灰色
        /// </summary>
        Gray,
        /// <summary>
        /// 绿色
        /// </summary>
        Green,
        /// <summary>
        /// 黄色
        /// </summary>
        Yellow,
        /// <summary>
        /// 紫色
        /// </summary>
        Purple,
        /// <summary>
        /// 薄荷色
        /// </summary>
        Mint,
        /// <summary>
        /// 棕色
        /// </summary>
        Brown,
        /// <summary>
        /// 黑色
        /// </summary>
        Black,

    }

    public enum EnumKnapsackType
    {
        None,
        Backpack_1,
        Backpack_2,
    }
}
