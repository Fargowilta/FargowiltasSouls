using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class RefreshedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Refreshed");
            // Description.SetDefault("Increased mobility and flight time");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().WingTimeModifier += 0.5f;
            player.hasMagiluminescence = true;
            player.frogLegJumpBoost = true;
        }
    }
}