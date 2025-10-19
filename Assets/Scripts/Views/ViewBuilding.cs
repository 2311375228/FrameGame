using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 建筑选择界面
/// </summary>
public class ViewBuilding : ViewBase
{
    public Button btnClose;
    public Button btnSell;

    public Transform tranMenuTool;
    public Transform tranBuildTool;

    List<Text> listBuildName = new List<Text>();
    List<int> listBuildID = new List<int>();

    int numBuildType;//建造类型 1=农园 2=牧场 3=工厂 4=其他

    int intSelectBuildType;
    int numIndexGround;
    string strModelNameOld;
    ViewMGBuyBuild mgBuild;
    protected override void Start()
    {
        base.Start();

        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewBuilding);
        });

        btnSell.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Unable);
            viewHint.actionConfirm = () =>
            {
                MessageDemolishBuild mgDemolishBuild = new MessageDemolishBuild();
                mgDemolishBuild.intIndexGround = numIndexGround;
                ManagerMessage.Instance.PostEvent(EnumMessage.DemolishBuild, mgDemolishBuild);
                ManagerView.Instance.Hide(EnumView.ViewBuilding);
            };
            //"出售这块土地可" + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Land) + "\n+" + ManagerValue.DemolishLandrecycleCoin(ManagerValue.intGroundCount - 1, false) + "金币";
            viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.SellTPOL) + "\n";
            viewHint.strHint += ManagerValue.DemolishLandrecycleCoin(ManagerValue.intGroundCount - 1, false) + ManagerLanguage.Instance.GetWord(EnumLanguageWords.GoldCoins);
            ManagerView.Instance.Show(EnumView.ViewHint);
            ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
        });

        for (int i = 0; i < tranMenuTool.childCount; i++)
        {
            tranMenuTool.GetChild(i).GetComponent<Button>().onClick.AddListener(OnClickMenu(i));
        }

        if (listBuildName.Count == 0)
        {
            for (int i = 0; i < tranBuildTool.childCount; i++)
            {
                tranBuildTool.GetChild(i).GetComponent<Button>().onClick.AddListener(OnClickBuildItem(i));
                listBuildName.Add(tranBuildTool.GetChild(i).GetChild(0).GetComponent<Text>());
            }
        }
        SelectBuild(0);
    }

    public override void Show()
    {
        base.Show();

        for (int i = 0; i < tranMenuTool.childCount; i++)
        {
            switch (i)
            {
                case 0:
                    tranMenuTool.GetChild(i).GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Farm);
                    break;
                case 1:
                    tranMenuTool.GetChild(i).GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Pasture);
                    break;
                case 2:
                    tranMenuTool.GetChild(i).GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Workshop);
                    break;
                case 3:
                    tranMenuTool.GetChild(i).GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Other);
                    break;
            }
        }

        btnSell.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Sell);
        if (listBuildName.Count != 0)
        {
            SelectBuild(intSelectBuildType);
        }
    }


    public override void SetData(Message message)
    {
        mgBuild = message as ViewMGBuyBuild;
        if (mgBuild != null)
        {
            numIndexGround = mgBuild.numIndexGround;
            strModelNameOld = mgBuild.strModelNameNew;
        }
    }


    void SelectBuild(int intIndex)
    {
        intSelectBuildType = intIndex;
        for (int i = 0; i < listBuildName.Count; i++)
        {
            listBuildName[i].transform.parent.gameObject.SetActive(false);
        }
        listBuildID.Clear();
        int intTemp = 0;
        Dictionary<int, JsonValue.DataTableBuildingItem> item = ManagerBuild.Instance.GetBuildAll();
        int[] intOres = new int[] { 1008, 1009, 1010, 1011 };
        foreach (JsonValue.DataTableBuildingItem temp in item.Values)
        {
            if (temp.intBuildID == 4003 || temp.intBuildID == 4002 || temp.intBuildID == 4001 || temp.intBuildID == 3006)//时间紧迫,暂时放弃
            {
                continue;
            }
            if (intOres.Contains(temp.intBuildID))//改为随机矿点,且不可增加或减少
            {
                continue;
            }
            if (temp.intBuildType == intIndex + 1)
            {
                listBuildName[intTemp].transform.parent.gameObject.SetActive(true);
                listBuildName[intTemp].text = ManagerBuild.Instance.GetBuildName(temp.intBuildID);
                listBuildID.Add(temp.intBuildID);

                intTemp++;
            }
        }
    }

    UnityAction OnClickMenu(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            SelectBuild(intIndex);
        };
    }
    UnityAction OnClickBuildItem(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            JsonValue.DataTableBuildingItem buildPr = ManagerBuild.Instance.GetBuildItem(listBuildID[intIndex]);
            ManagerView.Instance.Show(EnumView.ViewBuyHint);

            ViewMGBuyBuild messageBuyBuild = new ViewMGBuyBuild();
            messageBuyBuild.groundState = EnumGroudState.Purchased;//代表该地已经被购买
            messageBuyBuild.view = EnumView.ViewBuilding;
            messageBuyBuild.numPrice = buildPr.numPrice;
            messageBuyBuild.strModelNameOld = strModelNameOld;
            messageBuyBuild.strModelNameNew = buildPr.strModelName;
            messageBuyBuild.numIndexGround = numIndexGround;
            messageBuyBuild.intBuildID = buildPr.intBuildID;
            ManagerView.Instance.SetData(EnumView.ViewBuyHint, messageBuyBuild);
        };
    }

}
