using UnityEngine;
using Cards;
using Players;
using Gameplay;
using UnityEngine.UI;
using TMPro;

public class UIView : MonoBehaviour
{
    // Messy, if not useful, UI code
    public GameObject ActionsList;
    public GameObject CountersList;
    public GameObject ChallengeList;
    public GameObject TargetList;
    public GameObject SwappingUI;

    public TMP_Text Treasury;
    public Sprite XSprite;

    public Toggle ViewCards;
    public Slider ChallengeWeight;
    public Slider CounterChallengeWeight;
    public Slider CounterWeight;
    public TMP_Text ChallengeWeightText;
    public TMP_Text CounterChallengeWeightText;
    public TMP_Text CounterWeightText;

    public void LateUpdate()
    {
        HandlePlayerCards();
        HandleActionsList();
        HandleCountersList();
        HandleChallengeList();
        HandleTargetList();
        HandleSwappingUI();
        HandleTreasury();

        HandleOptions();
    }

    void HandleOptions()
    {
        Manager.Instance.ViewCards = ViewCards.isOn;
        Manager.Instance.AIChallengeWeight = ChallengeWeight.value;
        Manager.Instance.AICounterChallengeWeight = CounterChallengeWeight.value;
        Manager.Instance.AICounterWeight = CounterWeight.value;
        ChallengeWeightText.text = "" + Mathf.RoundToInt(ChallengeWeight.value * 10) / 10f;
        CounterChallengeWeightText.text = "" + Mathf.RoundToInt(CounterChallengeWeight.value * 10) / 10f;
        CounterWeightText.text = "" + Mathf.RoundToInt(CounterWeight.value * 10) / 10f;
    }

    void HandleTreasury()
    {
        Treasury.text = Manager.Instance.Treasury.ToString();
    }

    void HandleSwappingUI()
    {
        if (Manager.Instance.GameStarted && Manager.Instance.CurrentTurn == 0 && Manager.Instance.SwappableCards.Length > 0)
        {
            if (!SwappingUI.activeInHierarchy) SwappingUI.SetActive(true);
            SwappingUI.transform.GetChild(0).GetComponent<Image>().color = GetCardColor(Manager.Instance.SwappableCards[0].type);
            SwappingUI.transform.GetChild(1).GetComponent<Image>().color = GetCardColor(Manager.Instance.SwappableCards[1].type);
        }
        else if (SwappingUI.activeInHierarchy)
        {
            SwappingUI.SetActive(false);
        }
    }

    void HandleActionsList()
    {
        if (Manager.Instance.GameStarted && Manager.Instance.CurrentTurn == 0 && Manager.Instance.Players[0].CurrentAction == Actions.None)
        {
            if (!ActionsList.activeInHierarchy) ActionsList.SetActive(true);
        }
        else if (ActionsList.activeInHierarchy)
        {
            ActionsList.SetActive(false);
        }
    }

    void HandleTargetList()
    {
        if (Manager.Instance.GameStarted && Manager.Instance.CurrentTurn == 0 && Manager.Instance.Targetting)
        {
            if (!TargetList.activeInHierarchy) TargetList.SetActive(true);
        }
        else if (TargetList.activeInHierarchy)
        {
            TargetList.SetActive(false);
        }
    }

    void HandleCountersList()
    {
        if (Manager.Instance.GameStarted && Manager.Instance.CurrentCounter == 0 && Manager.Instance.Players[0].CurrentCounteraction == CounterActions.None)
        {
            if (!CountersList.activeInHierarchy) CountersList.SetActive(true);
        }
        else if (CountersList.activeInHierarchy)
        {
            CountersList.SetActive(false);
        }
    }

    void HandleChallengeList()
    {
        if (Manager.Instance.GameStarted && Manager.Instance.CurrentChallenge == 0)
        {
            if (!ChallengeList.activeInHierarchy) ChallengeList.SetActive(true);
        }
        else if (ChallengeList.activeInHierarchy)
        {
            ChallengeList.SetActive(false);
        }
    }

    void HandlePlayerCards()
    {
        if (Manager.Instance.GameStarted)
        {
            for (int i = 0; i < Manager.Instance.AmountOfPlayers; i++)
            {
                // Card 1
                if (!Manager.Instance.Players[i].Hand[0].revealed)
                {
                    Manager.Instance.PlayerObjects[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
                    Manager.Instance.PlayerObjects[i].transform.GetChild(0).gameObject.GetComponent<Image>().color = (Manager.Instance.ViewCards || i == 0) ? GetCardColor(Manager.Instance.Players[i].Hand[0].type) : Color.white;
                }
                else
                {
                    Manager.Instance.PlayerObjects[i].transform.GetChild(0).gameObject.GetComponent<Image>().color = GetCardColor(Manager.Instance.Players[i].Hand[0].type);
                    Manager.Instance.PlayerObjects[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = XSprite;
                }
                // Card 2
                if (!Manager.Instance.Players[i].Hand[1].revealed)
                {
                    Manager.Instance.PlayerObjects[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = null;
                    Manager.Instance.PlayerObjects[i].transform.GetChild(1).gameObject.GetComponent<Image>().color = (Manager.Instance.ViewCards || i == 0) ? GetCardColor(Manager.Instance.Players[i].Hand[1].type) : Color.white;
                }
                else
                {
                    Manager.Instance.PlayerObjects[i].transform.GetChild(1).gameObject.GetComponent<Image>().color = GetCardColor(Manager.Instance.Players[i].Hand[1].type);
                    Manager.Instance.PlayerObjects[i].transform.GetChild(1).gameObject.GetComponent<Image>().sprite = XSprite;
                }

                // arrow indicator
                if (Manager.Instance.CurrentCounter == -1 && Manager.Instance.CurrentChallenge == -1) Manager.Instance.PlayerObjects[i].transform.GetChild(2).gameObject.SetActive(Manager.Instance.CurrentTurn == i);
                else if (Manager.Instance.CurrentCounter >= 0) Manager.Instance.PlayerObjects[i].transform.GetChild(2).gameObject.SetActive(Manager.Instance.CurrentCounter == i);

                // challenge indicator
                if (Manager.Instance.CurrentChallenge == i)
                {
                    Manager.Instance.PlayerObjects[i].transform.GetChild(2).gameObject.SetActive(true);
                    Manager.Instance.PlayerObjects[i].transform.GetChild(2).gameObject.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    Manager.Instance.PlayerObjects[i].transform.GetChild(2).gameObject.GetComponent<Image>().color = Color.white;
                }

                // coins
                Manager.Instance.PlayerObjects[i].transform.GetChild(3).GetChild(0).GetComponent<TMP_Text>().text = Manager.Instance.Players[i].Coins.ToString();
            }
        }
        else
        {
            for (int i = 0; i < Manager.Instance.AmountOfPlayers; i++)
            {
                Manager.Instance.PlayerObjects[i].transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                Manager.Instance.PlayerObjects[i].transform.GetChild(1).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
    }

    Color GetCardColor(CardType type)
    {
        switch (type)
        {
            case CardType.Duke:
                return new Color(0.5f, 0, 1);

            case CardType.Assassin:
                return Color.black;

            case CardType.Captain:
                return Color.blue;

            case CardType.Ambassador:
                return Color.green;

            case CardType.Contessa:
                return Color.red;
        }
        return Color.white;
    }


}
