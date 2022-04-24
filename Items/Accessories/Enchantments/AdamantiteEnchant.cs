using FargowiltasSouls.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Projectiles;

namespace FargowiltasSouls.Items.Accessories.Enchantments
{
    public class AdamantiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Adamantite Enchantment");
            Tooltip.SetDefault("Every other projectile you spawn will split into 3" +
                "\nAll projectiles deal 50% damage" +
                "\n'Chaos'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "精金魔石");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "每秒会随机使你的一个弹幕分裂成三个" + 
                "\n所有弹幕伤害造成50%伤害" + 
                "\n'一气化三清！'");
        }

        protected override Color nameColor => new Color(221, 85, 125);

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            Item.rare = ItemRarityID.Lime;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            AdamantiteEffect(player);
        }

        public static void AdamantiteEffect(Player player)
        {
            FargoSoulsPlayer modplayer = player.GetModPlayer<FargoSoulsPlayer>();
            modplayer.AdamantiteEnchantActive = true;

            //if (modplayer.AdamantiteCD > 0)
            //    modplayer.AdamantiteCD--;
        }

        public static float ProjectileDamageRatio = 0.5f;

        public static void AdamantiteSplit(Projectile projectile)
        {
            //FargoSoulsPlayer modPlayer = Main.player[projectile.owner].GetModPlayer<FargoSoulsPlayer>();
            //modPlayer.AdamantiteCD = 60;
            //if (modPlayer.Eternity)
            //    modPlayer.AdamantiteCD = 0;
            //else if (modPlayer.TerrariaSoul)
            //    modPlayer.AdamantiteCD = 30;
            //else if (modPlayer.EarthForce || modPlayer.WizardEnchantActive)
            //    modPlayer.AdamantiteCD = 45;

            float damageRatio = ProjectileDamageRatio; //projectile.penetrate == 1 || projectile.usesLocalNPCImmunity ? 0.5f : 1;

            FargoSoulsGlobalProjectile.SplitProj(projectile, 3, MathHelper.Pi / 16, damageRatio);
            projectile.damage = (int)(projectile.damage * damageRatio);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyAdamHead")
                .AddIngredient(ItemID.AdamantiteBreastplate)
                .AddIngredient(ItemID.AdamantiteLeggings)
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.QuadBarrelShotgun)
                .AddIngredient(ItemID.DarkLance)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
