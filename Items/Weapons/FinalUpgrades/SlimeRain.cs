using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.FinalUpgrades
{
    public class SlimeRain : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Slime Rain");
            //Tolltip.SetDefault("'The King's innards spread across the land..'");
            //DisplayName.AddTranslation(GameCulture.Chinese, "史莱姆雨");
            //Tolltip.AddTranslation(GameCulture.Chinese, "史莱姆王的内腑撒得遍地都是..");
        }

        public override void SetDefaults()
        {
            item.damage = 6000;
            item.melee = true;
            item.width = 72;
            item.height = 90;
            item.useTime = 10;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.melee = true;
            item.knockBack = 6;
            item.value = Item.sellPrice(1);
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item34;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("SlimeRainBall");
            item.shootSpeed = 16f;
            item.useAnimation = 12;
            item.useTime = 4;
            item.reuseDelay = 14;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(0, Main.DiscoG, 255);
                }
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 240);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY,
            ref int type, ref int damage, ref float knockBack)
        {
            float x;
            float y = player.Center.Y - Main.rand.NextFloat(600, 700);
            const int timeLeft = 45 * 2;
            for (int i = 0; i < 5; i++)
            {
                x = player.Center.X + 2f * Main.rand.NextFloat(-400, 400);
                float ai1 = Main.rand.Next(timeLeft);
                int p = Projectile.NewProjectile(x, y, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(15f, 20f), type, damage, knockBack, player.whoAmI, 0f, ai1);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].timeLeft = timeLeft;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("SlimeSword"), 1);
            recipe.AddIngredient(mod.ItemType("Sadism"), 15);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}