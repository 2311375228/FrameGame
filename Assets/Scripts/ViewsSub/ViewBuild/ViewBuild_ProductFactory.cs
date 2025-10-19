using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_ProductFactory : ViewBuild_Base
{
    public Button btnAlter;

    public RectTransform rectRoot;

    int[] intCompoundingIDs;
    List<ViewBuild_FactoryItem> listItem = new List<ViewBuild_FactoryItem>();
    ViewBuild_ProductFactorySelect select;
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

        if (listItem.Count == 0)
        {
            for (int i = 0; i < rectRoot.childCount; i++)
            {
                listItem.Add(rectRoot.GetChild(i).GetComponent<ViewBuild_FactoryItem>());
            }

            select = getCentre(EnumViewCentre.ViewBuild_ProductFactorySelect) as ViewBuild_ProductFactorySelect;
            select.SendToGround = SendToGround;
            select.Show();
            select.Hide();
        }

        btnAlter.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Produce);
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
        ViewMGToViewInfoFactory mgViewFactory = message as ViewMGToViewInfoFactory;
        if (mgViewFactory != null)
        {
            //这个是更新当前工厂物品生产剩余时间
            for (int i = 0; i < mgViewFactory.intCompoundResidueTimes.Length; i++)
            {
                if (mgViewFactory.intCompoundResidueTimes[i] != -1)
                {
                    JsonValue.DataTableCompoundItem itemCompound = ManagerCompound.Instance.GetValue(intCompoundingIDs[i]);
                    JsonValue.DataTableBackPackItem factoryProduct = ManagerProduct.Instance.GetProductTableItem(itemCompound.intProductID);
                    listItem[i].textTime.text = mgViewFactory.intCompoundResidueTimes[i] + "/" + itemCompound.intRipeDay;
                    listItem[i].imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(factoryProduct.strIconName);
                    listItem[i].goProduction.SetActive(true);
                    listItem[i].scrollBar.size = (float)mgViewFactory.intCompoundResidueTimes[i] / (float)itemCompound.intRipeDay;
                }
                else
                {
                    listItem[i].goProduction.SetActive(false);
                }
            }
        }
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        EventBuildToViewFactory mgToInfoFactory = message as EventBuildToViewFactory;

        if (mgToInfoFactory != null)
        {
            intCompoundingIDs = mgToInfoFactory.intCompoundingIDs;
            //正在生产的产品
            for (int i = 0; i < mgToInfoFactory.intCompoundingIDs.Length; i++)
            {
                if (mgToInfoFactory.intCompoundingIDs[i] != -1)
                {
                    JsonValue.DataTableCompoundItem itemCompound = ManagerCompound.Instance.GetValue(mgToInfoFactory.intCompoundingIDs[i]);
                    JsonValue.DataTableBackPackItem factoryProduct = ManagerProduct.Instance.GetProductTableItem(itemCompound.intProductID);
                    listItem[i].textProductCount.text = itemCompound.intProductCount.ToString("N0");
                    listItem[i].textTime.text = mgToInfoFactory.intCompoundResidueTimes[i] + "/" + itemCompound.intRipeDay;
                    listItem[i].imageProduct.sprite = ManagerResources.Instance.GetBackpackSprite(factoryProduct.strIconName);
                    listItem[i].goProduction.SetActive(true);

                }
                else
                {
                    listItem[i].goProduction.SetActive(false);
                }
            }

            select.BuildMessage(message);
        }
    }
}
