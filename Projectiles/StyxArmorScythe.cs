using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class StyxArmorScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Styx Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;

            projectile.timeLeft = 300;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().CanSplit = false;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().TimeFreezeImmune = true;
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
            const int baseDamage = 100;

            if (projectile.velocity == Vector2.Zero || projectile.velocity.HasNaNs())
                projectile.velocity = -Vector2.UnitY;

            Player player = Main.player[projectile.owner];
            
            if (!projectile.friendly || projectile.hostile || !player.active || player.dead || player.ghost || !player.GetModPlayer<FargoPlayer>().StyxSet)
            {
                projectile.Kill();
                return;
            }

            projectile.timeLeft = 240;
            projectile.damage = (int)(baseDamage * player.meleeDamage);

            projectile.Center = player.Center;

            if (player.ownedProjectileCounts[projectile.type] > 0)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    int threshold = projectile.localAI[1] == 0 ? 300 : 5; //check more often when something seems off, check slower when everything seems normal
                    if (++projectile.localAI[0] > threshold)
                    {
                        projectile.localAI[0] = 0;
                        projectile.localAI[1] = 0;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == projectile.type && Main.projectile[i].owner == projectile.owner && projectile.whoAmI != i)
                            {
                                if (projectile.localAI[0] == Main.projectile[i].localAI[0])
                                    projectile.localAI[0] += 5; //deliberately desync
                                if (projectile.ai[0] == Main.projectile[i].ai[0])
                                {
                                    projectile.ai[0]++;
                                    projectile.localAI[1] = 1;
                                }
                            }
                        }
                        projectile.netUpdate = true;
                    }

                    if (projectile.ai[0] >= player.ownedProjectileCounts[projectile.type])
                    {
                        projectile.ai[0] = 0;
                        projectile.localAI[1] = 1;
                    }
                }

                Vector2 target = -150f * Vector2.UnitY.RotatedBy(MathHelper.TwoPi / player.ownedProjectileCounts[projectile.type] * projectile.ai[0]);
                projectile.velocity = Vector2.Lerp(projectile.velocity, target, 0.1f);
            }

            projectile.rotation += 1f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item71, projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 70, Scale: 2f);
                Main.dust[d].velocity *= 6f;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibble>(), 300);
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

            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, spriteEffects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = (projectile.ai[0] < 0 || Main.player[projectile.owner].ownedProjectileCounts[projectile.type] >= 12 ? Color.Yellow : Color.Purple) * projectile.Opacity;
            color.A = 0;
            return color;
        }
    }
}