using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesTask
{
    public EnumTaskType enumTask;
    public int intID;
    public int intRank;
    public int[] intGoodsIDs;
    public int[] intGoodsCounts;
    public int intAwardCion;
    public int intPenaltyCoin;
    public bool booDown;//是否完成任务
    public bool[] booFinish;//是否完成小任务

    //临时加的，以后为了扩展，要改为继承关系
    public int intDungeonID;
    public int intDungeonIndex;
}
