using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class ChampionBee : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_566";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Bee");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.hostile = true;
            projectile.timeLeft = 360;
            projectile.aiStyle = -1;
            cooldownSlot = 1;
            projectile.tileCollide = false;
            projectile.scale = 1.5f;
        }

        public override void AI()
        {
            projectile.spriteDirection = Math.Sign(projectile.velocity.X);
            projectile.rotation = projectile.velocity.X * .1f;

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

            if ((projectile.wet || projectile.lavaWet) && !projectile.honeyWet) //die in liquids
            {
                projectile.Kill();
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 300);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("Infested"), 300);
                target.AddBuff(mod.BuffType("Swarming"), 300);
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
            
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}