using FargowiltasSouls.Content.Items.BossBags;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class NavalRustrifle : SoulsItem
    {

        public override void SetStaticDefaults()
        {

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 116;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 82;
            Item.height = 24;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 15;
            Item.value = Item.sellPrice(0, 5);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item40;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder; //guns just have this, don't ask
            Item.shootSpeed = 30f;

            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-0, -0);
        }
        const int ShotType = ProjectileID.BulletHighVelocity;
        bool EmpoweredShot = false;
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (!modPlayer.RustRifleReloading)
            {
                player.itemRotation = (-Vector2.UnitY * player.direction).RotatedBy(player.direction * MathHelper.Pi / 4f).ToRotation();

                player.itemLocation = (Vector2)player.HandPosition - (Item.Size / 2) -
                    player.itemRotation.ToRotationVector2() * player.direction * (float)Math.Sin(MathHelper.Pi * player.itemAnimation / (float)player.itemAnimationMax) * Item.Size.Length() / 2f;
            }
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (EmpoweredShot)
            {
                type = ShotType;
                damage *= 2;
            }
        }
        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (EmpoweredShot)
            {
                crit = 100;
            }
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            if (EmpoweredShot)
            {
                knockback *= 3f;
            }
        }
        public override bool CanUseItem(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.RustRifleReloading)
            {
                float reloadProgress = ReloadProgress(modPlayer.RustRifleTimer);
                if (Math.Abs(reloadProgress - modPlayer.RustRifleReloadZonePos) < 0.15f)
                {
                    EmpoweredShot = true;
                    SoundEngine.PlaySound(SoundID.Item149 with { Pitch = 0.5f }, player.Center);
                    Item.UseSound = SoundID.Item68;
                }
                else
                {
                    EmpoweredShot = false;
                    SoundEngine.PlaySound(SoundID.Unlock with { Pitch = 0f }, player.Center);
                    Item.UseSound = SoundID.Item40;
                }
                modPlayer.RustRifleReloading = false;
                modPlayer.RustRifleTimer = 0;

                modPlayer.RustRifleReloadZonePos = 0;
                player.reuseDelay = 20;
                player.FargoSouls().RustRifleTimer = 0;
                //Timer = 0;
                return false;
            }
            return base.CanUseItem(player);
        }
        public override bool? UseItem(Player player)
        {
            player.FargoSouls().RustRifleReloading = true;
            player.FargoSouls().RustRifleReloadZonePos = 0.725f;
            player.FargoSouls().RustRifleTimer = 0;
            //Timer = 0;
            return base.UseItem(player);
        }

        public static float ReloadProgress(float timer)
        {
            return (1 + (float)Math.Sin(MathHelper.Pi * (timer - 30) / 60f)) / 2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<BanishedBaronBag>(2).AddTile(TileID.Solidifier).DisableDecraft().Register();
        }
    }
}