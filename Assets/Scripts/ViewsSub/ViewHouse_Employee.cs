using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewHouse_Employee : MonoBehaviour
{
    public Button btnAll;//全部
    public Button btnEmploy;//已雇佣
    public Button btnEmployNone;//没有雇佣
    public Button btnEmployWork;//工作中
    public Button btnEmployeeIdle;//闲置
    public Button btnClose;

    public ScrollCycleColumn columnItem;

    /// <summary>
    /// 当前总员工
    /// </summary>
    public List<PropertiesEmployee> listData = new List<PropertiesEmployee>();
    /// <summary>
    /// 临时存储
    /// </summary>
    public List<PropertiesEmployee> listTemp = new List<PropertiesEmployee>();
}
