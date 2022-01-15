using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class ChampionBeetle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Beetle");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.hostile = true;
            projectile.timeLeft = 600;
            projectile.aiStyle = -1;
            cooldownSlot = 1;

            projectile.penetrate = -1;
            projectile.scale = 1.5f;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().GrazeCD = 40; //dont check for this long
        }

        public override bool CanDamage()
        {
            return projectile.localAI[0] > 40;
        }

        public override void AI()
        {
            if (projectile.velocity.X > 0)
                projectile.spriteDirection = 1;
            else if (projectile.velocity.X < 0)
                projectile.spriteDirection = -1;

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                    projectile.frame = 0;
            }

            if (projectile.timeLeft < 120) //speed away
            {
                projectile.velocity *= 1.05f;
            }

            if (++projectile.localAI[0] == 40)
            {
                projectile.velocity = Vector2.Zero;
            }
            else if (projectile.localAI[0] == 40 + 30)
            {
                projectile.velocity.X = projectile.ai[0];
                projectile.velocity.Y = projectile.ai[1];
                projectile.netUpdate = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("Defenseless"), 300);
                target.AddBuff(BuffID.WitheredArmor, 300);
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

            SpriteEffects effects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.75f * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}