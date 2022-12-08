using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeBee2 : ModProjectile //note: also not a bee
	{
        public override string Texture => "Terraria/Images/NPC_75";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lesser Fairy");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.Pixie];
        }
        public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1.5f;
		}

		public override void AI()
		{
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X < 0 ? 1 : -1;
            Projectile.rotation = Projectile.velocity.X * .1f;

            if (Main.rand.NextBool(6))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.5f;
            }

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            Projectile.velocity = Projectile.velocity * 1.02f;
			if (Projectile.ai[0] > 600f || Projectile.ai[1] > 775)
			{
				Projectile.Kill();
			}
			Projectile.ai[0] += 1f;
            Projectile.ai[1] += 1f;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 100) * Projectile.Opacity;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
