using System;
using System.Collections.Generic;
using NUnit.Framework;
using Scripting_Taller2;

namespace Tests
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
        public void Add0CPCard()
        {
            Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");

            Console.WriteLine("Add card (0 CP)");
            Assert.IsTrue(player.Deck.AddCard(new Character("0 CP", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight)));

            Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");
        }

        [Test]
        public void AddMaxCPCard()
        {
            Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");

            Console.WriteLine($"Add card ({playerCostPoints} CP)");
            Assert.IsTrue(player.Deck.AddCard(new Character("Max CP", Card.Rarity.Common, playerCostPoints, 10, 10, Character.Affinity.Knight)));

            Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");
        }

        [Test]
        public void AddRandomCard()
        {
            Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");

            Card randomCard = GenerateRandomCard();
            Console.WriteLine($"Add card ({randomCard.CostPoints} CP)");
            Assert.IsTrue(player.Deck.AddCard(randomCard));

            Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");
        }

        [Test]
        public void Add0RPCharacter()
        {
            Assert.IsFalse(player.Deck.AddCard(new Character("Invalid Character", Card.Rarity.Common, 0, 0, 0, Character.Affinity.Knight)));
        }

        [Test]
        public void FillDeckWithRandomCards()
        {
            Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");

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

                Console.WriteLine($"Deck CP: {player.Deck.CostPoints}");

                iterations++;
            }
        }

        [Test]
        public void KnightVsKnight()
        {
            Character knight1 = new Character("Knight 1", Card.Rarity.Common, 0, 8, 10, Character.Affinity.Knight);
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
            Character knight = new Character("Knight", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);
            Character undead = new Character("Undead", Card.Rarity.Common, 0, 8, 10, Character.Affinity.Undead);

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
            Character mage = new Character("Mage", Card.Rarity.Common, 0, 8, 10, Character.Affinity.Mage);

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
            Character knight = new Character("Knight", Card.Rarity.Common, 0, 8, 10, Character.Affinity.Knight);

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
        public void WinMatch()
        {
            FillDeckCharacters(50);

            Character enemy = new Character("Undead Enemy", Card.Rarity.Rare, 0, 20, 20, Character.Affinity.Undead);

            List<Character> deckCharacters = player.Deck.GetCharacters();

            Console.WriteLine($"Undead Enemy attacks:");
            Console.WriteLine($"Deck cards: {player.Deck.Cards.Count}, lost: {player.Lost}\n");

            for (int i = 0; i < deckCharacters.Count; i++)
            {
                do
                {
                    Console.WriteLine($"Undead Enemy ({enemy.AttackPoints} AP) attacks {deckCharacters[i].Name} ({deckCharacters[i].ResistPoints} RP)");
                    enemy.Attack(deckCharacters[i]);
                    Console.WriteLine($"{deckCharacters[i].Name} RP: {deckCharacters[i].ResistPoints}");
                }
                while (deckCharacters[i].ResistPoints > 0); // In case it doesn't kill

                Console.WriteLine($"Deck cards: {player.Deck.Cards.Count}, lost: {player.Lost}\n");
            }

            Assert.IsTrue(player.Deck.Cards.Count == 0);
            Assert.IsTrue(player.Lost);
        }

        [Test]
        public void EquipsStats()
        {
            Character character = new Character("Character", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);

            Equip equipAP = new Equip("Equip AP", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.All);
            Equip equipRP = new Equip("Equip RP", Card.Rarity.Common, 0, Equip.TargetAttribute.RP, 7, Equip.Affinity.All);
            Equip equipAll = new Equip("Equip All", Card.Rarity.Common, 0, Equip.TargetAttribute.All, 10, Equip.Affinity.All);

            int ap = character.AttackPoints;
            int rp = character.ResistPoints;

            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Apply AP
            Console.WriteLine($"Apply Equip AP (+{equipAP.EffectPoints})");
            equipAP.ApplyToCharacter(character);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.AttackPoints == ap + equipAP.EffectPoints);
            ap += equipAP.EffectPoints;

            // Apply RP
            Console.WriteLine($"Apply Equip RP (+{equipRP.EffectPoints})");
            equipRP.ApplyToCharacter(character);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.ResistPoints == rp + equipRP.EffectPoints);
            rp += equipRP.EffectPoints;

            // Apply All
            Console.WriteLine($"Apply Equip All (+{equipAll.EffectPoints})");
            equipAll.ApplyToCharacter(character);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.AttackPoints == ap + equipAll.EffectPoints);
            Assert.IsTrue(character.ResistPoints == rp + equipAll.EffectPoints);
            ap += equipAll.EffectPoints;
            rp += equipAll.EffectPoints;
        }

        [Test]
        public void MaxEquips()
        {
            Character character = new Character("Character", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);

            Equip equip;

            for (int i = 0; i < Character.maxEquips; i++)
            {
                Console.WriteLine($"Apply Equip ({i + 1}/{Character.maxEquips}): Expect True");
                equip = new Equip($"Equip {i + 1}", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.All);

                Assert.IsTrue(equip.ApplyToCharacter(character));
            }

            Console.WriteLine($"Apply Equip ({Character.maxEquips + 1}/{Character.maxEquips}): Expect False");
            equip = new Equip($"Equip past limit", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.All);

            Assert.IsFalse(equip.ApplyToCharacter(character));
        }

        [Test]
        public void InvalidEquips()
        {
            Character knight = new Character("Knight", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);
            Character mage = new Character("Mage", Card.Rarity.Common, 0, 9, 10, Character.Affinity.Mage);
            Character undead = new Character("Undead", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Undead);

            Equip equipKnight = new Equip($"Equip Knight", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.Knight);
            Equip equipMage = new Equip($"Equip Mage", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.Mage);
            Equip equipUndead = new Equip($"Equip Undead", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.Undead);
            Equip equipAll = new Equip($"Equip All", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.All);

            // Equip Knight
            Console.WriteLine("Apply Equip Knight in Knight: Expect True");
            Assert.IsTrue(equipKnight.ApplyToCharacter(knight));

            Console.WriteLine("Apply Equip Knight in Mage: Expect False");
            Assert.IsFalse(equipKnight.ApplyToCharacter(mage));

            Console.WriteLine("Apply Equip Knight in Undead: Expect False\n");
            Assert.IsFalse(equipKnight.ApplyToCharacter(undead));

            // Equip Mage
            Console.WriteLine("Apply Equip Mage in Knight: Expect False");
            Assert.IsFalse(equipMage.ApplyToCharacter(knight));

            Console.WriteLine("Apply Equip Mage in Mage: Expect True");
            Assert.IsTrue(equipMage.ApplyToCharacter(mage));

            Console.WriteLine("Apply Equip Mage in Undead: Expect False\n");
            Assert.IsFalse(equipMage.ApplyToCharacter(undead));

            // Equip Undead
            Console.WriteLine("Apply Equip Undead in Knight: Expect False");
            Assert.IsFalse(equipUndead.ApplyToCharacter(knight));

            Console.WriteLine("Apply Equip Undead in Mage: Expect False");
            Assert.IsFalse(equipUndead.ApplyToCharacter(mage));

            Console.WriteLine("Apply Equip Undead in Undead: Expect True\n");
            Assert.IsTrue(equipUndead.ApplyToCharacter(undead));

            // Equip All
            Console.WriteLine("Apply Equip All in Knight: Expect True");
            Assert.IsTrue(equipAll.ApplyToCharacter(knight));

            Console.WriteLine("Apply Equip All in Mage: Expect True");
            Assert.IsTrue(equipAll.ApplyToCharacter(mage));

            Console.WriteLine("Apply Equip All in Undead: Expect True");
            Assert.IsTrue(equipAll.ApplyToCharacter(undead));
        }

        [Test]
        public void DestroyEquipSkill()
        {
            Character character = new Character("Character", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);

            Equip equipAP = new Equip("Equip AP", Card.Rarity.Common, 0, Equip.TargetAttribute.AP, 5, Equip.Affinity.All);
            Equip equipRP = new Equip("Equip RP", Card.Rarity.Common, 0, Equip.TargetAttribute.RP, 7, Equip.Affinity.All);
            Equip equipAll = new Equip("Equip All", Card.Rarity.Common, 0, Equip.TargetAttribute.All, 10, Equip.Affinity.All);

            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Apply AP
            Console.WriteLine($"Apply Equip AP (+{equipAP.EffectPoints})");
            equipAP.ApplyToCharacter(character);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Apply RP
            Console.WriteLine($"Apply Equip RP (+{equipRP.EffectPoints})");
            equipRP.ApplyToCharacter(character);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Apply All
            Console.WriteLine($"Apply Equip All (+{equipAll.EffectPoints})");
            equipAll.ApplyToCharacter(character);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            int ap = character.AttackPoints;
            int rp = character.ResistPoints;

            SupportSkill destroySkill = new SupportSkill("Destroy Skill", Card.Rarity.Common, 0, SupportSkill.EffectType.DestroyEquip, 10);
            Assert.IsTrue(destroySkill.EffectPoints == 0);

            // Destroy Equip AP
            Console.WriteLine($"Destroy Equip AP (-{equipAP.EffectPoints})");
            Assert.IsTrue(destroySkill.UseSkill(null, null, character, equipAP));
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.AttackPoints == ap - equipAP.EffectPoints);
            ap -= equipAP.EffectPoints;

            // Destroy Equip RP
            Console.WriteLine($"Destroy Equip RP (-{equipRP.EffectPoints})");
            Assert.IsTrue(destroySkill.UseSkill(null, null, character, equipRP));
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.ResistPoints == rp - equipRP.EffectPoints);
            rp -= equipRP.EffectPoints;

            // Destroy Equip All
            Console.WriteLine($"Destroy Equip All (-{equipAll.EffectPoints})");
            Assert.IsTrue(destroySkill.UseSkill(null, null, character, equipAll));
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.AttackPoints == ap - equipAll.EffectPoints);
            Assert.IsTrue(character.ResistPoints == rp - equipAll.EffectPoints);
            ap -= equipAll.EffectPoints;
            rp -= equipAll.EffectPoints;

            // Destroy Invalid Equip
            Console.WriteLine($"Destroy Invalid Equip");
            Assert.IsFalse(destroySkill.UseSkill(null, null, character, equipAll));
        }

        [Test]
        public void KillByDestroyEquipInLifeSupport()
        {
            Character character = new Character("Character", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);
            Character enemy = new Character("Enemy", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Knight);

            Equip equipRP = new Equip("Equip RP", Card.Rarity.Common, 0, Equip.TargetAttribute.RP, 1, Equip.Affinity.All);

            SupportSkill destroySkill = new SupportSkill("Destroy Skill", Card.Rarity.Common, 0, SupportSkill.EffectType.DestroyEquip, 10);

            player.Deck.AddCard(character);
            Console.WriteLine($"Deck cards: {player.Deck.Cards.Count}, lost: {player.Lost}");

            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Apply RP
            Console.WriteLine($"Apply Equip RP (+{equipRP.EffectPoints})");
            equipRP.ApplyToCharacter(character);
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Enemy attacks Character past base health, doesn't kill because of Equip RP
            Console.WriteLine($"Enemy ({enemy.AttackPoints} AP) attacks Character ({character.ResistPoints} RP)");
            Assert.IsFalse(enemy.Attack(character));
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            // Destroy Equip RP
            Console.WriteLine($"Destroy Equip RP (-{equipRP.EffectPoints})");
            Assert.IsTrue(destroySkill.UseSkill(null, player.Deck, character, equipRP));
            Console.WriteLine($"AP: {character.AttackPoints}, RP: {character.ResistPoints}");

            Assert.IsTrue(character.ResistPoints <= 0);
            Assert.IsTrue(player.Deck.Cards.Count == 0);
            Assert.IsTrue(player.Lost);

            Console.WriteLine($"Deck cards: {player.Deck.Cards.Count}, lost: {player.Lost}");
        }

        [Test]
        public void ReduceAPSkill()
        {
            FillDeckCharacters(50);

            SupportSkill reduceAPSkill = new SupportSkill("Reduce AP Skill", Card.Rarity.Common, 0, SupportSkill.EffectType.ReduceAP, 10);

            List<Character> characters = player.Deck.GetCharacters();

            int[] ap = new int[characters.Count];

            for (int i = 0; i < characters.Count; i++)
            {
                ap[i] = characters[i].AttackPoints;
            }

            Console.WriteLine($"Use Reduce AP Skill (-{reduceAPSkill.EffectPoints})");
            reduceAPSkill.UseSkill(null, player.Deck);

            for (int i = 0; i < characters.Count; i++)
            {
                Console.WriteLine($"{characters[i].Name}: {characters[i].AttackPoints} AP, {characters[i].ResistPoints} RP");
                Assert.IsTrue(characters[i].AttackPoints == Math.Max(ap[i] - reduceAPSkill.EffectPoints, 0));
            }
        }

        [Test]
        public void ReduceRPSkill()
        {
            FillDeckCharacters(50);

            SupportSkill reduceRPSkill = new SupportSkill("Reduce RP Skill", Card.Rarity.Common, 0, SupportSkill.EffectType.ReduceRP, 10);

            List<Character> characters = player.Deck.GetCharacters();

            int[] rp = new int[characters.Count];

            for (int i = 0; i < characters.Count; i++)
            {
                rp[i] = characters[i].ResistPoints;
            }

            Console.WriteLine($"Use Reduce RP Skill (-{reduceRPSkill.EffectPoints})");
            reduceRPSkill.UseSkill(null, player.Deck);

            for (int i = 0; i < characters.Count; i++)
            {
                Console.WriteLine($"{characters[i].Name}: {characters[i].AttackPoints} AP, {characters[i].ResistPoints} RP");
                Assert.IsTrue(characters[i].ResistPoints == Math.Max(rp[i] - reduceRPSkill.EffectPoints, 0));
            }
        }

        [Test]
        public void ReduceAllSkill()
        {
            FillDeckCharacters(50);

            SupportSkill reduceAllSkill = new SupportSkill("Reduce All Skill", Card.Rarity.Common, 0, SupportSkill.EffectType.ReduceAll, 10);

            List<Character> characters = player.Deck.GetCharacters();

            int[] ap = new int[characters.Count];
            int[] rp = new int[characters.Count];

            for (int i = 0; i < characters.Count; i++)
            {
                ap[i] = characters[i].AttackPoints;
                rp[i] = characters[i].ResistPoints;
            }

            Console.WriteLine($"Use Reduce All Skill (-{reduceAllSkill.EffectPoints})");
            reduceAllSkill.UseSkill(null, player.Deck);

            for (int i = 0; i < characters.Count; i++)
            {
                Console.WriteLine($"{characters[i].Name}: {characters[i].AttackPoints} AP, {characters[i].ResistPoints} RP");
                Assert.IsTrue(characters[i].AttackPoints == Math.Max(ap[i] - reduceAllSkill.EffectPoints, 0));
                Assert.IsTrue(characters[i].ResistPoints == Math.Max(rp[i] - reduceAllSkill.EffectPoints, 0));
            }
        }

        [Test]
        public void RestoreRPSkill()
        {
            FillDeckCharacters(50);

            Character enemy = new Character("Enemy", Card.Rarity.Common, 0, 10, 10, Character.Affinity.Undead);

            List<Character> characters = player.Deck.GetCharacters();

            List<int> rpOrigin = new List<int>(characters.Count);
            List<int> rp = new List<int>(characters.Count);

            for (int i = 0; i < characters.Count; i++)
            {
                Console.WriteLine($"Undead Enemy ({enemy.AttackPoints} AP) attacks {characters[i].Name} ({characters[i].ResistPoints} RP)");

                int originRP = characters[i].ResistPoints;

                if (!enemy.Attack(characters[i]))
                {
                    rpOrigin.Add(originRP);
                    rp.Add(characters[i].ResistPoints);
                }

                Console.WriteLine($"{characters[i].Name} RP: {characters[i].ResistPoints}");
            }

            SupportSkill restoreRPSkill = new SupportSkill("Restore RP Skill", Card.Rarity.Common, 0, SupportSkill.EffectType.RestoreRP, 10);

            Console.WriteLine($"\nUse Restore RP Skill (+{restoreRPSkill.EffectPoints})");
            restoreRPSkill.UseSkill(player.Deck, null);

            characters = player.Deck.GetCharacters();

            for (int i = 0; i < characters.Count; i++)
            {
                Console.WriteLine($"{characters[i].Name}: {characters[i].AttackPoints} AP, {characters[i].ResistPoints} RP");
                Assert.IsTrue(characters[i].ResistPoints == Math.Min(rp[i] + restoreRPSkill.EffectPoints, rpOrigin[i]));
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

                    card = new Character("Random Character", rarity, costPoints, random.Next(1, 10), random.Next(1, 10), GetRandomEnum<Character.Affinity>());
                    break;

                case 1:
                    rarity = (Card.Rarity)GetRandomIntWithProbability(20, 7, 2.5f, 0.5f);

                    card = new Equip("Random Equip", rarity, costPoints, GetRandomEnum<Equip.TargetAttribute>(), random.Next(1, 10), GetRandomEnum<Equip.Affinity>());
                    break;

                case 2:
                    rarity = (Card.Rarity)GetRandomIntWithProbability(37, 10, 2.5f, 0.5f);

                    card = new SupportSkill("Random Support", rarity, costPoints, GetRandomEnum<SupportSkill.EffectType>(), random.Next(1, 10));
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

        private void FillDeckCharacters(int maxStats)
        {
            for (int i = 0; i < Deck.maxCharacters; i++)
            {
                Character.Affinity affinity = GetRandomEnum<Character.Affinity>();
                Character character = new Character($"Char{i + 1}_{affinity}", Character.Rarity.Common, 0, random.Next(1, maxStats), random.Next(1, maxStats), affinity);

                Console.WriteLine($"Add {character.Name} ({character.AttackPoints} AP, {character.ResistPoints} RP)");
                player.Deck.AddCard(character);
            }

            Console.WriteLine();
        }

        #endregion

    }
}