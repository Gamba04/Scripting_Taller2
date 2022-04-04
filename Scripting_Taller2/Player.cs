using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting_Taller2
{
    public class Player
    {
        private Deck deck;

        public Deck Deck => deck;

        public Player(int costPoints)
        {
            deck = new Deck(costPoints);
            deck.onAllCharactersEliminated += Lose;
        }

        private void Lose()
        {

        }
    }
}