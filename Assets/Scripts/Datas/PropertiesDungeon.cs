using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesDungeon
{
    public int intDungeonID;
    public bool booFinishDungeon;//是否击败boss
    public DungeonPoint[] points;

    public class DungeonPoint
    {
        public int intPointIndex;
        public int intWinCount;//击败的次数,作为敌人血量增加的参考值
        public int intStar;//获得的星星数量
        /// <summary>
        /// 队伍集合
        /// </summary>
        public Team[] teams;
    }

    public class Team
    {
        /// <summary>
        /// 敌人ID集合
        /// </summary>
        public int[] intIDs;
    }
}
