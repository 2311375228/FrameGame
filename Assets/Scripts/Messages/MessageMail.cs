using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageMail : ManagerMessage.MessageBase
{
    public string strContent;
    public string strIconName;

    public int[] intIndexIDs;
    public int[] intRanks;
    public int[] intIndexCounts;

    public EnumKnapsackStockType[] gridItems;
    public ViewBarTop_ItemMail.EnumMail enumMail;
}
