using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTower_subItem : ColumnItemBase
{
    public Text textEmployeeName;
    public Text textEmployeeRank;
    public Text textEmployeeGround;
    public Image imageHead;
    public Image imageProperties;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { actionBase(numIndexItem, numIndexData); });
    }
}
