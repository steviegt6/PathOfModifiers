﻿using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryDefense : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-3, -2, 0.5),
                new TTInt.WeightedTier(-2, -1, 1),
                new TTInt.WeightedTier(-1, 0, 2),
                new TTInt.WeightedTier(1, 2, 2),
                new TTInt.WeightedTier(2, 3, 1),
                new TTInt.WeightedTier(3, 4, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Soft", 3),
            new WeightedTierName("Loose", 2),
            new WeightedTierName("Weakened", 0.5),
            new WeightedTierName("Studded", 0.5),
            new WeightedTierName("Layered", 2),
            new WeightedTierName("Reinforced", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 } defense";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.statDefense += Type1.GetValue();
        }
    }
}
