using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_Warehouse : ViewBuild_Base
{
    public Text textProductName;
    public Text textCapacity;
    public Text textRank;
    public Image imageProduct;
    public Button btnAlter;

    protected override void Start()
    {
        btnAlter.onClick.AddListener(() =>
        {

        });
    }

    ViewBuild_WarehouseSelect select;
    public override void BuildMessage(EventBuildToViewBase message)
    {

    }
}
