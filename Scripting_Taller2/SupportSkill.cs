using System;
using System.Collections.Generic;
using System.Text;

namespace Scripting_Taller2
{
    public class SupportSkill : Card
    {
        public enum EffectType
        {
            ReduceAP,
            ReduceRP,
            ReduceAll,
            DestroyEquip,
            RestoreRP
        }

        private EffectType effectType;
        private int effectPoints;

        public SupportSkill(string name, Rarity rarity, int costPoints, EffectType effectType, int effectPoints) : base(name, rarity, costPoints)
        {
            this.effectType = effectType;
            this.effectPoints = effectPoints;
        }

        public void UseSkill()
        {
            RemoveFromDeck();
        }
    }
}