using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 武器工厂
/// </summary>
public class BuildFactoryEquipment : BuildBase
{
    public Animator anim;

    BackpackGrid forgingGrid;//正在锻造的装备
    int intFinishDay = -1;//完成天数
    int[] intEquipmentIDs = new int[]
    {
        1001000,
        1001001,
        2001000,
        2001001,
        4001000,
        4001001,
        //5001000,
        //5001001,
    };

    //这里会有几件基本武器,剑,法杖,盾牌,鞋子,每种3个,并能强化

    ViewMGToViewEquipment mgEquipment = new ViewMGToViewEquipment();
    EventBuildToViewEquipment mgToViewEquipment = new EventBuildToViewEquipment();
    MessageMail mail = new MessageMail();
    public override void OnStart()
    {
        base.OnStart();
        forgingGrid = new BackpackGrid();
        forgingGrid.intID = -1;

        mail.enumMail = ViewBarTop_ItemMail.EnumMail.Anvil;
        mail.gridItems = new EnumKnapsackStockType[1];
        mail.strContent = "锻造";
        mail.intIndexIDs = new int[1];
        mail.intRanks = new int[1];
        mail.intIndexCounts = new int[1];
    }

    protected override void UpdateDate(MessageDate mgData)
    {
        if (forgingGrid.intID != -1)
        {

            if (intFinishDay-- == 0)
            {
                mail.strContent = "锻造的装备";
                mail.gridItems[0] = forgingGrid.enumStockType;
                mail.intIndexIDs[0] = forgingGrid.intID;
                mail.intRanks[0] = forgingGrid.intRank % 10;
                mail.intIndexCounts[0] = 1;
                ManagerMessage.Instance.PostEvent(EnumMessage.Mail, mail);

                forgingGrid.intID = -1;
                forgingGrid.enumStockType =  EnumKnapsackStockType.None;
                mgEquipment.intResidueTime = -1;
            }
            if (booBuildToView)
            {
                mgEquipment.intResidueTime = intFinishDay;
                ManagerView.Instance.SetData(EnumView.ViewFactoryEquipment, mgEquipment);
            }
        }
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        MGViewToBuildEquipment.Close mgClose = toGround as MGViewToBuildEquipment.Close;
        if (mgClose != null)
        {
            booBuildToView = false;
            return;
        }
        MGViewToBuildEquipment.Forging mgForging = toGround as MGViewToBuildEquipment.Forging;
        if (mgForging != null)
        {
            intFinishDay = mgForging.intFinishDay;
            mgToViewEquipment.intResidueTime = intFinishDay;
            mgToViewEquipment.forgingGrid = forgingGrid;
            mgToViewEquipment.intIndexGround = GetIndexGround;
            mgToViewEquipment.intEquipmentIDs = intEquipmentIDs;
            //是这个类改变View
            ManagerValue.actionViewBuildEquipment(mgToViewEquipment);
            return;
        }
    }

    public override void ShowViewBuildInfo()
    {
        booBuildToView = true;
        mgToViewEquipment.intBuildID = GetBuildID;
        mgToViewEquipment.intResidueTime = intFinishDay;
        mgToViewEquipment.forgingGrid = forgingGrid;
        mgToViewEquipment.intIndexGround = GetIndexGround;
        mgToViewEquipment.intEquipmentIDs = intEquipmentIDs;
        ManagerView.Instance.Show(EnumView.ViewFactoryEquipment);
        ManagerValue.actionViewBuildEquipment(mgToViewEquipment);
    }
}
