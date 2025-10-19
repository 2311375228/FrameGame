using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_PastureProduct : ViewBuild_Base
{
    public Text textProductName;
    public Text textProductCount;
    public Text textResidueTime;
    public Text textResidueTimeScroll;
    public Text textProductionState;
    public Image imageProduct;
    public Button btnAlter;
    public Scrollbar scrollbarResiduTime;

    public Text textProductExpendTag;//消耗标记
    public Text textProductExpend;
    public Image imageProductExpend;

    ViewBuild_ProductPastureSelect select;
    protected override void Start()
    {
        btnAlter.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            select.Show();
        });
    }

    public override void Show()
    {
        base.Show();

        textProductExpendTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Consume);
        if (select == null)
        {
            select = getCentre(EnumViewCentre.ViewBuild_ProductPastureSelect) as ViewBuild_ProductPastureSelect;
            select.SendToGround = SendToGround;
            select.Show();
            select.Hide();
        }

        btnAlter.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Change);
    }
    public override void Hide()
    {
        base.Hide();

        select.Hide();
    }

    public override void HideSub()
    {
        select.Hide();
    }

    public override void SetData(ViewBase.Message message)
    {
        ViewMGToViewInfoPasture mgViewPasture = message as ViewMGToViewInfoPasture;
        if (mgViewPasture != null)
        {
            textResidueTime.text = mgViewPasture.intResidueTime + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Days);
            scrollbarResiduTime.size = (float)((float)mgViewPasture.intResidueTime / (float)mgViewPasture.intRipeDay);
            textResidueTimeScroll.text = mgViewPasture.intResidueTime + "/" + mgViewPasture.intRipeDay;

            SetBuildTipsState(mgViewPasture.buildState);
        }
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        EventBuildToViewPasture mgToInfoPasture = message as EventBuildToViewPasture;
        if (mgToInfoPasture != null)
        {
            SetBuildTipsState(mgToInfoPasture.buildState);
            //正在生产的产品
            JsonValue.DataTableBackPackItem resProduct = ManagerProduct.Instance.GetProductTableItem(mgToInfoPasture.intPorductIDs[mgToInfoPasture.intIndexProduct]);
            imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(resProduct.strIconName);
            textProductName.text = ManagerProduct.Instance.GetName(resProduct.intProductID,false);

            JsonValue.DataTableCompoundItem itemCompound = ManagerCompound.Instance.GetValue(mgToInfoPasture.intCompoundIDs[mgToInfoPasture.intIndexProduct]);
            textResidueTime.text = mgToInfoPasture.intResidueTime + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Days);
            scrollbarResiduTime.size = (float)((float)mgToInfoPasture.intResidueTime / (float)itemCompound.intRipeDay);
            textResidueTimeScroll.text = mgToInfoPasture.intResidueTime + "/" + itemCompound.intRipeDay;
            int intTempCount = (int)((float)((float)mgToInfoPasture.intProductCount / (float)itemCompound.intRipeDay) * 365);
            textProductCount.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.AnnualOIA) + ":" + intTempCount;

            //正在消耗的产品
            JsonValue.DataTableBackPackItem resExpent = ManagerProduct.Instance.GetProductTableItem(mgToInfoPasture.intProductIDExpends[mgToInfoPasture.intIndexProductExpend]);
            imageProductExpend.sprite = ManagerResources.Instance.GetBackpackSprite(resExpent.strIconName);
            textProductExpend.text = mgToInfoPasture.intProductExpendNums[mgToInfoPasture.intIndexProductExpend].ToString();

            select.BuildMessage(message);
        }
    }

    protected void SetBuildTipsState(BuildTipsState buildState)
    {
        switch (buildState)
        {
            case BuildTipsState.None:
                textProductionState.text = "-";
                break;
            case BuildTipsState.Planting:
                textProductionState.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Planting);
                break;
            case BuildTipsState.NotExpend:
                textProductionState.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.InsufficientConsumables);
                break;
            case BuildTipsState.Expend:
                textProductionState.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Breeding);
                break;
            case BuildTipsState.NotWorker:
                textProductionState.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.NoEmployeesMining);
                break;
            case BuildTipsState.Minig:
                textProductionState.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Mining);
                break;
            case BuildTipsState.Smelt:
                textProductionState.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Smelting);
                break;
        }
    }
}
