using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class RainbowSlimeSpikeFriendly : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_605";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rainbow Slime Spike");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.alpha -= 50;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha == 0 && Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.position - Projectile.velocity * 3f, Projectile.width, Projectile.height, DustID.TintableDust, 0f, 0f, 50,
                    new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 150), 1.2f);
                Main.dust[d].velocity *= 0.3f;
                Main.dust[d].velocity += Projectile.velocity * 0.3f;
                Main.dust[d].noGravity = true;
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
            }
            Projectile.velocity.Y += 0.15f;

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.FlamesoftheUniverseBuff>(), 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 200);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}