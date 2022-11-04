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

            //in force fires more snowballs, more often, with much higher dmg cap
        }

        protected override Color nameColor => new Color(139, 116, 100);
        public override string wizardEffect => "Fires more snowballs more often";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BorealEffect(player, Item);
        }

        public static void BorealEffect(Player player, Item item)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.BorealEnchantItem = item;

            if (modPlayer.BorealCD > 0)
                modPlayer.BorealCD--;
        }

        public static void BorealSnowballs(FargoSoulsPlayer modPlayer, int damage)
        {
            Player player = modPlayer.Player;

            Vector2 vel = Vector2.Normalize(Main.MouseWorld - player.Center) * 17f;
            int snowballDamage = damage / 2;
            if (!modPlayer.TerrariaSoul)
                snowballDamage = Math.Min(snowballDamage, FargoSoulsUtil.HighestDamageTypeScaling(player, modPlayer.WoodForce ? 300 : 30));
            int p = Projectile.NewProjectile(player.GetSource_Accessory(modPlayer.BorealEnchantItem), player.Center, vel, ProjectileID.SnowBallFriendly, snowballDamage, 1, Main.myPlayer);

            int numSnowballs = modPlayer.WoodForce ? 7 : 3;
            if (p != Main.maxProjectiles)
                FargoSoulsGlobalProjectile.SplitProj(Main.projectile[p], numSnowballs, MathHelper.Pi / 10, 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.BorealWoodHelmet)
            .AddIngredient(ItemID.BorealWoodBreastplate)
            .AddIngredient(ItemID.BorealWoodGreaves)
            .AddIngredient(ItemID.Shiverthorn)
            .AddIngredient(ItemID.Plum)
            .AddIngredient(ItemID.Snowball, 300)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}
