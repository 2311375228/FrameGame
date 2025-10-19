using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_FarmProduct : ViewBuild_Base
{
    public Text textProductName;
    public Text textProductCount;
    public Text textResidueTime;
    public Text textResidueTimeScroll;
    public Text textProductionState;
    public Image imageProduct;
    public Scrollbar scrollbarResiduTime;

    int intProductRipeDay;
    protected override void Start()
    {

    }
    public override void Show()
    {
        base.Show();
    }
    public override void SetData(ViewBase.Message message)
    {
        ViewMGToViewInfoFarm mgViewFarm = message as ViewMGToViewInfoFarm;
        if (mgViewFarm != null)
        {
            textResidueTime.text = mgViewFarm.intResidueTime + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Days);
            scrollbarResiduTime.size = (float)((float)mgViewFarm.intResidueTime / (float)intProductRipeDay);
            textResidueTimeScroll.text = mgViewFarm.intResidueTime + "/" + intProductRipeDay;

            SetBuildTipsState(mgViewFarm.buildState);
        }
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        EventBuildToViewFarm mgToInfoFarm = message as EventBuildToViewFarm;
        if (mgToInfoFarm != null)
        {
            intProductRipeDay = mgToInfoFarm.intProductRipeDay;

            textResidueTime.text = mgToInfoFarm.intResidueTime + " " + ManagerLanguage.Instance.GetWord(EnumLanguageWords.Days);
            scrollbarResiduTime.size = (float)((float)mgToInfoFarm.intResidueTime / (float)mgToInfoFarm.intProductRipeDay);
            textResidueTimeScroll.text = mgToInfoFarm.intResidueTime + "/" + mgToInfoFarm.intProductRipeDay;
            int intTempCount = (int)((float)((float)mgToInfoFarm.intProductCount / (float)mgToInfoFarm.intProductRipeDay) * 365);
            textProductCount.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.AnnualOIA) + ":" + intTempCount;

            JsonValue.DataTableBackPackItem resProduct = ManagerProduct.Instance.GetProductTableItem(mgToInfoFarm.intProductID);
            textProductName.text = ManagerProduct.Instance.GetName(mgToInfoFarm.intProductID, false);
            imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(resProduct.strIconName);
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
