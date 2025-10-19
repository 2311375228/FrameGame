using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGViewToBuildEquipment
{
    public Close close;
    public Forging forging;

    public MGViewToBuildEquipment()
    {
        close = new Close();
        forging = new Forging();
    }

    public class Close : MGViewToBuildBase
    {

    }
    /// <summary>
    /// 锻造
    /// </summary>
    public class Forging : MGViewToBuildBase
    {
        public bool booView;
        public BackpackGrid forgingGrid;
        public int intFinishDay;
    }

}
