using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewShop_BackpackItem : ColumnItemBase
{
    public View_PropertiesItem itemProduct;
    public Text textPrice;
    public Text textDay;
    public Text textBtnItem;
    public Button btnSell;
    public GameObject goSelect;
    public System.Action<int, int> actionSell;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            actionBase(numIndexItem, numIndexData);
        });
        btnSell.onClick.AddListener(() =>
        {
            actionSell(numIndexItem, numIndexData);
        });
    }
}
