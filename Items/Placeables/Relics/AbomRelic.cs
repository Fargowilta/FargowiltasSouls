using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Placeables.Relics
{
    public class AbomRelic : BaseRelic
    {
        protected override int TileType => ModContent.TileType<Tiles.Relics.AbomRelic>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Abominationn Relic");
        }
    }
}
