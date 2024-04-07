using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Trophies
{
    public class BaronTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.BaronTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Abominationn Trophy");
        }
    }
}