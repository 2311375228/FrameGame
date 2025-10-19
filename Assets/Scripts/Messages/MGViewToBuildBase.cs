using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGViewToBuildBase
{
    public EnumMessageType messageType;
    public int intIndexGround;

    public enum EnumMessageType
    {
        None,
        Close,
        Info,
        Demolish,
        SetEmployee,
    }
}
