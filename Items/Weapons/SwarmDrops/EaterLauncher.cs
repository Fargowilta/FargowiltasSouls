using FargowiltasSouls.Items.Weapons.BossDrops;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class EaterLauncher : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Rockeater Launcher");
            Tooltip.SetDefault("Uses rockets for ammo\n50% chance to not consume ammo\nIncreased damage to enemies in the given range\n'The reward for slaughtering many..'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "吞噬者发射器");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励..'");
        }

        public override void SetDefaults()
        {
            Item.damage = 378;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item62;
            Item.useAmmo = AmmoID.Rocket;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Purple;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EaterRocket>();
            Item.shootSpeed = 24f;
            Item.scale = .7f;
        }

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12, -2);
        }

        public override void HoldItem(Player player)
        {
            if (player.itemTime > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * 300);
                    offset.Y += (float)(Math.Cos(angle) * 300);
                    Dust dust = Main.dust[Dust.NewDust(
                        player.Center + offset - new Vector2(4, 4), 0, 0,
                        DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
                        )];
                    dust.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust.velocity += Vector2.Normalize(offset) * 5f;
                    dust.noGravity = true;
                    dust.scale = 1f;

                    Vector2 offset2 = new Vector2();
                    double angle2 = Main.rand.NextDouble() * 2d * Math.PI;
                    offset2.X += (float)(Math.Sin(angle2) * 600);
                    offset2.Y += (float)(Math.Cos(angle2) * 600);
                    Dust dust2 = Main.dust[Dust.NewDust(
                        player.Center + offset2 - new Vector2(4, 4), 0, 0,
                        DustID.PurpleCrystalShard, 0, 0, 100, Color.White, 1f
                        )];
                    dust2.velocity = player.velocity;
                    if (Main.rand.NextBool(3))
                        dust2.velocity += Vector2.Normalize(offset2) * -5f;
                    dust2.noGravity = true;
                    dust2.scale = 1f;
                }
            }
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<EaterRocket>();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool();
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<EaterStaff>())
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerWorm"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}