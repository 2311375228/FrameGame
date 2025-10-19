using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBarTop_ItemMail : MonoBehaviour
{
    public Text textReceive;//领取
    public Text textContent;//内容
    public Image imageProduct;

    public Button btnSee;
    public Button btnGet;

    public View_PropertiesItem[] itemAwards;

    public int intIndex;

    [System.NonSerialized]
    public RectTransform rectItemRoot;
    RectTransform rectSelf;

    float floDis;
    // Start is called before the first frame update
    void Start()
    {
        rectSelf = GetComponent<RectTransform>();
        textReceive.transform.position = btnGet.transform.position;
    }
     void Update()
    {
        floDis = Vector2.Distance(rectItemRoot.position, rectSelf.position);
        if (floDis > 0.1f)
        {
            rectSelf.position = Vector2.MoveTowards(rectSelf.position, rectItemRoot.position, 700 * Time.deltaTime);
        }
    }

    public class Mail
    {
        public bool booSee;
        public bool booGet;
        public int intIndex;
        public EnumMail enumMail;
        public string strContent;
        public string strIconName;

        public EnumKnapsackStockType[] gridItems;
        public int[] intIndexIDs;
        public int[] intRanks;
        public int[] intIndexCounts;
    }

    public enum EnumMail
    {
        None,
        Hammer,//锤子
        Task,//任务
        Present,//礼物
        Anvil,//铁砧
        Armor,//护甲
        Weapon,//武器
        Shoes,//鞋子
    }
}
