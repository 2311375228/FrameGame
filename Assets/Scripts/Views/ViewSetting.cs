using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSetting : ViewBase
{
    public Text textTitle;
    public Text textMouseSpeedTag;
    public Text textSpeed;

    public Button btnClose;
    public Button btnArrowSpeedLeft;
    public Button btnArrowSpeedRight;

    public Button btnMenu;

    public RectTransform rectAudio;
    public RectTransform rectLanguage;

    public Slider sliderAudio;
    public Slider sliderBackgroundMusic;

    List<RectTransform> listAudio = new List<RectTransform>();
    List<RectTransform> listLanguage = new List<RectTransform>();
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

    // Start is called before the first frame update
    protected override void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Close);
            ManagerView.Instance.Hide(EnumView.ViewSetting);
        });

        btnArrowSpeedLeft.onClick.AddListener(() =>
        {
            if (ManagerValue.setting.intRankSpeed > 1)
            {
                ManagerValue.setting.intRankSpeed--;
                textSpeed.text = ManagerValue.setting.intRankSpeed.ToString();
            }
            ManagerValue.floTouchSpeed = ManagerValue.floMouseSpeeds[ManagerValue.setting.intRankSpeed - 1];
            ManagerValue.SettingSave();
        });
        btnArrowSpeedRight.onClick.AddListener(() =>
        {
            if (ManagerValue.setting.intRankSpeed < ManagerValue.floMouseSpeeds.Length)
            {
                ManagerValue.setting.intRankSpeed++;
                textSpeed.text = ManagerValue.setting.intRankSpeed.ToString();
            }
            ManagerValue.floTouchSpeed = ManagerValue.floMouseSpeeds[ManagerValue.setting.intRankSpeed - 1];
            ManagerValue.SettingSave();
        });

        btnMenu.onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerView.Instance.Show(EnumView.ViewESC);
        });

        for (int i = 0; i < rectAudio.childCount; i++)
        {
            listAudio.Add(rectAudio.GetChild(i).GetComponent<RectTransform>());
        }
        listAudio[0].GetComponent<Button>().onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerValue.setting.booAudio = !ManagerValue.setting.booAudio;
            listAudio[0].GetChild(2).gameObject.SetActive(ManagerValue.setting.booAudio);
            ManagerValue.SettingSave();
        });
        listAudio[1].GetComponent<Button>().onClick.AddListener(() =>
        {
            ManagerValue.actionAudio(EnumAudio.Ground);
            ManagerValue.setting.booBackgroudMusic = !ManagerValue.setting.booBackgroudMusic;
            listAudio[1].GetChild(2).gameObject.SetActive(ManagerValue.setting.booBackgroudMusic);
            ManagerValue.SettingSave();
        });

        sliderAudio.onValueChanged.AddListener((value) =>
        {
            ManagerValue.SetFloAudio(value);
        });
        sliderBackgroundMusic.onValueChanged.AddListener((value) =>
        {
            ManagerValue.SetFloBackgroundMusic(value);
        });

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
        GameObject goCopyLanguage = rectLanguage.GetChild(0).gameObject;
        for (int i = 0; i < listEnumLanguage.Count; i++)
        {
            listLanguage.Add(Instantiate(goCopyLanguage, goCopyLanguage.transform.parent, false).GetComponent<RectTransform>());
            listLanguage[i].GetChild(0).GetComponent<Text>().text = dicLanguage[listEnumLanguage[i]];
            listLanguage[i].GetComponent<Button>().onClick.AddListener(OnClickLanguage(i));
        }
        goCopyLanguage.SetActive(false);


        //PlayerPrefs.DeleteAll();

        textSpeed.text = ManagerValue.setting.intRankSpeed.ToString();
        listAudio[0].GetChild(2).gameObject.SetActive(ManagerValue.setting.booAudio);
        listAudio[1].GetChild(2).gameObject.SetActive(ManagerValue.setting.booBackgroudMusic);
        ItemSelect((int)ManagerValue.setting.enumLanguage, listLanguage);
        sliderAudio.value = ManagerValue.setting.floAudio;
        sliderBackgroundMusic.value = ManagerValue.setting.floBackgroundMusic;
    }

    public override void Show()
    {
        base.Show();
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return 0;
        textTitle.text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Settings);
        listAudio[0].GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Audio);
        listAudio[1].GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.BackgroundMusic);
        textMouseSpeedTag.text = ManagerLanguage.Instance.GetStatement(EnumLanguageStatement.AdjustSMS, null);
        btnMenu.transform.GetChild(0).GetComponent<Text>().text = ManagerLanguage.Instance.GetWord(EnumLanguageWords.Menu);
    }

    UnityEngine.Events.UnityAction OnClickLanguage(int intIndex)
    {
        return () =>
        {
            ItemSelect(intIndex, listLanguage);
            ManagerValue.enumLanguage = listEnumLanguage[intIndex];
            ManagerValue.setting.enumLanguage = ManagerValue.enumLanguage;
            ManagerValue.SettingSave();
            ManagerView.Instance.Hide(EnumView.ViewBuildMain);
            ManagerView.Instance.Hide(EnumView.ViewHouse);
            ManagerView.Instance.ChangeLanguage();
        };
    }

    void ItemSelect(int intIndex, List<RectTransform> listRect)
    {
        for (int i = 0; i < listRect.Count; i++)
        {
            if (i == intIndex)
            {
                listRect[i].GetChild(2).gameObject.SetActive(true);
                continue;
            }
            listRect[i].GetChild(2).gameObject.SetActive(false);
        }
    }
}
