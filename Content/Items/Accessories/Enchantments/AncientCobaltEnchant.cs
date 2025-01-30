﻿using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AncientCobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(53, 76, 116);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AncientCobaltEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ItemID.AncientCobaltHelmet)
            .AddIngredient(ItemID.AncientCobaltBreastplate)
            .AddIngredient(ItemID.AncientCobaltLeggings)
            .AddIngredient(ItemID.Bomb, 10)
            .AddIngredient(ItemID.Dynamite, 10)
            .AddIngredient(ItemID.Grenade, 10)

            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }

    public class AncientCobaltEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<AncientCobaltEnchant>();

        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.CobaltImmuneTimer > 0)
            {
                player.immune = true;
                modPlayer.CobaltImmuneTimer--;
            }
            if (modPlayer.CobaltCooldownTimer > 0)
            {
                modPlayer.CobaltCooldownTimer--;
            }


            if (player.jump <= 0 && player.velocity.Y == 0f)
            {
                modPlayer.CanCobaltJump = true;
                modPlayer.JustCobaltJumped = false;
            }
            else
            {
                modPlayer.CanCobaltJump = false;
            }

            if (player.controlJump && player.releaseJump && modPlayer.CanCobaltJump && !modPlayer.JustCobaltJumped && modPlayer.CobaltCooldownTimer <= 0)
            {
                bool upgrade = EffectItem(player).type != ModContent.ItemType<AncientCobaltEnchant>() || player.ForceEffect<AncientCobaltEffect>();

                int projType = ModContent.ProjectileType<CobaltExplosion>();
                int damage = 100;
                if (upgrade) 
                    damage = 250;

                Projectile.NewProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, projType, damage, 0, player.whoAmI);

                modPlayer.JustCobaltJumped = true;

                int time = upgrade ? 15 : 8;

                if (modPlayer.CobaltImmuneTimer <= 0)
                    modPlayer.CobaltImmuneTimer = time;

                if (modPlayer.CobaltCooldownTimer <= 10)
                    modPlayer.CobaltCooldownTimer = 10;
            }

            if (modPlayer.CanCobaltJump || modPlayer.JustCobaltJumped && !player.GetJumpState(ExtraJump.CloudInABottle).Active && !player.GetJumpState(ExtraJump.BlizzardInABottle).Active && !player.GetJumpState(ExtraJump.FartInAJar).Active && !player.GetJumpState(ExtraJump.TsunamiInABottle).Active && !player.GetJumpState(ExtraJump.SandstormInABottle).Active && !modPlayer.JungleJumping)
            {
                if (player.HasEffect<CobaltEffect>())
                {
                    player.jumpSpeedBoost += 10f;
                }
                else
                {
                    player.jumpSpeedBoost += 5f;
                }
            }
        }
    }

}
