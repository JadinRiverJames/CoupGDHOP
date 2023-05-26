using UnityEngine;
using UnityEngine.UI;
using Gameplay;

public class AssissinateButtonEnabled : MonoBehaviour
{
    void LateUpdate()
    {
        if (Manager.Instance.Players[Manager.Instance.CurrentTurn].Coins >= 3 && Manager.Instance.Players[Manager.Instance.CurrentTurn].Coins < 10) GetComponent<Button>().interactable = true;
        else GetComponent<Button>().interactable = false;
    }
}
