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

namespace PathOfModifiers.AffixesItem.Prefixes
{
    //Doesn't work with pvp, no hook.
    public class AccessoryReflect : Prefix, ITieredStatFloatAffix
    {
        public override float weight => 0.5f;

        public override string addedText => addedTextTiered;
        public override float addedTextWeight => addedTextWeightTiered;

        static float[] tiers = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
        static Tuple<int, double>[] tierWeights = new Tuple<int, double>[] {
            new Tuple<int, double>(0, 0.5),
            new Tuple<int, double>(1, 3),
            new Tuple<int, double>(2, 3),
            new Tuple<int, double>(3, 0.5),
        };
        static string[] tierNames = new string[] {
            "Prickly",
            "Barbed",
            "Spiky",
            "Spinous",
        };
        static int maxTier => tiers.Length - 2;

        int tierText => maxTier - tier + 1;

        int tier = 0;
        string addedTextTiered = string.Empty;
        float addedTextWeightTiered = 1;

        float tierMultiplier = 0;
        float multiplier = 0;


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsBodyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            float percent = multiplier * 100;
            int decimals = 0;
            if (percent < 1)
            {
                decimals = 2;
            }
            percent = (float)Math.Round(percent, decimals);
            return $"{percent}% melee damage reflected";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            if (PoMItem.IsAccessoryEquipped(item, player))
            {
                int reflectDamage = (int)Math.Round(damage * multiplier);
                float reflectKnockback = 0;
                int reflectDirection = 1;
                npc.StrikeNPC(reflectDamage, reflectKnockback, reflectDirection, crit);
            }
        }

        #region Interface Properties
        public float Weight => weight;
        public float[] Tiers => tiers;
        public Tuple<int, double>[] TierWeights => tierWeights;
        public string[] TierNames => tierNames;
        public int MaxTier => maxTier;
        public int TierText => tierText;
        public int Tier { get { return tier; } set { tier = value; } }
        public string AddedTextTiered { get { return AddedTextTiered; } set { addedTextTiered = value; } }
        public float AddedTextWeightTiered { get { return addedTextWeightTiered; } set { addedTextWeightTiered = value; } }
        public float TierMultiplier { get { return tierMultiplier; } set { tierMultiplier = value; } }
        public float Multiplier { get { return multiplier; } set { this.multiplier = value; } }
        #endregion
        #region Helped Methods
        void SetTier(int tier)
        {
            TieredAffixHelper.SetTier(this, tier);
        }
        void SetTierMultiplier(float tierMultiplier)
        {
            TieredAffixHelper.SetTierMultiplier(this, tierMultiplier);
        }
        public override Affix Clone()
        {
            return TieredAffixHelper.Clone(this, (ITieredStatFloatAffix)base.Clone());
        }
        public override void RollValue(bool rollTier = true)
        {
            TieredAffixHelper.RollValue(this, rollTier);
        }
        public override void ReforgePrice(Item item, ref int price)
        {
            TieredAffixHelper.ReforgePrice(this, item, ref price);
        }
        public override void Save(TagCompound tag, Item item)
        {
            TieredAffixHelper.Save(this, tag, item);
        }
        public override void Load(TagCompound tag, Item item)
        {
            TieredAffixHelper.Load(this, tag, item);
        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            TieredAffixHelper.NetSend(this, item, writer);
        }
        public override void NetReceive(Item item, BinaryReader reader)
        {
            TieredAffixHelper.NetReceive(this, item, reader);
        }
        public override string GetForgeText(Item item)
        {
            return TieredAffixHelper.GetForgeText(this, item);
        }
        #endregion
    }
}
