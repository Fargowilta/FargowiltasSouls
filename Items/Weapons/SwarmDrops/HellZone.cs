using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.SwarmDrops
{
    public class HellZone : SoulsItem
    {
        public int skullTimer;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Hell Zone");
            Tooltip.SetDefault("Uses bones for ammo\n80% chance to not consume ammo\n'The reward for slaughtering many...'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "地狱领域");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励...'");
        }

        public override void SetDefaults()
        {
            Item.damage = 276; //
            Item.knockBack = 4f;
            Item.shootSpeed = 12f; //

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 5; //
            Item.useTime = 5; //
            Item.width = 54;
            Item.height = 14;
            Item.shoot = ModContent.ProjectileType<HellSkull2>();
            Item.useAmmo = ItemID.Bone;
            Item.UseSound = SoundID.Item38;//SoundID.Item34;

            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 10); //
            Item.rare = ItemRarityID.Purple; //
            Item.DamageType = DamageClass.Ranged;
        }

        private int counter;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            /*Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (--skullTimer < 0)
            {
                skullTimer = 10;
                //float ai = Main.rand.NextFloat((float)Math.PI * 2);
                Projectile.NewProjectile(position, 1.5f * new Vector2(speedX, speedY), ModContent.ProjectileType<HellSkull>(), damage / 2, knockBack, player.whoAmI, -1);
            }*/

            position += Vector2.Normalize(velocity) * 40f;
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
                Projectile.NewProjectile(source, position, Main.rand.NextFloat(0.8f, 1.2f) * velocity.RotatedBy(rotation * i + Main.rand.NextFloat(-rotation, rotation)), newType, damage, knockback, player.whoAmI);
            }
            if (counter > 4)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    Projectile.NewProjectile(source, position, velocity * 1.25f, ModContent.ProjectileType<HellSkull2>(), damage, knockback, player.whoAmI, 0, j);
                }
                counter = 0;
            }
            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(5);

        //make them hold it different
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-30, -5);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "BoneZone")
            .AddIngredient(ModContent.Find<ModItem>("Fargowiltas", "EnergizerSkele"))
            .AddIngredient(ItemID.LunarBar, 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}