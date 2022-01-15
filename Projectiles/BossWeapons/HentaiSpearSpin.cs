using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class HentaiSpearSpin : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 164;
            projectile.height = 164;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.3f;
            projectile.hide = true;
            projectile.melee = true;
            projectile.alpha = 0;
            projectile.timeLeft = 45;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
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
            if (projectile.owner == Main.myPlayer && (!player.controlUseItem || (player.controlUp && player.controlDown)))
            {
                projectile.Kill();
                return;
            }

            if (player.dead || !player.active)
            {
                projectile.Kill();
                return;
            }

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2; //15;
            player.itemAnimation = 2; //15;
            player.reuseDelay = 10;
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

            if (++projectile.localAI[0] > 10) //6 if set duration?
            {
                projectile.localAI[0] = 0;
                Main.PlaySound(SoundID.Item1, projectile.Center);
                if (projectile.owner == Main.myPlayer && !Main.LocalPlayer.controlUseTile)
                {
                    Vector2 speed = -Vector2.UnitY.RotatedByRandom(Math.PI / 2) * Main.rand.NextFloat(9f, 12f);
                    float ai1 = Main.rand.Next(30, 60);
                    Projectile.NewProjectile(projectile.position + Main.rand.NextVector2Square(0f, projectile.width),
                        speed, ModContent.ProjectileType<PhantasmalEyeHoming>(), projectile.damage, projectile.knockBack / 2, projectile.owner, -1, ai1);
                }
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].hostile && Main.projectile[i].damage > 0
                    && projectile.Colliding(projectile.Hitbox, Main.projectile[i].Hitbox)
                    && FargoSoulsUtil.CanDeleteProjectile(Main.projectile[i]))
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        //Vector2 offset = Main.projectile[i].Center - Main.player[projectile.owner].Center;
                        //Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Souls.IronParry>(), 0, 0f, Main.myPlayer, offset.X, offset.Y);
                        Projectile.NewProjectile(Main.projectile[i].Center, Vector2.Zero, ModContent.ProjectileType<Souls.IronParry>(), 0, 0f, Main.myPlayer);
                    }

                    Main.projectile[i].hostile = false;
                    Main.projectile[i].friendly = true;
                    Main.projectile[i].owner = player.whoAmI;

                    // Turn away
                    Main.projectile[i].velocity = Main.projectile[i].DirectionFrom(player.Center) * Main.projectile[i].velocity.Length();

                    // Don't know if this will help but here it is
                    Main.projectile[i].netUpdate = true;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int clampedX = projHitbox.Center.X - targetHitbox.Center.X;
            int clampedY = projHitbox.Center.Y - targetHitbox.Center.Y;

            if (Math.Abs(clampedX) > targetHitbox.Width / 2)
                clampedX = targetHitbox.Width / 2 * Math.Sign(clampedX);
            if (Math.Abs(clampedY) > targetHitbox.Height / 2)
                clampedY = targetHitbox.Height / 2 * Math.Sign(clampedY);

            int dX = projHitbox.Center.X - targetHitbox.Center.X - clampedX;
            int dY = projHitbox.Center.Y - targetHitbox.Center.Y - clampedY;

            return Math.Sqrt(dX * dX + dY * dY) <= projectile.width / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 1; //balanceing

            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)),
                    Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), projectile.damage, projectile.knockBack * 3f, projectile.owner);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
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

            if (FargoSoulsWorld.MasochistModeReal)
            {
                for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 0.1f)
                {
                    Player player = Main.player[projectile.owner];
                    Texture2D glow = mod.GetTexture("Projectiles/BossWeapons/HentaiSpearSpinGlow");
                    Color color27 = Color.Lerp(new Color(51, 255, 191, 210), Color.Transparent, (float)Math.Cos(projectile.ai[0]) / 3 + 0.3f);
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    float scale = projectile.scale - (float)Math.Cos(projectile.ai[0]) / 5;
                    scale *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    int max0 = Math.Max((int)i - 1, 0);
                    Vector2 center = Vector2.Lerp(projectile.oldPos[(int)i], projectile.oldPos[max0], (1 - i % 1));
                    float smoothtrail = i % 1 * (float)Math.PI / 6.85f;
                    bool withinangle = projectile.rotation > -Math.PI / 2 && projectile.rotation < Math.PI / 2;
                    if (withinangle && player.direction == 1)
                        smoothtrail *= -1;
                    else if (!withinangle && player.direction == -1)
                        smoothtrail *= -1;

                    center += projectile.Size / 2;

                    Vector2 offset = (projectile.Size / 4).RotatedBy(projectile.oldRot[(int)i] - smoothtrail * (-projectile.direction));
                    Main.spriteBatch.Draw(
                        glow,
                        center - offset - Main.screenPosition + new Vector2(0, projectile.gfxOffY),
                        null,
                        color27,
                        projectile.rotation,
                        glow.Size() / 2,
                        scale * 0.4f,
                        SpriteEffects.None,
                        0f);
                }
            }

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