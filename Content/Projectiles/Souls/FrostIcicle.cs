using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class FrostIcicle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Frost Icicle");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.FargoSouls().CanSplit = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            Projectile.timeLeft++;
            Projectile.netUpdate = true;

            if (player.whoAmI == Main.myPlayer && !player.HasEffect<SnowEffect>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.localAI[2] = player.HasEffect<FrostEffect>() ? 0 : 1;

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

        public override void OnKill(int timeLeft)
        {
            Main.player[Projectile.owner].FargoSouls().IcicleCount--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.localAI[2] == 0 ? Projectile.type : ProjectileID.SnowBallFriendly].Value;
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, texture);
            return false;
        }
    }
}