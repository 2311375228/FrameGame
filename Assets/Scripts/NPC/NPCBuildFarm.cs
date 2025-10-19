using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBuildFarm : BuildBase
{
    int[] intProductIDs;
    string strBuildName = "农场商人";
    string strRefreshTime = "每月1号补充";

    BackpackGrid[] productGrids;

    //NPC在建筑列表里面,当点击市场跳转时再加载相关

    ViewMGToViewNPCBuy viewToBuy = new ViewMGToViewNPCBuy();
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    public override void OnStart()
    {
        if (productGrids == null)
        {
            CreateCrops();
        }

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate messageData = message as MessageDate;
        if (messageData != null)
        {
            if (messageData.numDay == 1)
            {
                CreateCrops();
                if (booBuildToView)
                {
                    viewToBuy.equipmentScrolls = productGrids;
                    viewToBuy.booRefreshData = true;
                    ManagerView.Instance.SetData(EnumView.ViewNPCBuy, viewToBuy);
                }
            }
        }
    }

    /// <summary>
    /// 创建农场作物
    /// </summary>
    void CreateCrops()
    {
        if (intProductIDs == null)
        {
            List<int> listIDs = new List<int>();
            for (int i = 10100; i <= 10106; i++)
            {
                listIDs.Add(i);
            }
            for (int i = 20001; i <= 20009; i++)
            {
                listIDs.Add(i);
            }
            listIDs.Add(20014);
            intProductIDs = listIDs.ToArray();
        }

        productGrids = new BackpackGrid[intProductIDs.Length];
        for (int i = 0; i < productGrids.Length; i++)
        {
            productGrids[i] = new BackpackGrid();
            ManagerValue.SetProductItem(intProductIDs[i], productGrids[i]);
            productGrids[i].intCount = 150;
            if (productGrids[i].intPrice < 5)
            {
                productGrids[i].intPrice = (productGrids[i].intPrice + 1);
            }
            else
            {
                productGrids[i].intPrice = (int)(productGrids[i].intPrice * 1.2f);
            }
        }
    }
    public override string GameSaveData()
    {
        string strSave = "";
        for (int i = 0; i < productGrids.Length; i++)
        {
            if (productGrids[i] == null)
            {
                strSave += 0;
            }
            else
            {
                strSave += 1;
            }
        }
        return strSave;
    }
    public override void GameReadData(string strData)
    {
        CreateCrops();
        for (int i = 0; i < productGrids.Length; i++)
        {
            if (strData[i] == '0')
            {
                productGrids[i] = null;
            }
        }
    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        viewToBuy.intIndexGround = GetIndexGround;
        viewToBuy.equipmentScrolls = productGrids;
        viewToBuy.strBuildName = ManagerLanguage.Instance.GetWord(EnumLanguageWords.FarmMerchant);//strBuildName;
        viewToBuy.strRefreshTime = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.RestockOTFDOTM, null);//strRefreshTime;
        viewToBuy.booRefreshData = true;

        ManagerView.Instance.Show(EnumView.ViewNPCBuy);
        ManagerView.Instance.SetData(EnumView.ViewNPCBuy, viewToBuy);
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        MGViewToBuildNPCBuy mg = toGround as MGViewToBuildNPCBuy;
        if (mg != null)
        {
            booBuildToView = mg.booShow;
            if (mg.intBuyEquipmentID != -1)
            {
                BackpackGrid item = productGrids[mg.intBuyEquipmentID];
                viewToBuy.booRefreshData = false;

                int intPrice = item.intPrice * item.intCount;

                //if (intPrice > UserValue.Instance.GetCoin)
                //{
                //    ManagerValue.actionAudio(EnumAudio.Unable);
                //    hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.InsufficientGUTMTP, null);//"金币不够,无法购买";
                //    ManagerView.Instance.Show(EnumView.ViewHintBar);
                //    ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                //    return;
                //}
                if (!UserValue.Instance.KnapsackProductAddGrid(item))
                {
                    ManagerValue.actionAudio(EnumAudio.Unable);
                    hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.NotEBSUTMTP, null);//"背包空间不足,无法购买";
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                    return;
                }

                if (UserValue.Instance.SetCoinReduce(intPrice))
                {
                    ManagerValue.actionAudio(EnumAudio.CoinBuy);
                    productGrids[mg.intBuyEquipmentID] = null;
                    ManagerView.Instance.SetData(EnumView.ViewNPCBuy, viewToBuy);
                    ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
                }
            }
        }
    }

    protected override void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }
}
