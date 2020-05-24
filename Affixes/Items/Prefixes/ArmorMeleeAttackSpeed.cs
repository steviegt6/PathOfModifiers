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
    public class ArmorMeleeAttackSpeed : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.7f, 0.5),
                new TTFloat.WeightedTier(0.8f, 1.2),
                new TTFloat.WeightedTier(0.9f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.1f, 1),
                new TTFloat.WeightedTier(1.2f, 0.5),
                new TTFloat.WeightedTier(1.3f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Sluggish", 4),
            new WeightedTierName("Slow", 2),
            new WeightedTierName("Lazy", 0.5),
            new WeightedTierName("Nimble", 0.5),
            new WeightedTierName("Agile", 2),
            new WeightedTierName("Fleeting", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsLegArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% melee attack speed";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.meleeSpeed += Type1.GetValue() - 1;
        }
    }
}
