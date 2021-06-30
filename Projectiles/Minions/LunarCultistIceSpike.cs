using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class LunarCultistIceSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Spike");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 180;
            projectile.penetrate = -1;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;

            if (ModLoader.GetMod("Fargowiltas") != null)
                ModLoader.GetMod("Fargowiltas").Call("LowRenderProj", projectile);
        }

        public override void AI()
        {
            projectile.alpha -= 10;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            /*if (projectile.ai[0] > -1f)
            {
                int ai0 = (int)projectile.ai[0];
                if (Main.projectile[ai0].active && Main.projectile[ai0].type == ModContent.\1Type<\2>\(\))
                {
                    projectile.Center = Main.projectile[ai0].Center + projectile.velocity * ++projectile.ai[1];
                }
                else
                {
                    projectile.ai[0] = -1f;
                }
            }*/

            int index3 = Dust.NewDust(projectile.Center + Utils.RandomVector2(Main.rand, -8f, 8f) / 2f, 8, 8, 197, 0.0f, 0.0f, 100, Color.Transparent, 1f);
            Main.dust[index3].noGravity = true;

            projectile.rotation = projectile.velocity.ToRotation();
            Lighting.AddLight(projectile.Center, 0.3f, 0.75f, 0.9f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 240);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}