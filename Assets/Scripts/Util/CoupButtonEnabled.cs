using UnityEngine;
using UnityEngine.UI;
using Gameplay;

public class CoupButtonEnabled : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<Button>().interactable = Manager.Instance.Players[0].Coins >= 7;
    }
}
