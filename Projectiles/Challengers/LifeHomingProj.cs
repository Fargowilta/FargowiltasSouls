using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeHomingProj : ModProjectile
	{
		public bool home = true;

        public override string Texture => "Terraria/Images/NPC_75";

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lesser Fairy");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.Pixie];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1.2f;
		}

        public override void AI()
		{
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.X < 0 ? Projectile.velocity.ToRotation() + (float)Math.PI : Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(6))
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
				Main.dust[d].noGravity = true;
				Main.dust[d].velocity *= 0.5f;
			}

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            if (Projectile.ai[0] > 30f)
			{
                if (Projectile.ai[0] == 31f)
                    Projectile.ai[1] = Player.FindClosest(Projectile.Center, 0, 0);

				if (Main.player[(int)Projectile.ai[1]].active && !Main.player[(int)Projectile.ai[1]].dead)
				{
					Vector2 vectorToIdlePosition = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
					float num = vectorToIdlePosition.Length();
					float speed = FargoSoulsWorld.MasochistModeReal ? 24f : 22f;
					float inertia = 15f;
					float deadzone = FargoSoulsWorld.MasochistModeReal ? 150f : 180f;
					if (num > deadzone && home)
					{
						vectorToIdlePosition.Normalize();
						vectorToIdlePosition *= speed;
						Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
					}
					else if (Projectile.velocity == Vector2.Zero)
					{
						Projectile.velocity.X = -0.15f;
						Projectile.velocity.Y = -0.05f;
					}
					if (num < deadzone)
					{
						home = false;
					}
				}
			}
			if (Projectile.ai[0] > 600f)
			{
				Projectile.Kill();
			}
			Projectile.ai[0] += 1f;
		}
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.3f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
