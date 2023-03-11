using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeNeggravProj : ModProjectile
	{
        int RotDirect = 1;

        private bool rTexture = false;

        public override string Texture => "FargowiltasSouls/Projectiles/Challengers/LifeProjLarge";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cross");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.5f;
            Projectile.scale = 1f;
        }

        public override void AI()
		{
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.Next(100);
                RotDirect = Main.rand.NextBool(2) ? -1 : 1;
                rTexture = Main.rand.NextBool(2);
            }
            Projectile.rotation += 0.2f * RotDirect;

            if (Main.rand.NextBool(6))
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.5f;
                Main.dust[index2].velocity.Y *= 0.5f;
            }
            Projectile.velocity.Y -= 0.25f;
			Projectile.velocity.X = Projectile.velocity.X * 0.999f;
			if (Projectile.ai[0] > 360f)
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = rTexture ? FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Challengers/LifeProjLarge2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value : FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Challengers/LifeProjLarge", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value; ;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.Purple * Projectile.Opacity * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
