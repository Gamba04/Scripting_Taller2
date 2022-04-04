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

        [Test]
        public void KnightVsKnight()
        {
            Character knight1 = new Character("Knight 1", Card.Rarity.Common, 0, 9, 10, Character.Affinity.Knight);
            Character knight2 = new Character("Knight 2", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);

            // Knight1 attacks Knight2, doesn't kill
            Console.WriteLine($"Knight1 ({knight1.AttackPoints} AP) attacks Knight2 ({knight2.ResistPoints} RP)");
            Assert.IsFalse(knight1.Attack(knight2));
            Console.WriteLine($"Knight2 RP: {knight2.ResistPoints}");

            // Knight2 attacks Knight1, kills
            Console.WriteLine($"Knight2 ({knight2.AttackPoints} AP) attacks Knight1 ({knight1.ResistPoints} RP)");
            Assert.IsTrue(knight2.Attack(knight1));
            Console.WriteLine($"Knight1 RP: {knight1.ResistPoints}");
        }

        [Test]
        public void KnightVsUndead()
        {
            Character knight = new Character("Knight", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Undead);
            Character undead = new Character("Undead", Card.Rarity.Common, 0, 9, 10, Character.Affinity.Mage);

            // Knight attacks Undead, doesn't kill
            Console.WriteLine($"Knight ({knight.AttackPoints} AP) attacks Undead ({undead.ResistPoints} RP)");
            Assert.IsFalse(knight.Attack(undead));
            Console.WriteLine($"Undead RP: {undead.ResistPoints}");

            // Undead attacks Knight, kills
            Console.WriteLine($"Undead ({undead.AttackPoints} AP) attacks Knight ({knight.ResistPoints} RP)");
            Assert.IsTrue(undead.Attack(knight));
            Console.WriteLine($"Knight RP: {knight.ResistPoints}");
        }

        [Test]
        public void UndeadVsMage()
        {
            Character undead = new Character("Undead", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Undead);
            Character mage = new Character("Mage", Card.Rarity.Common, 0, 9, 10, Character.Affinity.Mage);

            // Undead attacks Mage, doesn't kill
            Console.WriteLine($"Undead ({undead.AttackPoints} AP) attacks Mage ({mage.ResistPoints} RP)");
            Assert.IsFalse(undead.Attack(mage));
            Console.WriteLine($"Mage RP: {mage.ResistPoints}");

            // Mage attacks Undead, kills
            Console.WriteLine($"Mage ({mage.AttackPoints} AP) attacks Undead ({undead.ResistPoints} RP)");
            Assert.IsTrue(mage.Attack(undead));
            Console.WriteLine($"Undead RP: {undead.ResistPoints}");
        }

        [Test]
        public void MageVsKnight()
        {
            Character mage = new Character("Mage", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Mage);
            Character knight = new Character("Knight", Card.Rarity.Common, 0, 9, 10, Character.Affinity.Knight);

            // Mage attacks Knight, doesn't kill
            Console.WriteLine($"Mage ({mage.AttackPoints} AP) attacks Knight ({knight.ResistPoints} RP)");
            Assert.IsFalse(mage.Attack(knight));
            Console.WriteLine($"Knight RP: {knight.ResistPoints}");

            // Knight attacks Mage, kills
            Console.WriteLine($"Knight ({knight.AttackPoints} AP) attacks Mage ({mage.ResistPoints} RP)");
            Assert.IsTrue(knight.Attack(mage));
            Console.WriteLine($"Mage RP: {mage.ResistPoints}");
        }

        [Test]
        public void ApplyEquips()
        {
            Character character = new Character("Character", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);

            Equip equipAP = new Equip("AP", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.All);
            Equip equipRP = new Equip("AP", Card.Rarity.Common, 0, Equip.TargetAttribute.RP, 5, Equip.Affinity.All);
            Equip equipAll = new Equip("AP", Card.Rarity.Common, 0, Equip.TargetAttribute.All, 5, Equip.Affinity.All);

            int ap = character.AttackPoints;
            int rp = character.ResistPoints;

            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Apply AP
            Console.WriteLine($"Apply AP Equip (+{equipAP.EffectPoints})");
            character.AddEquip(equipAP);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.AttackPoints == ap + equipAP.EffectPoints);
            ap += equipAP.EffectPoints;

            // Apply RP
            Console.WriteLine($"Apply RP Equip (+{equipRP.EffectPoints})");
            character.AddEquip(equipRP);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.AttackPoints == rp + equipRP.EffectPoints);
            rp += equipRP.EffectPoints;

            // Apply All
            Console.WriteLine($"Apply All Equip (+{equipAll.EffectPoints})");
            character.AddEquip(equipAll);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.AttackPoints == rp + equipAll.EffectPoints);
            Assert.IsTrue(character.AttackPoints == ap + equipAll.EffectPoints);
            ap += equipAll.EffectPoints;
            rp += equipAll.EffectPoints;
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