using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items
{
    public class MasochistReal : SoulsItem
    {
        //public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forgotten Gift");
            Tooltip.SetDefault("[c/ff0000:Debug item]\nLeft click: Toggles session ability to play Maso\nRight click: Toggles world ability to play Maso");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanRightClick() => true;

        public override void RightClick(Player player) => Main.NewText($"world is currently maso: {FargoSoulsWorld.MasochistModeReal}");

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                FargoSoulsWorld.CanPlayMaso = !FargoSoulsWorld.CanPlayMaso;
                Main.NewText($"world: {FargoSoulsWorld.CanPlayMaso}");
            }
            else
            {
                player.GetModPlayer<FargoSoulsPlayer>().Toggler.CanPlayMaso = !player.GetModPlayer<FargoSoulsPlayer>().Toggler.CanPlayMaso;
                Main.NewText($"mod: {player.GetModPlayer<FargoSoulsPlayer>().Toggler.CanPlayMaso}");
            }
            return true;
        }
    }
}