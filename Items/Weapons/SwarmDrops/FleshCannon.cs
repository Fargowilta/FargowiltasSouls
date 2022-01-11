using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class FleshCannon : SoulsItem
    {
        public int counter;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Regurgitator");
            //Tooltip.SetDefault("'The enslaved face of a defeated foe...'");
        }

        public override void SetDefaults()
        {
            item.damage = 300;
            item.magic = true;
            item.channel = true;
            item.mana = 6;
            item.width = 24;
            item.height = 24;
            item.useTime = 8;
            item.useAnimation = 8;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item12;
            item.value = Item.sellPrice(0, 10);
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("Hungry2");
            item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            const int FACTOR = 14; // (Make sure this is even)

            Vector2 speed = new Vector2(speedX, speedY);

            counter++;
            if (player.ownedProjectileCounts[type] < 1 && counter % (FACTOR / 2) == 0)
            {
                Projectile.NewProjectile(position, speed * 2f, type, damage, knockBack, player.whoAmI, 0f, damage);
                Main.PlaySound(new LegacySoundStyle(4, 13), position);
            }

            float rotation = MathHelper.ToRadians(10) * (float)Math.Sin((counter + 0.2) * Math.PI / (FACTOR / 2));
            Projectile.NewProjectile(position, speed.RotatedBy(rotation) * 0.4f, ProjectileID.PurpleLaser, damage, knockBack, player.whoAmI);
            Projectile.NewProjectile(position, speed.RotatedBy(-rotation) * 0.4f, mod.ProjectileType("FleshLaser"), damage, knockBack, player.whoAmI);

            if (counter >= FACTOR) //reset
                counter = 0;

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "FleshHand");
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerWall"));
            recipe.AddIngredient(ItemID.LunarBar, 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}