using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse_EmployeeItem : ColumnItemBase
{
    public Text textEmployeeName;
    public Text textMonthly;//月薪
    public Text textEmployeeContent;
    public Image imageEmployeeHead;
    public View_PropertiesItem proCombatType;
    public View_PropertiesBase employeeProperties;
    public View_PropertiesBase combatValue;

    public GameObject goEmployeeState;
    public GameObject goMonthly;

    public Button btnEmploy;
    public Button btnTakeOff;
    public Button btnSee;

    public System.Action<int, int> actionEmploy;
    public System.Action<int, int> actionTakeOff;

    void Start()
    {
        btnSee.onClick.AddListener(() => { actionBase(numIndexItem, numIndexData); });
        btnEmploy.onClick.AddListener(() => { actionEmploy(numIndexItem, numIndexData); });
        btnTakeOff.onClick.AddListener(() => { actionTakeOff(numIndexItem, numIndexData); });
        btnTakeOff.transform.position = btnEmploy.transform.position;
    }
}
