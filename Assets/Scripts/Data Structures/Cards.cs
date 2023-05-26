using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public enum CardType
    {
        Duke = 0,
        Assassin,
        Captain,
        Ambassador,
        Contessa
    }

    public struct Card
    {
        public CardType type;
        public bool revealed;

        // constructor
        public Card(CardType myType)
        {
            type = myType;
            revealed = false;
        }

    }
}
