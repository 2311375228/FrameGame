using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCamera : MonoBehaviour
{
    float floSpeed = 0.2f;
    public Transform transCamera;
    public Transform transMoveTarget;

    public Camera cameraMap;//让这里统一处理
    public Camera cameraRole;//让这里统一处理
    [SerializeField]
    protected GameObject goEmployee;

    //Vector3 _vecPosMin = new Vector3(0.63f, 0, -0.26f);
    //Vector3 _vecPosMax = new Vector3(9f, 0, 8.7f);
    Vector3 _vecPosMin = new Vector3(0.63f, 0, -0.26f);
    Vector3 _vecPosMax = new Vector3(31.2f, 0, 18);

    Vector3 _vecDungeonMin = new Vector3(-0f, 0, -58.01f);
    Vector3 _vecDungeonMax = new Vector3(16.4f, 0, -49.82f);
    Vector3 vecCameraMin;
    Vector3 vecCameraMax;
    Vector3 InitBuildPos = new Vector3(1.8f, 2, 0.6f);
    Vector3 InitGungeonPosition = new Vector3(1f, 4.26f, -57f);

    Vector3 _vecRemenberPosition;
    public Vector3 vecRemenberPosition
    {
        get
        {
            return _vecRemenberPosition;
        }
        set
        {
            _vecRemenberPosition = value;
        }
    }
    [System.NonSerialized]
    public Vector3 vecRemenberPositionGundeon;

    int[] numMonths = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    float floTimeDate;
    MessageDate mgDate = new MessageDate();
    public static ControllerCamera Instance;

    EnumCameraPosition enumPosition = EnumCameraPosition.None;

    //任务系统目前设定为坚持1小时的游戏
    List<PropertiesTask> listTask;
    ViewHintBar.MessageHintBar hintBar = new ViewHintBar.MessageHintBar();

    //当前及其Update刷新率，可以得到平滑连续的效果

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        vecCameraMin = _vecPosMin;
        vecCameraMax = _vecPosMax;

        transCamera.position = new Vector3(10000, 0, 0);
        transCamera.localEulerAngles = Vector3.zero;
        transMoveTarget.position = new Vector3(10000, 0, 0);

        vecRemenberPosition = InitBuildPos;
        vecRemenberPositionGundeon = InitGungeonPosition;

        listTask = UserValue.Instance.listTask;

        ManagerValue.cameraEmployee = cameraRole;
        ManagerValue.cameraMap = cameraMap;

        ManagerValue.actionCameraLocation = CameraLocation;
        ManagerValue.ActionRoleTexture = ActionRoleTexture;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    transMoveTarget.position = new Vector3(5.954911f, 2, 6.502899f);
        //    transCamera.position= new Vector3(5.954911f, 2, 6.502899f);
        //}
        switch (enumPosition)
        {
            case EnumCameraPosition.Ground:
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    //if (Input.touchCount == 0)
                    //{
                    //    Debug.Log(1);
                    //    SlideSpeedPC();
                    //}
                    //else
                    {
                        SlideSpeedPCMove();
                    }
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    SlideSpeedIOS();
                }
                KeyboardSpeed();
                vecRemenberPosition = transMoveTarget.position;
                break;
            case EnumCameraPosition.Combat:
                break;
            case EnumCameraPosition.Market:
                break;
            case EnumCameraPosition.GameDungeon:
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    //SlideSpeedPC();
                    SlideSpeedPCMove();
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    SlideSpeedIOS();
                }
                KeyboardSpeed();
                vecRemenberPositionGundeon = transMoveTarget.position;
                break;
        }
    }

    private void LateUpdate()
    {
        if (ManagerValue.booGamePlaying)
        {
            transCamera.position = Vector3.Lerp(transCamera.position, transMoveTarget.position, 0.1f);

            floTimeDate += Time.deltaTime;
            if (floTimeDate > 1f)//1秒算1天
            {
                floTimeDate = 0;
                //发布新任务的日子
                if (mgDate.numMonth == 2 && mgDate.numDay == 1)
                {
                    ManagerValue.actionAudio(EnumAudio.Remind);
                    PropertiesTask[] tasks = ManagerTask.Instance.GetPropertiesTaskItem();
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    int intCoinIntPenalty = 0;
                    int intCoinAward = 0;
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        listTask.Add(tasks[i]);
                        intCoinAward += tasks[i].intAwardCion;
                        intCoinIntPenalty += tasks[i].intPenaltyCoin;
                    }
                    //tasks.Length + "个任务，完成任务时获得的总金币是" + intCoinAward.ToString("N0") + ",不能完成的任务的总罚金是" + intCoinIntPenalty.ToString("N0") + "金币";

                    string[] strCompetions = new string[3];
                    strCompetions[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, tasks.Length.ToString());
                    strCompetions[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intCoinAward.ToString("N0"));
                    strCompetions[2] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intCoinIntPenalty.ToString("N0"));
                    hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.CompletionOATAGCAAPOG, strCompetions);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                    ManagerMessage.Instance.PostEvent(EnumMessage.Update_Task);
                }

                //检查剩余任务的日子
                if (mgDate.numMonth == 1 && mgDate.numDay == 2)
                {
                    listTask = UserValue.Instance.listTask;
                    int intCoinIntPenalty = 0;
                    int intTaskCount = 0;
                    for (int i = 0; i < listTask.Count; i++)
                    {
                        intTaskCount++;
                        intCoinIntPenalty += listTask[i].intPenaltyCoin;
                    }
                    UserValue.Instance.SetCoinReduce(intCoinIntPenalty);
                    ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);

                    //listTask.Count + "个任务没有完成,共计罚金" + intCoinIntPenalty.ToString("N0") + "金币";
                    string[] strATotals = new string[2];
                    strATotals[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intTaskCount.ToString());
                    strATotals[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, intCoinIntPenalty.ToString("N0"));
                    hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.ATotalPOGCFNC, strATotals);
                    ManagerView.Instance.Show(EnumView.ViewHintBar);
                    ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);

                    UserValue.Instance.listTask.Clear();
                    ManagerMessage.Instance.PostEvent(EnumMessage.Update_Task);
                }

                if (mgDate.numMonth == 1 && mgDate.numDay == 15)
                {
                    //当大于20万的时候开始收税

                    //限制金额
                    float floTaxLimit = 2 * Mathf.Pow(10, 5);
                    //多于限制的金币
                    long longTaxMore = UserValue.Instance.GetCoin - (int)floTaxLimit;
                    //比去年多赚的钱
                    long longThan = UserValue.Instance.GetCoin - ManagerValue.longEndYearCoin;
                    long longThanYear = longThan;
                    if (longTaxMore > 0 && longThan > 0)
                    {
                        //每1万一个税，不足不计
                        longThan = longThan / 10000;
                        longThan *= 500;

                        if (longThan > 0)
                        {
                            UserValue.Instance.SetCoinReduce(longThan);
                            //大于20万开始收税，比去年多赚10000金币，收税500。
                            //"(超过" + 2 * Mathf.Pow(10, 5) + "金币，则开始收税" + 5 + "%) 税费：" + longTemp;
                            string[] strOvers = new string[3];
                            strOvers[0] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, floTaxLimit.ToString("N0"));
                            strOvers[1] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, longThanYear.ToString("N0"));
                            strOvers[2] = UserValue.Instance.GetStrColor(UserValue.EnumColorType.cyan, longThan.ToString("N0"));
                            hintBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.OverGCTTSA, strOvers);
                            ManagerView.Instance.Show(EnumView.ViewHintBar);
                            ManagerView.Instance.SetData(EnumView.ViewHintBar, hintBar);
                            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
                        }
                    }
                    ManagerValue.longEndYearCoin = UserValue.Instance.GetCoin;
                }

                ManagerValue.intDay += 1;
                ManagerValue.intTotalDay += 1;

                if (ManagerValue.intDay == numMonths[ManagerValue.intMonth - 1] + 1)
                {
                    ManagerValue.intDay = 1;
                    ManagerValue.intMonth += 1;
                    if (ManagerValue.intMonth == 13)
                    {
                        ManagerValue.intYear += 1;
                        ManagerValue.intMonth = 1;
                        if (ManagerValue.intYear % 4 == 0)
                        {
                            numMonths[1] = 29;
                        }
                        else
                        {
                            numMonths[1] = 28;
                        }
                    }
                }
                mgDate.numYear = ManagerValue.intYear;
                mgDate.numMonth = ManagerValue.intMonth;
                mgDate.numDay = ManagerValue.intDay;
                ManagerMessage.Instance.PostEvent(EnumMessage.Update_Date, mgDate);
            }
        }
    }
    void KeyboardSpeed()
    {
        float f = 2 * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
        {
            KeyboardMove(0, -f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            KeyboardMove(0, f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            KeyboardMove(f, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            KeyboardMove(-f, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            KeyboardMove(0, -f);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            KeyboardMove(0, f);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            KeyboardMove(f, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            KeyboardMove(-f, 0);
        }
    }
    void KeyboardMove(float x, float z)
    {

        if (x > 0 && transMoveTarget.position.x < vecCameraMin.x)
        {
            x = 0;
        }
        if (x < 0 && transMoveTarget.position.x > vecCameraMax.x)
        {
            x = 0;
        }
        if (z > 0 && transMoveTarget.position.z < vecCameraMin.z)
        {
            z = 0;
        }
        if (z < 0 && transMoveTarget.position.z > vecCameraMax.z)
        {
            z = 0;
        }
        transMoveTarget.position = new Vector3(transMoveTarget.position.x - x, transMoveTarget.position.y, transMoveTarget.position.z - z);
    }
    void SlideSpeedPC()
    {
        if (Input.GetMouseButton(0) && ManagerValue.booMoveCamera)
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            float x = Input.GetAxis("Mouse X");
            float z = Input.GetAxis("Mouse Y");
            if (x > 0 && transMoveTarget.position.x < vecCameraMin.x)
            {
                x = 0;
            }
            if (x < 0 && transMoveTarget.position.x > vecCameraMax.x)
            {
                x = 0;
            }
            if (z > 0 && transMoveTarget.position.z < vecCameraMin.z)
            {
                z = 0;
            }
            if (z < 0 && transMoveTarget.position.z > vecCameraMax.z)
            {
                z = 0;
            }
            transMoveTarget.position = new Vector3(transMoveTarget.position.x - x * floSpeed, transMoveTarget.position.y, transMoveTarget.position.z - z * floSpeed);
        }
    }

    Vector3 vecSpeedPCMove;
    Vector3 vecSpeedPCMoveDiff;
    void SlideSpeedPCMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            vecSpeedPCMove = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.touchCount == 0)
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
            }
            else
            {
                int intFinger = Input.GetTouch(0).fingerId;
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(intFinger))
                {
                    return;
                }
            }


            if (Vector3.Distance(vecSpeedPCMove, Input.mousePosition) > 150)
            {
                vecSpeedPCMove = Input.mousePosition;
            }
            vecSpeedPCMoveDiff = Input.mousePosition - vecSpeedPCMove;
            vecSpeedPCMoveDiff = vecSpeedPCMoveDiff * ManagerValue.floTouchSpeed;//0.005f;
            vecSpeedPCMove = Input.mousePosition;
            if (vecSpeedPCMoveDiff.x > 0 && transMoveTarget.position.x < vecCameraMin.x)
            {
                vecSpeedPCMoveDiff.x = 0;
            }
            if (vecSpeedPCMoveDiff.x < 0 && transMoveTarget.position.x > vecCameraMax.x)
            {
                vecSpeedPCMoveDiff.x = 0;
            }
            if (vecSpeedPCMoveDiff.y > 0 && transMoveTarget.position.z < vecCameraMin.z)
            {
                vecSpeedPCMoveDiff.y = 0;
            }
            if (vecSpeedPCMoveDiff.y < 0 && transMoveTarget.position.z > vecCameraMax.z)
            {
                vecSpeedPCMoveDiff.y = 0;
            }
            transMoveTarget.position = new Vector3(transMoveTarget.position.x - vecSpeedPCMoveDiff.x, transMoveTarget.position.y, transMoveTarget.position.z - vecSpeedPCMoveDiff.y);
        }
    }
    void SlideSpeedIOS()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            int intFinger = Input.GetTouch(0).fingerId;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(intFinger))
            {
                return;
            }
            float x = Input.GetAxis("Mouse X") * 0.02f;
            float z = Input.GetAxis("Mouse Y") * 0.02f;
            if (x > 0 && transMoveTarget.position.x < vecCameraMin.x)
            {
                x = 0;
            }
            if (x < 0 && transMoveTarget.position.x > vecCameraMax.x)
            {
                x = 0;
            }
            if (z > 0 && transMoveTarget.position.z < vecCameraMin.z)
            {
                z = 0;
            }
            if (z < 0 && transMoveTarget.position.z > vecCameraMax.z)
            {
                z = 0;
            }
            transMoveTarget.position = new Vector3(transMoveTarget.position.x - x, transMoveTarget.position.y, transMoveTarget.position.z - z);
        }
    }

    void CameraLocation(EnumCameraPosition enumPosition)
    {
        this.enumPosition = enumPosition;
        switch (enumPosition)
        {
            case EnumCameraPosition.None:
                vecRemenberPosition = InitBuildPos;
                transCamera.position = new Vector3(10000, 0, 0);
                transCamera.localEulerAngles = Vector3.zero;
                transMoveTarget.position = new Vector3(10000, 0, 0);
                break;
            case EnumCameraPosition.Ground:
                vecRemenberPositionGundeon = InitGungeonPosition;
                transCamera.position = vecRemenberPosition;
                transCamera.eulerAngles = new Vector3(60, 0, 0);
                transMoveTarget.position = vecRemenberPosition;

                vecCameraMin = _vecPosMin;
                vecCameraMax = _vecPosMax;
                break;
            case EnumCameraPosition.Combat:
                transCamera.position = new Vector3(61.66159f, 17.76f, -58.9f);
                transCamera.eulerAngles = new Vector3(60, 0, 0);
                transMoveTarget.position = new Vector3(61.66159f, 17.76f, -58.9f); ;
                break;
            case EnumCameraPosition.Market:
                vecRemenberPositionGundeon = InitGungeonPosition;

                transCamera.position = new Vector3(4.876872f, 2.87f, -10.04f);
                transCamera.eulerAngles = new Vector3(60, 0, 0);
                transMoveTarget.position = new Vector3(4.876872f, 2.87f, -10.04f);
                break;
            case EnumCameraPosition.GameDungeon:
                //暂时用固定视角
                transCamera.position = vecRemenberPositionGundeon;
                transCamera.eulerAngles = new Vector3(60, 0, 0);
                transMoveTarget.position = vecRemenberPositionGundeon;

                vecCameraMin = _vecDungeonMin;
                vecCameraMax = _vecDungeonMax;
                break;
            case EnumCameraPosition.GameDungeonChange:
                vecRemenberPositionGundeon = InitGungeonPosition;
                break;
        }
    }

    /// <summary>
    /// 外部调用 PropertiesRole 生成可用员工数据
    /// </summary>
    public void ActionRoleTexture(PropertiesEmployee pro)
    {
        StartCoroutine(LoadEmployeeHeadSprite(pro));
    }
    IEnumerator LoadEmployeeHeadSprite(PropertiesEmployee pro)
    {
        yield return 0;
        RenderTexture t = cameraRole.activeTexture;
        Sprite sprite = Tools.CaptureCamera(t);
        pro.spriteHead = sprite;
    }

    private void OnDestroy()
    {
        ManagerValue.actionCameraLocation -= CameraLocation;
        ManagerValue.ActionRoleTexture -= ActionRoleTexture;
    }

    public enum EnumCameraPosition
    {
        None,
        Ground,
        Market,//集市
        Combat,//战斗
        GameDungeon,//副本
        GameDungeonChange,//主要是重置摄像机视角
    }
}
