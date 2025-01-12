using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers
{
    public static class PoMUtil
    {
        public enum PlayerImmuneTime
        {
            Parry = 30,
            Hit = 60,
        }

        public static int CountBuffs(int[] buffs)
        {
            int buffCount = 0;
            for (int i = 0; i < buffs.Length; i++)
            {
                if (buffs[i] > 0)
                {
                    buffCount++;
                }
            }
            return buffCount;
        }

        public static Asset<Texture2D> GetItemTexture(int id)
        {
            Main.instance.LoadItem(id);

            return TextureAssets.Item[id];
        }

        /// <summary>
        /// Returns spawned NPC or null if the cap is reached
        /// </summary>
        public static NPC SpawnNPC(IEntitySource source, int x, int y, int type, bool rollAffixes = true, bool netSync = true, int start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255)
        {
            if (!rollAffixes)
            {
                Affixes.NPCs.NPCNPC.dontRollNextNPC = true;
            }

            int npcIndex = NPC.NewNPC(source, x, y, type, start, ai0, ai1, ai2, ai3, target);
            NPC npc = null;

            if (npcIndex != 200)
            {
                npc = Main.npc[npcIndex];

                if (netSync)
                {
                    NetMessage.SendData(23, -1, -1, null, npcIndex);
                }
            }

            return npc;
        }

        public static void DropItem(IEntitySource source, Vector2 pos, Item item, int syncWhenNetMode, bool noBroadcast = false, bool noGrabDelay = false)
        {
            int index = Item.NewItem(source, pos, item.type, item.stack, noBroadcast, item.prefix, noGrabDelay, false);
            Main.item[index] = item.Clone();
            Main.item[index].position = pos;
            if (Main.netMode == syncWhenNetMode)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index, 1f);
        }

        public struct Line
        {
            public static Line Zero => new Line(Point.Zero, Point.Zero);

            public Point p1;
            public Point p2;

            public int Length
            {
                get
                {
                    int coord = p2.X - p1.X;
                    if (coord != 0)
                        return coord;
                    else
                        return p2.Y - p1.Y;
                }
            }
            public Point Direction => new Point(Math.Sign(p2.X - p1.X), Math.Sign(p2.Y - p1.Y));

            public Line(Point p1, Point p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }
        public static void FindAdjacentBounds(Point startPos, List<Rectangle> recs, bool scanHoriz = true, bool scanVert = true)
        {
            Tile tile = Main.tile[startPos.X, startPos.Y];
            int? type = GetTileType(tile);
            if (type == null)
                return;

            List<Line> adjacentLines = new List<Line>();

            if (scanHoriz)
            {
                Line left = GetLine(startPos, new Point(-1, 0));
                Line right = GetLine(startPos, new Point(1, 0));
                Line lr = new Line(left.p2, right.p2);
                GetRectangles(lr, type.Value, recs);
            }
            if (scanVert)
            {
                Line up = GetLine(startPos, new Point(0, -1));
                Line down = GetLine(startPos, new Point(0, 1));
                Line ud = new Line(up.p2, down.p2);
                GetRectangles(ud, type.Value, recs);
            }
        }
        public static void GetRectangles(Line line, int tileType, List<Rectangle> recs)
        {
            Point direction = line.Direction;
            Point direction90 = new Point(-direction.Y, direction.X);
            Point direction270 = new Point(direction.Y, -direction.X);
            int length = line.Length;
            for (int i = 1; i <= length; i++)
            {
                Point centerTilePos = new Point(line.p1.X + direction.X * i, line.p1.Y + direction.Y * i);

                Tile tile = Main.tile[centerTilePos.X + direction90.X, centerTilePos.Y + direction90.Y];
                int? type = GetTileType(tile);
                if (type.HasValue && type == tileType)
                {
                    GetRectangles(centerTilePos, line.p1, GetLine(centerTilePos, direction90).p2, tileType, recs);
                }

                tile = Main.tile[centerTilePos.X + direction270.X, centerTilePos.Y + direction270.Y];
                type = GetTileType(tile);
                if (type.HasValue && type == tileType)
                {
                    GetRectangles(centerTilePos, line.p1, GetLine(centerTilePos, direction270).p2, tileType, recs);
                }
            }
        }
        public static void GetRectangles(Point corner, Point p1, Point p2, int tileType, List<Rectangle> recs)
        {
            Line baseL1 = new Line(corner, p1);
            Line baseL2 = new Line(corner, p2);
            int baseLength1 = Math.Abs(baseL1.Length);
            int baseLength2 = Math.Abs(baseL2.Length);
            Point baseDirection1 = baseL1.Direction;
            Point baseDirection2 = baseL2.Direction;

            bool l1Shortest = baseLength1 <= baseLength2;
            Line line = baseL1;
            int length = baseLength1;
            Point direction = baseDirection1;
            Point direction90 = baseDirection2;
            if (!l1Shortest)
            {
                line = baseL2;
                length = baseLength2;
                direction = baseDirection2;
                direction90 = baseDirection1;
            }

            for (int i = 1; i <= length; i++)
            {
                Point centerTilePos = new Point(line.p1.X + direction.X * i, line.p1.Y + direction.Y * i);

                Tile tile = Main.tile[centerTilePos.X + direction90.X, centerTilePos.Y + direction90.Y];
                int? type = GetTileType(tile);
                if (type.HasValue && type == tileType)
                {
                    Line l1 = new Line(corner, centerTilePos);
                    int length1 = Math.Abs(l1.Length);
                    for (int j = 1; j <= (l1Shortest ? baseLength2 : baseLength1); j++)
                    {
                        Point tilePos = new Point(centerTilePos.X + direction90.X * j, centerTilePos.Y + direction90.Y * j);
                        tile = Main.tile[tilePos.X, tilePos.Y];
                        type = GetTileType(tile);
                        if (type.HasValue && type == tileType)
                        {
                            Line l2 = new Line(l1.p2, tilePos);
                            Line l3 = GetLine(l2.p2, new Point(-direction.X, -direction.Y), length1);
                            if (Math.Abs(l3.Length) == length1)
                            {
                                Line horizLine, vertLine;
                                if (l1.Direction.X != 0)
                                {
                                    horizLine = l1;
                                    vertLine = l2;
                                }
                                else
                                {
                                    horizLine = l2;
                                    vertLine = l1;
                                }

                                int left = horizLine.p1.X;
                                int width = Math.Abs(horizLine.Length);
                                if (horizLine.p2.X < left)
                                {
                                    left = horizLine.p2.X;
                                    width = -horizLine.Length;
                                }

                                int top = vertLine.p1.Y;
                                int height = Math.Abs(vertLine.Length);
                                if (vertLine.p2.Y < top)
                                {
                                    top = vertLine.p2.Y;
                                    height = -vertLine.Length;
                                }
                                Rectangle rect = new Rectangle(left, top, width, height);
                                if (!recs.Contains(rect))
                                    recs.Add(rect);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        public static Rectangle? GetRectangle(Line l1, Line l2, int tileType)
        {
            Point startCorner = l1.p2;
            Point freeCorner;
            Point direction = l1.Direction;
            if (l2.p1 == l1.p1 || l2.p1 == l1.p2)
            {
                freeCorner = l2.p2;
                if (l2.p1 == l1.p2)
                {
                    startCorner = l1.p1;
                    direction = new Point(-direction.X, -direction.Y);
                }
            }
            else
            {
                freeCorner = l2.p1;
                if (l2.p2 == l1.p2)
                {
                    startCorner = l1.p1;
                    direction = new Point(-direction.X, -direction.Y);
                }
            }
            Line l3 = GetLine(freeCorner, direction, l1.Length);
            if (Math.Abs(l3.Length) != Math.Abs(l1.Length))
                return null;
            freeCorner = l3.p2;
            direction = new Point(startCorner.X - freeCorner.X, startCorner.Y - freeCorner.Y);
            Line l4 = GetLine(freeCorner, direction, l2.Length);
            if (Math.Abs(l4.Length) != Math.Abs(l2.Length))
                return null;

            Line horizLine, vertLine;
            if (l1.Direction.X != 0)
            {
                horizLine = l1;
                vertLine = l2;
            }
            else
            {
                horizLine = l2;
                vertLine = l1;
            }

            int left = horizLine.p1.X;
            int width = Math.Abs(horizLine.Length);
            if (horizLine.p2.X < left)
            {
                left = horizLine.p2.X;
                width = -horizLine.Length;
            }

            int top = vertLine.p1.Y;
            int height = Math.Abs(vertLine.Length);
            if (vertLine.p2.Y < top)
            {
                top = vertLine.p2.Y;
                height = -vertLine.Length;
            }
            return new Rectangle(left, top, width, height);
        }
        public static Line GetLine(Point startPos, Point direction, int maxLength = -1)
        {

            Tile tile = Main.tile[startPos.X, startPos.Y];
            int type = tile.TileType;

            Line line = new Line(startPos, startPos);
            direction = new Point(Math.Sign(direction.X), Math.Sign(direction.Y));
            Tile nextTile = Main.tile[startPos.X + direction.X, startPos.Y + direction.Y];
            int i = 2;
            while (GetTileType(nextTile) == type)
            {
                line.p2 = new Point(line.p2.X + direction.X, line.p2.Y + direction.Y);
                if (maxLength > 0)
                {
                    maxLength--;
                    if (maxLength == 0)
                        break;
                }
                nextTile = Main.tile[startPos.X + direction.X * i, startPos.Y + direction.Y * i];
                i++;
            }

            return line;
        }

        public static int? GetTileType(Tile tile)
        {
            if (tile.HasTile)
                return tile.TileType;
            else
                return null;
        }

        /// <summary>
        /// Queries for a TE that encompasses a tile at (i,j)
        /// </summary>
        public static bool TryGetTileEntity(int i, int j, int coordinateFrameWidth, int coordinateFrameHeight, out TileEntity te)
        {
            var tile = Main.tile[i, j];

            var tePos = new Point16(
                i - (tile.TileFrameX / coordinateFrameWidth),
                j - (tile.TileFrameY / coordinateFrameHeight));
            return TileEntity.ByPosition.TryGetValue(tePos, out te);
        }

        /// <summary>
        /// Sometimes shit doesn't draw where it should. Add this to the position.
        /// </summary>
        /// <returns></returns>
        public static Vector2 DrawToScreenOffset()
        {
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            return zero;
        }

        /// <summary>
        /// Returns current or real life if one exists
        /// </summary>
        public static int GetRealLife(NPC npc)
        {
            if (npc.realLife >= 0)
            {
                return Main.npc[npc.realLife].life;
            }
            else
            {
                return npc.life;
            }
        }
        /// <summary>
        /// Returns max or real max life if one exists
        /// </summary>
        public static int GetRealMaxLife(NPC npc)
        {
            if (npc.realLife >= 0)
            {
                return Main.npc[npc.realLife].lifeMax;
            }
            else
            {
                return npc.lifeMax;
            }
        }
        public static bool IsLowHP(NPC npc)
        {
            return (npc.life / (float)npc.lifeMax) <= PoMGlobals.lowHPThreshold;
        }
        public static bool IsLowHP(Player player)
        {
            return (player.statLife / (float)player.statLifeMax2) <= PoMGlobals.lowHPThreshold;
        }
        public static bool CanHitNPC(NPC npc)
        {
            return npc.active && !npc.friendly && !npc.dontTakeDamage;
        }
        public static bool CanHitPvp(Player player, Player target)
        {
            return target.active && !target.dead && target != player && (target.team != player.team || target.team == 0) && target.hostile && player.hostile;
        }

        public static void MakeImmune(Player player, int timeFrames = (int)PlayerImmuneTime.Hit)
        {
            player.immuneTime = timeFrames;
            player.immune = true;
        }
    }

    public static class PoMEffectHelper
    {
        public static void Heal(Player player, int amount)
        {
            CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealLife, amount, false, false);
            for (int i = 0; i < 7; i++)
            {
                Vector2 dustPosition = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height));
                Vector2 dustVelocity = new Vector2(0, -Main.rand.NextFloat(0.5f, 2.5f));
                float dustScale = Main.rand.NextFloat(1f, 2.5f);
                Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.HealEffect>(), dustVelocity, Scale: dustScale);
            }
        }
        public static void HealMana(Player player, int amount)
        {
            CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), CombatText.HealMana, amount, false, false);
            for (int i = 0; i < 7; i++)
            {
                Vector2 dustPosition = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height));
                Vector2 dustVelocity = new Vector2(0, -Main.rand.NextFloat(0.5f, 2.5f));
                float dustScale = Main.rand.NextFloat(1f, 2.5f);
                Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.HealManaEffect>(), dustVelocity, Scale: dustScale);
            }
        }
        public static void Crit(Vector2 position, int width, int height, int howMuch = 100)
        {
            int howMany = (width * height) / (5000 / howMuch);
            for (int i = 0; i < howMany; i++)
            {
                Dust.NewDust(position, width, height, ModContent.DustType<Dusts.LifeOrbDebris>(), newColor: new Color(1, 0.7f, 0.7f));
            }
        }
        public static void Bleed(Vector2 position)
        {
            Dust.NewDustPerfect(position, DustID.Blood);
        }
        public static void Poison(Vector2 position, int width, int height)
        {
            int dust = Dust.NewDust(position, width, height, 46, Alpha: 150, Scale: 0.2f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].fadeIn = 1.9f;
        }
        public static void Ignite(Vector2 position, int width, int height)
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 dustPosition = position + new Vector2(Main.rand.NextFloat(width), Main.rand.NextFloat(height));
                Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.FireDebris>(), new Vector2(0, Main.rand.NextFloat(-3f, -0.5f)), Alpha: 100, Scale: Main.rand.NextFloat(1f, 2f));
            }
        }
        public static void Shock(Vector2 position, int width, int height)
        {
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(position, width, height, ModContent.DustType<Dusts.Shock>(), Scale: Main.rand.NextFloat(0.3f, 0.7f));
            }
        }
        public static void Chill(Vector2 position, int width, int height)
        {
            if (Main.rand.NextBool(50))
            {
                Vector2 dustPosition = position + new Vector2(Main.rand.NextFloat(0, width), Main.rand.NextFloat(0, height)) + new Vector2(-14, -14);
                Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.FrostCloud>(), Vector2.Zero, 50, Color.White, Main.rand.NextFloat(0.8f, 1.6f));
            }
        }
        public static void MoveSpeed(Player player)
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 dustPosition = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height));
                Vector2 dustVelocity = player.velocity * 0.1f;
                if (dustVelocity == Vector2.Zero)
                {
                    dustVelocity = new Vector2(1, 0) * -player.direction;
                }
                float dustScale = Main.rand.NextFloat(0.5f, 1.5f);
                Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.SpeedEffect>(), dustVelocity, Scale: dustScale);
            }
        }
    }
}
