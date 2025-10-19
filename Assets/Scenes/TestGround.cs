using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGround : MonoBehaviour
{
    bool booIsPointUI;
    int numTargetDown;
    int numTargetUp;
    Vector2 vecMousePosion;
    [SerializeField]
    Material[] matGrounds;
    [SerializeField]
    GameObject[] goBuildHinders;
    [SerializeField]
    Transform transGroundChoice;
    [SerializeField]
    GameObject goLand;
    [SerializeField]
    Transform target;

    Dictionary<int, PropertiesGround> dicGround = new Dictionary<int, PropertiesGround>();

    int intTargetXmin = 0;
    int intTargetXmax = 1;
    int intTargetZmin = 0;
    int intTargetZman = 1;
    int intBuildOrder;//建筑的建造顺序
    string strGroundName = "Ground";
    ManagerGroundBuild managerBuild;
    Transform[] rectGrounds;

    ViewHintBar.MessageHintBar viewHintBar = new ViewHintBar.MessageHintBar();
    ViewBuild_Base.GroundToView messageGroundToView = new ViewBuild_Base.GroundToView();
    ViewBuild_Base.BuyBuildToView messageBuildToView = new ViewBuild_Base.BuyBuildToView();
    void Start()
    {

        GameObject goCopy = goLand.transform.GetChild(0).GetChild(0).gameObject;
        rectGrounds = new Transform[100];
        for (int i = 0; i < rectGrounds.Length; i++)
        {
            if (i == rectGrounds.Length - 1)
            {
                rectGrounds[i] = goCopy.transform;
                continue;
            }
        }

        UserValue.Instance.SetDicGround = dicGround;

        ManagerValue.actionGround = ActionViewBuildInfo;
        ManagerValue.ActionRandomBuildMap = RandomMap;
        ManagerValue.ActionReadSaveGame = ReadSaveGame;

        ManagerMessage.Instance.AddEvent(EnumMessage.Update_Ground, MessageUpdateGround);
        ManagerMessage.Instance.AddEvent(EnumMessage.Hide_GroundChoice, MessageHideGroundChoice);
        ManagerMessage.Instance.AddEvent(EnumMessage.DemolishBuild, MGDemolishBuild);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ManagerValue.booGamePlaying)
        {
            return;
        }

        MoveGround();

        //PC移动处理
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            //这里做Windows触控处理
            if (Input.GetMouseButtonDown(0))
            {
                //这个方法只适合PC
                booIsPointUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                PointDown();
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (booIsPointUI)
                {
                    return;
                }
                PointUp();
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                int intFinger = Input.GetTouch(0).fingerId;
                booIsPointUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(intFinger);
                PointDown();
            }
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (booIsPointUI)
                {
                    return;
                }
                PointUp();
            }
        }
    }

    void PointDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //单位格子，返回值 x=-10,z=-10
        if (Physics.Raycast(ray, out hit, 100))
        {
            int numVertical = (int)hit.point.z;
            int numHorizontal = (int)hit.point.x;
            if (numVertical >= 0 && numVertical <= 19 && numHorizontal >= 0 && numHorizontal <= 19)
            {
                int numGround = (numVertical * 20) + numHorizontal;
                numTargetDown = numGround;
                vecMousePosion = Input.mousePosition;
            }
            else
            {
                numTargetDown = -1;
            }
        }
    }

    void PointUp()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //单位格子，返回值 x=-10,z=-10
        if (Physics.Raycast(ray, out hit, 100))
        {
            int numVertical = (int)hit.point.z;
            int numHorizontal = (int)hit.point.x;
            if (numVertical >= 0 && numVertical <= 19 && numHorizontal >= 0 && numHorizontal <= 19
                && Vector3.Distance(Input.mousePosition, vecMousePosion) < 10f)
            {
                int numGround = (numVertical * 20) + numHorizontal;
                numTargetUp = numGround;
                if (numTargetDown == numTargetUp)
                {
                    CheckGroundState(numGround);
                }
                transGroundChoice.gameObject.SetActive(true);
                transGroundChoice.position = new Vector3(numHorizontal + 0.5f, transGroundChoice.position.y, numVertical + 0.5f);
            }

        }
    }

    void MoveGround()
    {
        //Z轴
        if (intTargetZmin > target.position.z)
        {
            intTargetZmin -= 1;
            intTargetZman -= 1;
        }
        if (intTargetZman < target.position.z)
        {
            intTargetZmin += 1;
            intTargetZman += 1;
        }
        //X轴
        if (intTargetXmin > target.position.x)
        {
            intTargetXmin -= 1;
            intTargetXmax -= 1;
        }
        if (intTargetXmax < target.position.x)
        {
            intTargetXmin += 1;
            intTargetXmax += 1;
        }
    }

    /// <summary>
    /// 创建地图
    /// </summary>
    void RandomMap(int intUnbuildableLand, int intHinderCount, int intOreCount)
    {
        //StartCoroutine(CreateMap());
    }

    IEnumerator CreateMap()
    {
        if (managerBuild == null)
        {
            managerBuild = new ManagerGroundBuild();
        }
        managerBuild.ResetGroundState();

        int intObstacle = 0;//随机障碍物编号
        int intObstaclePosition = 0;//随机障碍物位置编号
        int intMaxGround = 10000;
        List<int> listObstacle = new List<int>();
        PropertiesGround proTemp = null;
        for (int i = 0; i < 1500; i++)
        {
            //障碍物可以重复
            intObstacle = Random.Range(0, goBuildHinders.Length + 1);
            while (true)
            {
                //位置不可以重复
                intObstaclePosition = Random.Range(0, intMaxGround);
                if (!listObstacle.Contains(intObstaclePosition))
                {
                    listObstacle.Add(intObstaclePosition);
                    proTemp = managerBuild.GetGround(intObstaclePosition);
                    proTemp.SetState = EnumGroudState.Obstacle;
                    if (listObstacle[i] == goBuildHinders.Length)
                    {
                        //不用放置障碍物
                        proTemp.intObstacleMat = goBuildHinders.Length;
                    }
                    else
                    {
                        proTemp.intObstacleMat = intObstacle;
                    }

                    break;
                }
            }
        }

        yield return new WaitForEndOfFrame();
    }


    /// <summary>
    /// 读取保存的游戏
    /// </summary>
    public void ReadSaveGame(PropertiesGround[] ground)
    {
    }

    /// <summary>
    /// 检查土地状态，根据状态作处理
    /// </summary>
    void CheckGroundState(int numGround)
    {
        switch (dicGround[numGround].GetState)
        {
            //不可购买
            case EnumGroudState.Obstacle:
                ManagerValue.actionAudio(EnumAudio.Unable);
                viewHintBar.strHintBar = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ThisLCBP);//"该地无法购买!";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, viewHintBar);
                ManagerView.Instance.Hide(EnumView.ViewBuildMain);
                break;
            //土地有阻碍物品
            case EnumGroudState.Hinder:
                messageGroundToView.groundState = dicGround[numGround].GetState;
                messageGroundToView.numPrice = 500;
                messageGroundToView.intIndexGround = numGround;
                messageGroundToView.strModelNameOld = strGroundName;
                messageGroundToView.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuyGround;

                ManagerView.Instance.Show(EnumView.ViewBuildMain);
                ManagerValue.actionViewBuildMain(messageGroundToView);
                break;
            //未购买土地
            case EnumGroudState.Unpurchased:
                ManagerValue.actionAudio(EnumAudio.Ground);
                messageGroundToView.groundState = dicGround[numGround].GetState;
                messageGroundToView.numPrice = ManagerValue.DemolishLandrecycleCoin(ManagerValue.intGroundCount, true); //dicGround[numGround].GetPrice;
                messageGroundToView.intIndexGround = numGround;
                messageGroundToView.strModelNameOld = strGroundName;
                messageGroundToView.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuyGround;

                ManagerView.Instance.Show(EnumView.ViewBuildMain);
                ManagerValue.actionViewBuildMain(messageGroundToView);
                break;
            //已经购买土地 弹出建筑窗口
            case EnumGroudState.Purchased:
                ManagerValue.actionAudio(EnumAudio.Ground);
                messageBuildToView.intIndexGround = numGround;
                messageBuildToView.groundState = dicGround[numGround].GetState;
                messageBuildToView.strModelNameNew = strGroundName;
                messageBuildToView.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuyBuild;

                ManagerView.Instance.Show(EnumView.ViewBuildMain);
                ManagerValue.actionViewBuildMain(messageBuildToView);
                break;
            //已经购买了建筑 发送消息到BuildBase
            case EnumGroudState.BuildingPruchased:
                dicGround[numGround].buildBase.PlayAudio();
                dicGround[numGround].buildBase.ShowViewBuildInfo();
                break;

        }
    }
    /// <summary>
    /// 当前标记的选择图标
    /// </summary>
    void MessageHideGroundChoice(ManagerMessage.MessageBase message)
    {
        transGroundChoice.gameObject.SetActive(false);
    }
    /// <summary>
    /// 拆除建筑
    /// </summary>
    void MGDemolishBuild(ManagerMessage.MessageBase message)
    {
        MessageDemolishBuild mg = message as MessageDemolishBuild;
        if (mg != null)
        {
            PropertiesGround ground = dicGround[mg.intIndexGround];
            switch (ground.GetState)
            {
                case EnumGroudState.Purchased:
                    ground.SetState = EnumGroudState.Unpurchased;
                    Destroy(ground.goBuild);
                    ground.goBuild = GetPoolGameObject(null, ground.vecPosition);
                    UserValue.Instance.SetCoinAdd = ManagerValue.DemolishLandrecycleCoin(ManagerValue.intGroundCount - 1, false);
                    ManagerValue.intGroundCount--;
                    break;
                case EnumGroudState.BuildingPruchased:
                    ground.SetState = EnumGroudState.Purchased;
                    UserValue.Instance.RemoveBuild(mg.intIndexGround);
                    Destroy(ground.goBuild);
                    ground.goBuild = GetPoolGameObject(strGroundName, ground.vecPosition);
                    UserValue.Instance.SetCoinAdd = ManagerValue.DemolishBuildRecycleCoin(ground.buildBase.IntBuildTotalPrice);
                    break;
            }
            ManagerMessage.Instance.PostEvent(EnumMessage.Update_Coin);
        }
    }
    /// <summary>
    /// 更新土地状态，创建已购买的对应建筑
    /// 消息来源：ViewBuyHint.cs
    /// </summary>
    void MessageUpdateGround(ManagerMessage.MessageBase message)
    {
        MessageGround mgGround = message as MessageGround;
        if (mgGround != null)
        {
            PropertiesGround groundTemp = dicGround[mgGround.numIndexGround];
            groundTemp.SetState = mgGround.groundState;
            groundTemp.intBuildID = mgGround.intBuildID;
            //新旧模型名称是在 ViewBuilding 进行交换的
            //Ground的模型名称不用交换
            switch (groundTemp.GetState)
            {
                case EnumGroudState.Unpurchased://此时状态，为可以购买的草地
                    Destroy(groundTemp.goBuild);

                    //清除障碍物后，再次向界面发送消息
                    messageGroundToView.groundState = dicGround[mgGround.numIndexGround].GetState;
                    messageGroundToView.numPrice = ManagerValue.DemolishLandrecycleCoin(ManagerValue.intGroundCount, true); //dicGround[numGround].GetPrice;
                    messageGroundToView.intIndexGround = mgGround.numIndexGround;
                    messageGroundToView.strModelNameOld = strGroundName;

                    messageGroundToView.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuyGround;

                    ManagerView.Instance.Show(EnumView.ViewBuildMain);
                    ManagerValue.actionViewBuildMain(messageGroundToView);

                    break;
                case EnumGroudState.Purchased://此时状态为土地,没有建筑
                    ManagerValue.intGroundCount++;
                    groundTemp.goBuild = GetPoolGameObject(strGroundName, groundTemp.vecPosition);

                    messageBuildToView.intIndexGround = mgGround.numIndexGround;
                    messageBuildToView.groundState = dicGround[mgGround.numIndexGround].GetState;
                    messageBuildToView.strModelNameNew = strGroundName;

                    messageBuildToView.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuyBuild;

                    ManagerView.Instance.Show(EnumView.ViewBuildMain);
                    ManagerValue.actionViewBuildMain(messageBuildToView);
                    break;
                case EnumGroudState.BuildingPruchased:
                    if (groundTemp.goBuild != null)
                    {
                        Destroy(groundTemp.goBuild);
                    }
                    groundTemp.goBuild = GetPoolGameObject(mgGround.strModelNameNew, groundTemp.vecPosition);
                    break;
            }
            BuildBase buildBase = groundTemp.goBuild.GetComponent<BuildBase>();
            if (buildBase != null && groundTemp.GetState != EnumGroudState.Unpurchased)
            {
                buildBase.SetIndexGround = mgGround.numIndexGround;
                buildBase.SetBuildID = groundTemp.intBuildID;
                groundTemp.buildBase = buildBase;
                UserValue.Instance.SetBuild(buildBase.GetIndexGround, buildBase);

                groundTemp.SetIndex = intBuildOrder;
                intBuildOrder++;

                if (ManagerValue.booGamePlaying)
                {
                    dicGround[mgGround.numIndexGround].buildBase.ShowViewBuildInfo();
                }
            }
        }
    }

    /// <summary>
    /// 创建建筑
    /// </summary>
    GameObject GetPoolGameObject(string strName, Vector3 vecTemp)
    {
        GameObject goTemp = null;
        return goTemp;
    }

    void ActionViewBuildInfo(int numGround, MGViewToBuildBase toGround)
    {
        //这里如果没有承接对象，则代表与建筑无关
        if (dicGround[numGround].buildBase != null)
        {
            dicGround[numGround].buildBase.MGViewBuildInfo(toGround);
        }
    }

    private void OnDestroy()
    {
        ManagerValue.actionGround -= ActionViewBuildInfo;
        ManagerValue.ActionRandomBuildMap -= RandomMap;
        ManagerValue.ActionReadSaveGame -= ReadSaveGame;
        ManagerValue.actionGround = null;
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Update_Ground, MessageUpdateGround);
        ManagerMessage.Instance.RemoveEvent(EnumMessage.Hide_GroundChoice, MessageHideGroundChoice);
        ManagerMessage.Instance.RemoveEvent(EnumMessage.DemolishBuild, MGDemolishBuild);
    }
}
