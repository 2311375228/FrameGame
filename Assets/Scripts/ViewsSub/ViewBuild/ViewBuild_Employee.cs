using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBuild_Employee : ViewBuild_Base
{
    public StructEmployee[] employees;

    SetEmployee messageEmployee = new SetEmployee();

    protected override void Start()
    {
        for (int i = 0; i < employees.Length; i++)
        {
            employees[i].btnAddEmployee.onClick.AddListener(OnClickAddEmployee(i));
            employees[i].btnStepOut.onClick.AddListener(OnClickStepOut(i));
            employees[i].btnSee.onClick.AddListener(OnClickSee(i));

            //employees[i].btnAddEmployee.gameObject.SetActive(true);
            //employees[i].btnStepOut.transform.parent.gameObject.SetActive(false);
            employees[i].textName.gameObject.SetActive(false);
        }
    }

    public override void Show()
    {
        base.Show();

        for (int i = 0; i < employees.Length; i++)
        {
            employees[i].btnAddEmployee.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.EmployEmployees);
            employees[i].btnStepOut.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.DismissEmployees);
        }
    }

    public override void SetData(ViewBase.Message message)
    {
        ViewMGEmployeeAddTo mgEmployeeAdd = message as ViewMGEmployeeAddTo;
        if (mgEmployeeAdd != null)
        {
            //PropertiesEmployee itemEmployee = UserValue.Instance.GetEmployeeValue(mgEmployeeAdd.intEmployeeID);
            //employees[mgEmployeeAdd.intIndexSize].btnStepOut.transform.parent.gameObject.SetActive(true);
            //employees[mgEmployeeAdd.intIndexSize].btnAddEmployee.gameObject.SetActive(false);
            //employees[mgEmployeeAdd.intIndexSize].imageHead.sprite = itemEmployee.spriteHead;
            //employees[mgEmployeeAdd.intIndexSize].textName.text = itemEmployee.strEmployeeName;

            //ShowEmployeeProperties(itemEmployee, mgEmployeeAdd.intIndexSize);

            //SetShowEmployee();

            messageEmployee.intEmployeeID = mgEmployeeAdd.intEmployeeID;
            messageEmployee.intIndexSize = mgEmployeeAdd.intIndexSize;
            SendToGround(messageEmployee);
        }
    }

    public override void BuildMessage(EventBuildToViewBase message)
    {
        EventBuildToViewFarm mgToInfoFarm = message as EventBuildToViewFarm;
        EventBuildToViewPasture mgToInfoPasture = message as EventBuildToViewPasture;
        EventBuildToViewFactory mgToInfoFactory = message as EventBuildToViewFactory;
        int[] intEmployeeIDs = null;
        if (mgToInfoFarm != null)
        {
            intEmployeeIDs = mgToInfoFarm.intEmployeeSizes;
        }
        if (mgToInfoPasture != null)
        {
            intEmployeeIDs = mgToInfoPasture.intEmployeeSizes;
        }
        if (mgToInfoFactory != null)
        {
            intEmployeeIDs = mgToInfoFactory.intEmployeeSizes;
        }
        if (intEmployeeIDs != null)
        {
            for (int i = 0; i < intEmployeeIDs.Length; i++)
            {
                if (intEmployeeIDs[i] != -1)
                {
                    PropertiesEmployee itemEmployee = UserValue.Instance.GetEmployeeValue(intEmployeeIDs[i]);
                    employees[i].btnStepOut.transform.parent.gameObject.SetActive(true);
                    employees[i].btnAddEmployee.gameObject.SetActive(false);
                    employees[i].imageHead.sprite = itemEmployee.spriteHead;
                    employees[i].textName.text = itemEmployee.strEmployeeName;

                    employees[i].btnAddEmployee.gameObject.SetActive(false);
                    employees[i].btnStepOut.transform.parent.gameObject.SetActive(true);
                }
                else
                {
                    employees[i].btnAddEmployee.gameObject.SetActive(true);
                    employees[i].btnStepOut.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    void ShowEmployeeProperties(PropertiesEmployee itemEmployee, int intIndex)
    {
        //for (int k = 0; k < employeeItems[intIndex].employeeProperties.items.Length; k++)
        //{
        //    if (k < enumEmployeeProperties.Length)
        //    {
        //        Sprite sprite = ManagerResources.Instance.GetEmployeeProperties(enumEmployeeProperties[k]);
        //        employeeItems[intIndex].employeeProperties.items[k].imageValueMain.sprite = sprite;
        //        int intPropertiesRank = itemEmployee.dicEmployeeProperties[enumEmployeeProperties[k]];
        //        employeeItems[intIndex].employeeProperties.items[k].textValueMain.text = intPropertiesRank.ToString();
        //        employeeItems[intIndex].employeeProperties.items[k].gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        employeeItems[intIndex].employeeProperties.items[k].gameObject.SetActive(false);
        //    }
        //    employeeItems[intIndex].employeeProperties.items[k].gameObject.SetActive(false);
        //}
        //employeeItems[intIndex].textEmployeeExplain.gameObject.SetActive(false);
        //employeeItems[intIndex].employeeProperties.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        base.Hide();
    }

    UnityEngine.Events.UnityAction OnClickAddEmployee(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            actionEmployeeAdd(intIndex);
        };
    }
    UnityEngine.Events.UnityAction OnClickStepOut(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Unable);
            messageEmployee.intEmployeeID = -1;
            messageEmployee.intIndexSize = intIndex;
            SendToGround(messageEmployee);
        };
    }
    UnityEngine.Events.UnityAction OnClickSee(int intIndex)
    {
        return () =>
        {

        };
    }

    [Serializable]
    public struct StructEmployee
    {
        public Text textName;
        public Image imageHead;
        public Button btnAddEmployee;
        public Button btnStepOut;
        public Button btnSee;
    }
}
