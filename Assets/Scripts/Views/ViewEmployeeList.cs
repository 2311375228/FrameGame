using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ViewEmployeeList : ViewBase
{
    public Button btnClose;

    //public ViewEmployeeList_Item[] 

    public Button btnCloseRename;
    public InputField inputFieldRename;
    public GameObject goImageRename;

    public ScrollCycleColumn columnItem;

    List<PropertiesEmployee> listData = new List<PropertiesEmployee>();

    protected override void Start()
    {
        btnClose.onClick.AddListener(() => { ManagerView.Instance.Hide(EnumView.ViewEmployeeList); });
        SetDataTotal(0);

        goImageRename.SetActive(false);

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }

    public override void Show()
    {
        base.Show();
        ShowEmployeeList();

    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(EmployeeListWait());
        }
    }

    IEnumerator EmployeeListWait()
    {
        yield return 0;
        ShowEmployeeList();
    }
    void ShowEmployeeList()
    {
        Dictionary<int, PropertiesEmployee> dicTemp = UserValue.Instance.GetAllEmployee();
        listData.Clear();
        foreach (PropertiesEmployee temp in dicTemp.Values)
        {
            listData.Add(temp);
        }
        SetDataTotal(listData.Count);
    }

    void SetDataTotal(int intCount)
    {
        //查看当前员工

        RectTransform[] rectItems = columnItem.SetDataTotal(intCount);
        for (int i = 0; i < rectItems.Length; i++)
        {
            ViewEmployeeList_Item itemEmployee = rectItems[i].GetComponent<ViewEmployeeList_Item>();
            itemEmployee.numIndexItem = i;
            itemEmployee.numIndexData = i;
            itemEmployee.actionBase = ActionEmployeeSee;
            itemEmployee.actionDismissal = ActionEmpoyeeDismissal;
            itemEmployee.actionEmployeeRename = ActionEmployeeRename;
            itemEmployee.actionTakeOffice = ActionEmployeeTakeOffice;
            itemEmployee.actionWeapon = ActionEmployeeWeapon;
            itemEmployee.actionArmor = ActionEmployeeArmor;
            itemEmployee.actionShoes = ActionEmployeeShoes;
            RefreshData(itemEmployee, i, i);
        }
    }

    void RefreshData(ViewEmployeeList_Item itemTemp,int intIndexItem, int intIndexData)
    {
        if (intIndexData >= listData.Count)
        {
            itemTemp.gameObject.SetActive(false);
        }
        else
        {
            itemTemp.gameObject.SetActive(true);
            itemTemp.numIndexItem = intIndexItem;
            itemTemp.numIndexData = intIndexData;

            PropertiesEmployee employee = listData[intIndexData];
            itemTemp.textEmpoyeeName.text = employee.strEmployeeName;
            itemTemp.imageEmployeePicture.sprite = employee.spriteHead;
            itemTemp.employeeCombatType.imageValueMain.sprite = ManagerSkill.Instance.GetCombatType(employee.combatAttackType);
            itemTemp.employeeCombatType.textValueMain.text = employee.intCombatTypeRank.ToString();
            itemTemp.textMonthlyMoney.text = employee.intMonthlyMoney.ToString();
            itemTemp.textWorkState.text = employee.strState;
            itemTemp.textEmployeeContent.text = employee.strAddtionContent;
            itemTemp.textEmployeeType.text = ManagerEmployee.Instance.strEmployeeTypes[(int)employee.enumIdentification];

            itemTemp.employeeCombatValue.items[0].textValueMain.text = employee.intHP.ToString();
            itemTemp.employeeCombatValue.items[1].textValueMain.text = employee.intATK.ToString();
            itemTemp.employeeCombatValue.items[2].textValueMain.text = employee.intMP.ToString();
            itemTemp.employeeCombatValue.items[3].textValueMain.text = employee.floSpeed.ToString();
            int intTemp = 0;
            foreach (KeyValuePair<EnumEmployeeProperties, int> temp in employee.dicEmployeeProperties)
            {
                itemTemp.employeeProperties.items[intTemp].imageValueMain.sprite = ManagerResources.Instance.GetEmployeeProperties(temp.Key);
                itemTemp.employeeProperties.items[intTemp].textValueMain.text = temp.Value.ToString();
                intTemp++;
            }

            //PropertiesEquipment equipment = null;
            Sprite sprite = null;
            Vector2 vecSize = Vector2.zero;
            RectTransform rectSize = null;
            for (int i = 0; i < employee.intEquipmentIDs.Length; i++)
            {
                if (employee.intEquipmentIDs[i] != -1)
                {
                    //equipment = UserValue.Instance.GetEquipmentItem(employee.intEquipmentIDs[i]);
                    //sprite = ManagerResources.Instance.GetEquipmentSprite(equipment.strICON);
                    //rectSize = itemTemp.equipment.items[i].GetComponent<RectTransform>();
                    //vecSize = rectSize.sizeDelta;
                    //vecSize = Tools.SetSpriteRectSize(vecSize, sprite);
                    //rectSize.sizeDelta = vecSize;
                    //itemTemp.equipment.items[i].imageValueMain.sprite = sprite;
                }
            }

            if (employee.enumIdentification == EnumEmployeeIdentification.Farmer)
            { itemTemp.btnDismissal.gameObject.SetActive(false); }
            else { itemTemp.btnDismissal.gameObject.SetActive(true); }
        }
    }
    /// <summary>
    /// 查看员工
    /// </summary>
    void ActionEmployeeSee(int intIndexItem, int intIndexData)
    {

    }
    /// <summary>
    /// 改变员工名称
    /// </summary>
    void ActionEmployeeRename(int intIndexItem, int intIndexData)
    {

    }
    /// <summary>
    /// 移除员工
    /// </summary>
    void ActionEmpoyeeDismissal(int intIndexItem, int intIndexData)
    {

    }
    /// <summary>
    /// 取消任职
    /// </summary>
    void ActionEmployeeTakeOffice(int intIndexItem, int intIndexData)
    {

    }
    /// <summary>
    /// 员工武器
    /// </summary>
    void ActionEmployeeWeapon(int intIndexItem, int intIndexData)
    { }
    /// <summary>
    /// 员工护甲
    /// </summary>
    void ActionEmployeeArmor(int intIndexItem, int intIndexData)
    {

    }
    /// <summary>
    /// 员工鞋子
    /// </summary>
    void ActionEmployeeShoes(int intIndexItem, int intIndexData)
    {

    }


    private void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }
}
