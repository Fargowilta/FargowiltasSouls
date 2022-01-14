using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Debug
{
    public class MutantP1Reset : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Mutant P1 Reset");
            Tooltip.SetDefault(@"Left click to make Mutant forget you have defeated his first phase
Right click to make Mutant think you have defeated his first phase
Results not guaranteed in multiplayer
You probably shouldn't be reading this...");*/
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useAnimation = 45;
            item.useTime = 45;
            item.consumable = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                FargoSoulsWorld.skipMutantP1 = player.altFunctionUse == 2 ? 10 : 0;
                Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);
            }
            return true;
        }
    }
}