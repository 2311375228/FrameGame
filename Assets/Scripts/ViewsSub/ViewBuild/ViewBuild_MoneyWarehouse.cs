using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_MoneyWarehouse : ViewBuild_Base
{
    public Text textInfo;
    public Text textBuildRankTag;
    public Text textBuildRank;
    public Text textWarehouse;
    public Text textBuildWarehouse;

    string[] strStatement = new string[2];
    protected override void Start()
    {

    }
    public override void Show()
    {
        base.Show();
        textBuildRankTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Grade) + ": ";
        textWarehouse.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Capacity) + ": ";
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        MoneyWarehouseToView messageMoney = message as MoneyWarehouseToView;
        if (messageMoney != null)
        {
            textBuildRank.text = messageMoney.intRank.ToString();
            textBuildWarehouse.text = ManagerValue.GetCoin(messageMoney.intCoin);
            strStatement[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, messageMoney.intRankMax.ToString());
            strStatement[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ManagerValue.GetCoin(messageMoney.intCoinMax));
            textInfo.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.EachUWITCOTW, strStatement);
        }
    }
}
