using System;
using System.IO;
using Microsoft.Xna.Framework;
using PathOfModifiers.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using PathOfModifiers.Maps;
using System.Collections.Generic;
using System.Linq;

namespace PathOfModifiers.Tiles
{
    public class MapDevice : ModTile
    {
        public static TEMapDevice activeMD;

        public override void SetDefaults()
        {
            Main.tileSpelunker[Type] = false;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileValue[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEMapDevice>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			//TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			//TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Map Device");
			AddMapEntry(new Color(200, 200, 200), name);
			disableSmartCursor = false;
			drop = 0;
		}

		public override bool HasSmartInteract()
		{
			return true;
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != 1)
                Item.NewItem(new Vector2(i * 16, j * 16), mod.ItemType("MapDevice"));

            TEMapDevice tileEntity = (TEMapDevice)TileEntity.ByID[mod.GetTileEntity<TEMapDevice>().Find(i, j)];

            if (Main.netMode != 2 && activeMD == tileEntity)
                HideUI();

            tileEntity.Kill(i, j);
        }

        public override void RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			if (player.sign >= 0)
			{
				Main.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
            }
            TEMapDevice clickedMD = (TEMapDevice)TileEntity.ByPosition[new Point16(left, top)];
            if (MapDeviceUI.Instance.Visible && activeMD == clickedMD)
            {
                HideUI();
            }
            else
            {
                ShowUI(clickedMD);
            }
            return;
		}

