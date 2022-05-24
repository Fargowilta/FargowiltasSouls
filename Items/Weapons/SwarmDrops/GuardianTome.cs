using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class GuardianTome : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("The Guardian");
            Tooltip.SetDefault("'It's their turn to run'");

            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "守卫者");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "现在轮到他们跑了");
        }

        public override void SetDefaults()
        {
            Item.damage = 1499;
            Item.DamageType = DamageClass.Magic;
            Item.width = 24;
            Item.height = 28;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 70);
            Item.rare = ItemRarityID.Purple;
            Item.mana = 100;
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DungeonGuardian>();
            Item.shootSpeed = 18f;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltipItemNameLine = tooltips.FirstOrDefault(line => line.Name == "ItemName" && line.Mod == "Terraria");
            tooltipItemNameLine.OverrideColor = new Color(255, Main.DiscoG, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerDG"))
            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 15)
            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}