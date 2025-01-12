﻿using Microsoft.Xna.Framework;
using PathOfModifiers.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class HelmetReflectNova : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
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
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.006f, 0.010f, 3),
                new TTFloat.WeightedTier(0.010f, 0.014f, 2.5),
                new TTFloat.WeightedTier(0.014f, 0.018f, 2),
                new TTFloat.WeightedTier(0.018f, 0.022f, 1.5),
                new TTFloat.WeightedTier(0.022f, 0.026f, 1),
                new TTFloat.WeightedTier(0.026f, 0.030f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(6f, 5f, 3),
                new TTFloat.WeightedTier(5f, 4f, 2.5),
                new TTFloat.WeightedTier(4f, 3f, 2),
                new TTFloat.WeightedTier(3f, 2f, 1.5),
                new TTFloat.WeightedTier(2f, 1f, 1),
                new TTFloat.WeightedTier(1f, 0f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Bristles", 0.5),
            new WeightedTierName("of Barbs", 1),
            new WeightedTierName("or Thorns", 1.5),
            new WeightedTierName("of Brambles", 2),
            new WeightedTierName("of Spikes", 2.5),
            new WeightedTierName("of Reflect", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsHeadArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(1), Type3.GetMinValueFormat(1), Type3.GetMaxValueFormat(1), useChatTags);
            return $"Nova for { valueRange1 }% of damage taken + { valueRange2 }% of target's HP as damage when hit ({ valueRange3 }s CD)";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            SpawnNova(item, player, damage);
        }
        public override void OnHitByProjectile(Item item, Player player, Projectile projectile, int damage, bool crit)
        {
            SpawnNova(item, player, damage);
        }
        public override void OnHitByPvp(Item item, Player player, Player attacker, int damage, bool crit)
        {
            SpawnNova(item, player, damage);
        }

        void SpawnNova(Item item, Player player, int damageTaken)
        {
            if (ItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type3.GetValue() * 60))
            {
                PlaySound(player);

                Projectile.NewProjectile(
                    player.GetSource_FromThis(),
                    //player.GetSource_FromThis(),
                    player.Center, Vector2.Zero, ModContent.ProjectileType<ReflectNova>(), (int)Math.Round(damageTaken * Type1.GetValue()), 0, player.whoAmI, Type2.GetValue());

                lastProcTime = Main.GameUpdateCount;
            }
        }

        void PlaySound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item74.WithVolumeScale(0.5f).WithPitchOffset(0.3f), player.Center);
        }

        public override Affix Clone()
        {
            var affix = (HelmetReflectNova)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}