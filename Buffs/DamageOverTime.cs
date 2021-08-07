using Terraria;
using Terraria.ModLoader;
using PathOfModifiers;

namespace PathOfModifiers.Buffs
{
    public class DamageOverTime : ModBuff
    {
        /// <summary>
        /// Multiply DPS by this to get damage per 0.5s
        /// </summary>
        public const float damageMultiplierHalfSecond = 2.0f;

        public override string Texture => "Terraria/Images/Buff_20";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Taking damage over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            CanBeCleared = true;
        }
    }
    public class StackingDamageOverTime : DamageOverTime
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(GetType().Name);
            Description.SetDefault("Taking stacking damage over time");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            CanBeCleared = true;
        }
    }
}
