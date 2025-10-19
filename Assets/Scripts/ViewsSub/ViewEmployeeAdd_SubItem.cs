using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewEmployeeAdd_SubItem : ColumnItemBase
{

    public Text textEmployeeName;
    public Text textEmployeeRank;
    public Text textEmployeeType;
    public Image imageHead;

    public Button btnEmployeeSee;
    public Button btnEmployeeAdd;

    public View_PropertiesBase employeeProperties;

    public System.Action<int, int> actionEmployeeSee;

    private void Start()
    {
        btnEmployeeSee.onClick.AddListener(() => { actionEmployeeSee(numIndexItem, numIndexData); });
        btnEmployeeAdd.onClick.AddListener(() => { actionBase(numIndexItem, numIndexData); });
    }
}