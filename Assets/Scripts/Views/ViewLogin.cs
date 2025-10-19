using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewLogin : ViewBase
{
    public RawImage rawImageMap;

    public Text textPlaseInputName;//请输入昵称
    public Text textNickname;
    public Button btnGameReady;//准备游戏
    public Button btnReadGame;//读取存档
    public Button btnSetting;//游戏设置
    public Button btnGameIntroduction;//游戏介绍
    public Button[] btnSelectModes;//模式难度

    public Button btnSelectMode;//选择游戏难度
    public Button btnBack;
    public Button btnRandomMap;//随机地图
    public Button btnStartGame;//开始游戏
    public Button btnChangeName;//修改昵称
    public Button btnConfirmChange;//确定修改昵称

    public InputField inputNickName;

    public RectTransform rectGameStart;
    public RectTransform rectGameSelect;
    public GameObject goIntroduce;
    public GameObject goSelectMode;
    public GameObject goChangeNickname;
    public GameObject goSaveGame;

    int intUnbuildableLand = 0;//不能建造的地方
    int intHinderCount = 0;
    int intOreCount = 0;

    string strNickname = "NickName1";
    //Vector3 vecPositionRole = new Vector3(50.74f, 11.38f, -7.58f);
    //Vector3 vecPositionMap = new Vector3(15.9f, 30.51f, 10.33f);

    List<Text> listFileContent = new List<Text>();
    List<GameObject> listBtnRead = new List<GameObject>();
    List<GameObject> listBtnDelete = new List<GameObject>();

    JsonSaveGame.GameRoot[] dataReads;
    SaveGameRead readGame = new SaveGameRead();

    //普通模式,困难模式,维护金币,建造所需材料,会随着时间的增长而增长
    //困难模式=建筑维护+员工月薪+材料消耗数量+日期增长叠加的金币消耗
    //普通模式=建筑维护+员工月薪
    protected override void Start()
    {
        //byte[] getB = Tools.GetFileByte("hello");
        //Debug.Log(Tools.GetFileString(getB));

        //for (int i = 0; i < 5; i++)
        //{
        //    readGame.DeleteGame(i);
        //}
        //游戏准备
        btnGameReady.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            rectGameSelect.position = rectGameStart.position;
            rectGameSelect.gameObject.SetActive(true);
            rectGameStart.gameObject.SetActive(false);
            goIntroduce.SetActive(false);
            goSelectMode.SetActive(false);
            goChangeNickname.SetActive(false);
            goSaveGame.SetActive(false);
        });
        //读取存档
        btnReadGame.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            goSaveGame.SetActive(!goSaveGame.activeSelf);
            ReadGame();
        });
        //游戏设置
        btnSetting.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            ManagerView.Instance.Show(EnumView.ViewSetting);
        });
        //游戏介绍
        btnGameIntroduction.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            goIntroduce.SetActive(!goIntroduce.activeSelf);
        });
        //模式选择
        btnSelectMode.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            goSelectMode.SetActive(!goSelectMode.activeSelf);
        });
        btnSelectMode.gameObject.SetActive(false);
        //返回
        btnBack.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);

            rectGameStart.gameObject.SetActive(true);
            rectGameSelect.gameObject.SetActive(false);
            goSelectMode.SetActive(false);
            goChangeNickname.SetActive(false);
            goSaveGame.SetActive(false);
        });
        //开始游戏
        btnStartGame.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            StartGamePlay();
        });
        //随机地图
        btnRandomMap.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerValue.ActionRandomBuildMap(intUnbuildableLand, intHinderCount, intOreCount);
            goSelectMode.SetActive(false);
        });
        for (int i = 0; i < btnSelectModes.Length; i++)
        {
            btnSelectModes[i].onClick.AddListener(OnClickSelectGameMode(i));
        }
        //修改昵称
        btnChangeName.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            rectGameStart.gameObject.SetActive(false);
            rectGameSelect.gameObject.SetActive(true);
            goSelectMode.SetActive(false);
            goChangeNickname.SetActive(true);
            goSaveGame.SetActive(false);
        });
        //确定修改昵称
        btnConfirmChange.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            if (strNickname.Length > 0 && strNickname.Length < 16)
            {
                UserValue.Instance.SetNickname = strNickname;
                textNickname.text = strNickname;

                rectGameStart.gameObject.SetActive(false);
                rectGameSelect.gameObject.SetActive(true);
                goSelectMode.SetActive(false);
                goChangeNickname.SetActive(false);
                goSaveGame.SetActive(false);
            }
            else if (strNickname.Length >= 16)
            {
                string[] strNicknames = new string[] { "16" };
                viewHint.strHint = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.NicknameSNELOC, strNicknames);//"昵称不能超过16个字母或汉子";
                ManagerView.Instance.Show(EnumView.ViewHint);
                ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
            }
            else
            {
                viewHint.strHint = ManagerLanguage.Instance.GetWord(EnumLanguageWords.NicknameCBE);//"昵称不能为空";
                ManagerView.Instance.Show(EnumView.ViewHint);
                ManagerView.Instance.SetData(EnumView.ViewHint, viewHint);
            }
        });

        inputNickName.onValueChanged.AddListener((value) =>
        {
            strNickname = value;
        });

        textNickname.text = strNickname;
        inputNickName.text = strNickname;
        UserValue.Instance.SetNickname = strNickname;

        rectGameStart.gameObject.SetActive(true);
        rectGameSelect.gameObject.SetActive(false);

        StartCoroutine(WaitLoad());
        goIntroduce.SetActive(false);
        goSelectMode.SetActive(false);
        goChangeNickname.SetActive(false);
        goSaveGame.SetActive(false);
        goSelectMode.transform.position = goIntroduce.transform.position;
        goChangeNickname.transform.position = goIntroduce.transform.position;
        goSaveGame.transform.position = goIntroduce.transform.position;
        ManagerValue.enumGameMode = EnumGameMode.NormalMode;
        SelectGameMode(ManagerValue.enumGameMode);
        btnSelectMode.transform.GetChild(0).GetComponent<Text>().text = UserValue.Instance.GetGameModeName(ManagerValue.enumGameMode);

        for (int i = 0; i < goSaveGame.transform.GetChild(0).childCount; i++)
        {
            listFileContent.Add(goSaveGame.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>());
            listBtnRead.Add(goSaveGame.transform.GetChild(0).GetChild(i).GetChild(1).gameObject);
            listBtnDelete.Add(goSaveGame.transform.GetChild(0).GetChild(i).GetChild(2).gameObject);
            listBtnRead[i].GetComponent<Button>().onClick.AddListener(OnClickRead(i));
            listBtnDelete[i].GetComponent<Button>().onClick.AddListener(OnClickDelete(i));
        }

        rawImageMap.gameObject.SetActive(false);
        StartCoroutine(WaitRawImageShow());
    }

    public override void Show()
    {
        base.Show();

        rectGameStart.gameObject.SetActive(true);
        rectGameSelect.gameObject.SetActive(false);
        goSelectMode.SetActive(false);
        goChangeNickname.SetActive(false);
        goSaveGame.SetActive(false);

        textPlaseInputName.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.PleaseEAN) + ":";
        btnGameReady.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Prepare);
        btnReadGame.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Load);
        btnSetting.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Settings);
        btnGameIntroduction.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.GameIntroduction);
        btnBack.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Back);
        btnRandomMap.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.RandomMap);
        btnStartGame.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.StartGame);
        btnChangeName.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.ModifyNickname);
        btnConfirmChange.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Confirm);

        goIntroduce.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetStory(EnumLanguageStory.GameIntroduction);
    }

    UnityEngine.Events.UnityAction OnClickSelectGameMode(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            switch (intIndex)
            {
                case 0:
                    SelectGameMode(EnumGameMode.GuideMode);
                    break;
                case 1:
                    SelectGameMode(EnumGameMode.NoviceMode);
                    break;
                case 2:
                    SelectGameMode(EnumGameMode.NormalMode);
                    break;
                case 3:
                    SelectGameMode(EnumGameMode.DifficultMode);
                    break;
            }
            ManagerValue.ActionRandomBuildMap(intUnbuildableLand, intHinderCount, intOreCount);
        };
    }

    UnityEngine.Events.UnityAction OnClickRead(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerValue.booGamePlaying = true;

            ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.Ground);
            readGame.ReadGame(intIndex);

            ManagerView.Instance.Hide(EnumView.ViewLogin);
            ManagerView.Instance.Show(EnumView.ViewBarTop);
        };
    }
    UnityEngine.Events.UnityAction OnClickDelete(int intIndex)
    {
        return () =>
        {
            ManagerValue.actionAudio(EnumAudio.Unable);
            readGame.DeleteGame(intIndex);
            ReadGame();
        };
    }

    int SelectGameMode(EnumGameMode key)
    {
        ManagerValue.enumGameMode = key;
        switch (key)
        {
            case EnumGameMode.GuideMode:
                btnSelectMode.transform.GetChild(0).GetComponent<Text>().text = UserValue.Instance.GetGameModeName(ManagerValue.enumGameMode);
                UserValue.Instance.SetCoid = 100000;
                intUnbuildableLand = 0;
                intHinderCount = 50;
                intOreCount = 8;
                break;
            case EnumGameMode.NoviceMode:
                btnSelectMode.transform.GetChild(0).GetComponent<Text>().text = UserValue.Instance.GetGameModeName(ManagerValue.enumGameMode);
                UserValue.Instance.SetCoid = 15000;
                intUnbuildableLand = 0;
                intHinderCount = 50;
                intOreCount = 8;
                break;
            case EnumGameMode.NormalMode:
                btnSelectMode.transform.GetChild(0).GetComponent<Text>().text = UserValue.Instance.GetGameModeName(ManagerValue.enumGameMode);
                UserValue.Instance.SetCoid = 10000;
                //intUnbuildableLand = 25;
                intUnbuildableLand = 150;
                intHinderCount = 300;
                //intOreCount = 7;
                intOreCount = 26;
                break;
            case EnumGameMode.DifficultMode:
                btnSelectMode.transform.GetChild(0).GetComponent<Text>().text = UserValue.Instance.GetGameModeName(ManagerValue.enumGameMode);
                UserValue.Instance.SetCoid = 10000;
                intUnbuildableLand = 45;
                intHinderCount = 50;
                intOreCount = 4;
                break;
        }

        return 0;
    }

    void StartGamePlay()
    {
        ManagerValue.booGamePlaying = true;
        ManagerValue.intDay = 15;
        ManagerValue.intMonth = 1;
        ManagerValue.intYear = ManagerValue.intInitYear;
        ManagerValue.intTotalDay = 0;
        ManagerValue.intTaskRank = 1;

        ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.Ground);

        UserValue.Instance.SetCoinMax = ManagerValue.longCoinMax;
        UserValue.Instance.GetEmployeeAll().Clear();
        UserValue.Instance.GetEmployeeGuestAll().Clear();
        UserValue.Instance.listTask.Clear();

        ManagerValue.intNewMailCount = 0;
        ManagerValue.listMail.Clear();

        BackpackGrid[] backpacks = UserValue.Instance.GetKnapsackItems();
        for (int i = 0; i < backpacks.Length; i++)
        {
            backpacks[i].enumStockType = EnumKnapsackStockType.None;
        }

        SelectGameMode(ManagerValue.enumGameMode);
        ManagerValue.intGroundCount = 0;

        ManagerCombat.Instance.CreateDungeonDate();
        UserValue.Instance.dicDungeon[100100].booFinishDungeon = true;

        ManagerValue.saveGame = null;
        ManagerView.Instance.Hide(EnumView.ViewLogin);
        ManagerView.Instance.Show(EnumView.ViewBarTop);
    }

    void ReadGame()
    {
        dataReads = readGame.LoadingReadGame();
        for (int i = 0; i < dataReads.Length; i++)
        {
            if (dataReads[i] == null)
            {
                listFileContent[i].text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.None);
                listBtnRead[i].gameObject.SetActive(false);
                listBtnDelete[i].gameObject.SetActive(false);
                continue;
            }
            listFileContent[i].text = dataReads[i].strGameNickName;
            listBtnRead[i].gameObject.SetActive(true);
            listBtnDelete[i].gameObject.SetActive(true);
            listBtnRead[i].transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Load);
            listBtnDelete[i].transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Delete);
        }
    }

    IEnumerator WaitLoad()
    {
        yield return 0;
        ManagerValue.actionCameraLocation(ControllerCamera.EnumCameraPosition.None);
        ManagerValue.ActionRandomBuildMap(intUnbuildableLand, intHinderCount, intOreCount);

        rawImageMap.texture = ManagerValue.cameraMap.targetTexture;
    }

    IEnumerator WaitRawImageShow()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return 0;
        }
        rawImageMap.gameObject.SetActive(true);
    }
}
