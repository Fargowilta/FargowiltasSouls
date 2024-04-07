using FargowiltasSouls.Content.Projectiles.Souls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AncientCobaltEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Ancient Cobalt Enchantment");
            /* Tooltip.SetDefault(
@"Grants an explosion jump that inflicts Oiled
'Ancient Kobold'"); */
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
                int projType = ModContent.ProjectileType<CobaltExplosion>();
                int damage = 100;
                if (player.HasEffect<CobaltEffect>()) damage = 250;

                Projectile.NewProjectile(player.GetSource_Accessory(player.EffectItem<AncientCobaltEffect>()), player.Center, Vector2.Zero, projType, damage, 0, player.whoAmI);

                modPlayer.JustCobaltJumped = true;

                if (modPlayer.CobaltImmuneTimer <= 0)
                    modPlayer.CobaltImmuneTimer = 15;

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
