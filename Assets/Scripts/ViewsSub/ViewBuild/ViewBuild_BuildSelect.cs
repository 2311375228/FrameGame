using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_BuildSelect : ViewBuild_Base
{
    public Text textBuildName;
    public Text textProductionTag;
    public Text textBuildInfo;
    public Text textPrice;

    public Image imageBuild;

    public Button btnClose;
    public Button btnBuy;

    public RectTransform rectProductRoot;
    public GameObject goBuildProduction;//建筑生产信息
    public GameObject goBuildInfo;//建筑功能作用

    public ScrollCycleColumn column;

    int intSelectBuild;
    List<View_PropertiesItem> listItem = new List<View_PropertiesItem>();
    List<Image> listProduct = new List<Image>();
    List<int> listOre = new List<int>() { 1008, 1009, 1010, 1011 };
    List<int> listBuildID = new List<int>();

    float floTimeSprite;
    int intIndexSprite;
    Sprite[] spriteBuilds;

    int numIndexGround;
    string strModelNameOld;
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            gameObject.SetActive(false);
        });
        btnBuy.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.CoinBuy);

            JsonValue.DataTableBuildingItem buildPr = ManagerBuild.Instance.GetBuildItem(listBuildID[intSelectBuild]);
            UserValue.Instance.SetCoinReduce(buildPr.numPrice);

            MessageGround mgGround = new MessageGround();
            mgGround.numIndexGround = numIndexGround;
            mgGround.strModelNameNew = buildPr.strModelName;
            mgGround.strModelNameOld = strModelNameOld;
            //mgGround.groundState = EnumGroudState.BuildingPruchased;
            mgGround.groundState = EnumGroudState.BuildConstruction;
            mgGround.intBuildID = listBuildID[intSelectBuild];
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Ground, mgGround);

            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
        });
    }

    public override void Show()
    {
        base.Show();

        if (listProduct.Count == 0)
        {
            for (int i = 0; i < rectProductRoot.childCount; i++)
            {
                listProduct.Add(rectProductRoot.GetChild(i).GetChild(0).GetComponent<Image>());
            }
        }
        textProductionTag.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ItemsProduced);
        btnBuy.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Purchase);
    }

    private void Update()
    {
        if (spriteBuilds != null)
        {
            floTimeSprite += Time.deltaTime;
            if (floTimeSprite > 0.5f)
            {
                floTimeSprite = 0;
                imageBuild.sprite = spriteBuilds[intIndexSprite++];
                if (intIndexSprite == spriteBuilds.Length)
                {
                    intIndexSprite = 0;
                }
            }
        }
    }

    public override void SetData(ViewBase.Message message)
    {
        ViewBuild_BuyBuild.Message messageBuildType = message as ViewBuild_BuyBuild.Message;
        if (messageBuildType != null)
        {
            listBuildID.Clear();
            Dictionary<int, JsonValue.DataTableBuildingItem> item = ManagerBuild.Instance.GetBuildAll();
            foreach (JsonValue.DataTableBuildingItem temp in item.Values)
            {
                if (temp.intBuildID == 4003
                    || temp.intBuildID == 4002
                    || temp.intBuildID == 4001
                    || temp.intBuildID == 3006
                    || temp.intBuildID == 4008)//时间紧迫,暂时放弃
                {
                    continue;
                }
                if (temp.intBuildID == 4007)//工地建筑不应该出现在列表中
                {
                    continue;
                }
                if (listOre.Contains(temp.intBuildID))//改为随机矿点,且不可增加或减少
                {
                    continue;
                }
                if (temp.intBuildType == messageBuildType.intBuildType)
                {
                    listBuildID.Add(temp.intBuildID);
                }
            }
            listItem.Clear();
            RectTransform[] rectItems = column.SetDataTotal(listBuildID.Count);
            for (int i = 0; i < rectItems.Length; i++)
            {
                View_PropertiesItem itemInfoSub = rectItems[i].GetComponent<View_PropertiesItem>();
                itemInfoSub.GetComponent<Button>().onClick.AddListener(OnClickBuildItem(i));
                BuildItem(listBuildID[i], itemInfoSub);
                listItem.Add(itemInfoSub);
            }
            ShowBuildInfo(0);
            intSelectBuild = 0;
        }
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        BuyBuildToView messageBuild = message as BuyBuildToView;
        if (messageBuild != null)
        {
            numIndexGround = messageBuild.intIndexGround;
            strModelNameOld = messageBuild.strModelNameNew;
        }
    }

    /// <summary>
    /// 更新滑动列表显示的建筑信息
    /// </summary>
    void BuildItem(int intBuildID, View_PropertiesItem item)
    {
        JsonValue.DataTableBuildingItem itemBuild = ManagerBuild.Instance.GetBuildItem(intBuildID);
        item.textValueMain.text = ManagerBuild.Instance.GetBuildName(intBuildID);
        item.textValue.text = itemBuild.numPrice.ToString("N0");

        Sprite[] s = ManagerResources.Instance.GetBuildSprite(itemBuild.strModelName);
        item.imageValueMain.sprite = s[s.Length - 1];
    }

    /// <summary>
    /// 建筑信息详情
    /// </summary>
    void ShowBuildInfo(int intIndex)
    {
        for (int i = 0; i < listItem.Count; i++)
        {
            if (i == intIndex)
            {
                listItem[i].imageValue.gameObject.SetActive(true);
                continue;
            }
            listItem[i].imageValue.gameObject.SetActive(false);
        }

        JsonValue.DataTableBuildingItem itemBuild = ManagerBuild.Instance.GetBuildItem(listBuildID[intIndex]);
        textBuildName.text = ManagerBuild.Instance.GetBuildName(listBuildID[intIndex]);
        textPrice.text = itemBuild.numPrice.ToString("N0");

        intIndexSprite = 0;
        spriteBuilds = ManagerResources.Instance.GetBuildSprite(itemBuild.strModelName);
        imageBuild.sprite = spriteBuilds[0];
        int[] intCompoundIDs = ManagerBuildCompound.Instance.GetBuildCompoundProduct(listBuildID[intIndex]);

        if (intCompoundIDs.Length == 0)
        {
            goBuildProduction.SetActive(false);
            goBuildInfo.SetActive(true);
            switch (listBuildID[intIndex])
            {
                case 4004://城堡
                    textBuildInfo.text = ManagerLanguage.Instance.GetStory(EnumLanguageStory.BuildCastle);
                    break;
                case 4005://钱仓
                    textBuildInfo.text = ManagerLanguage.Instance.GetStory(EnumLanguageStory.BuildMoneyWarehouse);
                    break;
                case 4006://出货商店
                    textBuildInfo.text = ManagerLanguage.Instance.GetStory(EnumLanguageStory.BuildBuildSaleShop);
                    break;

            }
            return;
        }
        //一开始有两名员工，然后每年的建造城堡的建造日期会增加一名员工，最多增加到15名员工。员工可以去冒险，也可以增加产品的产量。
        //增加金币的容量。有12个等级，每次升级都需要消耗材料和金币，12级的时候，金币的容量是2000万。
        //出售一种产品，产品是随机产生。有4个等级的产品，选择等级或再次随机一个产品的时候，会消耗金币。建筑的建造日期，将会结算收益。
        //可以生产的物品：

        goBuildProduction.SetActive(true);
        goBuildInfo.SetActive(false);

        List<int> listProductID = new List<int>();
        for (int i = 0; i < intCompoundIDs.Length; i++)
        {
            JsonValue.DataTableCompoundItem compoundItem = ManagerCompound.Instance.GetValue(intCompoundIDs[i]);
            if (!listProductID.Contains(compoundItem.intProductID))
            {
                listProductID.Add(compoundItem.intProductID);
            }
        }
        for (int i = 0; i < listProduct.Count; i++)
        {
            if (i < listProductID.Count)
            {
                listProduct[i].transform.parent.gameObject.SetActive(true);

                JsonValue.DataTableBackPackItem product = ManagerProduct.Instance.GetProductTableItem(listProductID[i]);
                listProduct[i].sprite = ManagerResources.Instance.GetBackpackSprite(product.strIconName);
            }
            else
            {
                listProduct[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    UnityEngine.Events.UnityAction OnClickBuildItem(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intSelectBuild = intIndex;
            ShowBuildInfo(intIndex);
        };
    }
}
