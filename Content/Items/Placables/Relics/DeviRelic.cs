using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Relics
{
    public class DeviRelic : BaseRelic
    {
        protected override int TileType => ModContent.TileType<Tiles.Relics.DeviRelic>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Deviantt Relic");
        }
    }
}
