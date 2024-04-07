using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class RockSlide : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            // DisplayName.SetDefault("The Rockslide");
            // Tooltip.SetDefault("'The crumbling remains of a defeated foe..'");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "山崩");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'被击败的敌人的破碎残骸'");
        }

        public override void SetDefaults()
        {
            Item.damage = 88;
            Item.DamageType = DamageClass.Magic;
            Item.width = 38;
            Item.height = 46;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.value = 100000;
            Item.rare = ItemRarityID.Yellow;
            Item.mana = 10;
            Item.UseSound = SoundID.Item21;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GolemGib>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ItemShootSpeed = Item.shootSpeed;
            int ItemDamage = Item.damage;
            float ItemKnockBack = Item.knockBack;
            ItemKnockBack = player.GetWeaponKnockback(Item, ItemKnockBack);
            player.itemTime = Item.useTime;

            Vector2 mountedCenterRotation = player.RotatedRelativePoint(player.MountedCenter);
            Vector2.UnitX.RotatedBy(player.fullRotation);

            float localX = Main.mouseX + Main.screenPosition.X - mountedCenterRotation.X;
            float localY = Main.mouseY + Main.screenPosition.Y - mountedCenterRotation.Y;

            if (player.gravDir == -1f)
                localY = Main.screenPosition.Y + Main.screenHeight - Main.mouseY - mountedCenterRotation.Y;

            float sqrtSpeed = (float)Math.Sqrt(localX * localX + localY * localY);

            if (float.IsNaN(localX) && float.IsNaN(localY) || localX == 0f && localY == 0f)
            {
                localX = player.direction;
                localY = 0f;
                sqrtSpeed = ItemShootSpeed;
            }
            else
                sqrtSpeed = ItemShootSpeed / sqrtSpeed;

            localX *= sqrtSpeed;
            localY *= sqrtSpeed;

            int projCount = 2;

            if (Main.rand.NextBool())
                projCount++;

            if (Main.rand.NextBool(4))
                projCount++;

            if (Main.rand.NextBool(8))
                projCount++;

            if (Main.rand.NextBool(16))
                projCount++;

            for (int i = 0; i < projCount; i++)
            {
                float localProjX = localX;
                float localProjY = localY;
                float multiplier = 0.05f * i;
                localProjX += Main.rand.Next(-25, 26) * multiplier;
                localProjY += Main.rand.Next(-25, 26) * multiplier;

                sqrtSpeed = (float)Math.Sqrt(localProjX * localProjX + localProjY * localProjY);
                sqrtSpeed = ItemShootSpeed / sqrtSpeed;

                localProjX *= sqrtSpeed;
                localProjY *= sqrtSpeed;

                Projectile.NewProjectile(player.GetSource_ItemUse(Item), position.X, position.Y, localProjX, localProjY, ModContent.ProjectileType<GolemGib>(), ItemDamage, ItemKnockBack, Main.myPlayer, 0, Main.rand.Next(1, 12));
            }

            return false;
        }
    }
}