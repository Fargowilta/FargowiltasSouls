﻿using FargowiltasSouls.Content.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.BossDrops
{
    public class HiveStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TheSmallSting>();
            /*
            DisplayName.SetDefault("Hive Staff");
            Tooltip.SetDefault("'The enslaved minions of a defeated foe..'");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "蜂巢法杖");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "'战败敌人的仆从..'");
            */
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.mana = 10;
            Item.knockBack = 0.25f;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item78;
            Item.value = 50000;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<HiveSentry>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = Main.MouseWorld;

            Projectile.NewProjectile(source, mouse.X, mouse.Y - 10, 0f, 0f, type, damage, knockback, player.whoAmI);

            player.UpdateMaxTurrets();

            return false;
        }
    }
}