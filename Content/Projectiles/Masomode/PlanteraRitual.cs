using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
	public class PlanteraRitual : BaseArena
	{
		private const float realRotation = -MathHelper.Pi / 180f;
		public override string Texture => "Terraria/Images/Projectile_226";
		public PlanteraRitual() : base(realRotation, 1100, NPCID.Plantera, DustID.JungleSpore) { }

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
		}

		protected override void Movement(NPC npc)
		{
			Projectile.velocity = npc.Center - Projectile.Center;

			Projectile.velocity /= 40f;

			rotationPerTick = realRotation;
		}

		public override void AI()
		{
			base.AI();
			Projectile.rotation -= 1f;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return lightColor * Projectile.Opacity * (targetPlayer == Main.myPlayer ? 1f : 0.15f);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 60 * 2);
		}
		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw

			Color color26 = Projectile.GetAlpha(lightColor);

			for (int x = 0; x < 32; x++)
			{
				int frame = (Projectile.frame + x) % Main.projFrames[Projectile.type];
				int y3 = num156 * frame; //ypos of upper left corner of sprite to draw
				Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
				Vector2 origin2 = rectangle.Size() / 2f;

				Vector2 drawOffset = new Vector2(threshold * Projectile.scale / 2f, 0f).RotatedBy(Projectile.ai[0]);
				drawOffset = drawOffset.RotatedBy(2f * MathHelper.Pi / 32f * x);

				float rotation = drawOffset.ToRotation() + MathHelper.PiOver2;


				const int max = 4;
				for (int i = 0; i < max; i++)
				{
					Color color27 = color26;
					color27 *= (float)(max - i) / max;
					Vector2 value4 = Projectile.Center + drawOffset.RotatedBy(rotationPerTick * -i);
					float rot = rotation;
					Main.EntitySpriteDraw(texture2D13, value4 - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, rot, origin2, Projectile.scale, SpriteEffects.None, 0);
				}

				float finalRot = rotation;
				Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, finalRot, origin2, Projectile.scale, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}