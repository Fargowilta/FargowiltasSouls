using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FlamingJack : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_321";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flaming Jack");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }

            if (++Projectile.frameCounter > 0)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 2)
                    Projectile.frame = 0;
            }

            if (--Projectile.ai[1] > -60f && Projectile.ai[1] < 0f) //homing for 1sec, with delay
            {
                Player player = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
                if (player != null)
                {
                    Vector2 dist = player.Center - Projectile.Center;
                    dist.Normalize();
                    dist *= 8f;
                    Projectile.velocity.X = (Projectile.velocity.X * 40 + dist.X) / 41;
                    Projectile.velocity.Y = (Projectile.velocity.Y * 40 + dist.Y) / 41;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.velocity.X < 0)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = -Projectile.velocity.ToRotation();
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            int d = Dust.NewDust(Projectile.position + new Vector2(8, 8), Projectile.width - 16, Projectile.height - 16, 6, 0f, 0f, 0, new Color(), 1f);
            Main.dust[d].velocity *= 0.5f;
            Main.dust[d].velocity += Projectile.velocity * 0.5f;
            Main.dust[d].noGravity = true;
            Main.dust[d].noLight = true;
            Main.dust[d].scale = 1.4f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 20; ++i)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2f;
                d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, -Projectile.velocity.X * 0.2f, -Projectile.velocity.Y * 0.2f, 100, new Color(), 1f);
                Main.dust[d].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}