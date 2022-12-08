using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Placeables.Trophies
{
    public class LifeChallengerTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.LifeChallengerTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Lieflight Trophy");
        }
    }
}