using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_BuyGround : ViewBuild_Base
{
    public Text textInfo;
    public Text textPrice;
    public Button btnBuy;


    EnumView enumView;
    EnumBuy enumBuy;
    long numPrice;
    int numIndexGround;
    int intBuildID;
    string strModelNameNew;
    string strModelNameOld;

    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    protected override void Start()
    {
        btnBuy.onClick.AddListener(() =>
        {
            if (UserValue.Instance.SetCoinReduce(numPrice))
            {
                ManagerValue.actionAudio(EnumAudio.CoinBuy);
                MessageGround mgGround = new MessageGround();
                mgGround.numIndexGround = numIndexGround;
                mgGround.strModelNameNew = strModelNameNew;
                mgGround.strModelNameOld = strModelNameOld;
                switch (enumBuy)
                {
                    case EnumBuy.Hint_Hinder:
                        mgGround.groundState = EnumGroudState.Unpurchased;
                        break;
                    case EnumBuy.Hint_BuyGround:
                        mgGround.groundState = EnumGroudState.Purchased;

                        break;
                    case EnumBuy.Hint_BuyBuilding:
                        mgGround.groundState = EnumGroudState.BuildingPruchased;
                        mgGround.intBuildID = intBuildID;
                        break;
                }
                ManagerMessage.Instance.PostEvent(EnumMessage.Update_Ground, mgGround);
                ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

            }
            else
            {
                ManagerValue.actionAudio(EnumAudio.MoneyNone);

                hintBar.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.InsufficientCoins);//"金币不足";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
            }

            //enumBuy = EnumBuy.None;
            ManagerView.Instance.Hide(EnumView.ViewBuyHint);
            ManagerView.Instance.Hide(enumView);
            //ManagerMessage.Instance.PostEvent(EnumMessage.Hide_GroundChoice);
        });
    }

    public override void Show()
    {
        base.Show();

        btnBuy.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Purchase);
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        GroundToView mgBuyBuild = message as GroundToView;
        if (mgBuyBuild != null)
        {
            enumView = mgBuyBuild.view;
            numIndexGround = mgBuyBuild.intIndexGround;
            intBuildID = mgBuyBuild.intBuildID;
            strModelNameNew = mgBuyBuild.strModelNameNew;
            strModelNameOld = mgBuyBuild.strModelNameOld;

            textPrice.text = "-" + mgBuyBuild.numPrice.ToString("N0");

            numPrice = mgBuyBuild.numPrice;
            switch (mgBuyBuild.groundState)
            {
                case EnumGroudState.Hinder:
                    enumBuy = EnumBuy.Hint_Hinder;
                    textInfo.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.NeedTSCTCU, null);
                    btnBuy.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Confirm);
                    //textBuildName.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Obstacle);//"障碍物";
                    break;
                case EnumGroudState.Unpurchased:
                    textInfo.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.DoYWTBL);
                    btnBuy.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Purchase);
                    enumBuy = EnumBuy.Hint_BuyGround;
                    ///textBuildName.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Land);//"土地";
                    break;
                case EnumGroudState.Purchased:
                    enumBuy = EnumBuy.Hint_BuyBuilding;
                    //textBuildName.text = ManagerBuild.Instance.GetBuildName(mgBuyBuild.intBuildID);
                    break;
            }
        }
    }
    enum EnumBuy
    {
        None,
        Hint_Hinder,
        Hint_BuyGround,
        Hint_BuyBuilding,
    }
}
