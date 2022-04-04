using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting_Taller2
{
    public abstract class Card
    {
        public enum Rarity
        {
            Common,
            Rare,
            SuperRare,
            UltraRare
        }

        protected string name;
        protected Rarity rarity;
        protected int costPoints;

        internal event Action<Card> onRemoveFromDeck;

        public int CostPoints => costPoints;

        protected Card(string name, Rarity rarity, int costPoints)
        {
            this.name = name;
            this.rarity = rarity;
            this.costPoints = costPoints;
        }

        public void RemoveFromDeck()
        {
            onRemoveFromDeck?.Invoke(this);
        }
    }
}