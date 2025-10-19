using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMGToEmployeeAdd :ViewBase.Message
{
    public EnumView enumView;
    public EnumEmployeeProperties[] enumEmployeeProperties;
    public Dictionary<EnumEmployeeProperties, string> dicPropertiesInfo;
    public string strBuildName;
}
