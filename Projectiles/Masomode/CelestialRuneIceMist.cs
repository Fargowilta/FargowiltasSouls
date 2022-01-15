using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CelestialRuneIceMist : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_464";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ice Mist");
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
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
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item120, projectile.position);
            }

            projectile.alpha += projectile.timeLeft > 20 ? -10 : 10;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
                projectile.alpha = 255;

            if (projectile.timeLeft % 60 == 0)
            {
                Main.PlaySound(SoundID.Item120, projectile.position);
                Vector2 vel = Vector2.UnitX.RotatedBy(projectile.rotation);
                vel *= 12f;
                for (int i = 0; i < 6; i++)
                {
                    vel = vel.RotatedBy(2f * (float)Math.PI / 6f);
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(projectile.Center, vel, mod.ProjectileType("CelestialRuneIceSpike"), projectile.damage, projectile.knockBack, projectile.owner, projectile.velocity.X, projectile.velocity.Y);
                }
            }

            projectile.rotation += (float)Math.PI / 40f;
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