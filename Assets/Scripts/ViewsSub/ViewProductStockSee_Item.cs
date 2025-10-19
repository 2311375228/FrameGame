using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewProductStockSee_Item : ColumnItemBase
{
    public Text textProductName;
    public Text textStockCount;
    public Text textProductionCount;
    public Text textExpentCount;
    public Image imageProduct;
    public Button btnStockEnter;
    public Button btnStockOut;

    public Text textProductionTag;
    public Text textExpentTag;
    public Text textBtnStockEnterTag;
    public Text textBtnStockOutTag;

    public System.Action<int, int> actionEnter;
    public System.Action<int, int> actionOut;

    void Start()
    {
        btnStockEnter.onClick.AddListener(() => { actionEnter(numIndexItem, numIndexData); });
        btnStockOut.onClick.AddListener(() => { actionOut(numIndexItem, numIndexData); });
    }
}
