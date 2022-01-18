using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using FargowiltasSouls.Items;

namespace FargowiltasSouls.Patreon.Sasha
{
    public class MissDrakovisFishingPole : SoulsItem
    {
        private int mode = 1;

        public override string Texture => "Terraria/Item_2296";

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Miss Drakovi's Fishing Pole");
            Tooltip.SetDefault("Right click to cycle through options of attack\nEvery damage type has one");
            DisplayName.AddTranslation(GameCulture.Chinese, "Drakovi小姐的钓竿");
            Tooltip.AddTranslation(GameCulture.Chinese, "右键循环切换攻击模式\n每种伤害类型对应一种模式");*/
        }

        public override void SetDefaults()
        {
            item.damage = 300;
            item.width = 24;
            item.height = 28;
            item.value = Item.sellPrice(0, 15);
            item.rare = 10;
            item.autoReuse = true;

            SetUpItem();
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "tooltip", Language.GetTextValue("Mods.FargowiltasSouls.Patreon.Tooltip"));
            line.overrideColor = Color.Orange;
            tooltips.Add(line);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //right click
            if (player.altFunctionUse == 2)
            {
                mode++;

                if (mode > 5)
                {
                    mode = 1;
                }

                SetUpItem();

                return false;
            }

            switch (mode)
            {
                //melee
                case 1:
                    Projectile.NewProjectile(position, new Vector2(speedX, speedY), mod.ProjectileType("PufferRang"), damage, knockBack, player.whoAmI);
                    break;

                //range
                case 2:
                    {
                        Vector2 speed = new Vector2(speedX, speedY);

                        int numBullets = 4;
                        for (int num130 = 0; num130 < numBullets; num130++) //shotgun blast
                        {
                            Vector2 bulletSpeed = speed;
                            bulletSpeed.X += Main.rand.NextFloat(-1f, 1f);
                            bulletSpeed.Y += Main.rand.NextFloat(-1f, 1f);
                            Projectile.NewProjectile(position, bulletSpeed, type, damage, knockBack, player.whoAmI);
                        }

                        for (int i = 0; i < Main.maxNPCs; i++) //shoot an extra bullet at every nearby enemy
                        {
                            if (Main.npc[i].active && Main.npc[i].CanBeChasedBy() && player.Distance(Main.npc[i].Center) < 1000f)
                            {
                                Vector2 bulletSpeed = 2f * speed.Length() * player.DirectionTo(Main.npc[i].Center);
                                Projectile.NewProjectile(position, bulletSpeed, type, damage / 2, knockBack, player.whoAmI);
                            }
                        }
                    }
                    break;

                //magic
                case 3:
                    {
                        Vector2 speed = new Vector2(speedX, speedY);
                        for (int i = -2; i <= 2; i++)
                        {
                            float modifier = 1f - 0.75f / 2f * Math.Abs(i);
                            Projectile.NewProjectile(position, modifier * speed.RotatedBy(MathHelper.ToRadians(9) * i),
                                mod.ProjectileType("Bubble"), damage, knockBack, player.whoAmI);
                        }
                    }
                    break;

                //summon
                case 4:
                    Projectile.NewProjectile(position, new Vector2(speedX, speedY), mod.ProjectileType("FishMinion"), damage, knockBack, player.whoAmI);
                    break;

                //throwing
                default:
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectile(position, 4f * new Vector2(speedX + Main.rand.Next(-2, 2), speedY + Main.rand.Next(-2, 2)), mod.ProjectileType("SpikyLure"), damage, knockBack, player.whoAmI);
                    }
                    break;
            }

            return false;
        }

        private void SetUpItem()
        {
            ResetDamageType();

            switch (mode)
            {
                //melee
                case 1:
                    item.melee = true;
                    item.shoot = mod.ProjectileType("PufferRang");

                    item.useStyle = 1;
                    item.useTime = 12;
                    item.useAnimation = 12;
                    item.UseSound = SoundID.Item1;
                    item.knockBack = 6;
                    item.noMelee = false;
                    item.shootSpeed = 15f;
                    break;

                //range
                case 2:
                    item.ranged = true;
                    item.shoot = ProjectileID.Bullet;

                    item.knockBack = 6.5f;
                    item.useStyle = 5;
                    item.useAnimation = 50;
                    item.useTime = 50;
                    item.useAmmo = AmmoID.Bullet;
                    item.UseSound = SoundID.Item36;
                    item.shootSpeed = 12f;
                    item.noMelee = true;
                    break;

                //magic
                case 3:
                    item.magic = true;
                    item.mana = 15;
                    item.shoot = mod.ProjectileType("Bubble");

                    item.knockBack = 3f;
                    item.useStyle = 5;
                    item.useAnimation = 25;
                    item.useTime = 25;
                    item.UseSound = SoundID.Item85;
                    item.shootSpeed = 30f;
                    item.noMelee = true;
                    break;

                //minion
                case 4:
                    item.summon = true;
                    item.mana = 10;
                    item.shoot = mod.ProjectileType("FishMinion");

                    item.useTime = 36;
                    item.useAnimation = 36;
                    item.useStyle = 1;
                    item.noMelee = true;
                    item.knockBack = 3;
                    item.UseSound = SoundID.Item44;
                    item.shootSpeed = 10f;
                    item.buffType = mod.BuffType("FishMinionBuff");
                    item.buffTime = 3600;
                    item.autoReuse = true;
                    break;

                //throwing
                case 5:
                    item.thrown = true;
                    item.shoot = mod.ProjectileType("SpikyLure");

                    item.useStyle = 1;
                    item.shootSpeed = 5f;
                    item.knockBack = 1f;
                    item.UseSound = SoundID.Item1;
                    item.useAnimation = 15;
                    item.useTime = 15;
                    item.noMelee = true;
                    break;
            }
        }

        private void ResetDamageType()
        {
            item.melee = false;
            item.ranged = false;
            item.magic = false;
            item.summon = false;
            item.ranged = false;
            item.mana = 0;
            item.useAmmo = AmmoID.None;
        }

        public override void AddRecipes()
        {
            if (SoulConfig.Instance.PatreonFishingRod)
            {
                ModRecipe recipe = new ModRecipe(mod);
                recipe.AddIngredient(ItemID.GoldenFishingRod);
                recipe.AddIngredient(ItemID.BalloonPufferfish);
                recipe.AddIngredient(ItemID.PurpleClubberfish);
                recipe.AddIngredient(ItemID.FrostDaggerfish, 500);
                recipe.AddIngredient(ItemID.ZephyrFish);
                recipe.AddIngredient(ItemID.Toxikarp);
                recipe.AddIngredient(ItemID.Bladetongue);
                recipe.AddIngredient(ItemID.CrystalSerpent);
                recipe.AddIngredient(ItemID.ScalyTruffle);
                recipe.AddIngredient(ItemID.ObsidianSwordfish);

                recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

                recipe.SetResult(this);
                recipe.AddRecipe();
            }
        }
    }
}