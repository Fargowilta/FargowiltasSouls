using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class DragonBreath2 : SoulsItem
    {
        public int skullTimer;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Dragon's Demise");
            /*Tooltip.SetDefault(@"Uses gel for ammo
66% chance to not consume ammo
'The reward for slaughtering many..'");*/
        }

        public override void SetDefaults()
        {
            item.damage = 180;
            item.knockBack = 1f;
            item.shootSpeed = 12f;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.useAnimation = 30;
            item.useTime = 3;
            item.width = 54;
            item.height = 14;
            item.shoot = mod.ProjectileType("HellFlame");
            item.useAmmo = AmmoID.Gel;
            item.UseSound = SoundID.DD2_BetsyFlameBreath;

            item.noMelee = true;
            item.value = Item.sellPrice(0, 15);
            item.rare = ItemRarityID.Purple;
            item.ranged = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 speed = new Vector2(speedX, speedY);
            Projectile.NewProjectile(position + Vector2.Normalize(speed) * 60f, speed, type, damage, knockBack, player.whoAmI);
            if (--skullTimer < 0)
            {
                skullTimer = 5;
                Main.PlaySound(SoundID.DD2_BetsyFireballShot);
                //float ai = Main.rand.NextFloat((float)Math.PI * 2);
                /*for (int i = 0; i <= 4; i++)
                {
                    int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedByRandom(MathHelper.Pi / 18),
                        mod.ProjectileType("DragonFireball"), damage * 3, knockBack, player.whoAmI);
                    Main.projectile[p].netUpdate = true;
                }*/
                Projectile.NewProjectile(position, 2f * new Vector2(speedX, speedY),//.RotatedByRandom(MathHelper.Pi / 18),
                    mod.ProjectileType("DragonFireball"), damage, knockBack * 6f, player.whoAmI);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextBool(3);
        }

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DragonBreath");
            recipe.AddIngredient(null, "AbomEnergy", 10);
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerBetsy"));
            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}