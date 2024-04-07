using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.ManliestDove
{
    public class FigBranch : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Fig Branch");
            // Tooltip.SetDefault("Summons a Dove companion");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<DoveProj>();
            Item.buffType = ModContent.BuffType<DoveBuff>();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }

        public override void AddRecipes()
        {
            if (SoulConfig.Instance.PatreonDove)
            {
                CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyBird")
                .AddIngredient(ItemID.Wood, 50)
                .AddIngredient(ItemID.BorealWood, 50)
                .AddIngredient(ItemID.RichMahogany, 50)
                .AddIngredient(ItemID.PalmWood, 50)
                .AddIngredient(ItemID.Ebonwood, 50)
                .AddIngredient(ItemID.Shadewood, 50)

                .AddTile(TileID.LivingLoom)


                .Register();
            }
        }
    }
}