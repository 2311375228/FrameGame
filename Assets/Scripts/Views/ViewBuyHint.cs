using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuyHint : ViewBase
{
    public Text textBuildName;
    public Text textBuyNum;
    public Button btnClose;
    public Button btnBuy;

    EnumView enumView;
    EnumBuy enumBuy;
    long numPrice;
    int numIndexGround;
    int intBuildID;
    string strModelNameNew;
    string strModelNameOld;

    protected override void Start()
    {
        base.Start();

        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            enumBuy = EnumBuy.None;
            ManagerView.Instance.Hide(EnumView.ViewBuyHint);
            ManagerMessage.Instance.PostEvent(EnumMessage.Hide_GroundChoice);
        });
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
                viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.InsufficientCoins);//"金币不足";
                ManagerView.Instance.Show(EnumView.ViewHint);
                ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
            }

            enumBuy = EnumBuy.None;
            ManagerView.Instance.Hide(EnumView.ViewBuyHint);
            ManagerView.Instance.Hide(enumView);
            ManagerMessage.Instance.PostEvent(EnumMessage.Hide_GroundChoice);
        });
    }

    public override void Show()
    {
        base.Show();

        btnBuy.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Confirm);
    }

    public override void SetData(Message message)
    {
        ViewMGBuyBuild mgBuyBuild = message as ViewMGBuyBuild;
        if (mgBuyBuild != null)
        {
            enumView = mgBuyBuild.view;
            numIndexGround = mgBuyBuild.numIndexGround;
            intBuildID = mgBuyBuild.intBuildID;
            strModelNameNew = mgBuyBuild.strModelNameNew;
            strModelNameOld = mgBuyBuild.strModelNameOld;

            textBuyNum.text = "-" + mgBuyBuild.numPrice.ToString("N0");
            numPrice = mgBuyBuild.numPrice;
            switch (mgBuyBuild.groundState)
            {
                case EnumGroudState.Hinder:
                    enumBuy = EnumBuy.Hint_Hinder;
                    textBuildName.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Obstacle);//"障碍物";
                    break;
                case EnumGroudState.Unpurchased:
                    enumBuy = EnumBuy.Hint_BuyGround;
                    textBuildName.text = ManagerLanguage.Instance.GetWord( EnumLanguageWords.Land);//"土地";
                    break;
                case EnumGroudState.Purchased:
                    enumBuy = EnumBuy.Hint_BuyBuilding;
                    textBuildName.text = ManagerBuild.Instance.GetBuildName(mgBuyBuild.intBuildID);
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
