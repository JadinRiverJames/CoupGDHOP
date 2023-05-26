using System.Collections.Generic;
using UnityEngine;
using System;
using Gameplay;
using Cards;
using Players;
using Log;

namespace Util
{
    public static class IListExtensions
    {
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }

    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                }
                return instance;
            }
            set => instance = value;
        }

        public virtual void Awake()
        {
            if (Instance == this)
            {
                return;
            }

            if (Instance != null)
            {
                Debug.LogError($"Only one of {typeof(T).Name} may exist! Following errors will ping the problematic objects.");
                Debug.LogError("Existing object is here", context: Instance.gameObject);
                Debug.LogError("New object is here", context: gameObject);
                throw new Exception();
            }
            Instance = (T)this;
        }

        public virtual void OnDestroy()
        {
            Instance = null;
        }
    }

    public static class Utils
    {
        public static int NextInOrder(int v)
        {
            int newValue = v + 1;
            if (newValue >= Manager.Instance.AmountOfPlayers)
            {
                newValue = 0;
            }
            return newValue;
        }

        public static CounterActions GetCounterActionNeeded(Actions actionToCounter)
        {
            switch (actionToCounter)
            {
                case Actions.ForeignAid:
                    return CounterActions.BlockForeignAid;

                case Actions.Assassinate:
                    return CounterActions.BlockAssassination;

                case Actions.Steal:
                    return CounterActions.BlockStealing;

            }
            return CounterActions.None;
        }

        public static bool CanCounterAction(Actions actionToCounter)
        {
            switch (actionToCounter)
            {
                case Actions.ForeignAid:
                case Actions.Assassinate:
                case Actions.Steal:
                    return true;

            }
            return false;
        }

        public static bool CanChallengeAction(Actions actionToCounter)
        {
            switch (actionToCounter)
            {
                case Actions.Tax:
                case Actions.Assassinate:
                case Actions.Steal:
                case Actions.Exchange:
                    return true;

            }
            return false;
        }

        public static int HasCounterCard(int PlayerID, Actions actionToCounter)
        {
            // Could probably convert the player's hand to a list here, but since we're returning the index of the card to reveal, might end up being more code than this.

            switch (actionToCounter)
            {
                case Actions.ForeignAid:
                    if (!Manager.Instance.Players[PlayerID].Hand[0].revealed && Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Duke) return 0;
                    if (!Manager.Instance.Players[PlayerID].Hand[1].revealed && Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Duke) return 1;
                    return -1;

                case Actions.Assassinate:
                    if (!Manager.Instance.Players[PlayerID].Hand[0].revealed && Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Contessa) return 0;
                    if (!Manager.Instance.Players[PlayerID].Hand[1].revealed && Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Contessa) return 1;
                    return -1;

                case Actions.Steal:
                    if (!Manager.Instance.Players[PlayerID].Hand[0].revealed && (Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Ambassador || Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Captain)) return 0;
                    if (!Manager.Instance.Players[PlayerID].Hand[1].revealed && (Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Ambassador ||Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Captain)) return 1;
                    return -1;
            }

            return -1;
        }

        public static int HasChallengedCard(int PlayerID, Actions actionToCounter)
        {
            // Could probably convert the player's hand to a list here, but since we're returning the index of the card to reveal, might end up being more code than this.

            switch (actionToCounter)
            {
                case Actions.Tax:
                    if (!Manager.Instance.Players[PlayerID].Hand[0].revealed && Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Duke) return 0;
                    if (!Manager.Instance.Players[PlayerID].Hand[1].revealed && Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Duke) return 1;
                    return -1;

                case Actions.Exchange:
                    if (!Manager.Instance.Players[PlayerID].Hand[0].revealed && Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Ambassador) return 0;
                    if (!Manager.Instance.Players[PlayerID].Hand[1].revealed && Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Ambassador) return 1;
                    return -1;

                case Actions.Assassinate:
                    if (!Manager.Instance.Players[PlayerID].Hand[0].revealed && Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Contessa) return 0;
                    if (!Manager.Instance.Players[PlayerID].Hand[1].revealed && Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Contessa) return 1;
                    return -1;

                case Actions.Steal:
                    if (!Manager.Instance.Players[PlayerID].Hand[0].revealed && (Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Ambassador || Manager.Instance.Players[PlayerID].Hand[0].type == CardType.Captain)) return 0;
                    if (!Manager.Instance.Players[PlayerID].Hand[1].revealed && (Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Ambassador || Manager.Instance.Players[PlayerID].Hand[1].type == CardType.Captain)) return 1;
                    return -1;

            }

            return -1;
        }

        public static void RemoveInfluence(int PlayerID)
        {
            // Reveal their first card if not revealed, otherwise, reveal the second
            if (!Manager.Instance.Players[PlayerID].Hand[0].revealed)
            {
                Manager.Instance.Players[PlayerID].Hand[0].revealed = true;
            }
            else
            {
                Manager.Instance.Players[PlayerID].Hand[1].revealed = true;
            }
        }

        public static void TakeCoins(int PlayerID, int VictimID)
        {
            if (Manager.Instance.Players[VictimID].Coins > 1)
            {
                Manager.Instance.Players[VictimID].Coins -= 2;
                Manager.Instance.Players[PlayerID].Coins += 2;
                UILog.Instance.AddMessage(new LogMessage("Player " + (VictimID + 1).ToString() + " had 2 coins stolen from them!"));
            }
            else if (Manager.Instance.Players[VictimID].Coins == 1)
            {
                Manager.Instance.Players[VictimID].Coins -= 1;
                Manager.Instance.Players[PlayerID].Coins += 1;
                UILog.Instance.AddMessage(new LogMessage("Player " + (VictimID + 1).ToString() + " has 1 coin! Only stole 1 coin."));
            }
            else
            {
                UILog.Instance.AddMessage(new LogMessage("Player " + (VictimID + 1).ToString() + " has no coins! Cannot steal from them."));
            }
        }

        public static void PopulateSwappableCards()
        {
            Manager.Instance.SwappableCards = new Card[2];
            Manager.Instance.SwappableCards[0] = Manager.Instance.Court[1];
            Manager.Instance.SwappableCards[1] = Manager.Instance.Court[0];

            Manager.Instance.Court.RemoveRange(0, 2);
        }

    }

}
