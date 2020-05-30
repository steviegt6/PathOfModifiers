﻿using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorBlockChance : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
    {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.005f, 2.5),
                new TTFloat.WeightedTier(0.01f, 2),
                new TTFloat.WeightedTier(0.015f, 1.5),
                new TTFloat.WeightedTier(0.02f, 1),
                new TTFloat.WeightedTier(0.025f, 0.5),
                new TTFloat.WeightedTier(0.03f, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Covering", 0.5),
            new WeightedTierName("Blocking", 1),
            new WeightedTierName("Fending", 1.5),
            new WeightedTierName("Shielded", 2),
            new WeightedTierName("Secure", 2.5),
            new WeightedTierName("Walled", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to block damage";
        }

        public override void UpdateEquip(Item item, AffixItemPlayer player)
        {
            player.blockChance += Type1.GetValue();
        }
    }
}
