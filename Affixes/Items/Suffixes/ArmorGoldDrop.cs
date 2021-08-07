﻿using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using Terraria.ID;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ArmorGoldDrop : AffixTiered<TTFloat, TTInt>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
    {
                new TTFloat.WeightedTier(0, 3),
                new TTFloat.WeightedTier(0.005f, 2.5),
                new TTFloat.WeightedTier(0.01f, 2),
                new TTFloat.WeightedTier(0.015f, 1.5),
                new TTFloat.WeightedTier(0.02f, 1),
                new TTFloat.WeightedTier(0.025f, 0.5),
                new TTFloat.WeightedTier(0.03f, 0),
    },
        };
        public override TTInt Type2 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(1, 3),
                new TTInt.WeightedTier(3, 2.5),
                new TTInt.WeightedTier(5, 2),
                new TTInt.WeightedTier(7, 1.5),
                new TTInt.WeightedTier(9, 1),
                new TTInt.WeightedTier(11, 0.5),
                new TTInt.WeightedTier(13, 0)
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Gold", 0.5),
            new WeightedTierName("of Prosperity", 1),
            new WeightedTierName("of Riches", 1.5),
            new WeightedTierName("of Fortune", 2),
            new WeightedTierName("of Treasure", 2.5),
            new WeightedTierName("of Midas", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to drop {Type2.GetValueFormat()} gold on kill";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.GetModPlayer<ItemPlayer>().goldDropChances.AddOrUpdate(this, Type1.GetValue(), Type2.GetValue());
        }
    }
}