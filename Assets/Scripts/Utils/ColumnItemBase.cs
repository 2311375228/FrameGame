using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ColumnItemBase : MonoBehaviour
{
    [System.NonSerialized]
    public int numIndexItem;
    [System.NonSerialized]
    public int numIndexData;
    public System.Action<int, int> actionBase;
}
