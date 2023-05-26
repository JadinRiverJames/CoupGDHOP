using Cards;

namespace Players
{

    public enum Actions
    {
        None = 0,
        Income,
        ForeignAid,
        Coup,
        Tax,
        Assassinate,
        Steal,
        Exchange
    }

    public enum CounterActions
    {
        None = 0,
        Pass,
        BlockForeignAid,
        BlockAssassination,
        BlockStealing
    }

    public struct Player
    {
        public Card[] Hand;
        public int Coins;

        public Actions CurrentAction;
        public CounterActions CurrentCounteraction;
        public bool IsOutOfGame;

        public Player(Card[] toHand, int id)
        {
            Hand = toHand;
            Coins = 0;
            CurrentAction = Actions.None;
            CurrentCounteraction = CounterActions.None;
            IsOutOfGame = false;
        }
    }
}
