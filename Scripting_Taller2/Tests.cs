using System;
using NUnit.Framework;

namespace Scripting_Taller2
{
    public class Tests
    {
        #region General values

        private readonly Random random = new Random();

        const int playerCostPoints = 10;

        #endregion

        private Player player;

        #region Tests

        [SetUp]
        public void SetUp()
        {
            player = new Player(playerCostPoints);
        }

        [TearDown]
        public void TearDown()
        {
            player = null;
        }

        [Test]
        public void Add0Card()
        {
            PrintDeckCP();

            Console.WriteLine("Add card (0 CP)");
            Assert.IsTrue(player.Deck.AddCard(new Character("0 CP", Card.Rarity.Common, 0, 0, 0, Character.Affinity.Knight)));

            PrintDeckCP();
        }

        [Test]
        public void AddMaxCard()
        {
            PrintDeckCP();

            Console.WriteLine($"Add card ({playerCostPoints} CP)");
            Assert.IsTrue(player.Deck.AddCard(new Character("Max CP", Card.Rarity.Common, playerCostPoints, 0, 0, Character.Affinity.Knight)));

            PrintDeckCP();
        }

        [Test]
        public void AddRandomCard()
        {
            PrintDeckCP();

            Card randomCard = GenerateRandomCard();
            Console.WriteLine($"Add card ({randomCard.CostPoints} CP)");
            Assert.IsTrue(player.Deck.AddCard(randomCard));

            PrintDeckCP();
        }

        [Test]
        public void FillDeckWithRandomCards()
        {
            PrintDeckCP();

            int iterations = 0;

            while (player.Deck.CostPoints > 0)
            {
                if (iterations > player.Deck.CharactersCount + player.Deck.EquipsCount + player.Deck.SupportsCount) break; // Cant add any more cards

                Card randomCard = GenerateRandomCard();

                bool expected = ValidateCardAddition(randomCard, out int typeCount, out int typeLimit);
                bool result = player.Deck.AddCard(randomCard);

                Console.WriteLine($"Add card ({randomCard.GetType().Name} ({typeCount + 1}/{typeLimit}), {randomCard.CostPoints} CP): Expect {expected}");

                if (expected) Assert.IsTrue(result);
                else Assert.IsFalse(result);

                PrintDeckCP();

                iterations++;
            }
        }

        #endregion

        #region Other

        private Card GenerateRandomCard()
        {
            int type = GetRandomIntWithProbability(20, 30, 50);

            Card card = null;
            Card.Rarity rarity;
            int costPoints = random.Next(0, playerCostPoints + 1);

            switch (type)
            {
                case 0:
                    rarity = (Card.Rarity)GetRandomIntWithProbability(12, 5, 2.5f, 0.5f);

                    card = new Character("Random Character", rarity, costPoints, random.Next(0, 10), random.Next(0, 10), GetRandomEnum<Character.Affinity>());
                    break;

                case 1:
                    rarity = (Card.Rarity)GetRandomIntWithProbability(20, 7, 2.5f, 0.5f);

                    card = new Equip("Random Equip", rarity, costPoints, GetRandomEnum<Equip.TargetAttribute>(), random.Next(0, 10), GetRandomEnum<Equip.Affinity>());
                    break;

                case 2:
                    rarity = (Card.Rarity)GetRandomIntWithProbability(37, 10, 2.5f, 0.5f);

                    card = new SupportSkill("Random Support", rarity, costPoints, GetRandomEnum<SupportSkill.EffectType>(), random.Next(0, 10));
                    break;
            }

            return card;
        }

        private int GetRandomIntWithProbability(params float[] probabilities)
        {
            float rand = (float)random.NextDouble();

            float currentPosition = 0;

            float totalProbability = 0;
            foreach (float probability in probabilities) totalProbability += probability;

            if (totalProbability == 0) throw new Exception("The total probability must not be 0.");

            for (int i = 0; i < probabilities.Length; i++)
            {
                float probability = probabilities[i] / totalProbability;

                currentPosition += probability;

                if (probability > 0 && rand <= currentPosition) return i;
            }

            return -1;
        }

        private bool ValidateCardAddition(Card targetCard, out int typeCount, out int typeLimit)
        {
            typeCount = -1;
            typeLimit = -1;

            // Type validation
            if (targetCard is Character)
            {
                typeCount = player.Deck.CharactersCount;
                typeLimit = Deck.maxCharacters;
            }
            else if (targetCard is Equip)
            {
                typeCount = player.Deck.EquipsCount;
                typeLimit = Deck.maxEquips;
            }
            else if (targetCard is SupportSkill)
            {
                typeCount = player.Deck.SupportsCount;
                typeLimit = Deck.maxSupports;
            }

            if (typeCount >= typeLimit) return false;

            // CP validation
            return targetCard.CostPoints <= player.Deck.CostPoints;
        }

        private E GetRandomEnum<E>() where E : Enum
        {
            E[] values = Enum.GetValues(typeof(E)) as E[];

            return values[random.Next(0, values.Length)];
        }

        private void PrintDeckCP() => Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");

        #endregion

    }
}