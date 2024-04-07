using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.SwarmDrops
{
    public class HellZone : SoulsItem
    {
        public int skullTimer;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Hell Zone");
            // Tooltip.SetDefault("Uses bones for ammo\n80% chance to not consume ammo\n'The reward for slaughtering many...'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "地狱领域");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'屠戮众多的奖励...'");
        }
        private static readonly int[] RiffVariants = new int[4] { 1, 2, 3, 4 };
        private static readonly SoundStyle badtothebone = new SoundStyle("FargowiltasSouls/Assets/Sounds/Boneriff/boneriff") with { Variants = RiffVariants, Volume = 0.2f, MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.IgnoreNew };
        public override void SetDefaults()
        {
            Item.damage = 250; //
            Item.knockBack = 4f;
            Item.shootSpeed = 12f; //

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.useAnimation = 4;
            Item.useTime = 4;
            Item.width = 54;
            Item.height = 14;
            Item.shoot = ModContent.ProjectileType<HellSkull2>();
            Item.useAmmo = ItemID.Bone;
            Item.UseSound = badtothebone;//SoundID.Item38;//SoundID.Item34;

            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 10); //
            Item.rare = ItemRarityID.Purple; //
            Item.DamageType = DamageClass.Ranged;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useAnimation = 40;
                Item.useTime = 40;
            }
            else
            {
                Item.useAnimation = 4;
                Item.useTime = 4;
            }
            return base.CanUseItem(player);
        }

        private int counter;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += Vector2.Normalize(velocity) * 40f;
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<HellGuardian>();
                damage *= 4;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                return true;

            int max = Main.rand.Next(1, 4);
            float rotation = MathHelper.Pi / 4f / max * Main.rand.NextFloat(0.25f, 0.75f) * 0.75f;
            counter++;
            for (int i = -max; i <= max; i++)
            {
                var newType = Main.rand.Next(3) switch
                {
                    0 => ModContent.ProjectileType<HellBone>(),
                    1 => ModContent.ProjectileType<HellBonez>(),
                    _ => ModContent.ProjectileType<HellSkeletron>(),
                };
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