        public static void ShowUI(TEMapDevice md)
        {
            if (activeMD != null)
                Main.PlaySound(SoundID.MenuTick);
            else
                Main.PlaySound(SoundID.MenuOpen);
            activeMD = md;
            Main.playerInventory = true;
            MapDeviceUI.Instance.Visible = true;
        }
        public static void HideUI()
        {
            if (activeMD != null)
                Main.PlaySound(SoundID.MenuClose);
            activeMD = null;
            MapDeviceUI.Instance.Visible = false;
        }

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.frameX % 36 != 0)
			{
				left--;
			}
			if (tile.frameY != 0)
			{
				top--;
			}
			player.showItemIconText = "Map Device";
			if (player.showItemIconText == "Map Device")
			{
				player.showItemIcon2 = mod.ItemType("MapDevice");
				player.showItemIconText = "";
			}
			player.noThrow = 2;
			player.showItemIcon = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.showItemIconText == "")
			{
				player.showItemIcon = false;
				player.showItemIcon2 = 0;
			}
		}
	}

    public class TEMapDevice : PoMTileEntity
    {
        public enum MapAction
        {
            Open = 0,
            Close = 1,
        }

        public Item map = new Item();

        public int timeLeft = 0;

        public Rectangle? bounds = null;

        public bool DetectBounds()
        {
            List<Rectangle> boundss = new List<Rectangle>();
            List<Tuple<Point, bool, bool>> adjacentTiles = new List<Tuple<Point, bool, bool>>();
            
            Point size = new Point(2, 2);
            int length = size.X * 2 + size.Y * 2 + 4;
            int x = 0;
            int y = 0;
            bool lastTileOfType = false;
            for (int i = 0; i < length; i++)
            {
                bool scanHoriz = true;
                bool scanVert = true;
                if (i == 0) { }
                else if (i > 0 && i < size.X + 2)
                {
                    x++;
                    scanHoriz = !lastTileOfType;
                }
                else if (i < size.X + 2 + size.Y + 1)
                {
                    y++;
                    scanVert = !lastTileOfType;
                }
                else if (i < size.X + 2 + size.Y + 1 + size.X + 1)
                {
                    x--;
                    scanHoriz = !lastTileOfType;
                }
                else
                {
                    y--;
                    scanVert = !lastTileOfType;
                }

                Point tilePos = new Point(Position.X - 1 + x, Position.Y - 1 + y);
                Tile tile = Main.tile[tilePos.X, tilePos.Y];
                int? tileType = PoMHelper.GetTileType(tile);
                if (tileType.HasValue && tileType == TileID.IceBrick)
                {
                    adjacentTiles.Add(new Tuple<Point, bool, bool>(tilePos, scanHoriz, scanVert));
                    lastTileOfType = true;
                }
                else
                {
                    lastTileOfType = false;
                }
            }

            foreach (var tilePos in adjacentTiles)
            {
                PoMHelper.FindAdjacentBounds(tilePos.Item1, boundss, tilePos.Item2, tilePos.Item3);
            }

            Rectangle tileBounds = new Rectangle(Position.X, Position.Y, size.X - 1, size.Y - 1);
            Rectangle tileBoundsInflated = tileBounds;
            tileBoundsInflated.Inflate(2, 2);
            for (int i = boundss.Count - 1; i >= 0; i--)
            {
                Rectangle bound = boundss[i];
                bool intersecnt = tileBoundsInflated.X < bound.X + bound.Width && bound.X < tileBoundsInflated.X + tileBoundsInflated.Width && tileBoundsInflated.Y < bound.Y + bound.Height && bound.Y < tileBoundsInflated.Y + tileBoundsInflated.Height;
                if (!bound.Intersects(tileBoundsInflated) || bound.Contains(tileBounds))
                    boundss.RemoveAt(i);
            }

            if (boundss.Count > 0)
            {
                bounds = boundss.Aggregate((b1, b2) => b1.Width * b1.Height > b2.Width * b2.Height ? b1 : b2);
                return true;
            }
            else
            {
                bounds = null;
                return false;
            }
        }

        public bool CanBegin()
        {
            return !map.IsAir && map.modItem is Items.Map && timeLeft == 0 && DetectBounds();
        }
        public bool CanEnd()
        {
            return timeLeft > 0;
        }

        public void BeginMap()
        {
            if (!CanBegin())
                return;

            timeLeft = 10 * 60 * 60;

            Items.Map mapItem = ((Items.Map)map.modItem);
            Rectangle dimensions = new Rectangle(bounds.Value.X + 1, bounds.Value.Y + 1, bounds.Value.Width - 2, bounds.Value.Height - 2);
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                mapItem.map.Generate(dimensions);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                PoMNetMessage.GenerateMap(dimensions, mapItem.map);
            }

            Sync(ID, Main.myPlayer);
        }
        public void EndMap()
        {
            if (!CanEnd())
                return;

            timeLeft = 0;



            Sync(ID, Main.myPlayer);
        }

        public override void Update()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (timeLeft > 0)
                {
                    timeLeft--;
                    if (timeLeft == 0)
                    {
                        EndMap();
                    }
                    else if (timeLeft % 60 == 0)
                    {
                        Sync(ID);
                        MapDeviceUI.Instance.UpdateText();
                    }
                }
            }
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            //PathOfModifiers.Log($"NetSend{Main.netMode}");
            writer.Write(timeLeft);
            ItemIO.Send(map, writer, true);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            //PathOfModifiers.Log($"NetReceive{Main.netMode}");
            timeLeft = reader.ReadInt32();
            map = ItemIO.Receive(reader, true);
            if (Main.netMode != 2)
            {
                if (MapDevice.activeMD != null && MapDevice.activeMD.Position == Position)
                    MapDevice.ShowUI(this);
                else if (MapDevice.activeMD == this)
                    MapDeviceUI.Instance.SetItemSlot(map.Clone());
                MapDeviceUI.Instance.UpdateText();
            }
        }

        public override TagCompound Save()
        {
            //PathOfModifiers.Log($"Save{Main.netMode}");
            TagCompound tag = new TagCompound();
            tag.Set("timeLeft", timeLeft);
            tag.Set("map", ItemIO.Save(map));
            return tag;
        }
        public override void Load(TagCompound tag)
        {
            //PathOfModifiers.Log($"Load{Main.netMode}");
            timeLeft = tag.GetInt("timeLeft");
            map = ItemIO.Load(tag.GetCompound("map"));
            if (Main.netMode != 2)
                MapDeviceUI.Instance.UpdateText();
        }

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == mod.TileType<MapDevice>() && tile.frameX == 0 && tile.frameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            //Main.NewText("i " + i + " j " + j + " t " + type + " s " + style + " d " + direction);
            if (Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(87, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override void OnKill()
        {
            if (Main.netMode != 1)
            {
                if (!map.IsAir)
                {
                    PoMHelper.DropItem(new Vector2(Position.X * 16, Position.Y * 16), map, 2);
                }
            }
        }
    }
}