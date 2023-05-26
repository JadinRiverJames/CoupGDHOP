using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Util;

namespace Log
{

    public class UILog : Singleton<UILog>
    {
        public Transform content;
        public ScrollRect scrollRect;
        public GameObject textPrefab;
        public GameObject scrollBar;
        public int keepHistory = 100; // only keep 'n' messages


        public void AddMessage(LogMessage message)
        {
            if (content.childCount >= keepHistory)
            {
                for (int i = 0; i < content.childCount / 2; ++i)
                    Destroy(content.GetChild(i).gameObject);
            }

            // instantiate and initialize text prefab
            GameObject textSlot = Instantiate(message.textPrefab);
            textSlot.gameObject.transform.SetParent(content.transform, false);
            textSlot.GetComponent<TMP_Text>().text = message.message;
            textSlot.GetComponent<TMP_Text>().fontStyle = message.bold ? FontStyles.Bold : FontStyles.Normal;
            textSlot.GetComponent<UILogEntry>().message = message;
            AutoScroll();
        }

        void AutoScroll()
        {
            // update first so we don't ignore recently added messages, then scroll
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;
        }
    }

    [Serializable]
    public struct LogMessage
    {
        public string message;
        public GameObject textPrefab;
        public bool bold;

        public LogMessage(string message)
        {
            this.message = message;
            textPrefab = UILog.Instance.textPrefab;
            bold = false;
        }

        public LogMessage(string message, bool bolded)
        {
            this.message = message;
            textPrefab = UILog.Instance.textPrefab;
            bold = bolded;
        }
    }
}
