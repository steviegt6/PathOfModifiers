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

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    //Doesn't work with pvp, no hook. alternatively use on hit pvp hook
    public class AccessoryReflect : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.083f, 2.5),
                new TTFloat.WeightedTier(0.166f, 2),
                new TTFloat.WeightedTier(0.25f, 1.5),
                new TTFloat.WeightedTier(0.333f, 1),
                new TTFloat.WeightedTier(0.416f, 0.5),
                new TTFloat.WeightedTier(0.5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Prickly", 0.5),
            new WeightedTierName("Thorny", 1),
            new WeightedTierName("Barbed", 1.5),
            new WeightedTierName("Spiky", 2),
            new WeightedTierName("Bristly", 2.5),
            new WeightedTierName("Spinous", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            float valueFormat = Type1.GetValueFormat();
            return $"{ valueFormat }% melee damage reflected";
        }

        public override void UpdateEquip(Item item, AffixItemPlayer player)
        {
            player.reflectMeleeDamage += Type1.GetValue();
        }
    }
}
