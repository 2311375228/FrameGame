using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDungeon : MonoBehaviour
{
    public GameObject goTaskBase;

    [System.NonSerialized]
    public int intDungeonID;

    float floHeightMagic = 0.6f;//特效高度
    float floHeightEnemyRank = 1.275f;//敌人等级显示高度
    List<GameObject> listGoEnemy = new List<GameObject>();
    List<GameObject> listMagicEffect = new List<GameObject>();
    List<GameObject> listEnemyRank = new List<GameObject>();
    List<GameObject> listTaskTarget = new List<GameObject>();

    //每个任务点:一开始0至2秒内每个敌人点随机一个动作,之后2至5面内随机一个动作
    float[] floAnimTimes;
    float[] floAnimTimeRandoms;

    // Start is called before the first frame update
    void Start()
    {
        PropertiesDungeon itemDungeon = UserValue.Instance.dicDungeon[ intDungeonID];
        for (int i = 0; i < goTaskBase.transform.childCount; i++)
        {
            EventTaskEnemy eventTask = goTaskBase.transform.GetChild(i).GetComponent<EventTaskEnemy>();
            if (i < itemDungeon.points.Length)
            {
                eventTask.intDungeonID = intDungeonID;
                eventTask.intDungeonPointIndex = i;
                eventTask.actionMouseUp = ActionTaskPoint;
                GameObject goModel = eventTask.transform.GetChild(0).gameObject;
                Vector3 vecPosition = goModel.transform.position;
                Vector3 vecEulerAngles = goModel.transform.eulerAngles;
                Vector3 vecScale = goModel.transform.localScale;
                Destroy(goModel);
                JsonValue.DataTableEnemyItem itemEnemy = ManagerCombat.Instance.GetEnemyItem(itemDungeon.points[i].teams[0].intIDs[0]);
                goModel = ManagerResources.Instance.GetEnemyModel(itemEnemy.strModelName);
                goModel = Instantiate(goModel);
                goModel.transform.position = vecPosition;
                goModel.transform.eulerAngles = vecEulerAngles;
                goModel.transform.localScale = vecScale;
                listGoEnemy.Add(goModel);

            }
            else
            {
                eventTask.gameObject.SetActive(false);
            }
        }
        Show();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < listTaskTarget.Count; i++)
        {
            if (listTaskTarget[i] != null)
            {
                listTaskTarget[i].transform.RotateAround(listTaskTarget[i].transform.position, Vector3.up, 50 * Time.deltaTime);
            }
        }
        for (int i = 0; i < listGoEnemy.Count; i++)
        {
            floAnimTimes[i] += Time.deltaTime;
            if (floAnimTimeRandoms[i] < floAnimTimes[i])
            {
                floAnimTimes[i] = 0;
                floAnimTimeRandoms[i] = Random.Range(2.0f, 5.0f);
                Animator animTemp = listGoEnemy[i].GetComponent<Animator>();
                animTemp.SetBool("idle1", Random.Range(0, 2) == 0);
                animTemp.SetBool("idle2", Random.Range(0, 2) == 0);
            }
        }
    }

    public void Show()
    {

        Dictionary<int, PropertiesDungeon> dicDungeon = UserValue.Instance.dicDungeon;
        PropertiesDungeon dungeonItem = dicDungeon[intDungeonID];

        for (int i = 0; i < listEnemyRank.Count; i++)
        {
            Destroy(listEnemyRank[i]);
        }
        for (int i = 0; i < listMagicEffect.Count; i++)
        {
            Destroy(listMagicEffect[i]);
        }
        for (int i = 0; i < listTaskTarget.Count; i++)
        {
            Destroy(listTaskTarget[i]);
        }

        List<PropertiesTask> listTask = UserValue.Instance.listTask;
        GameObject goModel = null;
        JsonValue.DataGameDungeonItem itemDungeon = ManagerCombat.Instance.GetGameDungeonItem(intDungeonID);
        floAnimTimes = new float[dungeonItem.points.Length];
        floAnimTimeRandoms = new float[dungeonItem.points.Length];
        for (int i = 0; i < floAnimTimeRandoms.Length; i++)
        {
            floAnimTimeRandoms[i] = Random.Range(0.0f, 2.0f);
        }
        for (int i = 0; i < dungeonItem.points.Length; i++)
        {
            //标记boos
            Vector3 vecPosition = listGoEnemy[i].transform.position;
            if (!(itemDungeon.taskPoints[i].strEffect == null || itemDungeon.taskPoints[i].strEffect == ""))
            {
                goModel = ManagerResources.Instance.GetMapModel(itemDungeon.taskPoints[i].strEffect);
                goModel = Instantiate(goModel);
                vecPosition.y = floHeightMagic;
                goModel.transform.position = vecPosition;
                listMagicEffect.Add(goModel);

                if (dungeonItem.points[i].intStar == 0)
                {
                    goModel = ManagerResources.Instance.GetMapModel("boss");
                    goModel = Instantiate(goModel);
                    vecPosition.y = floHeightEnemyRank;
                    goModel.transform.position = vecPosition;
                    listEnemyRank.Add(goModel);
                }
            }
            //已经打过的副本,标记星星
            if (dungeonItem.points[i].intStar > 0)
            {
                goModel = ManagerResources.Instance.GetMapModel("finish");
                goModel = Instantiate(goModel);
                vecPosition.y = floHeightEnemyRank;
                goModel.transform.position = vecPosition;

                for (int j = 0; j < 3; j++)
                {
                    if (j >= dungeonItem.points[i].intStar)
                    {
                        goModel.transform.GetChild(j * 2 + 1).gameObject.SetActive(false);
                        goModel.transform.GetChild(j * 2).gameObject.SetActive(true);
                    }
                    else
                    {
                        goModel.transform.GetChild(j * 2 + 1).gameObject.SetActive(true);
                        goModel.transform.GetChild(j * 2).gameObject.SetActive(false);
                    }
                }

                listEnemyRank.Add(goModel);
            }
            //检查是否是任务点
            for (int j = 0; j < listTask.Count; j++)
            {
                if (listTask[j].enumTask == EnumTaskType.Dungeon)
                {
                    if (listTask[j].intDungeonID == intDungeonID && listTask[j].intDungeonIndex == i)
                    {
                        goModel = ManagerResources.Instance.GetMapModel("TaskTarget");
                        goModel = Instantiate(goModel);
                        vecPosition.y = floHeightEnemyRank;
                        goModel.transform.position = vecPosition;
                        listTaskTarget.Add(goModel);
                    }
                }
            }
        }
    }

    void ActionTaskPoint(int intDungeonID, int intDungeonPointIndex)
    {
        ManagerValue.actionAudio(EnumAudio.Ground);
        ManagerValue.intDungeonID = intDungeonID;
        ManagerValue.intDungeonPointIndex = intDungeonPointIndex;
        ManagerView.Instance.Show(EnumView.ViewGameDungeon);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < listGoEnemy.Count; i++)
        {
            Destroy(listGoEnemy[i]);
        }
        for (int i = 0; i < listMagicEffect.Count; i++)
        {
            Destroy(listMagicEffect[i]);
        }
        for (int i = 0; i < listEnemyRank.Count; i++)
        {
            Destroy(listEnemyRank[i]);
        }
        for (int i = 0; i < listTaskTarget.Count; i++)
        {
            Destroy(listTaskTarget[i]);
        }
    }
}
