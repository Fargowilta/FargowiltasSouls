using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Trophies
{
    public class EridanusTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.EridanusTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Eridanus Trophy");
        }
    }
}