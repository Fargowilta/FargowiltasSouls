using FargowiltasSouls.Content.Projectiles.JungleMimic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Misc
{
    public class Mahoguny : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("Mahoguny");
            /* Tooltip.SetDefault("Uses acorns as ammo\n" +
            "Fires leaves and acorns"); */
        }

        public override void SetDefaults()
        {
            Item.damage = 59;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 58;
            Item.height = 26;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(0, 8);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AcornProjectile>();
            Item.shootSpeed = 18f;
            Item.useAmmo = ItemID.Acorn;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 46f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            {
                //Item.damage = 80;
                int numberProjectiles = 2 + Main.rand.Next(2);
                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                    Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<MahogunyLeafProjectile>(), (int)(damage * 0.6), knockback, player.whoAmI);
                }
            }

            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, -3);
        }
    }
}