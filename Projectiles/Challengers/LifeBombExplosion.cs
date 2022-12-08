using FargowiltasSouls.NPCs.Challengers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeBombExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Bomb");
            Main.projFrames[Projectile.type] = 3;
        }
		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 50;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

        public override void AI()
		{
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
			Projectile.rotation += 2f;
			if (Main.rand.NextBool(6))
			{
				int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }

			//pulsate
			if (Projectile.localAI[0] == 0)
				Projectile.localAI[0] += Main.rand.Next(60);
            Projectile.scale = 1.1f + 0.1f * (float)Math.Sin(MathHelper.TwoPi / 15 * ++Projectile.localAI[1]);

            if (Projectile.ai[0] > 2400f || NPC.CountNPCS(ModContent.NPCType<LifeChallenger>()) < 1)
			{
				for (int i = 0; i < 20; i++)
				{
					int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
					Main.dust[d2].noGravity = true;
					Main.dust[d2].velocity *= 0.5f;
				}
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
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
