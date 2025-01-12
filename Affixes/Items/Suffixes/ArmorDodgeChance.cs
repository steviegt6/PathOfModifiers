﻿using PathOfModifiers.UI.Chat;
using System;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ArmorDodgeChance : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.03f, 0.05f, 3),
                new TTFloat.WeightedTier(0.05f, 0.07f, 2.5),
                new TTFloat.WeightedTier(0.07f, 0.09f, 2),
                new TTFloat.WeightedTier(0.09f, 0.11f, 1.5),
                new TTFloat.WeightedTier(0.11f, 0.13f, 1),
                new TTFloat.WeightedTier(0.13f, 0.15f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1.0f, 1.5f, 3),
                new TTFloat.WeightedTier(1.5f, 2.0f, 2.5),
                new TTFloat.WeightedTier(2.0f, 2.5f, 2),
                new TTFloat.WeightedTier(2.5f, 3.0f, 1.5),
                new TTFloat.WeightedTier(3.0f, 3.5f, 1),
                new TTFloat.WeightedTier(3.5f, 4.0f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(7f, 6f, 3),
                new TTFloat.WeightedTier(6f, 5f, 2.5),
                new TTFloat.WeightedTier(5f, 4f, 2),
                new TTFloat.WeightedTier(4f, 3f, 1.5),
                new TTFloat.WeightedTier(3f, 2f, 1),
                new TTFloat.WeightedTier(2f, 1f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Evasion", 0.5),
            new WeightedTierName("of Dodging", 1),
            new WeightedTierName("of Elusion", 1.5),
            new WeightedTierName("of Acrobat", 2),
            new WeightedTierName("of Blur", 2.5),
            new WeightedTierName("of Ghost", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(1), Type2.GetMinValueFormat(1), Type2.GetMaxValueFormat(1), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(1), Type3.GetMinValueFormat(1), Type3.GetMaxValueFormat(1), useChatTags);
            return $"Gain { valueRange1 }% { Keyword.GetTextOrTag(KeywordType.Dodge, useChatTags) } chance for { valueRange2 }s when hit ({ valueRange3 }s CD)";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            GainDodgeChance(item, player);
        }

        void GainDodgeChance(Item item, Player player)
        {
            if (ItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type3.GetValue() * 60))
            {
                int durationTicks = (int)Math.Round((Type2.GetValue() * 60));
                player.GetModPlayer<BuffPlayer>().AddDodgeChanceBuff(player, Type1.GetValue(), durationTicks, false);
                lastProcTime = Main.GameUpdateCount;
            }
        }

        public override Affix Clone()
        {
            var affix = (ArmorDodgeChance)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}