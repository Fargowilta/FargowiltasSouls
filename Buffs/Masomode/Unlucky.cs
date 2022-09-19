using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Unlucky : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unlucky");
            Description.SetDefault("You are feeling pretty unlucky");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().Unlucky = true;
        }
    }
}