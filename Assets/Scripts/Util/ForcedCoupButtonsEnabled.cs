using UnityEngine;
using UnityEngine.UI;
using Gameplay;

public class ForcedCoupButtonsEnabled : MonoBehaviour
{
    void LateUpdate()
    {
        if (Manager.Instance.Players[Manager.Instance.CurrentTurn].Coins >= 10) GetComponent<Button>().interactable = false;
        else GetComponent<Button>().interactable = true;
    }
}
