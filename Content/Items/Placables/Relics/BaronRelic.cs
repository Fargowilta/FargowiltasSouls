using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Relics
{
    public class BaronRelic : BaseRelic
    {
        protected override int TileType => ModContent.TileType<Tiles.Relics.BaronRelic>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Abominationn Relic");
        }
    }
}
