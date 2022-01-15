using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class DestroyerLaser : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_658";
        
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Death Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = -1;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.scale = 1.8f;
            projectile.tileCollide = false;

            projectile.extraUpdates = 4;
            projectile.timeLeft = 190 * projectile.extraUpdates + 1;
        }

        public override void AI()
        {
            int totalUpdates = projectile.extraUpdates + 1;

            if (projectile.ai[0] == 1f)
            {
                if (projectile.timeLeft > 91 * totalUpdates) //mp sync is important for this, which is why i do it this way
                    projectile.timeLeft = 91 * totalUpdates;
            }

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item12, projectile.Center);
            }
            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (projectile.timeLeft % totalUpdates == 0) //only run once per tick
            {
                if (++projectile.localAI[1] > 20 && projectile.localAI[1] < 90)
                    projectile.velocity *= 1.06f;

                if (projectile.alpha > 0 && projectile.timeLeft > 10 * totalUpdates)
                {
                    projectile.alpha -= 14;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;
                }
                else if (projectile.timeLeft <= 10 * totalUpdates)
                {
                    projectile.velocity *= 0.7f;
                    projectile.alpha += 25;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.destroyBoss, NPCID.TheDestroyer))
                target.AddBuff(BuffID.Electrified, 60);
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.brainBoss, NPCID.BrainofCthulhu))
            {
                target.AddBuff(BuffID.Poisoned, 120);
                target.AddBuff(BuffID.Darkness, 120);
                target.AddBuff(BuffID.Bleeding, 120);
                target.AddBuff(BuffID.Slow, 120);
                target.AddBuff(BuffID.Weak, 120);
                target.AddBuff(BuffID.BrokenArmor, 120);
            }
        }

        public override bool CanDamage() => projectile.timeLeft > 10 * (projectile.extraUpdates + 1);

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Red * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            Texture2D hitboxindicator = mod.GetTexture("Projectiles/MutantBoss/MutantSphereGlow");
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 scale = new Vector2(1, 1 + projectile.velocity.Length() / 5 * (projectile.extraUpdates + 1));
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, scale, SpriteEffects.None, 0f);

            if (projectile.timeLeft > 10 * (projectile.extraUpdates + 1))
            {
                Main.spriteBatch.Draw(hitboxindicator, projectile.Center - Main.screenPosition, new Rectangle(0, 0, hitboxindicator.Width, hitboxindicator.Height),
                    new Color(255, 133, 149) * projectile.Opacity, 0, hitboxindicator.Size() / 2, 0.25f, SpriteEffects.None, 0);
            }
            
            return false;
        }
    }
}