using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class SisypheanShrapnel : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_3";
        public override void SetDefaults() 
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public ref float timer => ref Projectile.ai[0];
        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += 0.2f;
            if (timer++ % 20 == 0) {
                Dust.NewDust(Projectile.Center, 10, 10, DustID.Stone);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center, 20, 20, DustID.Stone);
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation;
            SpriteEffects flip = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, drawPos, texture.Frame(), lightColor, rot, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale, flip);
            return false;
        }
    }
}
