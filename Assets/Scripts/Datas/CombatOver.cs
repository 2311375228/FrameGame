using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatOver : ViewBase.Message
{
    public bool booOver;//是否结束
    /// <summary>
    /// true=hero
    /// false=enemy
    /// </summary>
    public bool booTeam;//那只队伍获胜
    public bool booRound;
    public float floTime;
    public string strRoundInfo;//回合制信息

    public int intIndexTeam;
    public int intRoundCount;//进行了多少回合
    public int intRoleDeath;//阵亡角色个数

    public int intDungeonID;
    public int intDungeonIndex;
}
