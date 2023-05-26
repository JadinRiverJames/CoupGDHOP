using UnityEngine;
using Cards;
using Util;
using Players;
using Log;


namespace Gameplay
{
    public class ActionHandler : MonoBehaviour
    {
        #region BasicActions
        public static void ActionIncome()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentTurn + 1).ToString() + " attempts to use the Action -> Income", true));
            // No need for counter or challenge options.
            Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction = Actions.Income;
            Manager.Instance.CompleteTurn(false);
        }

        public static void ActionForeignAid()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentTurn + 1).ToString() + " attempts to use the Action -> Foreign Aid", true));
            Manager.Instance.CurrentCounter = Utils.NextInOrder(Manager.Instance.CurrentTurn);
            Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction = Actions.ForeignAid;
        }

        public static void ActionTax()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentTurn + 1).ToString() + " attempts to use the Action -> Tax", true));
            Manager.Instance.CurrentCounter = Utils.NextInOrder(Manager.Instance.CurrentTurn);
            Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction = Actions.Tax;
        }

        public static void ActionExchange()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentTurn + 1).ToString() + " attempts to use the Action -> Exchange", true));
            Manager.Instance.CurrentCounter = Utils.NextInOrder(Manager.Instance.CurrentTurn);
            Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction = Actions.Exchange;
        }

        public static void ActionPass()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentCounter + 1).ToString() + " does not counter or challenge"));
            Manager.Instance.CurrentCounter = Utils.NextInOrder(Manager.Instance.CurrentCounter);
        }

        public static void ActionChallenge()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentCounter + 1).ToString() + " has challenged player " + (Manager.Instance.CurrentTurn + 1).ToString() + "'s Action!", true));
            Manager.Instance.RunChallenge();
        }
        #endregion

        #region CounterChallengePass
        public static void ActionChallengeCounter()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentChallenge + 1).ToString() + " has challenged player " + (Manager.Instance.CurrentCounter + 1).ToString() + "'s Counter Action!", true));
            Manager.Instance.RunCounteractionChallenge();
        }

        public static void ActionPassChallenge()
        {
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentChallenge + 1).ToString() + " does not challenge", true));
            Manager.Instance.CurrentChallenge = Utils.NextInOrder(Manager.Instance.CurrentChallenge);
        }
        #endregion

        #region TargettableAbilities
        public static void ActionStartCoup()
        {
            Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction = Actions.Coup;
            Manager.Instance.Targetting = true;
        }

        public static void ActionStartAssasinate()
        {
            Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction = Actions.Assassinate;
            Manager.Instance.Targetting = true;
        }

        public static void ActionStartSteal()
        {
            Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction = Actions.Steal;
            Manager.Instance.Targetting = true;
        }

        public static void ActionTargetPlayer(int PlayerID)
        {
            // Security check for AI
            int newID = PlayerID;
            while (Manager.Instance.Players[newID].IsOutOfGame || newID == Manager.Instance.CurrentTurn)
            {
                newID = Utils.NextInOrder(newID);
            }

            switch (Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction)
            {
                case Actions.Coup:
                    UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentTurn + 1).ToString() + " launched a Coup against player " + (newID + 1).ToString() + "!", true));
                    Utils.RemoveInfluence(newID);
                    Manager.Instance.CompleteTurn(false);
                    break;

                case Actions.Assassinate:
                    UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentTurn + 1).ToString() + " attempts to use the Action -> Assassinate on player " + (newID + 1).ToString() + "!", true));
                    Manager.Instance.Target = newID;
                    Manager.Instance.CurrentCounter = Utils.NextInOrder(Manager.Instance.CurrentTurn);
                    break;

                case Actions.Steal:
                    UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentTurn + 1).ToString() + " attempts to use the Action -> Steal on player " + (newID + 1).ToString() + "!", true));
                    Manager.Instance.Target = newID;
                    Manager.Instance.CurrentCounter = Utils.NextInOrder(Manager.Instance.CurrentTurn);
                    break;

            }

            Manager.Instance.Targetting = false;
        }

        #endregion

        #region CounterAbilities
        public static void CounterForeignAid() {
            Manager.Instance.Players[Manager.Instance.CurrentCounter].CurrentCounteraction = Utils.GetCounterActionNeeded(Manager.Instance.Players[Manager.Instance.CurrentTurn].CurrentAction);
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentCounter + 1).ToString() + " has opted to counter player " + (Manager.Instance.CurrentTurn + 1).ToString() + "!", true));
            // Set up challenges
            Manager.Instance.CurrentChallenge = Utils.NextInOrder(Manager.Instance.CurrentCounter);
        }

        public static void CounterSteal()
        {
            Manager.Instance.Players[Manager.Instance.CurrentCounter].CurrentCounteraction = CounterActions.BlockStealing;
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentCounter + 1).ToString() + " has opted to counter player " + (Manager.Instance.CurrentTurn + 1).ToString() + "!", true));
            // Set up challenges
            Manager.Instance.CurrentChallenge = Utils.NextInOrder(Manager.Instance.CurrentCounter);
        }

        public static void CounterAssassinate()
        {
            Manager.Instance.Players[Manager.Instance.CurrentCounter].CurrentCounteraction = CounterActions.BlockAssassination;
            UILog.Instance.AddMessage(new LogMessage("Player " + (Manager.Instance.CurrentCounter + 1).ToString() + " has opted to counter player " + (Manager.Instance.CurrentTurn + 1).ToString() + "!", true));
            // Set up challenges
            Manager.Instance.CurrentChallenge = Utils.NextInOrder(Manager.Instance.CurrentCounter);
        }
        #endregion

        #region PlayerExchangeStuff
        public static void SwapHorizontal()
        {
            Card card = Manager.Instance.SwappableCards[0];
            Manager.Instance.SwappableCards[0] = Manager.Instance.SwappableCards[1];
            Manager.Instance.SwappableCards[1] = card;
        }

        public static void SwapHand0()
        {
            Card card = Manager.Instance.SwappableCards[0];
            Manager.Instance.SwappableCards[0] = Manager.Instance.Players[Manager.Instance.CurrentTurn].Hand[0];
            Manager.Instance.Players[Manager.Instance.CurrentTurn].Hand[0] = card;
        }

        public static void SwapHand1()
        {
            Card card = Manager.Instance.SwappableCards[1];
            Manager.Instance.SwappableCards[1] = Manager.Instance.Players[Manager.Instance.CurrentTurn].Hand[1];
            Manager.Instance.Players[Manager.Instance.CurrentTurn].Hand[1] = card;
        }

        public static void FinishSwapping()
        {
            Manager.Instance.FinishedSwap = true;
        }
        #endregion

    }
}
