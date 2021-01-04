using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class RefractorBlaster : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Refractor Blaster");
            Tooltip.SetDefault("'Modified from the arm of a defeated foe..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "变轨激光炮");
            Tooltip.AddTranslation(GameCulture.Chinese, "'由一个被击败的敌人的手臂改装而来..'");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.LaserRifle);
            item.damage = 30;
            item.useTime = 24;
            item.useAnimation = 24;
            item.shootSpeed = 15f;
            item.value = 100000;
            item.rare = ItemRarityID.Pink;
            //item.mana = 10;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = ModContent.ProjectileType<PrimeLaser>();

            int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);

            if (p < 1000)
            {
                SplitProj(Main.projectile[p], 21);
            }

            return false;
        }

        //cuts out the middle 5: num of 21 means 8 proj on each side
        public static void SplitProj(Projectile projectile, int number)
        {
            //if its odd, we just keep the original
            if (number % 2 != 0)
            {
                number--;
            }

            double spread = MathHelper.Pi / 2 / number;

            for (int i = 2; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = (j == 0) ? 1 : -1;
                    Projectile.NewProjectile(projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                }
            }

            projectile.active = false;
        }
    }
}
