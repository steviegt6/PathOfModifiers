﻿using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponDamagePerEnemyBuff : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.20f, -0.15f, 0.5),
                new TTFloat.WeightedTier(-0.15f, -0.10f, 1),
                new TTFloat.WeightedTier(-0.10f, -0.05f, 2),
                new TTFloat.WeightedTier(0.05f, 0.10f, 2),
                new TTFloat.WeightedTier(0.10f, 0.15f, 1),
                new TTFloat.WeightedTier(0.15f, 0.20f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Precipitating", 3),
            new WeightedTierName("Expediting", 2),
            new WeightedTierName("Urging", 0.5),
            new WeightedTierName("Impeding", 0.5),
            new WeightedTierName("Styming", 2),
            new WeightedTierName("Thwarthing", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% damage per enemy buff/debuff";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            float value = Type1.GetValue();
            damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            float value = Type1.GetValue();
            damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            if (player.HeldItem == item)
            {
                float value = Type1.GetValue();
                damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
            }
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (player.HeldItem == item)
            {
                float value = Type1.GetValue();
                damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
            }
        }
    }
}
