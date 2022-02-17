using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace PathOfModifiers.Buffs
{
    public class Shocked : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Damage taken is modified");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
