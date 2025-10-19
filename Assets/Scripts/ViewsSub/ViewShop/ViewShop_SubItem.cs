using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewShop_SubItem : ColumnItemBase
{
    public ViewShop_SubItemSell[] itemSells;

    public System.Action<int, int> actionSellDown;

    private void Start()
    {
        itemSells[0].btnSell.onClick.AddListener(() =>
        {
            actionSellDown(numIndexItem, numIndexData * 2);
        });
        itemSells[1].btnSell.onClick.AddListener(() =>
        {
            actionSellDown(numIndexItem, numIndexData * 2 + 1);
        });
    }
}
