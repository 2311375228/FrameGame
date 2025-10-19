using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMGBuyBuild : ViewBase.Message
{
    public EnumGroudState groundState;
    public EnumView view;//需要关联的界面
    public int numIndexGround;//建筑所在格子，接受者要知道，完成任务后发给发送者
    public long numPrice;//仅接收者知道，并由接受者处理
    public int intBuildID;
    public string strModelNameNew;//新的模型 ViewBuilding发送这个数据，ViewBuyHint接收数据
    public string strModelNameOld;//旧的模型
}
