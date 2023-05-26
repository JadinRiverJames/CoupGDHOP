using System.Collections.Generic;
using UnityEngine;
using Cards;
using Util;
using Players;
using Log;

namespace Gameplay
{
    public class Manager : Singleton<Manager>
    {
        [Header("Pertinent Globals")]
        public int AmountOfPlayers = 4;

        public int CurrentTurn = 0;
        public int CurrentCounter = -1;
        public int CurrentChallenge = -1;

        int WinningPlayer = 0;

        public Player[] Players;
        public GameObject[] PlayerObjects;

        public List<Card> Court = new List<Card>();
        public int Treasury = 50;

        public bool GameStarted = false;
        bool GameOver = false;

        [Header("Specialized Info")]
        [HideInInspector] public int Target = -1;
        [HideInInspector] public bool Targetting = false;
        [HideInInspector] public Card[] SwappableCards = new Card[0];
        [HideInInspector] public bool FinishedSwap = false;

        float timer = 0;

        [Header("Options")]
        public bool ViewCards = false;
        public float AIChallengeWeight = 0.25f;
        public float AICounterChallengeWeight = 0.25f;
        public float AICounterWeight = 0.5f;

        public void Start()
        {
            Application.targetFrameRate = 60;
        }

        public void Update()
        {
            if (GameStarted)
            {
                HandleTurns();
                HandleCounters();
                HandleChallenges();
            }
        }

        #region TurnLogic

        // Handles the turn order
        public void HandleTurns()
        {
            if (GameOver) return;

            // Check if player is out of the game. if they are, skip them
            if (Players[CurrentTurn].IsOutOfGame)
            {
                CurrentTurn = Utils.NextInOrder(CurrentTurn);
                return;
            }

            if (Players[CurrentTurn].CurrentAction != Actions.None) return;

            // if human player, let them choose an action, otherwise allow the ai to process
            if (CurrentTurn == 0)
            {

            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    timer = 0;
                    ProcessTurns();
                }
            }

        }

