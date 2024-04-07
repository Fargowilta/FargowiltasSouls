using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class RefractorBlaster : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Refractor Blaster");
            // Tooltip.SetDefault("'Modified from the arm of a defeated foe..'");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "变轨激光炮");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'由一个被击败的敌人的手臂改装而来..'");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LaserRifle);
            Item.damage = 30;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.shootSpeed = 15f;
            Item.value = 100000;
            Item.rare = ItemRarityID.Pink;
            //Item.mana = 10;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ModContent.ProjectileType<PrimeLaser>();

            int p = Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, velocity, type, damage, knockback, player.whoAmI);

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
                    int factor = j == 0 ? 1 : -1;
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, projectile.damage, projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                }
            }

            projectile.active = false;
        }
    }
}
