using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomScytheSpin : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_274";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 420;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item71, projectile.Center);
            }

            if (projectile.timeLeft == 390)
            {
                projectile.velocity = Vector2.Zero;
                projectile.netUpdate = true;
            }
            else if (projectile.timeLeft == 360)
            {
                Main.PlaySound(SoundID.Item84, projectile.Center);
            }
            else if (projectile.timeLeft < 360)
            {
                NPC abom = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.AbomBoss.AbomBoss>());
                if (abom == null)
                {
                    projectile.Kill();
                    return;
                }
                Vector2 pivot = abom.Center;
                projectile.velocity = (pivot - projectile.Center).RotatedBy(Math.PI / 2 * projectile.ai[1]);
                projectile.velocity *= 2 * (float)Math.PI / 360;
            }

            projectile.spriteDirection = (int)projectile.ai[1];
            projectile.rotation += projectile.spriteDirection * 0.5f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item71, projectile.Center);
            for (int index1 = 0; index1 < 20; ++index1) //put some dust here ig
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Shadowflame, 0.0f, 0.0f, 0, new Color(), 1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].scale++;
                Main.dust[index2].velocity *= 4f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) //fire at player
            {
                int p = Player.FindClosest(projectile.Center, 0, 0);
                if (p != -1)
                {
                    Vector2 speed = 15f * projectile.DirectionTo(Main.player[p].Center);
                    Projectile.NewProjectile(projectile.Center, speed, ModContent.ProjectileType<AbomSickle3>(), projectile.damage, projectile.knockBack, projectile.owner, p);
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(mod.BuffType("Unstable"), 240);
                target.AddBuff(mod.BuffType("Berserked"), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
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
                Color color27 = color26;
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
            return Color.White;
        }
    }
}