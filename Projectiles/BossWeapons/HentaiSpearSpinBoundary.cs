using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearSpinBoundary : HentaiSpearSpin
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.melee = false;
            projectile.ranged = true;
        }

        public override void AI()
        {
            //dust!
            int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 15, projectile.velocity.X * 0.2f,
                projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            Player player = Main.player[projectile.owner];
            if (projectile.owner == Main.myPlayer && (!player.controlUseTile || player.altFunctionUse != 2 || (player.controlUp && player.controlDown)))
            {
                projectile.Kill();
                return;
            }

            if (player.dead || !player.active)
            {
                projectile.Kill();
                return;
            }

            player.velocity *= 0.9f; //move slower while holding it

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2; //15;
            player.itemAnimation = 2; //15;
            //player.itemAnimationMax = 15;
            projectile.Center = ownerMountedCenter;
            projectile.timeLeft = 2;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            projectile.rotation += (float)Math.PI / 6.85f * player.direction;
            projectile.ai[0] += MathHelper.Pi / 45;
            projectile.velocity = projectile.rotation.ToRotationVector2();
            projectile.position -= projectile.velocity;
            player.itemRotation = projectile.rotation;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            if (++projectile.localAI[0] > 2)
            {
                Main.PlaySound(SoundID.Item12, projectile.Center);
                projectile.localAI[0] = 0;
                projectile.localAI[1] += (float)Math.PI / 4 / 360 * ++projectile.ai[1] * player.direction;
                if (projectile.localAI[1] > (float)Math.PI)
                    projectile.localAI[1] -= (float)Math.PI * 2;
                if (projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(player.Center, new Vector2(0, -9f).RotatedBy(projectile.localAI[1] + Math.PI / 3 * i),
                            ModContent.ProjectileType<PhantasmalEyeBoundary>(), projectile.damage, projectile.knockBack / 2, projectile.owner);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),
                projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}