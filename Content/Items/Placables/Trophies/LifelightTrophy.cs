using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Trophies
{
    public class LifelightTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.LifelightTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Lifelight Trophy");
        }
    }
}