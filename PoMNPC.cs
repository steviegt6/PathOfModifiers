﻿using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace PathOfModifiers
{
    public class PoMNPC : GlobalNPC
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            var isHardmode = new LeadingConditionRule(new Conditions.IsHardmode());
            isHardmode.OnSuccess(
                new CommonDrop(
                    ItemType<Items.ModifierFragment>(),
                    1,
                    PoMGlobals.DropRate.Fragment.fromBossHardmode,
                    PoMGlobals.DropRate.Fragment.fromBossHardmode));
            isHardmode.OnFailedConditions(
                new CommonDrop(
                    ItemType<Items.ModifierFragment>(),
                    1,
                    PoMGlobals.DropRate.Fragment.fromBoss,
                    PoMGlobals.DropRate.Fragment.fromBoss));

            var isPostPlantera = new LeadingConditionRule(new Conditions.DownedPlantera());
            isPostPlantera.OnSuccess(
                new CommonDrop(
                    ItemType<Items.ModifierFragment>(),
                    1,
                    PoMGlobals.DropRate.Fragment.fromBossPostPlantera,
                    PoMGlobals.DropRate.Fragment.fromBossPostPlantera));
            isPostPlantera.OnFailedConditions(isHardmode);

            var isBoss = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
            isBoss.OnSuccess(isPostPlantera);
            isBoss.OnFailedConditions(
                new PoMGlobals.DropRate.CommonDropScalingWithValue(
                    ItemType<Items.ModifierFragment>(),
                    PoMGlobals.DropRate.Fragment.chanceDenominator,
                    PoMGlobals.DropRate.Fragment.baseMin,
                    PoMGlobals.DropRate.Fragment.baseMax,
                    PoMGlobals.DropRate.Fragment.multiplyPerValue));

            globalLoot.Add(isBoss);
        }
        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Wizard)
            {
                shop.item[nextSlot].SetDefaults(ItemType<Items.ModifierFragment>());
                nextSlot++;
            }
        }
    }
}