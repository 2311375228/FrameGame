using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewCombatTeam_Team : MonoBehaviour
{
    public Text textEmployeeName;
    public Text textCombatRank;
    public Image imageCombatType;
    public Image imageEmployeeHead;

    public Button btnAddEmployee;
    public Button btnRandomSkill;
    public Button btnEmployeeSee;
    public Button btnEmployeeRemove;

    public View_PropertiesBase equipment;
    public View_PropertiesBase employeeCombatValue;
    public View_PropertiesBase employeeSkill;
    public View_PropertiesItem employeeType;

    public GameObject goImageEmployeeCombatInfo;
}
