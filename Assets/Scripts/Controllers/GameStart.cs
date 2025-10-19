using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public AudioClip acBtn;
    public AudioClip acBackgroundMusic;
    public AudioClip acBackgroundMusic2;
    public AudioClip acBackgroundMusic3;

    public Text textSound;
    public Text textScreen;

    public Button btnQuit;
    public Button btnStart;
    public Button btnSetting;
    public Button btnBack;

    public Slider sliderAudio;
    public Slider sliderBcakground;

    public Transform transLanguage;
    public Transform transSound;
    public Transform transScreen;

    public GameObject goLogin;
    public GameObject goSetting;
    public RectTransform rectCursor;

    List<GameObject> listLanguage = new List<GameObject>();
    List<GameObject> listScreen = new List<GameObject>();
    List<ManagerValue.EnumLanguageType> listEnumLanguage = new List<ManagerValue.EnumLanguageType>();
    Dictionary<ManagerValue.EnumLanguageType, string> dicLanguage = new Dictionary<ManagerValue.EnumLanguageType, string>()
    {
        { ManagerValue.EnumLanguageType.English,"English" },//0
        { ManagerValue.EnumLanguageType.China,"简体中文"},//1
        { ManagerValue.EnumLanguageType.Chinese_,"繁體中文"},//2
        { ManagerValue.EnumLanguageType.Japan,"日本語"},//3
        { ManagerValue.EnumLanguageType.French,"Français"},//4
        { ManagerValue.EnumLanguageType.Portuguese,"Português"},//5
        { ManagerValue.EnumLanguageType.Russian,"Русский язык"},//6
        { ManagerValue.EnumLanguageType.Korean,"한국어"},//7
        { ManagerValue.EnumLanguageType.Spanish,"Español"},//8
        //{ ManagerValue.EnumLanguageType.Hungarian,"Magyar"},//9
        { ManagerValue.EnumLanguageType.Italian,"Italiano"},//10
        { ManagerValue.EnumLanguageType.German,"Deutsch"},//11
        { ManagerValue.EnumLanguageType.Dutch,"Nederlands"},//12
    };

    bool booMusic;
    float floTime;

    float floTimeAudio;
    float floAudio;
    float floBackgroundMusic;

    bool booLoginSteam;
    float floLoginSteam;

    AudioSource audioBtn;
    AudioSource audioBackgroundMusic;
    // Start is called before the first frame update
    void Start()
    {
        btnQuit.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        btnStart.onClick.AddListener(() =>
        {
            AudioBtnPlay();
            if (booLoginSteam)
            {
                SceneManager.LoadScene("Main");
            }
            else
            {
                Application.Quit();
            }
        });
        btnSetting.onClick.AddListener(() =>
        {
            AudioBtnPlay();
            goLogin.SetActive(false);
            goSetting.SetActive(true);
        });
        btnBack.onClick.AddListener(() =>
        {
            AudioBtnPlay();
            goLogin.SetActive(true);
            goSetting.SetActive(false);
        });
        transSound.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioBtnPlay();
            transSound.GetChild(0).GetChild(1).gameObject.SetActive(!transSound.GetChild(0).GetChild(1).gameObject.activeSelf);
            ManagerValue.setting.booAudio = !ManagerValue.setting.booAudio;
            ManagerValue.SettingSave();
            transSound.GetChild(0).GetChild(1).gameObject.SetActive(ManagerValue.setting.booAudio);
        });
        transSound.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            AudioBtnPlay();
            transSound.GetChild(1).GetChild(1).gameObject.SetActive(!transSound.GetChild(1).GetChild(1).gameObject.activeSelf);
            ManagerValue.setting.booBackgroudMusic = !ManagerValue.setting.booBackgroudMusic;
            ManagerValue.SettingSave();
            transSound.GetChild(1).GetChild(1).gameObject.SetActive(ManagerValue.setting.booBackgroudMusic);
        });

        sliderAudio.onValueChanged.AddListener((value) =>
        {
            floAudio = value;
            audioBtn.volume = value;


        });
        sliderBcakground.onValueChanged.AddListener((value) =>
        {
            floBackgroundMusic = value;
            audioBackgroundMusic.volume = value;
        });

        GameObject goCopyLanguage = transLanguage.GetChild(0).gameObject;
        ManagerValue.EnumLanguageType[] lanuages = (ManagerValue.EnumLanguageType[])System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType));
        listEnumLanguage = new List<ManagerValue.EnumLanguageType>();
        for (int i = 0; i < lanuages.Length; i++)
        {
            if (lanuages[i] == ManagerValue.EnumLanguageType.Hungarian)
            {
                continue;
            }
            listEnumLanguage.Add(lanuages[i]);
        }
        int intIndexTemp = 0;
        for (int i = 0; i < listEnumLanguage.Count; i++)
        {
            listLanguage.Add(Instantiate(goCopyLanguage, goCopyLanguage.transform.parent, false));
            listLanguage[intIndexTemp].transform.GetChild(2).GetComponent<Text>().text = dicLanguage[listEnumLanguage[i]];
            listLanguage[intIndexTemp].GetComponent<Button>().onClick.AddListener(OnClickLanguage(i));
            intIndexTemp++;
        }
        goCopyLanguage.SetActive(false);

        for (int i = 0; i < transScreen.childCount; i++)
        {
            listScreen.Add(transScreen.GetChild(i).gameObject);
            listScreen[i].GetComponent<Button>().onClick.AddListener(OnClickScreen(i));
        }

        //Cursor.visible = false;

        goLogin.SetActive(true);
        goSetting.SetActive(false);
        //btnSetting.gameObject.SetActive(false);

        audioBtn = gameObject.AddComponent<AudioSource>();
        audioBtn.clip = acBtn;
        audioBackgroundMusic = gameObject.AddComponent<AudioSource>();
        audioBackgroundMusic.clip = acBackgroundMusic;

        ManagerValue.SettingRead();
        transSound.GetChild(0).GetChild(1).gameObject.SetActive(ManagerValue.setting.booAudio);
        transSound.GetChild(1).GetChild(1).gameObject.SetActive(ManagerValue.setting.booBackgroudMusic);
        SelectItem((int)ManagerValue.setting.enumLanguage, listLanguage);
        SelectItem((int)ManagerValue.setting.screenType, listScreen);

        //初始界面，要小窗化
        Vector2 vecScreen = ManagerValue.vecScreens[(int)ManagerValue.setting.screenType];
        Screen.SetResolution((int)vecScreen.x, (int)vecScreen.y, false);

        floAudio = ManagerValue.setting.floAudio;
        floBackgroundMusic = ManagerValue.setting.floBackgroundMusic;
        sliderAudio.value = floAudio;
        sliderBcakground.value = floBackgroundMusic;
        audioBtn.volume = floAudio;
        audioBackgroundMusic.volume = floBackgroundMusic;

        StartCoroutine(IEWait());
    }

    IEnumerator IEWait()
    {
        yield return 0;
        SetLanguage();
        {
            booLoginSteam = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rectCursor.position = Input.mousePosition;

        //登录页的音乐控制
        if (ManagerValue.setting.booBackgroudMusic)
        {
            if (!booMusic)
            {
                floTime += Time.deltaTime;
                if (floTime > 5)
                {
                    floTime = 0;
                    booMusic = true;
                    audioBackgroundMusic.Play();
                }
            }
            if (!audioBackgroundMusic.isPlaying && booMusic == true)
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        audioBackgroundMusic.clip = acBackgroundMusic;
                        break;
                    case 1:
                        audioBackgroundMusic.clip = acBackgroundMusic2;
                        break;
                    case 2:
                        audioBackgroundMusic.clip = acBackgroundMusic3;
                        break;
                }
                booMusic = false;
            }
        }
        else
        {
            if (audioBackgroundMusic.isPlaying)
            {
                audioBackgroundMusic.Stop();
            }
        }

        //保存数据
        floTimeAudio += Time.deltaTime;
        if (floTimeAudio > 0.5f)
        {
            floTimeAudio = 0;
            if (floAudio != ManagerValue.setting.floAudio)
            {
                ManagerValue.setting.floAudio = floAudio;
                ManagerValue.SettingSave();
            }
            if (floBackgroundMusic != ManagerValue.setting.floBackgroundMusic)
            {
                ManagerValue.setting.floBackgroundMusic = floBackgroundMusic;
                ManagerValue.SettingSave();
            }
        }

        //判断是否在线
        if (!booLoginSteam)
        {
            floLoginSteam += Time.deltaTime;
            if (floLoginSteam > 5)
            {
                floLoginSteam = 0;
                {
                    booLoginSteam = true;
                }
            }
        }
    }

    UnityEngine.Events.UnityAction OnClickLanguage(int intIndex)
    {
        return () =>
        {
            AudioBtnPlay();
            //ManagerValue.EnumLanguageType[] languages = (ManagerValue.EnumLanguageType[])System.Enum.GetValues(typeof(ManagerValue.EnumLanguageType));
            ManagerValue.enumLanguage = listEnumLanguage[intIndex];
            ManagerValue.setting.enumLanguage = ManagerValue.enumLanguage;
            ManagerValue.SettingSave();
            SelectItem(intIndex, listLanguage);
            SetLanguage();
        };
    }

    UnityEngine.Events.UnityAction OnClickScreen(int intIndex)
    {
        return () =>
        {
            AudioBtnPlay();
            ManagerValue.ScreenType[] screens = (ManagerValue.ScreenType[])System.Enum.GetValues(typeof(ManagerValue.ScreenType));
            ManagerValue.setting.screenType = screens[intIndex];
            ManagerValue.SettingSave();
            SelectItem(intIndex, listScreen);
        };
    }

    void AudioBtnPlay()
    {
        if (ManagerValue.setting.booAudio)
        {
            audioBtn.Play();
        }
    }

    void SelectItem(int intIndex, List<GameObject> listGo)
    {
        for (int i = 0; i < listGo.Count; i++)
        {
            if (i == intIndex)
            {
                listGo[i].transform.GetChild(1).gameObject.SetActive(true);
                continue;
            }
            listGo[i].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    void SetLanguage()
    {
        textSound.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Sound);
        textScreen.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.WindowResolution);
        btnStart.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Enter);
        btnSetting.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Settings);
        btnBack.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Back);
        transSound.GetChild(0).GetChild(2).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Audio);
        transSound.GetChild(1).GetChild(2).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.BackgroundMusic);
        listScreen[listScreen.Count - 1].transform.GetChild(2).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.FullScreen);
    }
}
