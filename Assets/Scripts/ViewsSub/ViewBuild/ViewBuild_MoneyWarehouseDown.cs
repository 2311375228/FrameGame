using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_MoneyWarehouseDown : ViewBuild_Base
{
    public Text textInfo;
    public Text textPrice;
    public Button btnConfirm;

    public RectTransform rectExpend;

    List<View_PropertiesItem> listItem = new List<View_PropertiesItem>();
    MoneyWarehouseToView messageMoney;

    protected override void Start()
    {
        btnConfirm.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            MoneyWarehouseToBuild message = new MoneyWarehouseToBuild();
            SendToGround(message);
        });
    }

    public override void Show()
    {
        base.Show();

        if (listItem.Count == 0)
        {
            for (int i = 0; i < rectExpend.childCount; i++)
            {
                listItem.Add(rectExpend.GetChild(i).GetComponent<View_PropertiesItem>());
            }
        }

        btnConfirm.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Upgrade);
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        messageMoney = message as MoneyWarehouseToView;
        if (messageMoney != null)
        {
            int intIndexTemp = 0;
            for (int i = 0; i < listItem.Count; i++)
            {
                if (i < messageMoney.intUpgradeMaterials.Length / 2)
                {
                    listItem[i].imageValueMain.sprite = ManagerResources.Instance.GetBackpackSprite(ManagerProduct.Instance.GetProductTableItem(messageMoney.intUpgradeMaterials[intIndexTemp]).strIconName);
                    listItem[i].textValueMain.text = messageMoney.intUpgradeMaterials[intIndexTemp + 1].ToString();
                    listItem[i].gameObject.SetActive(true);

                    intIndexTemp += 2;
                    continue;
                }
                listItem[i].gameObject.SetActive(false);
            }
            textPrice.text = messageMoney.intExpence.ToString("N0");

            if (messageMoney.intRank == messageMoney.intRankMax)
            {
                for (int i = 0; i < listItem.Count; i++)
                {
                    listItem[i].gameObject.SetActive(false);
                }
                textInfo.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.TheMLHBR);//已经达到最大等级。
                textPrice.transform.parent.gameObject.SetActive(false);
                btnConfirm.gameObject.SetActive(false);
            }
            else
            {
                textInfo.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.RequiredMAC);//需要的材料和金币
                textPrice.transform.parent.gameObject.SetActive(true);
                btnConfirm.gameObject.SetActive(true);
            }
        }
    }
}
