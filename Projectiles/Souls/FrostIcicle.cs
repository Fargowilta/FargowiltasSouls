using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class FrostIcicle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Icicle");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 14;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            Projectile.timeLeft++;
            Projectile.netUpdate = true;

            if (player.whoAmI == Main.myPlayer && player.dead)
            {
                modPlayer.FrostEnchantActive = false;
            }

            if (player.whoAmI == Main.myPlayer && (!player.GetToggleValue("Frost") || !modPlayer.FrostEnchantActive))
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                //rotation mumbo jumbo
                float distanceFromPlayer = 32;

                Projectile.position = player.Center + new Vector2(distanceFromPlayer, 0f).RotatedBy(Projectile.ai[1]);
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;

                float rotation = (float)Math.PI / 60;
                Projectile.ai[1] += rotation;
                if (Projectile.ai[1] > (float)Math.PI)
                {
                    Projectile.ai[1] -= 2f * (float)Math.PI;
                    Projectile.netUpdate = true;
                }

                Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation() - 5;
            }

            if (Main.netMode == NetmodeID.Server)
                Projectile.netUpdate = true;
        }

        public override void Kill(int timeLeft)
        {
            Main.player[Projectile.owner].GetModPlayer<FargoSoulsPlayer>().IcicleCount--;
        }
    }
}