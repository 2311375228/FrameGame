using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBuildFactory : BuildBase
{
    ViewMGToViewNPCBuy viewToBuy = new ViewMGToViewNPCBuy();
    string strBuildName = "装备商人";
    string strRefreshTime = "每年1月1号补充";

    //合成材料都是代码固定的,所以只是标记
    int[] intEquipmentIDs = new int[]
    {
        1001000,
        1001001,
        1001002,
        2001000,
        2001001,
        2001002,
        3001000,
        3001001,
        3001002,
        4001000,
        4001001,
        4001002,
        //5001000,
        //5001001,
        //5001002,

    };
    BackpackGrid[] intEquipmentScrolls;
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    public override void OnStart()
    {
        CreateWeapenScroll();

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate messageData = message as MessageDate;
        if (messageData != null)
        {
            if (messageData.numMonth == 1 && messageData.numDay == 1)
            {
                CreateWeapenScroll();
                if (booBuildToView)
                {
                    viewToBuy.equipmentScrolls = intEquipmentScrolls;
                    viewToBuy.booRefreshData = true;
                    ManagerView.Instance.SetData(EnumView.ViewNPCBuy, viewToBuy);
                }
            }
        }
    }

    /// <summary>
    /// 生成武器卷轴
    /// </summary>
    void CreateWeapenScroll()
    {
        int intIndexTemp = 0;
        intEquipmentScrolls = new BackpackGrid[intEquipmentIDs.Length * 5];
        for (int i = 0; i < intEquipmentIDs.Length; i++)
        {
            for (int j = 11; j <= 15; j++)//单个装备创建5个级别卷轴ICON
            {
                BackpackGrid item = new BackpackGrid();
                ManagerValue.SetEquipmentItem(intEquipmentIDs[i], item);
                item.intRank = j;
                intEquipmentScrolls[intIndexTemp] = item;
                intIndexTemp++;
            }
        }
    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        viewToBuy.intIndexGround = GetIndexGround;
        viewToBuy.equipmentScrolls = intEquipmentScrolls;
        viewToBuy.strBuildName = strBuildName;
        viewToBuy.strRefreshTime = strRefreshTime;
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
                BackpackGrid item = intEquipmentScrolls[mg.intBuyEquipmentID];
                viewToBuy.booRefreshData = false;
                if (item.intPrice > UserValue.Instance.GetCoin)
                {
                    hintBar.strHintBar = "金币不够,无法购买";
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                    return;
                }
                if (!UserValue.Instance.KnapsackProductAddGrid(item))
                {
                    hintBar.strHintBar = "背包空间不足,无法购买";
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                    return;
                }

                if (UserValue.Instance.SetCoinReduce(item.intPrice))
                {
                    intEquipmentScrolls[mg.intBuyEquipmentID] = null;
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
