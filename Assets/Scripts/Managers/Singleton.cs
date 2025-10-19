using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
//{
//    protected static T instance;
//    public static T Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = (T)FindObjectOfType(typeof(T));

//                if (instance == null)
//                {
//                    GameObject go = GameObject.Find("GameManager");
//                    if (go == null)
//                    {
//                        go = new GameObject("GameManager");
//                        DontDestroyOnLoad(go);
//                    }

//                    instance = go.AddComponent<T>();
//                }

//                if (instance == null)
//                {
//                    Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
//                }
//            }
//            return instance;
//        }
//    }
//    public static void SetNull()
//    {
//        instance = null;
//    }
//}

//public class TSingleton<T> where T : new()
//{
//    static private T mInstance;
//    private static object _lock = new object();

//    public static T Instance
//    {
//        get
//        {
//            lock (_lock)
//            {
//                if (mInstance == null)
//                {
//                    mInstance = new T();
//                }
//            }

//            return mInstance;
//        }

//    }
//}

public class TSingleton<T> where T : new()
{
    static private T mInstance;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new T();
            }
            return mInstance;
        }

    }
}