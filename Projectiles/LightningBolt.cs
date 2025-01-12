using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PathOfModifiers.Dusts;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Projectiles
{
    public class LightningBolt : ModProjectile, INonTriggerringProjectile
    {
        static Asset<Texture2D> textureGlow;

        static readonly Vector3 emittedLight = new Vector3(1, 0.952f, 0.552f);
        static readonly Vector2 minVariance = new Vector2(8f, 16f);
        const int baseTime = 60 * 2;    //* 2 because extra updates
        const float varianceHeight = 128;
        const float varianceYOffset = -8f;
        const float snapRadiusSqr = 64f * 64f;
        const float dustInterval = 5f;
        const float airRadius = 48f;

        List<Vector2> nodes = new List<Vector2>();
        Rectangle airRect;
        Rectangle boltRect;
        Vector2 targetPosition;
        float boundLeft;
        float boundRight;
        bool justWentUp;
        bool isCloud;
        bool init;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("LightningBolt");
        }
        public override void SetDefaults()
        {
            textureGlow = ModContent.Request<Texture2D>("PathOfModifiers/Projectiles/LightningBoltGlow");
            Projectile.penetrate = 1;
            Projectile.width = 32;
            Projectile.height = 6;
            Projectile.timeLeft = baseTime;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreAI()
        {
            if (!init)
            {
                nodes.Add(Projectile.Center);
                targetPosition = Projectile.position + new Vector2(0, Projectile.ai[1]);
                boltRect = new Rectangle(
                    (int)boundLeft,
                    (int)Projectile.position.Y,
                    Projectile.width,
                    (int)Projectile.ai[1]);
                airRect = new Rectangle(
                    (int)(targetPosition.X - airRadius),
                    (int)(targetPosition.Y - airRadius),
                    (int)(airRadius * 2),
                    (int)(airRadius * 2));
                float halfWidth = Projectile.width / 2f;
                boundLeft = targetPosition.X - halfWidth;
                boundRight = targetPosition.X + halfWidth - (minVariance.X * 2);
                init = true;
            }

            if (isCloud)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead)
                    {
                        Rectangle playerRect = player.getRect();
                        if (playerRect.Intersects(airRect))
                        {
                            player.GetModPlayer<BuffPlayer>().AddShockedAirBuff(player, Projectile.ai[0]);
                        }
                    }
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && (!npc.friendly || npc.townNPC) && !npc.dontTakeDamage)
                    {
                        Rectangle npcRect = npc.getRect();
                        if (npcRect.Intersects(airRect))
                        {
                            BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
                            pomNPC.AddShockedAirBuff(npc, Projectile.ai[0]);
                        }
                    }
                }

                Lighting.AddLight(targetPosition, emittedLight);
            }
            else
            {
                if (Projectile.position != targetPosition)
                {
                    float nextX = Main.rand.NextFloat(boundLeft, boundRight);
                    float diff = nextX - Projectile.position.X;
                    if (Math.Abs(diff) < minVariance.X)
                    {
                        nextX += minVariance.X * 2;
                    }

                    float velocityY = Main.rand.NextFloat(0, varianceHeight) + varianceYOffset;
                    if (velocityY < 0)
                    {
                        if (justWentUp)
                        {
                            velocityY = minVariance.Y;
                            justWentUp = false;
                        }
                        else if (velocityY > -minVariance.Y)
                        {
                            velocityY = -minVariance.Y;
                        }
                    }
                    else
                    {
                        justWentUp = false;
                    }
                    if (velocityY < 0)
                    {
                        justWentUp = true;
                    }

                    Vector2 position = new Vector2(nextX, Projectile.position.Y + velocityY);

                    if ((position.Y > targetPosition.Y ||
                        (targetPosition - position).LengthSquared() < snapRadiusSqr))
                    {
                        position = targetPosition;
                        Projectile.timeLeft = PoMGlobals.ailmentDuration;
                    }

                    Projectile.position = position;

                    nodes.Add(Projectile.Center);

                    foreach (var node in nodes)
                    {
                        Lighting.AddLight(node, emittedLight);
                    }
                }
                else
                {
                    Explode();
                }
            }


            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (isCloud)
            {
            }
            else
            {
                if (nodes.Count > 0)
                {
                    var glow = textureGlow.Value;
                    Vector2 glowPosition = nodes[0];
                    float glowLength = Projectile.ai[1];
                    Vector2 textureHalf = new Vector2(glow.Width / 2, glow.Height / 2);
                    Rectangle destination = new Rectangle(
                        (int)(glowPosition.X - Main.screenPosition.X),
                        (int)(glowPosition.Y - Main.screenPosition.Y),
                        glow.Width,
                        (int)glowLength);
                    var drawData = new DrawData(glow, destination, null, new Color(0.01f, 0.01f, 0.01f, 0.005f), 0, new Vector2(textureHalf.X, 0), SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(drawData);
                }

                for (int i = 1; i < nodes.Count; i++)
                {
                    Vector2 position1 = nodes[i - 1];
                    Vector2 position2 = nodes[i];
                    Vector2 velocity = position2 - position1;
                    Vector2 direction = velocity.SafeNormalize(Vector2.Zero);
                    Vector2 drawPosition = position1 - (direction * 5);
                    Vector2 drawPositionScreen = drawPosition - Main.screenPosition;
                    float drawLength = velocity.Length() + 5;
                    float rotation = velocity.ToRotation() - MathHelper.PiOver2;

                    Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                    Vector2 textureHalf = new Vector2(texture.Width / 2, texture.Height / 2);
                    Rectangle destination = new Rectangle(
                        (int)(drawPositionScreen.X),
                        (int)(drawPositionScreen.Y),
                        texture.Width,
                        (int)drawLength);
                    var drawData = new DrawData(texture, destination, null, new Color(1f, 1f, 1f, 0.9f), rotation, new Vector2(textureHalf.X, 0), SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(drawData);
                }
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override bool CanHitPvp(Player target)
        {
            return false;
        }

        public void Explode()
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                Vector2 position1 = nodes[i - 1];
                Vector2 position2 = nodes[i];
                Vector2 velocity = position2 - position1;
                Vector2 direction = velocity.SafeNormalize(Vector2.Zero);
                int howMany = (int)(velocity.Length() / dustInterval);

                for (int j = 0; j < howMany; j++)
                {
                    Dust.NewDustPerfect(position1 + (direction * dustInterval * j), ModContent.DustType<LightningVapour>(), Velocity: new Vector2(0, Main.rand.NextFloat(-0.3f, -1f)), Scale: Main.rand.NextFloat(0.3f, 0.8f));
                }
            }

            for (int i = 0; i < 40; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7f, 12f);
                Dust.NewDustPerfect(targetPosition, ModContent.DustType<LightningDebris>(), velocity, Scale: Main.rand.NextFloat(1f, 2f));
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Player owner = Main.player[Projectile.owner];

                Player player = Main.LocalPlayer;
                if (PoMUtil.CanHitPvp(owner, player))
                {
                    Rectangle localRect = player.getRect();
                    if (localRect.Intersects(boltRect) || localRect.Intersects(airRect))
                    {
                        player.Hurt(PlayerDeathReason.ByPlayer(Projectile.owner), Projectile.damage, player.direction, true);
                    }
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (PoMUtil.CanHitNPC(npc))
                        {
                            Rectangle npcRect = npc.getRect();
                            if (npcRect.Intersects(boltRect) || npcRect.Intersects(airRect))
                            {
                                owner.ApplyDamageToNPC(npc, Projectile.damage, 1, npc.direction, false);
                            }
                        }
                    }
                }
            }

            int smallerRadius = (int)(airRadius * 0.8f);
            for (int i = 0; i < 24; i++)
            {
                Vector2 position = targetPosition + new Vector2(Main.rand.NextFloat(-smallerRadius, smallerRadius), Main.rand.NextFloat(-smallerRadius, smallerRadius));
                Dust.NewDustPerfect(position, ModContent.DustType<ShockedAir>(), Scale: Main.rand.NextFloat(0.6f, 1.5f));
            }
            isCloud = true;
            Projectile.timeLeft = PoMGlobals.ailmentDuration * (Projectile.extraUpdates + 1);
        }
    }
}