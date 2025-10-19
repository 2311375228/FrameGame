using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSound : MonoBehaviour
{
    public AudioClip audioGround;//点击土地
    public AudioClip audioBuild;//点击可以建造的土地
    public AudioClip audioUnable;//无法购买的
    public AudioClip audioClose;//关闭的声音
    public AudioClip audioOpen;//打开的声音
    public AudioClip audioCoinBuy;//花钱的声音
    public AudioClip audioMakeMoney;//赚钱的声音
    public AudioClip audioMoneyNone;//没钱的声音

    public AudioClip audioMemind;//提醒

    public AudioClip audioChicken;//鸡叫
    public AudioClip audioPig;//猪叫
    public AudioClip audioCow;//牛叫
    public AudioClip audioSheep;//羊叫

    public AudioClip[] audioBGMusic;
    public AudioClip audioCombatBG;
    public AudioClip audioMerchant;

    float floTimeAudio;
    float floAudio;
    float floBackgroundMusic;
    AudioSource audio1;
    AudioSource audioBG;
    Dictionary<EnumAudio, AudioClip> dicAudio = new Dictionary<EnumAudio, AudioClip>();
    Dictionary<EnumAudioCombat, AudioClip> dicAudioCombat = new Dictionary<EnumAudioCombat, AudioClip>();

    bool booMusic;
    float floTime;
    ControllerCamera.EnumCameraPosition enumPosition;
    int intIndexAudioCombat = 0;
    AudioSource[] audioCombat = new AudioSource[6];
    // Start is called before the first frame update
    void Start()
    {
        audio1 = gameObject.AddComponent<AudioSource>();
        audioBG = gameObject.AddComponent<AudioSource>();
        dicAudio.Add(EnumAudio.Ground, audioGround);
        dicAudio.Add(EnumAudio.Build, audioBuild);
        dicAudio.Add(EnumAudio.Unable, audioUnable);
        dicAudio.Add(EnumAudio.Close, audioClose);
        dicAudio.Add(EnumAudio.Open, audioOpen);
        dicAudio.Add(EnumAudio.CoinBuy, audioCoinBuy);
        dicAudio.Add(EnumAudio.MakeMoney, audioMakeMoney);
        dicAudio.Add(EnumAudio.MoneyNone, audioMoneyNone);

        dicAudio.Add(EnumAudio.Remind, audioMemind);

        dicAudio.Add(EnumAudio.Chicken, audioChicken);
        dicAudio.Add(EnumAudio.Pig, audioPig);
        dicAudio.Add(EnumAudio.Cow, audioCow);
        dicAudio.Add(EnumAudio.Sheep, audioSheep);

        for (int i = 0; i < audioCombat.Length; i++)
        {
            audioCombat[i] = gameObject.AddComponent<AudioSource>();
        }
        EnumAudioCombat[] enumAudioCombats = (EnumAudioCombat[])System.Enum.GetValues(typeof(EnumAudioCombat));
        for (int i = 1; i < enumAudioCombats.Length; i++)
        {
            dicAudioCombat.Add(enumAudioCombats[i], Resources.Load<AudioClip>("sound/Combat/" + enumAudioCombats[i]));
        }

        if (ManagerValue.setting == null)
        {
            ManagerValue.SettingRead();
        }

        floAudio = ManagerValue.setting.floAudio;
        floBackgroundMusic = ManagerValue.setting.floBackgroundMusic;
        audioBG.clip = audioBGMusic[0];

        if (ManagerValue.setting == null)
        {
            ManagerValue.SettingRead();
        }
        audio1.volume = ManagerValue.setting.floAudio;
        audioBG.volume = ManagerValue.setting.floBackgroundMusic;

        ManagerValue.actionAudio += PlayAudio;
        ManagerValue.actionAudioCombat += PlayAudioCombat;

        ManagerValue.SetFloAudio += SetAudio;
        ManagerValue.SetFloBackgroundMusic += SetBackgroundMusic;

        ManagerValue.actionCameraLocation += CameraLocation;
    }

    private void Update()
    {
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

        if (ManagerValue.setting.booBackgroudMusic)
        {
            if (!booMusic)
            {
                floTime += Time.deltaTime;
                if (floTime > 5)
                {
                    floTime = 0;
                    booMusic = true;
                    audioBG.Play();
                }
            }
            if (!audioBG.isPlaying && booMusic == true)
            {
                booMusic = false;
                switch (enumPosition)
                {
                    case ControllerCamera.EnumCameraPosition.Ground:
                        audioBG.clip = audioBGMusic[Random.Range(0, audioBGMusic.Length)];
                        break;
                    case ControllerCamera.EnumCameraPosition.Market:
                        audioBG.clip = audioMerchant;
                        break;
                    case ControllerCamera.EnumCameraPosition.GameDungeon:
                        audioBG.clip = audioCombatBG;
                        break;
                    default:
                        booMusic = true;
                        break;
                }
            }
        }
        else
        {
            if (audioBG.isPlaying)
            {
                audioBG.Stop();
            }
        }
    }

    void PlayAudio(EnumAudio key)
    {
        if (ManagerValue.setting.booAudio)
        {
            audio1.clip = dicAudio[key];
            audio1.Play();
        }
    }

    void PlayAudioCombat(EnumAudioCombat key)
    {
        if (ManagerValue.setting.booAudio)
        {
            audioCombat[intIndexAudioCombat].clip = dicAudioCombat[key];
            audioCombat[intIndexAudioCombat].Play();
            intIndexAudioCombat++;
            if (intIndexAudioCombat == audioCombat.Length)
            {
                intIndexAudioCombat = 0;
            }
        }
    }

    void SetAudio(float floAudio)
    {
        audio1.volume = floAudio;
        this.floAudio = floAudio;
    }

    void SetBackgroundMusic(float floMusic)
    {
        audioBG.volume = floMusic;
        floBackgroundMusic = floMusic;
    }

    void CameraLocation(ControllerCamera.EnumCameraPosition enumPosition)
    {
        this.enumPosition = enumPosition;
    }

    private void OnDestroy()
    {
        ManagerValue.actionAudio -= PlayAudio;
        ManagerValue.actionAudioCombat -= PlayAudioCombat;

        ManagerValue.SetFloAudio -= SetAudio;
        ManagerValue.SetFloBackgroundMusic -= SetBackgroundMusic;

        ManagerValue.actionCameraLocation -= CameraLocation;
    }
}
