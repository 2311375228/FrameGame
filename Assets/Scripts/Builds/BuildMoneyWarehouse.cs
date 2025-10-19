using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMoneyWarehouse : BuildBase
{
    public Transform transCoinFlag;
    public Transform transBox;

    //一个银行升级满了，存储5千万
    //拆除，建筑正常扣除，升级材料的钱返还10%

    Vector3 vecAxis = new Vector3(0, 1, 0);
    List<GameObject> listBoxCoin = new List<GameObject>();

    int intRank = 1;
    int intExpence = 15000;
    int[] intRanks;
    //金属薄板 钉子 木板
    int[] intUpgradeMaterials = new int[] { 301005, 5, 301006, 100, 20014, 20 };
    ViewBuild_Base.MoneyWarehouseToView messageMoney = new ViewBuild_Base.MoneyWarehouseToView();
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();
    public override void OnStart()
    {
        base.OnStart();

        intRanks = new int[12] {
            16 * (int)Mathf.Pow(10, 5),//160万
            32 * (int)Mathf.Pow(10, 5),//320万
            48 * (int)Mathf.Pow(10, 5),//480万
            64 * (int)Mathf.Pow(10, 5),//640
            80 * (int)Mathf.Pow(10, 5),//800
            96 * (int)Mathf.Pow(10, 5),//960万
            112 * (int)Mathf.Pow(10, 5),//1120万
            128 * (int)Mathf.Pow(10, 5),//1280万
            144 * (int)Mathf.Pow(10, 5),//1440
            166 * (int)Mathf.Pow(10, 5),//1660
            176 * (int)Mathf.Pow(10, 5),//1760
            200 * (int)Mathf.Pow(10, 5),//2000万
        };

        for (int i = 0; i < transBox.childCount; i++)
        {
            listBoxCoin.Add(transBox.GetChild(i).gameObject);
            listBoxCoin[i].SetActive(false);
        }
        for (int i = 0; i < intRank; i++)
        {
            listBoxCoin[i].SetActive(true);
        }

        int intIndexTemp = 0;
        for (int i = 0; i < intUpgradeMaterials.Length / 2; i++)
        {
            int intPrice = ManagerCompound.Instance.GetProductPrice(intUpgradeMaterials[intIndexTemp]);
            intPrice = intPrice * intUpgradeMaterials[intIndexTemp + 1];
            intIndexTemp += 2;
            IntBuildTotalPrice += intPrice;
        }
        IntBuildTotalPrice += (intRank - 1) * intExpence;

        transCoinFlag.RotateAround(transCoinFlag.position, vecAxis, Random.Range(0, 90));
        UserValue.Instance.CoinMaxAdd(intRanks[intRank - 1]);
    }

    private void Update()
    {
        transCoinFlag.RotateAround(transCoinFlag.position, vecAxis, 10 * Time.deltaTime);
    }

    public override void MGViewBuildInfo(MGViewToBuildBase toGround)
    {
        ViewBuild_Base.MoneyWarehouseToBuild messageUpgrade = toGround as ViewBuild_Base.MoneyWarehouseToBuild;
        if (messageUpgrade != null)
        {
            if (intRank < intRanks.Length)
            {
                int intIndexTemp = 0;
                string strTemp = "";
                bool boo = false;

                for (int i = 0; i < intUpgradeMaterials.Length / 2; i++)
                {
                    if (!UserValue.Instance.KnapsackProductChectCount(intUpgradeMaterials[intIndexTemp], intUpgradeMaterials[intIndexTemp + 1]))
                    {
                        boo = true;
                        strTemp += "(" + ManagerProduct.Instance.GetName(intUpgradeMaterials[intIndexTemp], false) + ")";
                    }
                    intIndexTemp += 2;
                }
                if (boo)
                {
                    hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.InsufficientQOTFIITI, null) + strTemp;
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                    return;
                }

                intIndexTemp = 0;
                for (int i = 0; i < intUpgradeMaterials.Length / 2; i++)
                {
                    UserValue.Instance.KnapsackProductReduce(intUpgradeMaterials[intIndexTemp], intUpgradeMaterials[intIndexTemp + 1]);
                    intIndexTemp += 2;
                }
                UserValue.Instance.SetCoinReduce(intExpence);
                ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

                intRank += 1;
                messageMoney.intRank = intRank;
                messageMoney.intRankMax = intRanks.Length;
                messageMoney.intCoin = intRanks[intRank - 1];
                messageMoney.intCoinMax = intRanks[intRanks.Length - 1];
                messageMoney.intUpgradeMaterials = intUpgradeMaterials;
                messageMoney.intExpence = intExpence;

                for (int i = 0; i < intRank; i++)
                {
                    listBoxCoin[i].SetActive(true);
                }
                UserValue.Instance.CoinMaxReduce(intRanks[intRank - 2]);
                UserValue.Instance.CoinMaxAdd(intRanks[intRank - 1]);
            }
            ManagerValue.actionViewBuildMain(messageMoney);
        }

        ViewBuild_Base.DemolishBuild messageDemolishBuild = toGround as ViewBuild_Base.DemolishBuild;
        if (messageDemolishBuild != null)
        {

        }
    }
    public override string GameSaveData()
    {
        return intRank.ToString();
    }
    public override void GameReadData(string strData)
    {
        intRank = int.Parse(strData);
    }

    public override void ShowViewBuildInfo()
    {
        messageMoney.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_MoneyWarehouse;
        messageMoney.enumKeyDown = ViewBuild_Base.EnumViewDown.ViewBuild_MoneyWarehouseDown;
        messageMoney.intIndexGround = GetIndexGround;
        messageMoney.intBuildID = GetBuildID;
        messageMoney.intRank = intRank;
        messageMoney.intRankMax = intRanks.Length;
        messageMoney.intCoin = intRanks[intRank - 1];
        messageMoney.intCoinMax = intRanks[intRanks.Length - 1];
        messageMoney.intUpgradeMaterials = intUpgradeMaterials;
        messageMoney.intExpence = intExpence;
        ManagerView.Instance.Show(EnumView.ViewBuildMain);
        ManagerValue.actionViewBuildMain(messageMoney);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UserValue.Instance.CoinMaxReduce(intRanks[intRank - 1]);
    }
}