        // Processes the turn after input has been given and counteractions and challenges have been dealt with
        public void CompleteTurn(bool wasCountered)
        {

            if (!wasCountered)
            {
                switch (Players[CurrentTurn].CurrentAction)
                {
                    case Actions.Income:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " takes a Coin from the Treasury!", true));
                        Treasury -= 1;
                        Players[CurrentTurn].Coins++;
                        break;

                    case Actions.ForeignAid:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " takes 2 Coins from the Treasury!", true));
                        Treasury -= 2;
                        Players[CurrentTurn].Coins += 2;
                        break;

                    case Actions.Coup:
                        Treasury += 7;
                        Players[CurrentTurn].Coins -= 7;
                        break;

                    case Actions.Tax:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " takes 3 Coins from the Treasury!", true));
                        Treasury -= 3;
                        Players[CurrentTurn].Coins += 3;
                        break;

                    case Actions.Assassinate:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " assassinates player " + (Target + 1).ToString() + "!", true));
                        Treasury += 3;
                        Players[CurrentTurn].Coins -= 3;
                        Utils.RemoveInfluence(Target);
                        Target = -1;
                        break;

                    case Actions.Steal:
                        Utils.TakeCoins(CurrentTurn, Target);
                        Target = -1;
                        break;

                    case Actions.Exchange:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " exchanges cards with the Court!", true));
                        Court.Add(SwappableCards[0]);
                        Court.Add(SwappableCards[1]);
                        Court.Shuffle();
                        SwappableCards = new Card[0];
                        FinishedSwap = false;
                        break;
                }

            }
            else
            {
                switch (Players[CurrentTurn].CurrentAction)
                {                   
                    case Actions.ForeignAid:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " was countered! They gain no coins this turn.", true));
                        break;

                    case Actions.Tax:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " was countered! They gain no coins this turn.", true));
                        break;

                    case Actions.Assassinate:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " was countered! They lose 3 coins.", true));
                        Treasury += 3;
                        Players[CurrentTurn].Coins -= 3;
                        Target = -1;
                        break;

                    case Actions.Steal:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " was countered! They gain no coins this turn.", true));
                        Target = -1;
                        break;

                    case Actions.Exchange:
                        UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " was countered! No cards exchanged this turn.", true));
                        break;
                }
            }

            EndTurn();
        }

        // Ends the current turn, resets all pertinent values, and begins next turn
        public void EndTurn()
        {
            int amountLost = 0;
            // Check for player losses
            // Reset all player actions and counteractions
            for (int i = 0; i < AmountOfPlayers; i++)
            {
                if (Players[i].Hand[0].revealed && Players[i].Hand[1].revealed)
                {
                    Players[i].IsOutOfGame = true;
                }
                Players[i].CurrentAction = Actions.None;
                Players[i].CurrentCounteraction = CounterActions.None;

                if (Players[i].IsOutOfGame) amountLost++;
            }

            // Check for victory
            if (amountLost == AmountOfPlayers - 1)
            {
                for (int i = 0; i < AmountOfPlayers; i++)
                {
                    if (!Players[i].IsOutOfGame) WinningPlayer = i;
                }
                UILog.Instance.AddMessage(new LogMessage("Player " + (WinningPlayer + 1).ToString() + " has won the game! Click 'Start Game' to restart!"));
                GameOver = true;
                return;
            }
            else
            {
                if (!GameOver)
                {
                    CurrentTurn = Utils.NextInOrder(CurrentTurn);
                
                    while (Players[CurrentTurn].IsOutOfGame)
                    {
                        CurrentTurn = Utils.NextInOrder(CurrentTurn);
                    }

                    UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + "  turn start.", true));
                }
            }
        }

        // Handles the Counteraction "turn order"
        public void HandleCounters()
        {
            // Cancel out if we're not in countering mode
            if (CurrentCounter == -1 || CurrentChallenge > -1) return;

            // Counters went all the way back around. This means noone countered or challenged. Do the action logic now
            if (CurrentCounter == CurrentTurn)
            {
                if (Players[CurrentTurn].CurrentAction == Actions.Exchange)
                {
                    HandleExchange();
                }
                else
                {
                    CurrentCounter = -1;
                    CompleteTurn(false);
                }
                return;
            }

            // Check if the player is out of the game. If they are, skip them
            if (Players[CurrentCounter].IsOutOfGame)
            {
                CurrentCounter = Utils.NextInOrder(CurrentCounter);
                return;
            }

            // if human player, let them choose an action, otherwise allow the ai to process
            if (CurrentCounter == 0)
            {

            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    timer = 0;
                    ProcessCounters();
                }
            }
        }
       
        // Handles the Challenge "turn order". This should only be used if someone wants to challenge a counter
        public void HandleChallenges()
        {
            // Cancel out if we're not in challenging mode
            if (CurrentChallenge == -1) return;

            // Challenges went all the way back around. No one challenged our counter, run the turn
            if (CurrentChallenge == CurrentCounter)
            {
                CurrentChallenge = -1;
                CurrentCounter = -1;
                CompleteTurn(true);                
                return;
            }

            // Check if the player is out of the game. If they are, skip them
            if (Players[CurrentChallenge].IsOutOfGame)
            {
                CurrentChallenge = Utils.NextInOrder(CurrentChallenge);
                return;
            }

            // if human player, let them choose an action, otherwise allow the ai to process
            if (CurrentChallenge == 0)
            {

            }
            else
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                {
                    timer = 0;
                    ProcessCounterChallenges();
                }
            }
        }

        // bit of a specialized function to allow the AI to process Exchanging cards
        void HandleExchange()
        {
            if (SwappableCards.Length == 0)
            {
                Utils.PopulateSwappableCards();
            }

            // if we're an AI player, process differently
            if (CurrentTurn != 0)
            {
                List<Card> cards = new List<Card>(Players[CurrentTurn].Hand);
                cards.Add(SwappableCards[0]);
                cards.Add(SwappableCards[1]);
                cards.Shuffle();
                Players[CurrentTurn].Hand[0] = cards[0];
                Players[CurrentTurn].Hand[1] = cards[1];
                SwappableCards[0] = cards[2];
                SwappableCards[1] = cards[3];
                FinishedSwap = true;
            }

            if (FinishedSwap)
            {
                CurrentCounter = -1;
                CompleteTurn(false);
            }
        }

        #endregion

        #region ChallengeLogic

        public void RunCounteractionChallenge()
        {
            // Does the defender have the appropriate card to do the counter they want to?
            int cardInHand = Utils.HasCounterCard(CurrentCounter, Players[CurrentTurn].CurrentAction);
            
            // He does! Challenger loses, counteraction will go through
            if (cardInHand != -1)
            {
                UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentCounter + 1).ToString() + " has the card required to counter " + (CurrentTurn + 1).ToString() + "'s Action! Player " + (CurrentChallenge + 1).ToString() + "'s challenge is unsuccessful. They lose an influence point!", true));
                
                // Reveal their first card if not revealed, otherwise, reveal the second
                Utils.RemoveInfluence(CurrentChallenge);

                // Shuffle challenged card back in
                Court.Add(Players[CurrentCounter].Hand[cardInHand]);
                Court.Shuffle();
                Players[CurrentCounter].Hand[cardInHand] = Court[0];
                Court.RemoveAt(0);

                // Finish turn
                CurrentCounter = -1;
                CurrentChallenge = -1;
                CompleteTurn(true);
                return;
            }
            // He doesn't! Challenger wins!
            else
            {
                UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentCounter + 1).ToString() + " does NOT have the card required to counter " + (CurrentTurn + 1).ToString() + "'s Action! They lose an influence point!", true));

                // Reveal their first card if not revealed, otherwise, reveal the second
                Utils.RemoveInfluence(CurrentCounter);

                // Remove the counter action
                Players[CurrentCounter].CurrentCounteraction = CounterActions.None;
                CurrentCounter = -1;
                CurrentChallenge = -1;
                CompleteTurn(false);
                return;
            }
        }

        public void RunChallenge()
        {
            // Does the defender have the appropriate card to do the counter they want to?
            int cardInHand = Utils.HasChallengedCard(CurrentTurn, Players[CurrentTurn].CurrentAction);

            // He does! Challenger loses
            if (cardInHand != -1)
            {
                UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " has the card required to use this Action! Player " + (CurrentCounter + 1).ToString() + "'s challenge is unsuccessful. They lose an influence point!", true));

                // Reveal their first card if not revealed, otherwise, reveal the second
                Utils.RemoveInfluence(CurrentCounter);

                // Shuffle challenged card back in
                Court.Add(Players[CurrentTurn].Hand[cardInHand]);
                Court.Shuffle();
                Players[CurrentTurn].Hand[cardInHand] = Court[0];
                Court.RemoveAt(0);

                // Special case for Exchange, unfortunately.
                if (Players[CurrentTurn].CurrentAction == Actions.Exchange)
                {
                    CurrentCounter = CurrentTurn;
                    return;
                }

                // Finish turn
                CurrentCounter = -1;
                CurrentChallenge = -1;
                CompleteTurn(false);
                return;
            }
            // He doesn't! Challenger wins!
            else
            {
                UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " does NOT have the card required to counter to use this Action! They lose an influence point!", true));

                // Reveal their first card if not revealed, otherwise, reveal the second
                Utils.RemoveInfluence(CurrentTurn);

                CurrentCounter = -1;
                CurrentChallenge = -1;
                CompleteTurn(true);
                return;
            }
        }

        #endregion

        #region AI

        public void ProcessTurns()
        {
            // cancel out if they have a chosen action
            if (Players[CurrentTurn].CurrentAction != Actions.None) return;

            // There is a rule about you HAVING to coup if you have 10 or more coins, so let's do that for the AI here
            if (Players[CurrentTurn].Coins >= 10)
            {
                Players[CurrentTurn].CurrentAction = Actions.Coup;
                ActionHandler.ActionTargetPlayer(UnityEngine.Random.Range(0, AmountOfPlayers));
                return;
            }

            // AI is supposed to be random, so let's choose a random action
            int rnd = UnityEngine.Random.Range(0, 6);

            // Based on the rng, pick one of our actions.
            switch (rnd)
            {
                case 0:
                    ActionHandler.ActionIncome();
                    return;

                case 1:
                    ActionHandler.ActionForeignAid();
                    return;

                case 2: // Coup
                    if (Players[CurrentTurn].Coins >= 7)
                    {
                        Players[CurrentTurn].CurrentAction = Actions.Coup;
                        ActionHandler.ActionTargetPlayer(UnityEngine.Random.Range(0, AmountOfPlayers));
                    }
                    else
                    {
                        ActionHandler.ActionIncome();
                    }
                    return;

                case 3:
                    ActionHandler.ActionTax();
                    return;

                case 4:
                    if (Players[CurrentTurn].Coins >= 3)
                    {
                        Players[CurrentTurn].CurrentAction = Actions.Assassinate;
                        ActionHandler.ActionTargetPlayer(UnityEngine.Random.Range(0, AmountOfPlayers));
                    }
                    else
                    {
                        ActionHandler.ActionIncome();
                    }
                    return;

                case 5:
                    Players[CurrentTurn].CurrentAction = Actions.Steal;
                    ActionHandler.ActionTargetPlayer(UnityEngine.Random.Range(0, AmountOfPlayers));
                    return;

                case 6:
                    Players[CurrentTurn].CurrentAction = Actions.Exchange;
                    UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentTurn + 1).ToString() + " attempts to use the Action -> Exchange", true));
                    CurrentCounter = Utils.NextInOrder(CurrentTurn);
                    return;

                default:
                    ActionHandler.ActionIncome();
                    return;
            }
        }

        public void ProcessCounters()
        {       
            // Check if they'll challenge first. 
            if (UnityEngine.Random.Range(0, 1) >= 1 - AIChallengeWeight && Utils.CanChallengeAction(Players[CurrentTurn].CurrentAction))
            {
                UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentCounter + 1).ToString() + " has challenged player " + (CurrentTurn + 1).ToString() + "!", true));
                RunChallenge();
                return;
            }

            // we didn't challenge, let's continue the counters
            if (UnityEngine.Random.Range(0f, 1f) >= 1 - AICounterWeight && Utils.CanCounterAction(Players[CurrentTurn].CurrentAction))
            {
                Players[CurrentCounter].CurrentCounteraction = Utils.GetCounterActionNeeded(Players[CurrentTurn].CurrentAction);
                UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentCounter + 1).ToString() + " has opted to counter player " + (CurrentTurn + 1).ToString() + "!", true));
                // Set up challenges
                CurrentChallenge = Utils.NextInOrder(CurrentCounter);
                return;
            }


            UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentCounter + 1).ToString() + " does not counter or challenge"));
            CurrentCounter = Utils.NextInOrder(CurrentCounter);
        }

        public void ProcessCounterChallenges()
        {       
            // Check if they'll challenge first. This is a counter so let's bump up the % chance a bit. Say 35?
            if (UnityEngine.Random.Range(0, 1) >= 1 - AICounterChallengeWeight)
            {
                UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentChallenge + 1).ToString() + " has challenged player " + (CurrentCounter + 1).ToString() + "'s Counter Action!", true));
                RunCounteractionChallenge();
                return;
            }
            

            UILog.Instance.AddMessage(new LogMessage("Player " + (CurrentChallenge + 1).ToString() + " does not challenge"));
            CurrentChallenge = Utils.NextInOrder(CurrentChallenge);
        }

        #endregion

        #region GameStart

        public void StartGame()
        {
            Treasury = 50;
            PopulateCourt();
            CreatePlayers();

            UILog.Instance.AddMessage(new LogMessage("Started Game", true));
            UILog.Instance.AddMessage(new LogMessage("Player 1 turn start. Choose your action"));

            CurrentTurn = WinningPlayer;
            CurrentCounter = -1;
            CurrentChallenge = -1;

            GameStarted = true;
            GameOver = false;
        }

        public void PopulateCourt()
        {
            Court.Clear();
            for (int i = 0; i < 3; i++)
            {
                Court.Add(new Card(CardType.Duke));
                Court.Add(new Card(CardType.Assassin));
                Court.Add(new Card(CardType.Captain));
                Court.Add(new Card(CardType.Ambassador));
                Court.Add(new Card(CardType.Contessa));
            }
            Court.Shuffle();
        }

        public void CreatePlayers()
        {
            // clear old players (game resets)
            Players = new Player[AmountOfPlayers];

            for (int i = 0; i < AmountOfPlayers; i++)
            {
                List<Card> hand = new List<Card>();
                // add the first two
                hand.Add(Court[0]);
                hand.Add(Court[1]);
                // remove them from the Court
                Court.RemoveAt(1);
                Court.RemoveAt(0);

                Players[i] = new Player(hand.ToArray(), i);
                Players[i].Coins = 2;
            }
            Treasury -= AmountOfPlayers * 2;
        }
        #endregion
    }
}
