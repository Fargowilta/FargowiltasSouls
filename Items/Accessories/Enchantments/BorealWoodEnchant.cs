using FargowiltasSouls.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class BorealWoodEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Boreal Wood Enchantment");
            Tooltip.SetDefault(
@"Attacks will periodically be accompanied by several snowballs
'The cooler wood'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "针叶木魔石");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, 
            // @"攻击时定期释放雪球
            // '冷木'");
        }

        protected override Color nameColor => new Color(139, 116, 100);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BorealEffect(player);
        }

        public static void BorealEffect(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().BorealEnchantActive = true;
            player.GetModPlayer<FargoSoulsPlayer>().AdditionalAttacks = true;
        }

        public static void BorealSnowballs(FargoSoulsPlayer modPlayer, int damage)
        {
            Player player = modPlayer.Player;

            Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
            int snowballDamage = damage / 2;
            if (!modPlayer.TerrariaSoul)
                snowballDamage = Math.Min(snowballDamage, FargoSoulsUtil.HighestDamageTypeScaling(player, modPlayer.WoodForce ? 300 : 30));
            int p = Projectile.NewProjectile(player.GetSource_Misc(""), player.Center, vel, ProjectileID.SnowBallFriendly, snowballDamage, 1, Main.myPlayer);

            int numSnowballs = modPlayer.WoodForce ? 5 : 3;
            if (p != Main.maxProjectiles)
                FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, MathHelper.Pi / 10, 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BorealWoodHelmet)
            .AddIngredient(ItemID.BorealWoodBreastplate)
            .AddIngredient(ItemID.BorealWoodGreaves)
            .AddIngredient(ItemID.Snowball, 300)
            .AddIngredient(ItemID.Shiverthorn)
            .AddIngredient(ItemID.Plum)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
