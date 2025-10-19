using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTaskEnemy : MonoBehaviour
{
    [System.NonSerialized]
    public int intDungeonID;
    [System.NonSerialized]
    public int intDungeonPointIndex;
    public Action<int, int> actionMouseUp;

    Vector3 vecMousePosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseDown()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
Application.platform == RuntimePlatform.WindowsPlayer ||
Application.platform == RuntimePlatform.OSXEditor ||
Application.platform == RuntimePlatform.OSXPlayer)
        {
            vecMousePosition = Input.mousePosition;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount == 1)
            {
                vecMousePosition = Input.GetTouch(0).position;
            }
        }
    }
    private void OnMouseUp()
    {
        bool booIsPointUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        if (booIsPointUI)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.WindowsEditor ||
    Application.platform == RuntimePlatform.WindowsPlayer ||
    Application.platform == RuntimePlatform.OSXEditor ||
    Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (actionMouseUp != null && Vector3.Distance(Input.mousePosition, vecMousePosition) < 10f)
            {
                actionMouseUp(intDungeonID, intDungeonPointIndex);
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount == 1)
            {
                if (actionMouseUp != null && Vector3.Distance(Input.GetTouch(0).position, vecMousePosition) < 10f)
                {
                    actionMouseUp(intDungeonID, intDungeonPointIndex);
                }
            }
        }
        vecMousePosition = Vector3.zero;
    }
}
