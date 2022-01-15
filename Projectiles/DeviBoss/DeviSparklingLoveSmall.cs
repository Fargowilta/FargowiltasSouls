using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviSparklingLoveSmall : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/SparklingLove";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Sparkling Love");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        const int maxTime = 60;

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.scale = 1.5f;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = maxTime;
            //projectile.alpha = 250;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;

            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.hide = false; //to avoid edge case tick 1 wackiness

            //the important part
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc != null)
            {
                if (projectile.localAI[0] == 0)
                    projectile.localAI[1] = projectile.ai[1] / maxTime; //do this first
                
                projectile.velocity = projectile.velocity.RotatedBy(projectile.ai[1]);
                projectile.ai[1] -= projectile.localAI[1];
                projectile.Center = npc.Center + new Vector2(50, 50).RotatedBy(projectile.velocity.ToRotation() - MathHelper.PiOver4) * projectile.scale;
            }
            else
            {
                projectile.Kill();
                return;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Vector2 basePos = projectile.Center - projectile.velocity * 141 / 2 * projectile.scale;
                for (int i = 0; i < 40; i++)
                {
                    int d = Dust.NewDust(basePos + projectile.velocity * Main.rand.NextFloat(141) * projectile.scale, 0, 0, 86, Scale: 3f);
                    Main.dust[d].velocity *= 4.5f;
                    Main.dust[d].noGravity = true;
                }
                Main.PlaySound(SoundID.Item1, projectile.Center);
            }
            
            projectile.direction = projectile.spriteDirection = Math.Sign(projectile.ai[1]);
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(projectile.direction < 0 ? 135 : 45);
            //Main.NewText(MathHelper.ToDegrees(projectile.velocity.ToRotation()) + " " + MathHelper.ToDegrees(projectile.ai[1]));
        }

        public override void Kill(int timeLeft)
        {
            Vector2 basePos = projectile.Center - projectile.velocity * 141 / 2 * projectile.scale;
            for (int i = 0; i < 40; i++)
            {
                int d = Dust.NewDust(basePos + projectile.velocity * Main.rand.NextFloat(141) * projectile.scale, 0, 0, 86, Scale: 3f);
                Main.dust[d].velocity *= 4.5f;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Berserked"), 240);
            target.AddBuff(mod.BuffType("MutantNibble"), 240);
            target.AddBuff(mod.BuffType("Guilty"), 240);
            target.AddBuff(mod.BuffType("Lovestruck"), 240);
            target.AddBuff(mod.BuffType("Rotting"), 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = lightColor * projectile.Opacity;
            color.A = (byte)Math.Min(255, 255 * Math.Sin(Math.PI * (maxTime - projectile.timeLeft) / maxTime) * 1f);
            return color;
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

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, projectile.rotation, origin2, projectile.scale, effects, 0f);
            Texture2D texture2D14 = mod.GetTexture("Items/Weapons/FinalUpgrades/SparklingLove_glow");
            Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}