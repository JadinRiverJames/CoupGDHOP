using UnityEngine;
using UnityEngine.UI;
using Util;
using Gameplay;

public class ChallengeButtonEnabled : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Utils.CanChallengeAction(Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction)) GetComponent<Button>().interactable = true;
        else GetComponent<Button>().interactable = false;
    }
}