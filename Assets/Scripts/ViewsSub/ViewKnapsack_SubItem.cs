using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewKnapsack_SubItem : ColumnItemBase
{
    public View_PropertiesItem[] items;
    public System.Action<int> actionData;
    private void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<Button>().onClick.AddListener(OnClickProductItem(numIndexItem * items.Length + i));
        }
    }

    UnityAction OnClickProductItem(int intIndex)
    {
        return delegate
        {
            actionData(intIndex);
        };
    }
}
