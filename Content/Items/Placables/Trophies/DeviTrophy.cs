using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Trophies
{
    public class DeviTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.DeviTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Deviantt Trophy");
        }
    }
}