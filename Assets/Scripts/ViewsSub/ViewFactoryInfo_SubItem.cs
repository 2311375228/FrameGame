using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewFactoryInfo_SubItem : ColumnItemBase
{
    public View_PropertiesItem itemProduct;
    public View_PropertiesBase itemExpend;
        
    public GameObject goFrame;
    public Button btnItem;

    private void Start()
    {
        btnItem.onClick.AddListener(() => { actionBase(numIndexItem, numIndexData); });
    }
}
