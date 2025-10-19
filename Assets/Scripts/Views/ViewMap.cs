using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ViewMap : ViewBase
{
    public Button btnBack;
    public Button btnMap;
    public Button btnCombatTeam;
    public Button btnEmployeeSee;

    public RectTransform rectLevel;
    public Button btnEnter;//进入
    public Button btnSweep;//扫荡

    public Image imageArraw;
    public GameObject goImageMap;
    public RectTransform transMapPoint;

    int intIndexDungeon;
    Text[] textDungeonNames;
    RectTransform[] mapPoints;
    Dungeon[] dungeonStates;
    List<Image[]> listPointWay = new List<Image[]>();

    MapDungeon mapDungeon;

    //要加载 地图,地图关卡 数据
    //TableGameDungeon 描述这个关卡怪物位置,然后关联 TableTask
    //TableMap 描述有多少个副本,选择副本后进入副本地图 TableGameDungeon
    protected override void Start()
    {
        btnBack.onClick.AddListener(() =>
        {
            if (goImageMap.activeSelf)
            {
                ManagerValue.actionAudio(EnumAudio.Close);
                ManagerView.Instance.Hide(EnumView.ViewMap);
                ManagerView.Instance.Show(EnumView.ViewBarTop);
                ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.Ground);
            }
            else
            {
                ManagerValue.actionAudio(EnumAudio.Ground);
                goImageMap.SetActive(true);
                ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.GameDungeonChange);
            }

            if (mapDungeon != null)
            {
                Destroy(mapDungeon.gameObject);
            }
        });
        btnMap.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (mapDungeon != null)
            {
                Destroy(mapDungeon.gameObject);
            }
            goImageMap.SetActive(true);
        });
        btnCombatTeam.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewCombatTeam);
        });
        btnEmployeeSee.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewEmployeeList);
        });
        btnEmployeeSee.gameObject.SetActive(false);

        //进入副本
        btnEnter.onClick.AddListener(() =>
        {
            PropertiesDungeon dungeonItem = UserValue.Instance.dicDungeon[dungeonStates[intIndexDungeon].intID];
            if (dungeonItem.booFinishDungeon)
            {
                ManagerValue.actionAudio(EnumAudio.Ground);
                goImageMap.SetActive(false);
                rectLevel.gameObject.SetActive(false);
                SetGameDungeonMap(ManagerCombat.Instance.GetGameDungeonItem(dungeonItem.intDungeonID));
            }
            else
            {
                ManagerValue.actionAudio(EnumAudio.Unable);
                JsonValue.DataGameDungeonItem item = ManagerCombat.Instance.GetGameDungeonItem(dungeonStates[intIndexDungeon].intID - 1);
                ViewHintBar.MessageHintBar mgBar = new ViewHintBar.MessageHintBar();
                mgBar.strHintBar = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.ToEYNTDTBO, new string[] { ManagerCombat.Instance.GetGameDungeonName(dungeonStates[intIndexDungeon].intID - 1) });//"请击败" + item.GetName + "的boss";
                ManagerView.Instance.Show(EnumView.ViewHintBar);
                ManagerView.Instance.SetData(EnumView.ViewHintBar, mgBar);
            }
        });
        //扫荡
        btnSweep.onClick.AddListener(() =>
        {
        });
        btnSweep.gameObject.SetActive(false);

        imageArraw.gameObject.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        btnEnter.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Enter);
        btnBack.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Back);
        btnMap.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Map);
        btnCombatTeam.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.BattleQueue);

        rectLevel.gameObject.SetActive(false);
        goImageMap.SetActive(true);

        if (listPointWay.Count == 0)
        {
            mapPoints = new RectTransform[transMapPoint.childCount];
            dungeonStates = new Dungeon[mapPoints.Length];
            textDungeonNames = new Text[mapPoints.Length];
            for (int i = 0; i < transMapPoint.childCount; i++)
            {
                transMapPoint.GetChild(i).GetComponent<Button>().onClick.AddListener(OnClickMapPoint(i));
                mapPoints[i] = transMapPoint.GetChild(i).GetComponent<RectTransform>();
                textDungeonNames[i] = transMapPoint.GetChild(i).GetChild(0).GetChild(0).GetComponent<Text>();
                dungeonStates[i] = new Dungeon();
                dungeonStates[i].intID = 100100 + i;

                //设置路径
                if (i != 0)
                {
                    Vector3 vecTempStart = transMapPoint.GetChild(i - 1).position;
                    Vector3 vecTempEnd = transMapPoint.GetChild(i).position;
                    Vector3 vecTempNormal = vecTempEnd - vecTempStart;
                    vecTempNormal = vecTempNormal.normalized;
                    float floDis = Vector3.Distance(vecTempStart, vecTempEnd);
                    float flo = 70 * (Screen.width / 3840f);
                    int intArrowCount = (int)(floDis / flo);
                    if (transMapPoint.GetChild(i).gameObject.activeSelf == false)
                    {
                        continue;
                    }
                    Image[] images = new Image[intArrowCount];
                    for (int j = 0; j < intArrowCount; j++)
                    {
                        GameObject goTemp = Instantiate(imageArraw.gameObject, imageArraw.transform.parent, false);
                        goTemp.transform.position = vecTempStart + vecTempNormal * j * flo;
                        images[j] = goTemp.GetComponent<Image>();
                    }
                    listPointWay.Add(images);
                }
            }
        }
        for (int i = 0; i < textDungeonNames.Length; i++)
        {
            textDungeonNames[i].text = ManagerCombat.Instance.GetGameDungeonName(dungeonStates[i].intID);
        }

        Dictionary<int, PropertiesDungeon> dicDungeon = UserValue.Instance.dicDungeon;
        int[] intDungeonIDs = UserValue.Instance.dicDungeon.Keys.ToArray();
        for (int i = 0; i < intDungeonIDs.Length; i++)
        {
            for (int j = i; j < intDungeonIDs.Length; j++)
            {
                if (intDungeonIDs[i] > intDungeonIDs[j])
                {
                    int intTemp = intDungeonIDs[i];
                    intDungeonIDs[i] = intDungeonIDs[j];
                    intDungeonIDs[j] = intTemp;
                }
            }
        }
        for (int i = 1; i < intDungeonIDs.Length; i++)
        {
            for (int j = 0; j < listPointWay[i - 1].Length; j++)
            {
                if (!dicDungeon[intDungeonIDs[i]].booFinishDungeon)
                {
                    listPointWay[i - 1][j].color = new Color32(128, 128, 128, 200);
                }
                else
                {
                    listPointWay[i - 1][j].color = new Color32(0, 255, 0, 100);//new Color32(128,128,128,100);
                }
            }
        }

        if (mapDungeon != null)
        {
            goImageMap.SetActive(false);
            mapDungeon.Show();
        }
    }

    UnityAction OnClickMapPoint(int intIndex)
    {
        return delegate
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            intIndexDungeon = intIndex;
            rectLevel.position = mapPoints[intIndex].position;
            rectLevel.gameObject.SetActive(true);
            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.GameDungeon);
        };
    }

    /// <summary>
    /// 设置副本地图内容
    /// 清空地图点击事件标记,重新加载点击事件标记
    /// </summary>
    void SetGameDungeonMap(JsonValue.DataGameDungeonItem itemGameDungeon)
    {
        mapDungeon = Instantiate(ManagerResources.Instance.GetMapModel(itemGameDungeon.strDungeonModel), null, false).GetComponent<MapDungeon>();
        mapDungeon.intDungeonID = itemGameDungeon.intGameDungeonID;
    }

    public class Dungeon
    {
        public int intID;
    }
}
