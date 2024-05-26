
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Boss
{
    public class MoonFangBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/Boss/MutantFangBuff";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.bleed = true;
            player.moonLeech = true;
        }
    }
}
