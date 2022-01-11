using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class HellZone : SoulsItem
    {
        public int skullTimer;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Hell Zone");
            //Tooltip.SetDefault("Uses bones for ammo\n80% chance to not consume ammo\n'The reward for slaughtering many...'");
            //DisplayName.AddTranslation(GameCulture.Chinese, "地狱领域");
            //Tooltip.AddTranslation(GameCulture.Chinese, "'屠戮众多的奖励...'");
        }

        public override void SetDefaults()
        {
            item.damage = 230; //
            item.knockBack = 4f;
            item.shootSpeed = 12f; //

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.useAnimation = 5; //
            item.useTime = 5; //
            item.width = 54;
            item.height = 14;
            item.shoot = ModContent.ProjectileType<HellSkull2>();
            item.useAmmo = ItemID.Bone;
            item.UseSound = SoundID.Item38;//SoundID.Item34;

            item.noMelee = true;
            item.value = Item.sellPrice(0, 10); //
            item.rare = ItemRarityID.Purple; //
            item.ranged = true;
        }

        private int counter;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            /*Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (--skullTimer < 0)
            {
                skullTimer = 10;
                //float ai = Main.rand.NextFloat((float)Math.PI * 2);
                Projectile.NewProjectile(position, 1.5f * new Vector2(speedX, speedY), mod.ProjectileType("HellSkull"), damage / 2, knockBack, player.whoAmI, -1);
            }*/

            Vector2 speed = new Vector2(speedX, speedY);
            position += Vector2.Normalize(speed) * 40f;
            int max = Main.rand.Next(1, 4);
            float rotation = MathHelper.Pi / 4f / max * Main.rand.NextFloat(0.25f, 0.75f) * 0.75f;
            counter++;
            for (int i = -max; i <= max; i++)
            {
                int newType;
                switch (Main.rand.Next(3))
                {
                    case 0: newType = ModContent.ProjectileType<HellBone>(); break;
                    case 1: newType = ModContent.ProjectileType<HellBonez>(); break;
                    default: newType = ModContent.ProjectileType<HellSkeletron>(); break;
                }
                Projectile.NewProjectile(position, Main.rand.NextFloat(0.8f, 1.2f) * speed.RotatedBy(rotation * i + Main.rand.NextFloat(-rotation, rotation)), newType, damage, knockBack, player.whoAmI);
            }
            if (counter > 4)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    Projectile.NewProjectile(position, speed * 1.25f, ModContent.ProjectileType<HellSkull2>(), damage, knockBack, player.whoAmI, 0, j);
                }
                counter = 0;
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return Main.rand.NextBool(5);
        }

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, -5);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BoneZone");
            recipe.AddIngredient(ModLoader.GetMod("Fargowiltas").ItemType("EnergizerSkele"));
            recipe.AddIngredient(ItemID.LunarBar, 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}