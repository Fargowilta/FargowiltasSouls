using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.FinalUpgrades
{
    public class SlimeRain : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Slime Rain");
            Tooltip.SetDefault("'The King's innards spread across the land..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "史莱姆雨");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "史莱姆王的内腑撒得遍地都是..");
        }

        public override void SetDefaults()
        {
            Item.damage = 6000;
            Item.DamageType = DamageClass.Melee;
            Item.width = 72;
            Item.height = 90;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(1);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item34;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlimeRainBall>();
            Item.shootSpeed = 16f;

            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = 0;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = new Color(0, Main.DiscoG, 255);
                }
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 240);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float x;
            float y = player.Center.Y - Main.rand.NextFloat(600, 700);
            const int timeLeft = 45 * 2;
            for (int i = 0; i < 5; i++)
            {
                x = player.Center.X + 2f * Main.rand.NextFloat(-400, 400);
                float ai1 = Main.rand.Next(timeLeft);
                int p = Projectile.NewProjectile(source, x, y, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(15f, 20f), type, damage, knockback, player.whoAmI, 0f, ai1);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].timeLeft = timeLeft;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<SlimeSlingingSlasher>(), 1)
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 15)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}