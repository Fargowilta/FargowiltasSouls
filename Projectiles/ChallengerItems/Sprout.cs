using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.ChallengerItems
{
    public class Sprout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 32;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 360;

            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0]);
            if (npc == null || !npc.CanBeChasedBy())
            {
                Projectile.Kill();
                return;
            }

            Projectile.Bottom = npc.Top + 0.1f * Vector2.UnitY * npc.height;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Dusts();
                Projectile.frame = Main.rand.Next(Main.projFrames[Projectile.type]);
            }
        }

        private void Dusts()
        {
            for (int i = 0; i < 30; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 2);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 30;
        }

        public override void Kill(int timeLeft)
        {
            Dusts();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}