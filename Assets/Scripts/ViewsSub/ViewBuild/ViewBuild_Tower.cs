using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_Tower : ViewBuild_Base
{
    public Text textInfo;
    public Text textHouseRankTag;
    public Text textHouseRank;
    public Text textPersonNumTag;
    public Text textPersonNum;

    protected override void Start()
    {
        textHouseRankTag.gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();
        textPersonNumTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.NumberOfPeople) + ":";
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        TowerToView mgViewTower = message as TowerToView;
        if (mgViewTower != null)
        {
            textPersonNum.text = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, mgViewTower.dicEmployee.Count.ToString());

            string[] strStatements = new string[3];
            strStatements[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ManagerValue.GetStrEnglishMonth(mgViewTower.intMonth));
            strStatements[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, mgViewTower.intDay.ToString());
            strStatements[2] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, mgViewTower.intPersonMax.ToString());
            textInfo.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.AddOEOEYTCB, strStatements);
        }
    }
}
