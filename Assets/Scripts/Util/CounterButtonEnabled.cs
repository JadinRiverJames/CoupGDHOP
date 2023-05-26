using UnityEngine;
using UnityEngine.UI;
using Players;
using Gameplay;

public class CounterButtonEnabled : MonoBehaviour
{
    public Actions actionToCheck;

    // Update is called once per frame
    void Update()
    {
        GetComponent<Button>().interactable = Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction == actionToCheck;
    }
}
