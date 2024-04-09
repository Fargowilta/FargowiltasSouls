using FargowiltasSouls.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.DanielTheRobot
{
    public class ROBGyro : PatreonModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Crimetroid Egg");
            // Tooltip.SetDefault("Summons the Baby\nNot to be confused with criminal androids");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<ROB>();
            Item.buffType = ModContent.BuffType<ROBBuff>();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            base.UseStyle(player, heldItemFrame);
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
        public override void AddRecipes()
        {
            if (SoulConfig.Instance.PatreonROB)
            {
                CreateRecipe()
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddRecipeGroup("IronBar", 10)
                .AddIngredient(ItemID.Wire, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
            }
        }
    }
}