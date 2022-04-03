using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting_Taller2
{
    public class Equip : Card
    {
        public enum TargetAttribute
        {
            AP,
            RP,
            All
        }

        public enum Affinity
        {
            Knight,
            Mage,
            Undead,
            All
        }

        private TargetAttribute targetAttribute;
        private int effectPoints;
        private Affinity affinity;

        public TargetAttribute GetTargetAttribute => targetAttribute;
        public int EffectPoints => effectPoints;

        public Equip(string name, Rarity rarity, int costPoints, TargetAttribute targetAttribute, int effectPoints, Affinity affinity) : base(name, rarity, costPoints)
        {
            this.targetAttribute = targetAttribute;
            this.effectPoints = effectPoints;
            this.affinity = affinity;
        }

        public void UseOnCharacter(Character character)
        {
            if (affinity == Affinity.All || (Affinity)(int)character.GetAffinity == affinity)
            {
                character.EquipItem(this);

                RemoveFromDeck();
            }
        }
    }
}