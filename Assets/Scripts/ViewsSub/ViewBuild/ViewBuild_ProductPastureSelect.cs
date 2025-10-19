using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_ProductPastureSelect : ViewBuild_Base
{
    public Text textSelectProduction;
    public Text textSelectProductExpend;

    public Button btnSelectProduct;
    public Button btnClose;

    public RectTransform rectProductions;
    public RectTransform rectExpends;

    int intBuildID;
    int intIndexProduction;
    int intIndexExpend;
    List<View_PropertiesItem> listProduction = new List<View_PropertiesItem>();
    List<View_PropertiesItem> listExpend = new List<View_PropertiesItem>();

    SelectPasture messageSelect = new SelectPasture();
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            gameObject.SetActive(false);
        });
        btnSelectProduct.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            messageSelect.intIndexProduction = intIndexProduction;
            messageSelect.intIndexExpend = intIndexExpend;
            SendToGround(messageSelect);
        });
    }

    public override void Show()
    {
        base.Show();

        if (listProduction.Count == 0)
        {
            for (int i = 0; i < rectProductions.childCount; i++)
            {
                listProduction.Add(rectProductions.GetChild(i).GetComponent<View_PropertiesItem>());
                listProduction[i].GetComponent<Button>().onClick.AddListener(OnClickSelectProduction(i));
            }
            for (int i = 0; i < rectExpends.childCount; i++)
            {
                listExpend.Add(rectExpends.GetChild(i).GetComponent<View_PropertiesItem>());
                listExpend[i].GetComponent<Button>().onClick.AddListener(OnClickSelectExpend(i));
            }
        }
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        EventBuildToViewPasture mgToInfoPasture = message as EventBuildToViewPasture;
        if (mgToInfoPasture != null)
        {
            intIndexProduction = mgToInfoPasture.intIndexProduct;
            intBuildID = mgToInfoPasture.intBuildID;
            intIndexExpend = mgToInfoPasture.intIndexProductExpend;
            PastureSelectExpend(mgToInfoPasture.intIndexProductExpend);
            PastureSelectProduction(mgToInfoPasture.intIndexProduct);

            JsonValue.DataTableBackPackItem resProduct = ManagerProduct.Instance.GetProductTableItem(mgToInfoPasture.intPorductIDs[mgToInfoPasture.intIndexProduct]);
            JsonValue.DataTableCompoundItem itemCompound = ManagerCompound.Instance.GetValue(mgToInfoPasture.intCompoundIDs[mgToInfoPasture.intIndexProduct]);
            JsonValue.DataTableBackPackItem resExpent = ManagerProduct.Instance.GetProductTableItem(mgToInfoPasture.intProductIDExpends[mgToInfoPasture.intIndexProductExpend]);

            //当前牧场产出
            string[] strProductions = null;
            EnumLanguageStatement enumStatement = EnumLanguageStatement.None;
            if (itemCompound.intRipeDay > 1 && itemCompound.intProductCount > 1)
            {
                enumStatement = EnumLanguageStatement.HarvestNumsDays;//
                strProductions = new string[3];
                strProductions[0] = itemCompound.intRipeDay.ToString();
                strProductions[1] = itemCompound.intProductCount.ToString();
                strProductions[2] = ManagerProduct.Instance.GetName(resProduct.intProductID, true);// resProduct.GetName(true);
            }
            else if (itemCompound.intRipeDay > 1 && itemCompound.intProductCount <= 1)
            {
                enumStatement = EnumLanguageStatement.ObtainNumDays;
                strProductions = new string[2];
                strProductions[0] = itemCompound.intRipeDay.ToString();
                strProductions[1] = ManagerProduct.Instance.GetName(resProduct.intProductID, false);//resProduct.GetName(false);
            }
            else if (itemCompound.intRipeDay <= 1 && itemCompound.intProductCount > 1)
            {
                enumStatement = EnumLanguageStatement.HarvestNumsDay;
                strProductions = new string[2];
                strProductions[0] = itemCompound.intProductCount.ToString();
                strProductions[1] = ManagerProduct.Instance.GetName(resProduct.intProductID, true);//resProduct.GetName(true);
            }
            else if (itemCompound.intRipeDay <= 1 && itemCompound.intProductCount <= 1)
            {
                enumStatement = EnumLanguageStatement.ObtainNumDay;
                strProductions = new string[1];
                strProductions[0] = ManagerProduct.Instance.GetName(resProduct.intProductID, false);//resProduct.GetName(false);
            }
            //当前牧场消耗
            if (mgToInfoPasture.intProductExpendNums[mgToInfoPasture.intIndexProductExpend] > 1)
            {
                enumStatement = EnumLanguageStatement.ConsumeNumsDay;//
                strProductions = new string[2];
                strProductions[0] = mgToInfoPasture.intProductExpendNums[mgToInfoPasture.intIndexProductExpend].ToString();
                strProductions[1] = ManagerProduct.Instance.GetName(resExpent.intProductID, true);//resExpent.GetName(true);
            }
            else if (mgToInfoPasture.intProductExpendNums[mgToInfoPasture.intIndexProductExpend] <= 1)
            {
                enumStatement = EnumLanguageStatement.ConsumeNumDay;
                strProductions = new string[1];
                strProductions[0] = ManagerProduct.Instance.GetName(resExpent.intProductID, false);//resExpent.GetName(false);
            }



            //string strProductSelect = resProduct.GetName(false) + "\n每" + itemCompound.intRipeDay + "天生产" + itemCompound.intProductCount;
            textSelectProduction.text = ManagerLanguage.Instance.GetStatement(enumStatement, strProductions);
            for (int i = 0; i < listProduction.Count; i++)
            {
                if (i < mgToInfoPasture.intPorductIDs.Length)
                {
                    listProduction[i].gameObject.SetActive(true);
                    JsonValue.DataTableBackPackItem resProduction = ManagerProduct.Instance.GetProductTableItem(mgToInfoPasture.intPorductIDs[i]);
                    JsonValue.DataTableCompoundItem compoundItem = ManagerCompound.Instance.GetValue(mgToInfoPasture.intCompoundIDs[i]);
                    string strProductionInfo = ManagerProduct.Instance.GetName(resProduction.intProductID, false) + "\n" + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Yield) + ":" + compoundItem.intProductCount;
                    listProduction[i].textValueMain.text = strProductionInfo;
                    listProduction[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(resProduction.strIconName);
                }
                else { listProduction[i].gameObject.SetActive(false); }
            }

            //textSelectProductExpend.text = resExpent.GetName(false) + "\n" + "每天消耗" + mgToInfoPasture.intProductExpendNums[mgToInfoPasture.intIndexProductExpend];
            textSelectProductExpend.text = ManagerLanguage.Instance.GetStatement(enumStatement, strProductions);
            for (int i = 0; i < listExpend.Count; i++)
            {
                if (i < mgToInfoPasture.intProductIDExpends.Length)
                {
                    listExpend[i].gameObject.SetActive(true);
                    JsonValue.DataTableBackPackItem resExpend = ManagerProduct.Instance.GetProductTableItem(mgToInfoPasture.intProductIDExpends[i]);
                    string strExpend = ManagerProduct.Instance.GetName(resExpent.intProductID, false) + "\n" + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Consume) + ":" + mgToInfoPasture.intProductExpendNums[i];
                    listExpend[i].textValueMain.text = strExpend;
                    listExpend[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(resExpend.strIconName);
                }
                else { listExpend[i].gameObject.SetActive(false); }
            }
        }
    }

    UnityEngine.Events.UnityAction OnClickSelectProduction(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intIndexProduction = intIndex;
            PastureSelectProduction(intIndex);
            if (3005 == intBuildID)//矿石冶炼厂
            {
                intIndexExpend = PastureSelectExpend(intIndex);
            }
        };
    }
    UnityEngine.Events.UnityAction OnClickSelectExpend(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intIndexExpend = intIndex;
            PastureSelectExpend(intIndex);
            if (3005 == intBuildID)//矿石冶炼厂
            {
                intIndexProduction = PastureSelectProduction(intIndex);
            }
        };
    }

    public int PastureSelectExpend(int intIndex)
    {
        int intTemp = 0;
        for (int i = 0; i < listExpend.Count; i++)
        {
            if (i == intIndex)
            {
                intTemp = i;
                listExpend[i].imageValue.gameObject.SetActive(true);
                continue;
            }
            listExpend[i].imageValue.gameObject.SetActive(false);
        }
        return intTemp;
    }

    public int PastureSelectProduction(int intIndex)
    {
        int intTemp = 0;
        for (int i = 0; i < listProduction.Count; i++)
        {
            if (i == intIndex)
            {
                intTemp = i;
                listProduction[i].imageValue.gameObject.SetActive(true);
                continue;
            }
            listProduction[i].imageValue.gameObject.SetActive(false);
        }
        return intTemp;
    }
}
