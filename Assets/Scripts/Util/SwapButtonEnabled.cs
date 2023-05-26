using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;
using Util;
using Players;
using Log;
using Gameplay;

public class SwapButtonEnabled : MonoBehaviour
{
    void Update()
    {
        GetComponent<Button>().interactable = !Manager.Instance.Players[Manager.Instance.CurrentTurn].Hand[0].revealed;
    }
}
