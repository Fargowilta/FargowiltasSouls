using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace FargowiltasSouls.Projectiles.ChallengerItems
{
    public class TheLightningRodProj : ModProjectile
    {
        public int damage;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Lightning Rod");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 132;
            projectile.height = 132;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.5f;
            projectile.hide = true;
            projectile.melee = true;
            projectile.alpha = 0;
            projectile.timeLeft = 45;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (projectile.owner == Main.myPlayer && !player.controlUseItem && projectile.ai[0] == 0f)
            {
                projectile.Kill();
                return;
            }

            if (player.dead || !player.active || player.ghost)
            {
                projectile.Kill();
                return;
            }

            projectile.localAI[0]++;

            if (projectile.localAI[0] % 20 == 0)
                Main.PlaySound(SoundID.Item1, projectile.Center);

            Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter);
            if (projectile.ai[0] == 0 && player.velocity.X != 0)
                player.ChangeDir(Math.Sign(player.velocity.X));
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2; //15;
            player.itemAnimation = 2; //15;
            //player.itemAnimationMax = 15;
            projectile.timeLeft = 2;

            const float maxRotation = MathHelper.Pi / 6.85f / 1.5f; //spin up to full speed
            float modifier = maxRotation * (projectile.ai[0] == 0 ? Math.Min(1f, projectile.localAI[0] / 80f) : 1f);

            if (projectile.ai[0] == 0f) //while held
            {
                damage = projectile.damage;
                projectile.numHits = 0;

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
                projectile.rotation += modifier * player.direction;
                projectile.velocity = projectile.rotation.ToRotationVector2();
                projectile.position -= projectile.velocity;
                player.itemRotation = MathHelper.WrapAngle(projectile.rotation);
                
                if (projectile.localAI[0] == 40)
                {
                    projectile.ai[1] = 1f;
                }
                else if (projectile.localAI[0] == 120) //time to throw
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        projectile.ai[0] = 1f;
                        projectile.localAI[0] = 0f;
                        projectile.localAI[1] = projectile.Distance(Main.MouseWorld);
                        if (projectile.localAI[1] < 200) //minimum throwing distance
                            projectile.localAI[1] = 200;
                        projectile.velocity = projectile.DirectionTo(Main.MouseWorld);
                        projectile.netUpdate = true;
                    }
                }
            }
            else //while thrown
            {
                projectile.damage = (int)(damage * Math.Pow(0.85, projectile.numHits));
                if (projectile.damage < damage / 2)
                    projectile.damage = damage / 2;

                const int maxTime = 80;

                if (projectile.localAI[0] > maxTime)
                {
                    projectile.Kill();
                }
                else //player faces where this was thrown
                {
                    projectile.direction = Math.Sign(projectile.Center.X - player.Center.X);
                    player.ChangeDir(projectile.direction);
                    player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, projectile.velocity.X * projectile.direction);
                }

                projectile.rotation += modifier * player.direction * 1.25f; //spin faster when thrown

                float distanceModifier = (float)Math.Sin(Math.PI / maxTime * projectile.localAI[0]); //fly out and back
                projectile.velocity = Vector2.Normalize(projectile.velocity) * distanceModifier * projectile.localAI[1];

                if (projectile.localAI[0] % 10 == 0 && projectile.owner == Main.myPlayer) //rain lightning
                {
                    Vector2 spawnPos = projectile.Center + Main.rand.NextVector2Circular(projectile.width / 2, projectile.height / 2);
                    Projectile.NewProjectile(spawnPos - Vector2.UnitY * 900, Vector2.UnitY * 7f, ModContent.ProjectileType<TheLightning>(),
                        projectile.damage, projectile.knockBack / 2, projectile.owner, Vector2.UnitY.ToRotation(), spawnPos.Y);
                }
            }

            if (projectile.ai[1] != 0f)
            {
                //dust!
                int dustId = Dust.NewDust(projectile.position, projectile.width, projectile.height, 156, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dustId].noGravity = true;
                int dustId3 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 156, 0f, 0f, 100, default(Color), 1f);
                Main.dust[dustId3].noGravity = true;
            }

            projectile.Center = ownerMountedCenter;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ai[0] == 0f)
                damage /= 2;

            hitDirection = Math.Sign(target.Center.X - Main.player[projectile.owner].Center.X);
        }

        public override bool CanDamage()
        {
            return projectile.ai[1] != 0f && Collision.CanHitLine(Main.player[projectile.owner].Center, 0, 0, projectile.Center, 0, 0);
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            if (projectile.ai[1] != 0f)
            {
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
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), 
                projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}