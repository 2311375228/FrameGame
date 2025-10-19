using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_SaleShopUp : ViewBuild_Base
{
    public Text textInfo;
    public Text textBuildRankTag;
    public Text textBuildRank;

    public Button btnUpRank;
    public Button btnRandomProduct;
    string[] strStatements = new string[3];

    SaleShopRandomToBuild messageRandom = new SaleShopRandomToBuild();
    SaleShopUpgradeToBuild messageUpgrage = new SaleShopUpgradeToBuild();
    protected override void Start()
    {
        btnUpRank.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            SendToGround(messageUpgrage);
        });
        btnRandomProduct.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            SendToGround(messageRandom);
        });
    }
    public override void Show()
    {
        base.Show();

        textBuildRankTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Grade) + ": ";

        btnUpRank.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Upgrade);
        btnRandomProduct.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Random);
    }
    public override void BuildMessage(EventBuildToViewBase message)
    {
        SaleShopToView messageSaleShop = message as SaleShopToView;
        if (messageSaleShop != null)
        {
            textBuildRank.text = messageSaleShop.intRank.ToString();

            strStatements[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, ManagerValue.GetStrEnglishMonth(messageSaleShop.intMonth));
            strStatements[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, messageSaleShop.intDay.ToString());
            strStatements[2] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, messageSaleShop.intSellMaxCount.ToString());
            textInfo.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.OnEYTSWIISV, strStatements);
        }
    }
}
