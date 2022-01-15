using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomFrostWave : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_348";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Frost Wave");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.tileCollide = false;
            projectile.aiStyle = 1;
            aiType = ProjectileID.FrostWave;
            projectile.hostile = true;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 180;
            projectile.penetrate = -1;
            cooldownSlot = 1;
            projectile.coldDamage = true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                //target.AddBuff(BuffID.Frozen, 60);
            }
            target.AddBuff(BuffID.Frostburn, 120);
            //target.AddBuff(BuffID.Chilled, 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}