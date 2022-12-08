using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeNuke : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Bomb");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1.5f;
		}

        public override bool? CanDamage() => false;

        public override void AI()
		{
			Projectile.ai[0] += 1f;
            Projectile.rotation += Projectile.velocity.Length() * 0.075f * Math.Sign(Projectile.velocity.X);
			Projectile.alpha = (int)(150 * Math.Sin(Projectile.ai[0]/3));

			for (int i = 0; i < 4; i++)
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, Scale: 3f);
				Main.dust[d].noGravity = true;
				Main.dust[d].velocity *= 0.5f;
			}

            if (Projectile.ai[0] > 80f)
			{
				Projectile.Kill();
			}
		}
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
        public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            const int max = 32;
            for (int i = 0; i < max; i++)
            {
                float rad = MathHelper.TwoPi / max * i;
                int damage = Projectile.damage / 2;
                int knockBack = 3;
                float speed = 0.8f;
                Vector2 vector = Projectile.velocity.RotatedBy(rad) * speed;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int type = FargoSoulsWorld.EternityMode ? ModContent.ProjectileType<LifeSplittingProjSmall>() : ModContent.ProjectileType<LifeProjSmall>();
                    float ai0 = FargoSoulsWorld.EternityMode ? -180 : 0;
                    float ai1 = FargoSoulsWorld.EternityMode ? 2 : 0;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vector, type, damage, knockBack, Main.myPlayer, ai0, ai1);
                }
            }


            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 91, 0f, 0f, 100, default(Color), 3f);
                Main.dust[dust].velocity *= 1.4f;
            }

            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 91, 0f, 0f, 100, default(Color), 3.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 7f;
                dust = Dust.NewDust(Projectile.position, Projectile.width,
                    Projectile.height, 91, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].velocity *= 3f;
            }

            float scaleFactor9 = 0.5f;
            for (int j = 0; j < 4; j++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center,
                    default(Vector2),
                    Main.rand.Next(61, 64));

                Main.gore[gore].velocity *= scaleFactor9;
                Main.gore[gore].velocity += new Vector2(1f, 1f).RotatedBy(MathHelper.TwoPi / 4 * j);
            }
		}
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.HotPink * Projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100) * Projectile.Opacity;
	}
}
