using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Items.Materials;
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
    public class StyxGazer : SoulsItem
    {
        public bool flip;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Styx Gazer");
            Tooltip.SetDefault(@"Right click to wield a blade of infernal magic
'The blazing scythe wand sword destruction ray of a defeated foe...'");
        }

        public override void SetDefaults()
        {
            Item.damage = 1700;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.shootSpeed = 16f;
            Item.knockBack = 14f;
            Item.width = 20;
            Item.height = 20;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.StyxScythe>();
            Item.value = Item.sellPrice(0, 70);
            //Item.noMelee = true; //no melee hitbox
            //Item.noUseGraphic = true; //dont draw Item
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.StyxGazer>();
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.DamageType = DamageClass.Magic;
                Item.noUseGraphic = true;
                Item.noMelee = true;
                Item.mana = 200;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<Projectiles.BossWeapons.StyxScythe>();
                Item.useStyle = ItemUseStyleID.Swing;
                Item.DamageType = DamageClass.Melee;
                Item.noUseGraphic = false;
                Item.noMelee = false;
                Item.mana = 0;
            }
            return true;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                var lineshader = GameShaders.Misc["PulseDiagonal"].UseColor(new Color(255, 170, 12)).UseSecondaryColor(new Color(210, 69, 203));
                lineshader.Apply(null);
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override string Texture => base.Texture;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            flip = !flip;

            if (player.altFunctionUse == 2) //right click
            {
                velocity = velocity.RotatedBy(Math.PI / 2 * (flip ? 1 : -1));
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (float)Math.PI / 120 * (flip ? -1 : 1));
            }
            else
            {
                const int max = 5;
                for (int i = 0; i < max; i++)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(2 * Math.PI / max * i), type,
                        damage, knockback, player.whoAmI, 0, (Main.MouseWorld - position).Length() * (flip ? 1 : -1));
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<EternalEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 30)
            .AddIngredient(ModContent.ItemType<BrokenHilt>())
            .AddIngredient(ModContent.ItemType<AbominableWand>())

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}