using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Placables.Trophies
{
    public class MutantTrophy : BaseTrophy
    {
        protected override int TileType => ModContent.TileType<Tiles.Trophies.MutantTrophy>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Mutant Trophy");
        }
    }
}