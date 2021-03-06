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

        public int EffectPoints => effectPoints;

        public SupportSkill(string name, Rarity rarity, int costPoints, EffectType effectType, int effectPoints) : base(name, rarity, costPoints)
        {
            this.effectType = effectType;
            this.effectPoints = effectType == EffectType.DestroyEquip ? 0 : effectPoints;
        }

        public bool UseSkill(Deck deck, Deck opponentDeck, Character targetCharacter = null, Equip targetEquip = null)
        {
            switch (effectType)
            {
                case EffectType.ReduceAP:
                    opponentDeck.ReduceAP(effectPoints);
                    break;

                case EffectType.ReduceRP:
                    opponentDeck.ReduceRP(effectPoints);
                    break;

                case EffectType.ReduceAll:
                    opponentDeck.ReduceAP(effectPoints);
                    opponentDeck.ReduceRP(effectPoints);
                    break;

                case EffectType.DestroyEquip:
                    return targetCharacter.DestroyEquip(targetEquip);

                case EffectType.RestoreRP:
                    deck.RestoreRP(effectPoints);
                    break;
            }

            // Consume skill
            RemoveFromDeck();

            return true;
        }
    }
}