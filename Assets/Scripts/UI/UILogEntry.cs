using UnityEngine;
using TMPro;

namespace Log
{
    public class UILogEntry : MonoBehaviour
    {
        public TMP_Text text;

        [HideInInspector] public LogMessage message;

    }
}