using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Items;
using Terraria.Localization;

namespace FargowiltasSouls.Patreon.Duck
{
    public class ScientificRailgun : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Scientific Railgun");
            Tooltip.SetDefault(
@"Uses coins for ammo
Higher valued coins do more damage
'Particular and specific'");*/
        }

        public override void SetDefaults()
        {
            item.damage = 1800;
            item.crit = 26;
            item.ranged = true;
            item.noMelee = true;
            item.width = 64;
            item.height = 26;
            item.useTime = 120;
            item.useAnimation = 120;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 20;
            item.value = Item.sellPrice(0, 10);
            item.rare = ItemRarityID.Purple;
            //item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<RailgunBlast>();
            item.shootSpeed = 1000f;
            item.useAmmo = AmmoID.Coin;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 speed = new Vector2(speedX, speedY).SafeNormalize(Vector2.Zero);
            speedX = speed.X;
            speedY = speed.Y;
            type = item.shoot;
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CoinGun);
            recipe.AddIngredient(ItemID.ChargedBlasterCannon);
            recipe.AddIngredient(ItemID.LastPrism);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient(ItemID.MartianConduitPlating, 100);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
