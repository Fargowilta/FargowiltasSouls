using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.FinalUpgrades
{
    public class HentaiSpear : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            Tooltip.SetDefault("Has different attacks when using left or right click" +
                "\nHas different attacks when used while holding up, down, or both" +
                "\n'The reward for embracing eternity...'");

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 1700;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.shootSpeed = 6f;
            Item.knockBack = 7f;
            Item.width = 72;
            Item.height = 72;
            //Item.scale = 1.3f;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.HentaiSpear>();
            Item.value = Item.sellPrice(0, 70);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.useTurn = false;

            if (player.altFunctionUse == 2)
            {
                if (player.controlUp && player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearWand>();
                    Item.shootSpeed = 6f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                }
                else if (player.controlUp && !player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearSpinThrown>();
                    Item.shootSpeed = 6f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearSpinBoundary>();
                    Item.shootSpeed = 1f;
                    Item.useAnimation = 16;
                    Item.useTime = 16;
                    Item.useTurn = true;
                }
                else
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearThrown>();
                    Item.shootSpeed = 25f;
                    Item.useAnimation = 85;
                    Item.useTime = 85;
                }

                Item.DamageType = DamageClass.Ranged;
            }
            else
            {
                if (player.controlUp && !player.controlDown)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearSpin>();
                    Item.shootSpeed = 1f;
                    Item.useTurn = true;
                }
                else if (player.controlDown && !player.controlUp)
                {
                    Item.shoot = ModContent.ProjectileType<HentaiSpearDive>();
                    Item.shootSpeed = 6f;
                }
                else
                {
                    Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.HentaiSpear>();
                    Item.shootSpeed = 6f;
                }

                Item.useAnimation = 16;
                Item.useTime = 16;

                Item.DamageType = DamageClass.Melee;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // Right-click
            {
                if (player.controlUp)
                {
                    if (player.controlDown) // Giga-beam
                        return player.ownedProjectileCounts[Item.shoot] < 1;

                    if (player.ownedProjectileCounts[Item.shoot] < 1) // Remember to transfer any changes here to hentaispearspinthrown!
                    {
                        Vector2 speed = Main.MouseWorld - player.MountedCenter;

                        if (speed.Length() < 360)
                            speed = Vector2.Normalize(speed) * 360;

                        Projectile.NewProjectile(source, position, Vector2.Normalize(speed), Item.shoot, damage, knockback, player.whoAmI, speed.X, speed.Y);
                    }

                    return false;
                }

                return true;
            }

            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                if (player.controlUp && !player.controlDown)
                    return true;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<Dash>()] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<Dash2>()] < 1)
                {
                    float dashAI = 0;
                    float speedModifier = 2f;
                    int dashType = ModContent.ProjectileType<Dash>();

                    if (player.controlUp && player.controlDown) // Super-dash
                    {
                        dashAI = 1;
                        speedModifier = 2.5f;
                    }

                    Vector2 speed = velocity;

                    if (player.controlDown && !player.controlUp) //dive
                    {
                        dashAI = 2;
                        speed = new Vector2(Math.Sign(velocity.X) * 0.0001f, speed.Length());
                        dashType = ModContent.ProjectileType<Dash2>();
                    }

                    int p = Projectile.NewProjectile(source, position, Vector2.Normalize(speed) * speedModifier * Item.shootSpeed,
                        dashType, damage, knockback, player.whoAmI, speed.ToRotation(), dashAI);
                    if (p != Main.maxProjectiles)
                        Projectile.NewProjectile(source, position, speed, Item.shoot, damage, knockback, player.whoAmI, Main.projectile[p].identity, 1f);
                }
            }

            return false;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                var lineshader = GameShaders.Misc["PulseUpwards"].UseColor(new Color(28, 222, 152)).UseSecondaryColor(new Color(168, 245, 228));
                lineshader.Apply();
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerMoon"))
                .AddIngredient(ModContent.ItemType<EternalEnergy>(), 30)
                .AddIngredient(ModContent.ItemType<AbomEnergy>(), 30)
                .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 30)
                .AddIngredient(ModContent.ItemType<PhantasmalEnergy>())
                .AddIngredient(ModContent.ItemType<Accessories.Masomode.MutantEye>())
                .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
                .Register();
        }
    }
}