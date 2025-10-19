using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesGround
{
    int _index = -1;
    public int GetIndex { get { return _index; } }
    public int SetIndex { set { _index = value; } }
    int _intGround;
    public int GetIntGround { get { return _intGround; } }
    public int SetIntGround { set { _intGround = value; } }

    //土地状态
    EnumGroudState _state = EnumGroudState.Unpurchased;
    public EnumGroudState GetState { get { return _state; } }
    public EnumGroudState SetState { set { _state = value; } }

    //土地价格
    int _price = 200;
    public int GetPrice { get { return _price; } }
    public int SetPrice { set { _price = value; } }

    public int intObstacleMat;//不可购买土地的材质编号
    public int intBuildID;
    public string strReadData;

    public Vector3 vecPosition;

    public GameObject goBuild;
    public BuildBase buildBase;
}
