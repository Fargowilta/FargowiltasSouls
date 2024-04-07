using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
	public class MonkDashDamage : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

		}
		public override void SetDefaults()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true;
			Projectile.width = player.width;
			Projectile.height = player.height;
			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 30;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 30;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			if (player == null || !player.active || player.dead)
			{
				Projectile.Kill();
			}
			Projectile.Center = player.Center;

		}

		public override string Texture => FargoSoulsUtil.EmptyTexture;

		/*public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.position, Projectile.oldPos[1], Projectile.width, ref collisionPoint))
            {
                return true;
            }
            return false;
        }*/

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.damage = (int)(Math.Round(Projectile.damage * 0.8));
			//CHANGE THIS SOUND EFFECT!!!!!!!!!!!!!!!!!!!!!! TOO MUCH RAINBOW GUN
			SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, target.Center);

			//vanilla muramasa slash spawn logic
			NPC nPC = target;
			Rectangle hitbox = nPC.Hitbox;
			hitbox.Inflate(30, 16);
			hitbox.Y -= 8;
			Vector2 vector3 = Main.rand.NextVector2FromRectangle(hitbox);
			Vector2 vector4 = hitbox.Center.ToVector2();
			Vector2 spinningpoint = (vector4 - vector3).SafeNormalize(new Vector2(hit.HitDirection, 0.5f)) * 8f;
			Main.rand.NextFloat();
			float num6 = (float)(Main.rand.Next(2) * 2 - 1) * ((float)Math.PI / 5f + (float)Math.PI * 4f / 5f * Main.rand.NextFloat());
			num6 *= 0.5f;
			spinningpoint = spinningpoint.RotatedBy(0.7853981852531433);
			int num7 = 3;
			int num8 = 10 * num7;
			int num9 = 5;
			int num10 = num9 * num7;
			vector3 = vector4;
			for (int k = 0; k < num10; k++)
			{
				vector3 -= spinningpoint;
				spinningpoint = spinningpoint.RotatedBy((0f - num6) / (float)num8);
			}
			vector3 += nPC.velocity * num9;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector3, spinningpoint, ModContent.ProjectileType<MonkDashSlash>(), (int)((float)Projectile.damage * 0.5f), 0f, Main.myPlayer, num6);
		}

	}
	public class MonkDashSlash : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_729";
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.SuperStarSlash);
			Projectile.aiStyle = -1;
		}
		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200);
		public override void AI()
		{
			float num = (float)Math.PI / 2f;
			Projectile.alpha -= 10;
			int num2 = 100;
			if (Projectile.alpha < num2)
			{
				Projectile.alpha = num2;
			}
			if (Projectile.ai[0] != 0f)
			{
				int num3 = 10 * Projectile.MaxUpdates;
				Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0] / (float)num3);
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + num;
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle rect = new(0, 0, texture.Width, texture.Height);
			Vector2 origin = rect.Size() / 2;
			int num149 = 18;
			int num147 = 0;
			int num148 = -2;
			float value12 = 1.3f;
			float num150 = 15f;

			for (int num152 = num149; (num148 > 0 && num152 < num147) || (num148 < 0 && num152 > num147); num152 += num148)
			{
				Projectile proj = Projectile;
				if (num152 >= proj.oldPos.Length)
				{
					continue;
				}
				Color color32 = Color.White;
				color32 = proj.GetAlpha(color32);

				float num157 = num147 - num152;
				if (num148 < 0)
				{
					num157 = num149 - num152;
				}
				color32 *= num157 / ((float)ProjectileID.Sets.TrailCacheLength[proj.type] * 1.5f);
				Vector2 vector29 = proj.oldPos[num152];
				float num158 = proj.rotation;
				SpriteEffects effects2 = SpriteEffects.None;
				if (ProjectileID.Sets.TrailingMode[proj.type] == 2 || ProjectileID.Sets.TrailingMode[proj.type] == 3 || ProjectileID.Sets.TrailingMode[proj.type] == 4)
				{
					num158 = proj.oldRot[num152];
				}
				if (vector29 == Vector2.Zero)
				{
					continue;
				}
				Vector2 position3 = vector29 + Vector2.Zero + proj.Size / 2f - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
				Main.EntitySpriteDraw(texture, position3, rect, color32, num158, origin, MathHelper.Lerp(proj.scale, value12, (float)num152 / num150), effects2);
			}

			return false;
		}
	}
}
