using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BetsyFury : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_709";

        Vector2 spawn;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Sky Dragon's Fury");
            Main.projFrames[projectile.type] = Main.projFrames[ProjectileID.MonkStaffT3_AltShot];
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.hostile = true;
            projectile.scale = 1.2f;
            projectile.penetrate = -1;
            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                Main.PlayTrackedSound(SoundID.DD2_SkyDragonsFuryShot, projectile.Center);
                spawn = projectile.Center;
            }

            if (projectile.Distance(spawn) > 1200)
                projectile.Kill();

            if (++projectile.localAI[0] < 120)
                projectile.velocity *= 1.03f;

            projectile.alpha = projectile.alpha - 30;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            
            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                    projectile.frame = 0;
            }
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0.4f, 0.85f, 0.9f);

            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            //target.AddBuff(BuffID.OnFire, 600);
            //target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.WitheredArmor, 300);
            target.AddBuff(BuffID.WitheredWeapon, 300);
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override void Kill(int timeLeft)
        {
            int num1 = 3;
            int num2 = 10;
            
            for (int index1 = 0; index1 < num1; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, default, 1.5f);
                Main.dust[index2].position = new Vector2(projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + projectile.Center;
            }
            for (int index1 = 0; index1 < num2; ++index1)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0.0f, 0.0f, 0, default, 1.5f);
                Main.dust[index2].position = new Vector2(projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + projectile.Center;
                Main.dust[index2].noGravity = true;
            }
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<BetsyElectrosphere>(), 
                    projectile.damage, 0f, Main.myPlayer, spawn.X, spawn.Y);
            }
            
            Main.PlayTrackedSound(SoundID.DD2_SkyDragonsFuryCircle, projectile.Center);
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

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}