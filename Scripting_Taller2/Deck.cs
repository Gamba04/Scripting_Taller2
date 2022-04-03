using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting_Taller2
{
    public class Deck
    {
        private const int maxCharacters = 5;
        private const int maxEquips = 10;
        private const int maxSupports = 5;

        private int charactersCount;
        private int equipsCount;
        private int supportsCount;

        private int costPoints;

        private List<Card> cards = new List<Card>();

        public event Action onAllCharactersEliminated;

        public Deck(int costPoints)
        {
            this.costPoints = costPoints;
        }

        public bool AddCard(Card card)
        {
            // Type validation
            if (card is Character)
            {
                if (charactersCount < maxCharacters)
                {
                    charactersCount++;
                }
                else return false;
            }

            if (card is Equip)
            {
                if (equipsCount < maxEquips)
                {
                    equipsCount++;
                }
                else return false;
            }

            if (card is SupportSkill)
            {
                if (supportsCount < maxSupports)
                {
                    supportsCount++;
                }
                else return false;
            }

            // CP validation
            if (card.CostPoints <= costPoints)
            {
                costPoints -= card.CostPoints;
            }
            else return false;

            card.onRemoveFromDeck += RemoveFromDeck;
            cards.Add(card);

            return true;
        }

        private void RemoveFromDeck(Card card)
        {
            // Type validation
            if (card is Character) charactersCount--;
            if (card is Equip) equipsCount--;
            if (card is SupportSkill) supportsCount--;

            cards.Remove(card);

            if (charactersCount <= 0) onAllCharactersEliminated?.Invoke();
        }

        public void ReduceAP(int amount)
        {
            foreach (Card card in cards)
            {
                if (card is Character character)
                {
                    character.ReduceAP(amount);
                }
            }
        }

        public void ReduceRP(int amount)
        {
            foreach (Card card in cards)
            {
                if (card is Character character)
                {
                    character.RestoreRP(amount);
                }
            }
        }

        public void RestoreRP(int amount)
        {
            foreach (Card card in cards)
            {
                if (card is Character character)
                {
                    character.RestoreRP(amount);
                }
            }
        }
    }
}