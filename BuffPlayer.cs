﻿using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace PathOfModifiers
{
    public class BuffPlayer : ModPlayer
    {
        TimedValueInstanceCollection timedValueInstanceCollection;

        int staticStrikeDamage;
        int staticStrikeIntervalTicks;
        int staticStrikeCurrentInterval;
        int staticStrikeTimeLeft;

        float moltenShellDustAngle;
        int moltenShellStoredDamage;
        int moltenShellTimeLeft;

        int noManaCostTimeLeft;
        int knockbackImmunityTimeLeft;

        float weaponMoveSpeedValue;
        float weaponMoveSpeedTimeLeft;

        float greavesMoveSpeedValue;
        float greavesMoveSpeedTimeLeft;

        public override void Initialize()
        {
            timedValueInstanceCollection = new TimedValueInstanceCollection();
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            damage = ShockModifyDamageTaken(damage);
            return true;
        }
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (moltenShellTimeLeft > 0)
            {
                moltenShellStoredDamage += (int)damage;
            }
        }
        public override void ResetEffects()
        {
            timedValueInstanceCollection.ResetEffects();
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void PostUpdateEquips()
        {
            var affixPlayer = Player.GetModPlayer<ItemPlayer>();
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.DodgeChance), out var dodgeChances))
            {
                affixPlayer.dodgeChance += dodgeChances.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.MoveSpeed), out var moveSpeeds))
            {
                affixPlayer.moveSpeed += moveSpeeds.totalValue;
            }

            if (greavesMoveSpeedTimeLeft > 0)
            {
                affixPlayer.moveSpeed += greavesMoveSpeedValue;
            }

            if (weaponMoveSpeedTimeLeft > 0)
            {
                affixPlayer.moveSpeed += weaponMoveSpeedValue;
            }

            if (moltenShellTimeLeft > 0)
            {
                affixPlayer.damageTaken += -0.1f;

                if (Main.GameUpdateCount % 2 == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dustPosition = Player.Center + new Vector2(0, 32).RotatedBy(moltenShellDustAngle + (2.095f * i));
                        Dust.NewDustPerfect(dustPosition, DustType<Dusts.MoltenShell>(), Velocity: Vector2.Zero, Scale: 2f);
                        Dust.NewDustPerfect(dustPosition, DustType<Dusts.FireDebris>(), Scale: 1.5f);
                    }

                    moltenShellDustAngle += 0.2f;
                }
            }

            if (knockbackImmunityTimeLeft > 0)
            {
                Player.noKnockback = true;
            }
        }
        public override void UpdateBadLifeRegen()
        {
            float totalDPS = 0;

            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Bleed), out var bleeds))
            {
                totalDPS += bleeds.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Poison), out var poisons))
            {
                totalDPS += poisons.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Ignite), out var ignites))
            {
                totalDPS += ignites.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.BurningAir), out var burningAirs))
            {
                int maxDps = (int)Math.Round(Player.statLifeMax2 * 0.01f);
                totalDPS += Math.Min(burningAirs.totalValue, maxDps);
            }


            if (totalDPS > 0)
            {
                int totalDamage = (int)Math.Round(totalDPS * Buffs.DamageOverTime.damageMultiplierHalfSecond);
                Player.lifeRegenTime = 0;
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }
                Player.lifeRegen -= totalDamage;
            }
        }
        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (removeDebuffs)
            {
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Bleed));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Poison));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Shock));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Ignite));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Chill));
            }
        }
        public override void PreUpdate()
        {
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Bleed), out var bleeds))
            {
                if (bleeds.totalValue > 0)
                {
                    PoMEffectHelper.Bleed(Player.Center);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Poison), out var poisons))
            {
                if (poisons.totalValue > 0)
                {
                    PoMEffectHelper.Poison(Player.position, Player.width, Player.height);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Ignite), out var ignites))
            {
                if (ignites.totalValue > 0)
                {
                    PoMEffectHelper.Ignite(Player.position, Player.width, Player.height);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Shock), out var shocks))
            {
                if (shocks.totalValue > 0)
                {
                    PoMEffectHelper.Shock(Player.position, Player.width, Player.height);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Chill), out var chills))
            {
                if (chills.totalValue > 0)
                {
                    PoMEffectHelper.Chill(Player.position, Player.width, Player.height);
                }
            }

            if (staticStrikeTimeLeft > 0)
            {
                staticStrikeCurrentInterval++;

                if (staticStrikeCurrentInterval >= staticStrikeIntervalTicks)
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            position: Player.Center,
                            velocity: Vector2.Zero,
                            Type: ProjectileType<StaticStrike>(),
                            Damage: staticStrikeDamage,
                            KnockBack: 0,
                            Owner: Player.whoAmI);
                    }

                    staticStrikeCurrentInterval = 0;
                }

                staticStrikeTimeLeft--;
            }

            if (greavesMoveSpeedTimeLeft > 0)
            {
                if (greavesMoveSpeedValue > 0)
                {
                    PoMEffectHelper.MoveSpeed(Player);
                }

                greavesMoveSpeedTimeLeft--;
            }

            if (weaponMoveSpeedTimeLeft > 0)
            {
                if (weaponMoveSpeedTimeLeft > 0)
                {
                    PoMEffectHelper.MoveSpeed(Player);
                }

                weaponMoveSpeedTimeLeft--;
            }

            if (moltenShellTimeLeft > 0)
            {
                moltenShellTimeLeft--;

                if (moltenShellTimeLeft <= 0)
                {
                    PlayMoltenShellExplodeSound(Player);

                    if (Player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            Player.Center,
                            Vector2.Zero,
                            ProjectileType<MoltenShellExplosion>(),
                            moltenShellStoredDamage,
                            5,
                            Player.whoAmI,
                            moltenShellStoredDamage / (float)Player.statLifeMax2);
                        moltenShellStoredDamage = 0;
                    }
                }
            }

            if (noManaCostTimeLeft > 0)
            {
                noManaCostTimeLeft--;
            }
            if (knockbackImmunityTimeLeft > 0)
            {
                knockbackImmunityTimeLeft--;
            }
        }
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (noManaCostTimeLeft > 0)
            {
                mult = -10f;
            }
        }

        public void AddBleedBuff(Player player, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Bleed), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddBleedBuffPlayer(player.whoAmI, dps, durationTicks);
            }
        }
        public void AddPoisonBuff(Player player, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Poison), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddPoisonBuffPlayer(player.whoAmI, dps, durationTicks);
            }
        }
        public void AddMoveSpeedBuff(Player player, float speedBoost, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.MoveSpeed), speedBoost, durationTicks);

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                BuffPacketHandler.SendAddMoveSpeedBuffPlayer(player.whoAmI, speedBoost, durationTicks);
            }
        }
        public void AddWeaponMoveSpeedBuff(Player player, float speedBoost, int durationTicks, bool syncMP = true)
        {
            weaponMoveSpeedValue = speedBoost;
            weaponMoveSpeedTimeLeft = durationTicks;

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                BuffPacketHandler.SendAddWeaponMoveSpeedBuffPlayer(player.whoAmI, speedBoost, durationTicks);
            }
        }
        public void AddGreavesMoveSpeedBuff(Player player, float speedBoost, int durationTicks, bool syncMP = true)
        {
            greavesMoveSpeedValue = speedBoost;
            greavesMoveSpeedTimeLeft = durationTicks;

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                BuffPacketHandler.SendAddGreavesMoveSpeedBuffPlayer(player.whoAmI, speedBoost, durationTicks);
            }
        }
        public void AddBurningAirBuff(Player player, int dps)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.BurningAir), dps, 2);
        }
        public void AddIgnitedBuff(Player player, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Ignite), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddIgnitedBuffPlayer(player.whoAmI, dps, durationTicks);
            }
        }
        public void AddShockedAirBuff(Player player, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ShockedAir), multiplier, 2);
        }
        public void AddShockedBuff(Player player, float multiplier, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Shock), multiplier, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddShockedBuffPlayer(player.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledBuff(Player player, float multiplier, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Chill), multiplier, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddChilledBuffPlayer(player.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledAirBuff(Player player, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ChilledAir), multiplier, 2);
        }
        public void AddStaticStrikeBuff(Player player, int damage, int intervalTicks, int durationTicks, bool syncMP = true)
        {
            if (staticStrikeTimeLeft <= 0)
            {
                PlayGainStaticStrikeSound(player);
                staticStrikeCurrentInterval = 0;
            }
            staticStrikeDamage = damage;
            staticStrikeIntervalTicks = intervalTicks;
            staticStrikeTimeLeft = durationTicks;

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddStaticStrikeBuffPlayer(player.whoAmI, damage, intervalTicks, durationTicks);
            }
        }
        public void AddDodgeChanceBuff(Player player, float chance, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.DodgeChance), chance, durationTicks);

            if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
            {
                BuffPacketHandler.SendAddDodgeChanceBuffPlayer(player.whoAmI, chance, durationTicks);
            }
        }
        public void AddNoManaCostBuff(Player player, int durationTicks, bool syncMP = true)
        {
            if (durationTicks > noManaCostTimeLeft)
            {
                noManaCostTimeLeft = durationTicks;

                if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    BuffPacketHandler.SendAddNoManaCostBuffPlayer(player.whoAmI, durationTicks);
                }
            }

        }
        public void AddKnockbackImmunityBuff(Player player, int durationTicks, bool syncMP = true)
        {
            if (durationTicks > knockbackImmunityTimeLeft)
            {
                knockbackImmunityTimeLeft = durationTicks;

                if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    BuffPacketHandler.SendAddKnockbackImmunityBuffPlayer(player.whoAmI, durationTicks);
                }
            }
        }
        public void AddMoltenShellBuff(Player player, int durationTicks, bool syncMP = true)
        {
            if (durationTicks > moltenShellTimeLeft)
            {
                PlayGainMoltenShellSound(player);

                moltenShellTimeLeft = durationTicks;

                if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    BuffPacketHandler.SendAddMoltenShellBuffPlayer(player.whoAmI, durationTicks);
                }
            }
        }

        public int ShockModifyDamageTaken(int damage)
        {
            float totalMultiplier = 1;

            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Shock), out var shocks))
            {
                totalMultiplier += shocks.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.ShockedAir), out var shockedAirs))
            {
                totalMultiplier += shockedAirs.totalValue;
            }

            return (int)Math.Round(damage * totalMultiplier);
        }
        public int ChillModifyDamageDealt(int damage)
        {
            float totalMultiplier = 1;

            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Chill), out var chills))
            {
                totalMultiplier += chills.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.ChilledAir), out var chilledAirs))
            {
                totalMultiplier += chilledAirs.totalValue;
            }

            return (int)Math.Round(damage * totalMultiplier);
        }

        void PlayGainStaticStrikeSound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item92.WithVolumeScale(1f).WithPitchOffset(0.3f), player.Center);
        }
        void PlayGainMoltenShellSound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item45.WithVolumeScale(1f).WithPitchOffset(0.3f), player.Center);
        }
        void PlayMoltenShellExplodeSound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item62.WithVolumeScale(1f).WithPitchOffset(0.3f), player.Center);
        }
    }
}