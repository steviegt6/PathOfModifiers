using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace PathOfModifiers.Buffs
{
    public class BurningAir : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Losing or restoring life over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.isOnBurningAir = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().isOnBurningAir = true;
        }
    }
}
