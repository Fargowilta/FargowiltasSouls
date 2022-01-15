using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BrainIllusionProj : ModProjectile
    {
        public override string Texture => "Terraria/NPC_266";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Brain of Cthulhu");
            DisplayName.AddTranslation(GameCulture.Chinese, "克苏鲁之脑");
            Main.projFrames[projectile.type] = Main.npcFrameCount[NPCID.BrainofCthulhu];
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 120;
            projectile.height = 80;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.penetrate = -1;

            projectile.scale += 0.25f;
        }

        public override bool CanDamage()
        {
            return projectile.ai[1] == 2f;
        }

        private const int attackDelay = 120;

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], NPCID.BrainofCthulhu);
            if (npc == null)
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.projectileTexture[projectile.type] = Main.npcTexture[NPCID.BrainofCthulhu];
            }

            if (++projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
            }

            if (projectile.frame < 4 || projectile.frame > 7)
                projectile.frame = 4;

            if (projectile.ai[1] == 0f)
            {
                projectile.alpha = (int)(255f * npc.life / npc.lifeMax);
            }
            else if (projectile.ai[1] == 1f)
            {
                projectile.alpha = (int)MathHelper.Lerp(projectile.alpha, 0, 0.02f);

                projectile.position += 0.5f * (Main.player[npc.target].position - Main.player[npc.target].oldPosition) * (1f - projectile.localAI[0] / attackDelay);
                projectile.velocity = Vector2.Zero;
                projectile.timeLeft = 180;

                if (++projectile.localAI[0] > attackDelay)
                {
                    projectile.ai[1] = 2f;
                    projectile.velocity = 18f * projectile.DirectionTo(Main.player[npc.target].Center);
                    projectile.netUpdate = true;

                    projectile.localAI[0] = Main.player[npc.target].Center.X;
                    projectile.localAI[1] = Main.player[npc.target].Center.Y;
                }
            }
            else
            {
                projectile.alpha = 0;
                projectile.velocity *= 1.015f;

                if (projectile.Distance(new Vector2(projectile.localAI[0], projectile.localAI[1])) < projectile.velocity.Length() + 1f)
                {
                    projectile.Kill();
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            /*if (projectile.ai[1] == 2f)
            {
                Main.PlaySound(SoundID.NPCDeath1);

                for (int i = 0; i < 25; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 5);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 2f;
                }
            }*/
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color = projectile.GetAlpha(lightColor);

            if (CanDamage())
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
                {
                    Color color27 = color * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                    Vector2 value4 = projectile.oldPos[i];
                    float num165 = projectile.oldRot[i];
                    Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
                }
            }

            Vector2 warningShake;
            if (projectile.ai[1] == 1f)
            {
                float radius = 16f * projectile.localAI[0] / attackDelay;
                warningShake = Main.rand.NextVector2Circular(radius, radius);
            }
            else
            {
                warningShake = Vector2.Zero;
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center + warningShake - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}