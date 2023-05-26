using UnityEngine;
using UnityEngine.UI;
using Gameplay;

public class CoupSelectButtonEnabled : MonoBehaviour
{
    public int PlayerIDToCheck;

    // Update is called once per frame
    void Update()
    {
        GetComponent<Button>().interactable = !Manager.Instance.Players[PlayerIDToCheck].IsOutOfGame;
    }
}