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

        public Character(string name, Rarity rarity, int costPoints, int attackPoints, int resistPoints, Affinity affinity) : base(name, rarity, costPoints)
        {
            this.attackPoints = attackPoints;
            this.resistPoints = resistPoints;
            this.affinity = affinity;
        }

        public void Attack(Character character)
        {
            int attackBonus = 0;

            character.Resist(attackPoints + attackBonus);
        }

        public void Resist(int attack)
        {
            int defenseBonus = 0;

            attack -= defenseBonus;
            attack = Math.Max(attack, 0);

            resistPoints -= attack;

            if (resistPoints <= 0)
            {
                RemoveFromDeck();
            }
        }

        public void EquipItem(Equip equip)
        {
            if (equips.Count < maxEquips) equips.Add(equip);
        }
    }
}