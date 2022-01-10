using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class BoneZone : SoulsItem
    {
        private int counter = 1;

        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("The Bone Zone");
            Tooltip.SetDefault("Uses bones for ammo" +
                "\n33% chance to not consume ammo" +
                "\n'The shattered remains of a defeated foe..'");

            DisplayName.AddTranslation(GameCulture.Chinese, "骸骨领域");
            Tooltip.AddTranslation(GameCulture.Chinese, "'被击败的敌人的残骸..'");*/
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.ranged = true;
            item.width = 54;
            item.height = 14;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item2;
            item.value = 50000;
            item.rare = ItemRarityID.Orange;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("Bonez");
            item.shootSpeed = 5.5f;
            item.useAmmo = ItemID.Bone;
        }

        // Manually reposition the item when held out
        public override Vector2? HoldoutOffset() => new Vector2(-30, 4);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int shoot;

            if (counter > 2)
            {
                shoot = ProjectileID.ClothiersCurse;
                counter = 0;
            }
            else
                shoot = ModContent.ProjectileType<Bonez>();

            Main.projectile[Projectile.NewProjectile(position.X, position.Y, speedX, speedY, shoot, damage, knockBack, player.whoAmI)].ranged = true;

            counter++;

            return false;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(3) != 0;
    }
}
