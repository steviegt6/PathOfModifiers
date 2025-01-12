﻿using System;
using Terraria;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryManaSpend : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.1f, 0.09f, 3),
                new TTFloat.WeightedTier(0.09f, 0.08f, 2.5),
                new TTFloat.WeightedTier(0.08f, 0.07f, 2),
                new TTFloat.WeightedTier(0.07f, 0.06f, 1.5),
                new TTFloat.WeightedTier(0.06f, 0.05f, 1),
                new TTFloat.WeightedTier(0.05f, 0.04f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.010f, -0.025f, 3),
                new TTFloat.WeightedTier(-0.025f, -0.040f, 2.5),
                new TTFloat.WeightedTier(-0.040f, -0.055f, 2),
                new TTFloat.WeightedTier(-0.055f, -0.070f, 1.5),
                new TTFloat.WeightedTier(-0.070f, -0.085f, 1),
                new TTFloat.WeightedTier(-0.085f, -0.100f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Simplicity", 0.5),
            new WeightedTierName("of Stupidity", 1),
            new WeightedTierName("of Nonsense", 1.5),
            new WeightedTierName("of Absurdity", 2),
            new WeightedTierName("of Puerility", 2.5),
            new WeightedTierName("of Lunacy", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            return $"Spend { valueRange1 }% mana to reduce damage taken by { valueRange2 }%";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (ItemItem.IsAccessoryEquipped(item, player) && TryConsumeMana(player))
            {
                damageMultiplier += Type2.GetValue();
            }

            return true;
        }

        bool TryConsumeMana(Player player)
        {
            int amount = (int)Math.Round(player.statManaMax2 * Type1.GetValue());
            return player.CheckMana(amount, true);
        }
    }
}