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

        private const int maxEquips = 3;

        private int attackPoints;
        private int resistPoints;
        private List<Equip> equips = new List<Equip>();
        private Affinity affinity;

        private int restorableRP;

        public Affinity GetAffinity => affinity;

        public Character(string name, Rarity rarity, int costPoints, int attackPoints, int resistPoints, Affinity affinity) : base(name, rarity, costPoints)
        {
            this.attackPoints = attackPoints;
            this.resistPoints = resistPoints;
            this.affinity = affinity;
        }

        public void Attack(Character character)
        {
            character.Resist(attackPoints);
        }

        public void Resist(int attackPoints)
        {
            resistPoints -= attackPoints;
            restorableRP += attackPoints;

            // Check death
            if (resistPoints <= 0)
            {
                RemoveFromDeck();
            }
        }

        public void EquipItem(Equip equip)
        {
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

            if (equips.Count < maxEquips) equips.Add(equip);
        }

        public void DestroyEquip(Equip equip)
        {
            if (equips.Contains(equip))
            {
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
            }
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

            restorableRP -= amount;
        }
    }
}