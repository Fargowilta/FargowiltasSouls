using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Refreshed : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Refreshed");
            Description.SetDefault("Increased mobility and flight time");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().WingTimeModifier += 0.5f;
            player.hasMagiluminescence = true;
            player.frogLegJumpBoost = true;
        }
    }
}