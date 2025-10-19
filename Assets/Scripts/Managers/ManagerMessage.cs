using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMessage
{
    Dictionary<EnumMessage, MessageDelegate> dicEvent = new Dictionary<EnumMessage, MessageDelegate>();
    public delegate void MessageDelegate(MessageBase messageBase);
    static ManagerMessage _instance;
    public static ManagerMessage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ManagerMessage();
            }
            return _instance;
        }
    }
    ManagerMessage() { }

    public void AddEvent(EnumMessage enumKey, MessageDelegate even)
    {
        if (dicEvent.ContainsKey(enumKey))
        {
            dicEvent[enumKey] += even;
        }
        else
        {
            dicEvent.Add(enumKey, even);
        }
    }
    public void PostEvent(EnumMessage enumKey, MessageBase message = null)
    {
        if (dicEvent.ContainsKey(enumKey) && dicEvent[enumKey] != null)
        {
            dicEvent[enumKey](message);
        }
    }
    public void RemoveEvent(EnumMessage enumKey, MessageDelegate even)
    {
        if (dicEvent.ContainsKey(enumKey))
        {
            dicEvent[enumKey] -= even;
        }
    }

    public class MessageBase
    {
        public int Number;
        public string StrName;
    }
}
