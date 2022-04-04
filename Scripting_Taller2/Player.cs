using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting_Taller2
{
    public class Player
    {
        private Deck deck;

        private bool lost;

        public Deck Deck => deck;

        public bool Lost => lost;

        public Player(int costPoints)
        {
            deck = new Deck(costPoints);
            deck.onAllCharactersEliminated += Lose;
        }

        private void Lose()
        {
            lost = true;
        }
    }
}