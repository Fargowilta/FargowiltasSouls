using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs
{
    public class GladiatorSpiritBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/GladiatorBuff";
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.3f;
            player.endurance += 0.15f;
            player.noKnockback = true;
        }
    }
}