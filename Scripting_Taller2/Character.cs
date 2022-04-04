using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting_Taller2
{
    public class Character : Card
    {
        public enum Affinity
        {
            Knight,
            Mage,
            Undead
        }

        public const int maxEquips = 3;

        private int attackPoints;
        private int resistPoints;
        private List<Equip> equips = new List<Equip>();
        private Affinity affinity;

        private int restorableRP;

        public Affinity GetAffinity => affinity;
        public int AttackPoints => attackPoints;
        public int ResistPoints => resistPoints;

        public Character(string name, Rarity rarity, int costPoints, int attackPoints, int resistPoints, Affinity affinity) : base(name, rarity, costPoints)
        {
            this.attackPoints = attackPoints;
            this.resistPoints = resistPoints;
            this.affinity = affinity;
        }

        public bool Attack(Character character)
        {
            int affinityBonus = EvaluateAffinity(character.affinity);

            return !character.Resist(attackPoints + affinityBonus);
        }

        public bool Resist(int attackPoints)
        {
            resistPoints -= attackPoints;
            restorableRP += attackPoints;

            // Check death
            if (resistPoints <= 0)
            {
                RemoveFromDeck();
                return false;
            }

            return true;
        }

        internal bool AddEquip(Equip equip)
        {
            if (equips.Count >= maxEquips) return false;

            // Apply equip stats
            switch (equip.GetTargetAttribute)
            {
                case Equip.TargetAttribute.AP:
                    attackPoints += equip.EffectPoints;
                    break;

                case Equip.TargetAttribute.RP:
                    resistPoints += equip.EffectPoints;
                    break;

                case Equip.TargetAttribute.All:
                    attackPoints += equip.EffectPoints;
                    resistPoints += equip.EffectPoints;
                    break;
            }

            equips.Add(equip);

            return true;
        }

        internal bool DestroyEquip(Equip equip)
        {
            if (!equips.Contains(equip)) return false;

            // Deduct equip stats
            switch (equip.GetTargetAttribute)
            {
                case Equip.TargetAttribute.AP:
                    attackPoints -= equip.EffectPoints;
                    break;

                case Equip.TargetAttribute.RP:
                    resistPoints -= equip.EffectPoints;
                    break;

                case Equip.TargetAttribute.All:
                    attackPoints -= equip.EffectPoints;
                    resistPoints -= equip.EffectPoints;
                    break;
            }

            equips.Remove(equip);

            // Check death
            if (resistPoints <= 0)
            {
                RemoveFromDeck();
            }

            return true;
        }

        public void ReduceAP(int amount)
        {
            attackPoints -= amount;
            attackPoints = Math.Max(attackPoints, 0);
        }

        public void ReduceRP(int amount)
        {
            resistPoints -= amount;
            resistPoints = Math.Max(resistPoints, 0);
        }

        public void RestoreRP(int amount)
        {
            amount = Math.Min(amount, restorableRP);

            resistPoints += amount;
            restorableRP -= amount;
        }

        private int EvaluateAffinity(Affinity targetAffinity)
        {
            if (targetAffinity == affinity) return 0;

            int nextAffinity = (int)affinity + 1;
            if (nextAffinity == 3) nextAffinity = 0;

            return targetAffinity == (Affinity)nextAffinity ? 2 : -2;
        }
    }
}