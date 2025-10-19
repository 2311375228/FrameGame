using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewEmployeeList_Item : ColumnItemBase
{
    public Text textEmpoyeeName;//员工名称
    public Text textMonthlyMoney;//月薪
    public Text textWorkState;//当前工作状态,休闲,工作中,战斗中
    public Text textEmployeeContent;//员工介绍
    public Text textEmployeeType;//员工类型
    public Image imageEmployeePicture;//员工头像

    public Button btnEmployeeRename;
    public Button btnDismissal;//解雇
    public Button btnTakeOffice;//取消任职
    public Button btnEmployeeSee;//产看员工

    public View_PropertiesItem employeeCombatType;
    public View_PropertiesBase equipment;
    public View_PropertiesBase employeeCombatValue;
    public View_PropertiesBase employeeProperties;

    public System.Action<int, int> actionEmployeeRename;
    public System.Action<int, int> actionDismissal;
    public System.Action<int, int> actionTakeOffice;

    public System.Action<int, int> actionWeapon;
    public System.Action<int, int> actionArmor;
    public System.Action<int, int> actionShoes;

    private void Start()
    {
        btnEmployeeRename.onClick.AddListener(() => { actionEmployeeRename(numIndexItem, numIndexData); });
        btnDismissal.onClick.AddListener(() => { actionDismissal(numIndexItem, numIndexData); });
        btnTakeOffice.onClick.AddListener(() => { actionTakeOffice(numIndexItem, numIndexData); });
        btnEmployeeSee.onClick.AddListener(() => { actionBase(numIndexItem,numIndexData); });

        equipment.items[0].GetComponent<Button>().onClick.AddListener(() => { actionWeapon(numIndexItem, numIndexData); });
        equipment.items[1].GetComponent<Button>().onClick.AddListener(() => { actionArmor(numIndexItem, numIndexData); });
        equipment.items[2].GetComponent<Button>().onClick.AddListener(() => { actionShoes(numIndexItem, numIndexData); });
    }
}
