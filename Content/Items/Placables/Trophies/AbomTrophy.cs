using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Trophies
{
    public class AbomTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.AbomTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Abominationn Trophy");
        }
    }
}