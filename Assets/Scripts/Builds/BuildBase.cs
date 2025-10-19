using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBase : MonoBehaviour
{
    //工坊建筑有3个升级方向，初定可以升级5级，必须有员工参与
    //升级产量：产量与时间增加的比例：50%：20%
    //升级效率：产量与时间增加的比例：20%：-10%
    //自动化生产：产能减半，可以添加员工，按员工属性改变生产状态

    [SerializeField]
    protected EnumEmployeeProperties[] enumEmployeeProperties;
    protected PropertiesBuild proBuild;
    protected Dictionary<EnumEmployeeProperties, string> dicEmployeePropertiesInfo = new Dictionary<EnumEmployeeProperties, string>();

    [System.NonSerialized]
    public int intFarmProductID;//正在生产的产品ID
    [System.NonSerialized]
    public int intFarmProductCountsing;//生产产量
    [System.NonSerialized]
    public int intFarmRipeDay;//生产天数

    [System.NonSerialized]
    public int intPastureProductID;
    [System.NonSerialized]
    public int intPastureProductCount;
    [System.NonSerialized]
    public int intPastureRipeDay;
    [System.NonSerialized]
    public int intPastureExpendProductID;//消耗品ID
    [System.NonSerialized]
    public int intPastureExpendProductCount;//消耗品消耗量

   int _intBuildTotalPrice;//建筑原来的价格+升级的价格
    public int IntBuildTotalPrice
    {
        get
        {
            return _intBuildTotalPrice;
        }
        set
        {
            _intBuildTotalPrice = value;
        }
    }

    /// <summary>
    /// 仓库容量
    /// </summary>
    protected int intStockMax;
    public int GetStockMax { get { return intStockMax; } }

    int _intIndexGround = -1;
    public int GetIndexGround { get { return _intIndexGround; } }
    public int SetIndexGround { set { _intIndexGround = value; } }
    int _intBuildID = -1;
    public int GetBuildID { get { return _intBuildID; } }
    public int SetBuildID { set { _intBuildID = value; } }

    /// <summary>
    /// 是否向界面发送消息
    /// </summary>
    protected bool booBuildToView = false;
    /// <summary>
    /// GameObject是在ControllerGround.cs->MessageUpdateGround()中实例化
    /// </summary>
    public virtual void OnStart()
    {
        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Date, MessageUpdateDate);

        proBuild = new PropertiesBuild();
        JsonValue.DataTableBuildingItem itemBuild = ManagerBuild.Instance.GetBuildItem(GetBuildID);
        if (itemBuild != null)
        {
            proBuild.numPrice = itemBuild.numPrice;
            proBuild.intBuildType = itemBuild.intBuildType;
            proBuild.intBuildID = itemBuild.intBuildID;
            proBuild.intMaintain = itemBuild.intMaintain;
            proBuild.strBuildName = ManagerBuild.Instance.GetBuildName(itemBuild.intBuildID);
            proBuild.strModelName = itemBuild.strModelName;

            IntBuildTotalPrice = ManagerBuild.Instance.GetBuildItem(itemBuild.intBuildID).numPrice;
        }
    }

    public virtual void PlayAudio()
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
    }

    protected virtual void OnDestroy()
    {
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Date, MessageUpdateDate);
    }

    void MessageUpdateDate(ManagerMessage.MessageBase message)
    {
        MessageDate mgDate = message as MessageDate;
        if (mgDate != null)
        {
            UpdateDate(mgDate);
        }
    }

    /// <summary>
    /// 消息来源：ViewBuildInfo ViewBuildFactoryInfo
    /// 由ControllerGround执行
    /// </summary>
    public virtual void MGViewBuildInfo(MGViewToBuildBase toGround) { }

    public virtual void ShowViewBuildInfo() { }

    /// <summary>
    /// 写入 信息
    /// </summary>
    public virtual string GameSaveData()
    {
        return "";
    }
    /// <summary>
    /// 读取 存储信息
    /// </summary>
    public virtual void GameReadData(string strData) { }

    /// <summary>
    /// 消息来源：由ControllerGame
    /// 本类MessageUpdateDate执行
    /// </summary>
    protected virtual void UpdateDate(MessageDate mgData) { }

}
