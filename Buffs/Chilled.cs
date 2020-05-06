using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.Buffs
{
    public class Chilled : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Damage dealt is modified");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.isChilled = true;

            if (Main.rand.NextBool(50))
            {
                Vector2 position = npc.position + new Vector2(Main.rand.NextFloat(0, npc.width), Main.rand.NextFloat(0, npc.height)) + new Vector2(-14, -14);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.FrostCloud>(), Vector2.Zero, 50, Color.White, Main.rand.NextFloat(0.8f, 1.6f));
            }
        }
        public override void Update(Player player, ref int buffIndex)
        {
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.isChilled = true;

            if (Main.rand.NextBool(50))
            {
                Vector2 position = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height)) + new Vector2(-14, -14);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.FrostCloud>(), Vector2.Zero, 50, Color.White, Main.rand.NextFloat(0.8f, 1.6f));
            }
        }
    }
}
