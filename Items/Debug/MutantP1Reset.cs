using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Debug
{
    public class MutantP1Reset : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant P1 Reset");
            Tooltip.SetDefault(@"Makes Mutant forget you have defeated his first phase
Results not guaranteed in multiplayer
You probably shouldn't be reading this...");
            DisplayName.AddTranslation(GameCulture.Chinese, "突变体1阶段重置");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使突变体遗忘你已击败过他的第一阶段
不能保证在多人游戏中使用后的后果
你也许不该读这个...");
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

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                FargoSoulsWorld.skipMutantP1 = 0;
                Main.PlaySound(SoundID.Roar, (int)player.position.X, (int)player.position.Y, 0);
            }
            return true;
        }
    }
}
