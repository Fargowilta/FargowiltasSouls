using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using FargowiltasSouls.Utilities;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class HentaiSpear : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            Tooltip.SetDefault("Has different attacks when using left or right click" +
                "\nHas different attacks when used while holding up, down, or both" +
                "\n'The reward for embracing eternity...'");

            DisplayName.AddTranslation(GameCulture.Chinese, "洞察者");
            Tooltip.AddTranslation(GameCulture.Chinese, "左右键攻击不同"+
"\n按住上键、下键和同时按下的攻击不同"+
"\n'屠戮众多的奖励...'");

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 10));
        }

        public override void SetDefaults()
        {
            item.damage = 1700;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 16;
            item.useTime = 16;
            item.shootSpeed = 6f;
            item.knockBack = 7f;
            item.width = 72;
            item.height = 72;
            //item.scale = 1.3f;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item1;
            item.shoot = mod.ProjectileType("HentaiSpear");
            item.value = Item.sellPrice(0, 70);
            item.noMelee = true;
            item.noUseGraphic = true;
            item.melee = true;
            item.autoReuse = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            item.useTurn = false;

            if (player.altFunctionUse == 2)
            {
                if (player.controlUp && player.controlDown)
                {
                    item.shoot = mod.ProjectileType("HentaiSpearWand");
                    item.shootSpeed = 6f;
                    item.useAnimation = 16;
                    item.useTime = 16;
                }
                else if (player.controlUp && !player.controlDown)
                {
                    item.shoot = mod.ProjectileType("HentaiSpearSpinThrown");
                    item.shootSpeed = 6f;
                    item.useAnimation = 16;
                    item.useTime = 16;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    item.shoot = mod.ProjectileType("HentaiSpearSpinBoundary");
                    item.shootSpeed = 1f;
                    item.useAnimation = 16;
                    item.useTime = 16;
                    item.useTurn = true;
                }
                else
                {
                    item.shoot = mod.ProjectileType("HentaiSpearThrown");
                    item.shootSpeed = 25f;
                    item.useAnimation = 85;
                    item.useTime = 85;
                }

                item.ranged = true;
                item.melee = false;
            }
            else
            {
                if (player.controlUp && !player.controlDown)
                {
                    item.shoot = mod.ProjectileType("HentaiSpearSpin");
                    item.shootSpeed = 1f;
                    item.useTurn = true;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    item.shoot = mod.ProjectileType("HentaiSpearDive");
                    item.shootSpeed = 6f;
                }
                else
                {
                    item.shoot = mod.ProjectileType("HentaiSpear");
                    item.shootSpeed = 6f;
                }

                item.useAnimation = 16;
                item.useTime = 16;
                item.ranged = false;
                item.melee = true;
            }

            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2) // Right-click
            {
                if (player.controlUp)
                {
                    if (player.controlDown) // Giga-beam
                        return player.ownedProjectileCounts[item.shoot] < 1;

                    if (player.ownedProjectileCounts[item.shoot] < 1) // Remember to transfer any changes here to hentaispearspinthrown!
                    {
                        Vector2 speed = Main.MouseWorld - player.MountedCenter;

                        if (speed.Length() < 360)
                            speed = Vector2.Normalize(speed) * 360;

                        Projectile.NewProjectile(position, Vector2.Normalize(speed), item.shoot, damage, knockBack, player.whoAmI, speed.X, speed.Y);
                    }

                    return false;
                }

                return true;
            }

            if (player.ownedProjectileCounts[item.shoot] < 1)
            {
                if (player.controlUp && !player.controlDown)
                    return true;

                if (player.ownedProjectileCounts[mod.ProjectileType("Dash")] < 1 && player.ownedProjectileCounts[mod.ProjectileType("Dash2")] < 1)
                {
                    float dashAI = 0;
                    float speedModifier = 2f;
                    int dashType = mod.ProjectileType("Dash");

                    if (player.controlUp && player.controlDown) // Super-dash
                    {
                        dashAI = 1;
                        speedModifier = 2.5f;
                    }

                    Vector2 speed = new Vector2(speedX, speedY);

                    if (player.controlDown && !player.controlUp) //dive
                    {
                        dashAI = 2;
                        speed = new Vector2(Math.Sign(speedX) * 0.0001f, speed.Length());
                        dashType = mod.ProjectileType("Dash2");
                    }

                    int p = Projectile.NewProjectile(position, Vector2.Normalize(speed) * speedModifier * item.shootSpeed,
                        dashType, damage, knockBack, player.whoAmI, speed.ToRotation(), dashAI);
                    if (p != Main.maxProjectiles)
                        Projectile.NewProjectile(position, speed, item.shoot, damage, knockBack, player.whoAmI, p, 1f);
                }
            }

            return false;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                var lineshader = GameShaders.Misc["PulseUpwards"].UseColor(new Color(28, 222, 152)).UseSecondaryColor(new Color(168, 245, 228));
                lineshader.Apply(null);
                Utils.DrawBorderString(Main.spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            //recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerMoon"));
            recipe.AddIngredient(mod.ItemType("Sadism"), 30);
            recipe.AddIngredient(mod.ItemType("MutantScale"), 30);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 30);
            recipe.AddIngredient(mod.ItemType("PhantasmalEnergy"));
            recipe.AddIngredient(mod.ItemType("MutantEye"));

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
