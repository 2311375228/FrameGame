using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControllerGround : MonoBehaviour
{
    bool booIsPointUI;
    int numTargetDown;
    int numTargetUp;
    int intIntGroundSelect;
    Vector2 vecMousePosion;
    [SerializeField]
    Material[] matGrounds;
    [SerializeField]
    GameObject[] goBuildHinders;
    [SerializeField]
    Transform transGroundChoice;
    [SerializeField]
    GameObject goLand;

    int intX = 32;
    int intY = 20;
    Dictionary<int, PropertiesGround> dicGround = new Dictionary<int, PropertiesGround>();
    List<MeshRenderer> listGroundRender = new List<MeshRenderer>();

    int intIndexNPC = 10000;
    Dictionary<string, int> dicBuildNPC = new Dictionary<string, int>();
    int[] intOres = new int[] { 1008, 1009, 1010, 1011 };

    int intBuildOrder;//建筑的建造顺序
    string strGroundName = "Ground";
    ViewMGBuyBuild groundMessage = new ViewMGBuyBuild();

    ViewHint.MessageHint viewHint = new ViewHint.MessageHint();
    ViewHintBar.MessageHintBar viewHintBar = new ViewHintBar.MessageHintBar();
    ViewBuild_Base.GroundToView messageGroundToView = new ViewBuild_Base.GroundToView();
    ViewBuild_Base.BuyBuildToView messageBuildToView = new ViewBuild_Base.BuyBuildToView();
    void Start()
    {
        UserValue.Instance.SetDicGround = dicGround;

        //添加NPC=
        int intIndexNPCAdd = intIndexNPC;
        dicBuildNPC.Add("NPCBuildFarm", intIndexNPCAdd++);
        //dicBuildNPC.Add("NPCBuildFactory", intIndexNPCAdd++);
        dicBuildNPC.Add("NPCBuildSell", intIndexNPCAdd++);
        AddNPC();

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
            if (numVertical >= 0 && numVertical <= 100 && numHorizontal >= 0 && numHorizontal <= 100)
            {
                int numGround = (numVertical * intX) + numHorizontal;
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
            if (dicBuildNPC.ContainsKey(hit.transform.name))
            {
                CheckGroundState(dicBuildNPC[hit.transform.name]);
                return;
            }
            int numVertical = (int)hit.point.z;
            int numHorizontal = (int)hit.point.x;
            if (numVertical >= 0 && numVertical <= 100 && numHorizontal >= 0 && numHorizontal <= 100
                && Vector3.Distance(Input.mousePosition, vecMousePosion) < 10f)
            {
                int numGround = (numVertical * intX) + numHorizontal;
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

    /// <summary>
    /// 创建地图
    /// </summary>
    void RandomMap(int intUnbuildableLand, int intHinderCount, int intOreCount)
    {
        intBuildOrder = 0;
        UserValue.Instance.GetAllBuild().Clear();
        UserValue.Instance.GetAllBuildType().Clear();

        if (listGroundRender.Count == 0)
        {
            for (int i = 0; i < goLand.transform.childCount; i++)
            {
                for (int j = 0; j < goLand.transform.GetChild(i).childCount; j++)
                {
                    listGroundRender.Add(goLand.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>());
                }
            }
        }
        else
        {
            for (int i = 0; i < listGroundRender.Count; i++)
            {
                listGroundRender[i].material = matGrounds[1];
            }
        }

        //26个不能购买的地方，50个需要花钱购买的地方
        //0，200，1600，12800，
        //50金币，100金币，150金币；350金币，450金币，550金币；
        List<int> listRandomHinder = new List<int>();//有障碍物的土地
        List<int> listRandomNone = new List<int>();//不能购买的土地
        List<int> listIndexMat = new List<int>();//材质下标

        int intRandomTempY = 0;
        int intRandomTempX = 0;
        List<int> listIndexOre = new List<int>();//有效且没有阻碍物品
        //已经筛选好,具体位置
        while (true)
        {
            //筛选
            intRandomTempX = Random.Range(0, intX);//0-10,20-30,40-50的格子下标有效
            intRandomTempY = Random.Range(0, intY);
            int intIndex = intRandomTempY * intX + intRandomTempX;
            if (listRandomHinder.Count != intHinderCount)//阻碍物数量
            {
                if (!listRandomHinder.Contains(intIndex))
                {
                    listRandomHinder.Add(intIndex);
                }
                continue;
            }
            //达到不能建造土地上限,则停止循环的执行
            if (listRandomNone.Count == intUnbuildableLand)
            {
                if (listIndexOre.Count == intOreCount)
                {
                    break;
                }
                if (!listRandomHinder.Contains(intIndex) && !listRandomNone.Contains(intIndex) && !listIndexOre.Contains(intIndex))
                {
                    listIndexOre.Add(intIndex);
                }
                continue;
            }
            //这种写法 阻碍物 与 不能建造 在同一个地点
            if (!listRandomHinder.Contains(intIndex) && !listRandomNone.Contains(intIndex))
            {
                int intTemp = intRandomTempY * intX + intRandomTempX;
                listGroundRender[intTemp].material = matGrounds[0];
                listRandomNone.Add(intIndex);
                listIndexMat.Add(intTemp);
            }
        }

        PropertiesGround groudProperties = null;
        int intIndexOre = 0;
        for (int i = 0; i < 1000; i++)
        {
            if (dicGround.ContainsKey(i))
            {
                groudProperties = dicGround[i];
                if (groudProperties.buildBase != null)
                {
                    groudProperties.buildBase.SetBuildID = -1;
                }
                Destroy(groudProperties.goBuild);
            }
            else
            {
                groudProperties = new PropertiesGround();
                groudProperties.SetIndex = -1;
                dicGround.Add(i, groudProperties);
            }

            groudProperties.SetState = EnumGroudState.Unpurchased;
            groudProperties.SetPrice = 200;

            groudProperties.SetIntGround = i;
            groudProperties.vecPosition.x = (i % intX + 0.5f);
            groudProperties.vecPosition.z = i / intX + 0.5f;

            //这里产生障碍物会执行400次,但是符合下标的只有少数
            //随机生成哪一个阻碍物
            int intRandom = Random.Range(0, goBuildHinders.Length);
            if (listRandomHinder.Contains(i))
            {
                GameObject go = Instantiate(goBuildHinders[intRandom]);
                go.transform.position = groudProperties.vecPosition;
                go.SetActive(true);
                groudProperties.intBuildID = intRandom;
                groudProperties.goBuild = go;
                groudProperties.SetState = EnumGroudState.Hinder;
            }

            //不能建造的地
            if (listRandomNone.Contains(i))
            {
                GameObject go = Instantiate(goBuildHinders[intRandom]);
                go.transform.position = groudProperties.vecPosition;
                go.SetActive(true);
                groudProperties.intObstacleMat = listIndexMat[listRandomNone.FindIndex(x => (x.Equals(i)))];
                groudProperties.intBuildID = intRandom;
                groudProperties.goBuild = go;
                groudProperties.SetState = EnumGroudState.Obstacle;
            }

            //矿点
            if (listIndexOre.Contains(i))
            {
                MessageGround mgGround = new MessageGround();
                mgGround.groundState = EnumGroudState.BuildingPruchased;
                mgGround.numIndexGround = i;
                switch (intOreCount)
                {
                    case 4:
                        mgGround.intBuildID = intOres[intIndexOre++];
                        break;
                    case 8:
                        mgGround.intBuildID = intOres[intIndexOre++];
                        if (intIndexOre == 4)
                        {
                            intIndexOre = 0;
                        }
                        break;
                    default:
                        if (intIndexOre < 4)
                        {
                            mgGround.intBuildID = intOres[intIndexOre++];
                        }
                        else
                        {
                            intIndexOre++;
                            mgGround.intBuildID = intOres[Random.Range(0, intOres.Length)];
                        }
                        break;
                }

                mgGround.strModelNameNew = ManagerBuild.Instance.GetBuildItem(mgGround.intBuildID).strModelName;
                MessageUpdateGround(mgGround);
            }
        }

        UserValue.Instance.GetAllBuildProduceSee().Clear();
        if (UserValue.Instance.GetStockCountAll() != null)
        {
            UserValue.Instance.GetStockCountAll().Clear();
        }

        UserValue.Instance.UpdateStock();
    }


    /// <summary>
    /// 读取保存的游戏
    /// </summary>
    public void ReadSaveGame(PropertiesGround[] ground)
    {
        UserValue.Instance.GetAllBuild().Clear();
        UserValue.Instance.GetAllBuildType().Clear();
        UserValue.Instance.GetAllBuildProduceSee().Clear();
        if (UserValue.Instance.GetStockCountAll() != null)
        {
            UserValue.Instance.GetStockCountAll().Clear();
        }

        intBuildOrder = 0;
        if (listGroundRender.Count == 0)
        {
            for (int i = 0; i < goLand.transform.childCount; i++)
            {
                for (int j = 0; j < goLand.transform.GetChild(i).childCount; j++)
                {
                    listGroundRender.Add(goLand.transform.GetChild(i).GetChild(j).GetComponent<MeshRenderer>());
                }
            }
        }
        else
        {
            for (int i = 0; i < listGroundRender.Count; i++)
            {
                listGroundRender[i].material = matGrounds[1];
            }
        }
        foreach (PropertiesGround temp in dicGround.Values)
        {
            if (temp.goBuild != null)
            {
                Destroy(temp.goBuild);
            }
        }
        dicGround.Clear();

        int intTempMax = 0;//建筑建造顺序的最大值
        //key=建筑建造顺序
        Dictionary<int, PropertiesGround> dicTemp = new Dictionary<int, PropertiesGround>();
        for (int i = 0; i < ground.Length; i++)
        {
            if (ground[i].GetState == EnumGroudState.Obstacle)
            {
                listGroundRender[ground[i].intObstacleMat].material = matGrounds[0];
                if (ground[i].intBuildID != -1)
                {
                    ground[i].goBuild = Instantiate(goBuildHinders[ground[i].intBuildID]);
                    ground[i].goBuild.transform.position = ground[i].vecPosition;
                }
            }
            else if (ground[i].GetState == EnumGroudState.Hinder)
            {
                if (ground[i].intBuildID != -1)
                {
                    ground[i].goBuild = Instantiate(goBuildHinders[ground[i].intBuildID]);
                    ground[i].goBuild.transform.position = ground[i].vecPosition;
                }
            }
            else if (ground[i].GetState == EnumGroudState.Purchased)
            {
                ground[i].goBuild = GetPoolGameObject(strGroundName, ground[i].vecPosition);
                ground[i].goBuild.transform.position = ground[i].vecPosition;
            }
            else if (ground[i].GetState == EnumGroudState.BuildConstruction)
            {
                if (intTempMax < ground[i].GetIndex)
                {
                    intTempMax = ground[i].GetIndex;
                }
                dicTemp.Add(ground[i].GetIndex, ground[i]);
            }
            else if (ground[i].GetState == EnumGroudState.BuildingPruchased)
            {
                if (intIndexNPC > ground[i].GetIntGround)
                {
                    if (intTempMax < ground[i].GetIndex)
                    {
                        intTempMax = ground[i].GetIndex;
                    }
                    dicTemp.Add(ground[i].GetIndex, ground[i]);
                }
                else
                {
                    foreach (KeyValuePair<string, int> temp in dicBuildNPC)
                    {
                        if (temp.Value == ground[i].GetIntGround)
                        {
                            ground[i].goBuild = ManagerResources.Instance.GetBuildGameObject(temp.Key);
                            ground[i].goBuild = Instantiate(ground[i].goBuild);
                            ground[i].goBuild.name = temp.Key;
                            BuildBase buildBase = ground[i].goBuild.GetComponent<BuildBase>();
                            buildBase.SetIndexGround = temp.Value;
                            buildBase.GameReadData(ground[i].strReadData);
                            ground[i].buildBase = buildBase;
                            UserValue.Instance.SetBuild(buildBase.GetIndexGround, buildBase);
                            ground[i].goBuild.transform.position = ground[i].vecPosition;
                            break;
                        }
                    }
                }
            }
            dicGround.Add(ground[i].GetIntGround, ground[i]);
        }
        //为了让 “产看库存” 中的物品按保存时的顺序排列
        for (int i = 0; i < intTempMax + 1; i++)
        {
            foreach (KeyValuePair<int, PropertiesGround> temp in dicTemp)
            {
                if (temp.Key == i)
                {
                    JsonValue.DataTableBuildingItem item = ManagerBuild.Instance.GetBuildItem(temp.Value.intBuildID);
                    temp.Value.goBuild = GetPoolGameObject(item.strModelName, temp.Value.vecPosition);
                    temp.Value.buildBase = temp.Value.goBuild.GetComponent<BuildBase>();
                    temp.Value.buildBase.GameReadData(temp.Value.strReadData);
                    if (temp.Value.buildBase != null && temp.Value.GetState != EnumGroudState.Unpurchased)
                    {
                        temp.Value.buildBase.SetIndexGround = temp.Value.GetIntGround;
                        temp.Value.buildBase.SetBuildID = temp.Value.intBuildID;
                        UserValue.Instance.SetBuild(temp.Value.buildBase.GetIndexGround, temp.Value.buildBase);
                    }
                    temp.Value.goBuild.transform.position = temp.Value.vecPosition;
                    temp.Value.SetIndex = intBuildOrder;

                    intBuildOrder++;
                    break;
                }
            }
        }

        UserValue.Instance.UpdateStock();
    }

    /// <summary>
    /// 添加NPC建筑
    /// </summary>
    void AddNPC()
    {
        foreach (KeyValuePair<string, int> temp in dicBuildNPC)
        {
            PropertiesGround groudProperties = new PropertiesGround();
            dicGround.Add(temp.Value, groudProperties);
            groudProperties.SetIndex = -1;
            groudProperties.SetIntGround = temp.Value;
            groudProperties.SetState = EnumGroudState.BuildingPruchased;
            groudProperties.goBuild = ManagerResources.Instance.GetBuildGameObject(temp.Key);
            groudProperties.goBuild = Instantiate(groudProperties.goBuild);
            groudProperties.goBuild.name = temp.Key;
            BuildBase buildBase = groudProperties.goBuild.GetComponent<BuildBase>();
            if (buildBase != null && groudProperties.GetState != EnumGroudState.Unpurchased)
            {
                buildBase.SetIndexGround = temp.Value;
                groudProperties.buildBase = buildBase;
                UserValue.Instance.SetBuild(buildBase.GetIndexGround, buildBase);
            }
            switch (temp.Value - intIndexNPC)
            {
                case 0:
                    groudProperties.goBuild.transform.position = new Vector3(2.5f, 0, -8);
                    break;
                case 1:
                    groudProperties.goBuild.transform.position = new Vector3(3.5f, 0, -8);
                    break;
                case 2:
                    groudProperties.goBuild.transform.position = new Vector3(7.5f, 0, -8);
                    break;
                case 3:
                    break;
            }
            groudProperties.vecPosition = groudProperties.goBuild.transform.position;
        }
    }

    /// <summary>
    /// 检查土地状态，根据状态作处理
    /// </summary>
    void CheckGroundState(int numGround)
    {
        intIntGroundSelect = numGround;
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
                ManagerValue.actionAudio(EnumAudio.Ground);
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
            //已经购买了建筑，新建一个工地
            case EnumGroudState.BuildConstruction:
                ManagerValue.actionAudio(EnumAudio.Ground);
                messageBuildToView.intIndexGround = numGround;
                messageBuildToView.groundState = dicGround[numGround].GetState;
                messageBuildToView.strModelNameNew = strGroundName;
                messageBuildToView.enumKeyUp = ViewBuild_Base.EnumViewUp.ViewBuild_BuildConstruction;

                BuildBase build = dicGround[numGround].buildBase;
                if (build != null)
                {
                    build.ShowViewBuildInfo();
                }
                break;
            //已经建造好的建筑，发送消息到BuildBase
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
                case EnumGroudState.BuildConstruction:
                    ground.SetState = EnumGroudState.Purchased;
                    UserValue.Instance.RemoveBuild(mg.intIndexGround);
                    Destroy(ground.goBuild);
                    ground.goBuild = GetPoolGameObject(strGroundName, ground.vecPosition);
                    break;
                case EnumGroudState.BuildingPruchased:
                    Debug.Log("?");
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
                case EnumGroudState.BuildConstruction://此时状态为工地
                    if (groundTemp.goBuild != null)
                    {
                        Destroy(groundTemp.goBuild);
                    }
                    JsonValue.DataTableBuildingItem tableBuild = ManagerBuild.Instance.GetBuildItem(4007);
                    groundTemp.goBuild = GetPoolGameObject(tableBuild.strModelName, groundTemp.vecPosition);
                    BuildConstruction buildConstruction = groundTemp.goBuild.GetComponent<BuildConstruction>();
                    if (buildConstruction != null)
                    {
                        buildConstruction.SetValue(mgGround.intBuildID);
                        groundTemp.intBuildID = tableBuild.intBuildID;
                    }
                    break;
                case EnumGroudState.BuildingPruchased://此时状态为有建筑
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

                if (ManagerValue.booGamePlaying && mgGround.numIndexGround == intIntGroundSelect)
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
        GameObject goBuild = null;
        if (strName != null)
        {
            goBuild = ManagerResources.Instance.GetBuildGameObject(strName);
        }
        //需要获得建筑类型，建筑ID
        if (goBuild != null)
        {
            goTemp = Instantiate(goBuild);
            vecTemp.y = goTemp.transform.position.y;
            goTemp.transform.position = vecTemp;
        }
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
