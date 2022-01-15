using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomScytheFlaming : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_329";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Abominationn Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 720;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            cooldownSlot = 1;
        }

        public override bool CanDamage()
        {
            return projectile.ai[1] <= 0;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
                projectile.localAI[1] = projectile.ai[1] - projectile.ai[0]; //store difference for animated spin startup
                projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            if (--projectile.ai[0] == 0)
            {
                projectile.netUpdate = true;
                projectile.velocity = Vector2.Zero;
            }

            if (--projectile.ai[1] == 0)
            {
                projectile.netUpdate = true;
                Player target = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)];
                projectile.velocity = projectile.DirectionTo(target.Center);
                if (FargoSoulsUtil.BossIsAlive(ref NPCs.EModeGlobalNPC.abomBoss, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>()) && Main.npc[NPCs.EModeGlobalNPC.abomBoss].localAI[3] > 1)
                    projectile.velocity *= 7f;
                else
                    projectile.velocity *= 24f;
                Main.PlaySound(SoundID.Item84, projectile.Center);
            }

            float rotation = projectile.ai[0] < 0 && projectile.ai[1] > 0 ? 1f - projectile.ai[1] / projectile.localAI[1] : 0.8f;
            projectile.rotation += rotation * projectile.localAI[0];
        }

        public override void Kill(int timeLeft)
        {
            int dustMax = 20;
            float speed = 12;
            for (int i = 0; i < dustMax; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, Scale: 3.5f);
                Main.dust[d].velocity *= speed;
                Main.dust[d].noGravity = true;
            }
            for (int i = 0; i < dustMax; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, Scale: 3.5f);
                Main.dust[d].velocity *= speed;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(BuffID.Burning, 180);
                //target.AddBuff(mod.BuffType("Rotting"), 900);
                //target.AddBuff(mod.BuffType("LivingWasteland"), 900);
            }
            target.AddBuff(BuffID.OnFire, 900);
            target.AddBuff(BuffID.Weak, 900);
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
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, projectile.ai[1] < 0 ? 150 : 255) * projectile.Opacity * (projectile.ai[1] < 0 ? 1f : 0.5f);
        }
    }
}