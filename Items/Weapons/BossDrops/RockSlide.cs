using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.BossWeapons;
using System.Collections.Generic;
using System.Linq;
using FargowiltasSouls.Utilities;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class RockSlide : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Rockslide");
            Tooltip.SetDefault("'The crumbling remains of a defeated foe..'");

            DisplayName.AddTranslation(GameCulture.Chinese, "山崩");
            Tooltip.AddTranslation(GameCulture.Chinese, "'被击败的敌人的破碎残骸'");
        }

        public override void SetDefaults()
        {
            item.damage = 88;
            item.magic = true;
            item.width = 24;
            item.height = 28;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.value = 100000;
            item.rare = ItemRarityID.Yellow;
            item.mana = 10;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<GolemGib>();
            item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY,
            ref int type, ref int damage, ref float knockBack)
        {
            float itemShootSpeed = item.shootSpeed;
            int itemDamage = item.damage;
            float itemKnockBack = item.knockBack;
            itemKnockBack = player.GetWeaponKnockback(item, itemKnockBack);
            player.itemTime = item.useTime;

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
                sqrtSpeed = itemShootSpeed;
            }
            else
                sqrtSpeed = itemShootSpeed / sqrtSpeed;

            localX *= sqrtSpeed;
            localY *= sqrtSpeed;

            int projCount = 2;

            if (Main.rand.NextBool(2))
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
                sqrtSpeed = itemShootSpeed / sqrtSpeed;

                localProjX *= sqrtSpeed;
                localProjY *= sqrtSpeed;

                Projectile.NewProjectile(position.X, position.Y, localProjX, localProjY, ModContent.ProjectileType<GolemGib>(), itemDamage, itemKnockBack, Main.myPlayer, 0, Main.rand.Next(1, 12));
            }

            return false;
        }
    }
}