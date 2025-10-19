using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ControllerView : MonoBehaviour
{
    public RectTransform rectCursor;
    // Start is called before the first frame update
    void Start()
    {
        //valuma.Setting setting = JsonUtility.FromJson<Setting>(PlayerPrefs.GetString(ViewSetting.strSaveName));

        //if (setting != null)
        //{
        //    ManagerValue.booAudio = setting.booAudio;
        //    ManagerValue.enumLanguage = setting.enumLanguage;
        //    ManagerValue.floTouchSpeed = setting.floMouseSpeeds[setting.intRankSpeed];
        //}
        //else
        //{
        //    ManagerValue.floTouchSpeed = 0.005f;
        //}

        if (ManagerValue.setting == null)
        {
            ManagerValue.SettingRead();
        }

        ManagerView.Instance.Show(EnumView.ViewLogin);

        Screen.orientation = ScreenOrientation.AutoRotation;//设置为自由方向
        Screen.autorotateToLandscapeLeft = true;//允许自动转到左横屏
        Screen.autorotateToLandscapeRight = true;//允许自动转到右横屏
        Screen.autorotateToPortrait = false;//不允许转到纵向
        Screen.autorotateToPortraitUpsideDown = false;//不允许自动转到纵上下
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//随眠时间为从不随眠

        if ((int)ManagerValue.setting.screenType < ManagerValue.vecScreens.Length)
        {
            Vector2 vecScreen = ManagerValue.vecScreens[(int)ManagerValue.setting.screenType];
            Screen.SetResolution((int)vecScreen.x, (int)vecScreen.y, false);
        }
        else
        {
            Screen.SetResolution(2560, 1440, true);
        }

        //创建保存路径
        //for (int i = 0; i < 5; i++)
        //{
        //    if (!File.Exists(ManagerValue.GetSavePath + i))
        //    {
        //        Directory.CreateDirectory(ManagerValue.GetSavePath);
        //        File.Create(ManagerValue.GetSavePath + i);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        rectCursor.position = Input.mousePosition;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ViewBase viewBase = ManagerView.Instance.GetView(EnumView.ViewLogin);
            if (viewBase != null && !viewBase.gameObject.activeSelf)
            {
                viewBase = ManagerView.Instance.GetView(EnumView.ViewESC);
                if (viewBase == null)
                {
                    ManagerView.Instance.Show(EnumView.ViewESC);
                }
                else
                {
                    if (viewBase.gameObject.activeSelf)
                    {
                        viewBase.Hide();
                    }
                    else
                    {
                        viewBase.Show();
                    }
                }
            }
        }

    }

    void UnlockBuild()
    {
    }

    void ResetUnlockBuild()
    {
    }
}
