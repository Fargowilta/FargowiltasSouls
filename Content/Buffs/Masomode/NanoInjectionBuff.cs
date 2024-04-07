using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class NanoInjectionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Nano Injection");
            // Description.SetDefault("Life regeneration and stats reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().NanoInjection = true;
            player.GetDamage(DamageClass.Generic) -= 0.1f;
            player.moveSpeed -= 0.1f;
            player.statDefense -= 10;
        }
    }
}