using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
	public class HentaiSword : ModProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_454";

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.alpha = 0;
			Projectile.FargoSouls().CanSplit = false;
			Projectile.FargoSouls().TimeFreezeImmune = true;
			Projectile.FargoSouls().noInteractionWithNPCImmunityFrames = true;

			Projectile.timeLeft = MAXTIME;
		}

		const int MAXTIME = 15;
		public const int MUTANT_SWORD_SPACING = 80;
		const int MUTANT_SWORD_MAX = 8;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (projHitbox.Intersects(targetHitbox))
				return true;

			if (Projectile.velocity == Vector2.Zero)
				return false;

			//do multiple checks to account for how far it rotates from 1 tick to the next
			const int max = 3;
			for (int i = 0; i < max; i++)
			{
				float length = MUTANT_SWORD_SPACING * MUTANT_SWORD_MAX + Projectile.width;
				float dummy = 0f;
				Vector2 offset = length * Projectile.scale * Vector2.Lerp(Projectile.rotation.ToRotationVector2(), Projectile.oldRot[1].ToRotationVector2(), 1f / max * i);
				Vector2 end = Projectile.Center;
				Vector2 tip = Projectile.Center + offset;

				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, tip, Projectile.width / 2 * Projectile.scale, ref dummy))
					return true;
			}

			return false;
		}

		public override void AI()
		{
			//dont rotate on first tick
			if (Projectile.localAI[0]++ > 1)
				Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi / MAXTIME * Projectile.ai[0]);
			Projectile.rotation = Projectile.velocity.ToRotation();

			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter);
			Projectile.Center = ownerMountedCenter - Projectile.velocity / 2;
			Projectile.direction = projOwner.direction;
			projOwner.heldProj = Projectile.whoAmI;
			if (projOwner.itemTime < 2)
				projOwner.itemTime = 2;
			if (projOwner.itemAnimation < 2)
				projOwner.itemAnimation = 2;

			if (++Projectile.frameCounter >= 6)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.frame = 0;
			}
		}

		public override void OnKill(int timeLeft)
		{
			if (Projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < MUTANT_SWORD_MAX; i++)
				{
					Vector2 offset = Vector2.Normalize(Projectile.velocity) * MUTANT_SWORD_SPACING * Projectile.scale * i;
					Vector2 spawnPos = Projectile.Center + offset;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos,
						Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), Projectile.damage, Projectile.knockBack * 3f, Projectile.owner);
				}

				for (int i = -1; i <= 1; i += 2)
				{
					Vector2 offset = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.ToRadians(45 * i)) * MUTANT_SWORD_SPACING * 1.5f * Projectile.scale;
					Vector2 spawnPos = Projectile.Center + offset;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos,
						Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), Projectile.damage, Projectile.knockBack * 3f, Projectile.owner);
				}
			}
		}

		public override bool? CanDamage()
		{
			Projectile.maxPenetrate = 1;
			return null;
		}

		public override bool? CanHitNPC(NPC target)
		{
			if (Projectile.localNPCImmunity[target.whoAmI] >= 3)
				return false;
			return null;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.localNPCImmunity[target.whoAmI]++;

			if (Projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < 3; i++)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position + new Vector2(Main.rand.Next(target.width), Main.rand.Next(target.height)),
						Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), Projectile.damage, Projectile.knockBack * 3f, Projectile.owner);
				}
			}
			target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 600);

			if (Projectile.owner == Main.myPlayer && target.lifeMax > 5)
			{
				if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<HentaiSwordBlast>()] < 8)
				{
					Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<HentaiSwordBlast>()] += 8;

					Vector2 spawnPos = target.Center;

					if (!Main.dedServ && Main.LocalPlayer.active)
						Main.LocalPlayer.FargoSouls().Screenshake = 30;

					if (!Main.dedServ)
						SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Thunder"), spawnPos);

					Vector2 baseDirection = Main.rand.NextVector2Unit();
					const int max = 8; //spread
					for (int i = 0; i < max; i++)
					{
						Vector2 angle = baseDirection.RotatedBy(MathHelper.TwoPi / max * i);
						float ai1 = 30; //number of chains
						if (FargoSoulsUtil.HostCheck)
						{
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), Vector2.Zero, ModContent.ProjectileType<HentaiSwordBlast>(),
								Projectile.damage, Projectile.knockBack * 3, Projectile.owner, MathHelper.WrapAngle(angle.ToRotation()), ai1);
						}
					}
				}
			}
		}

		public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

		public override bool PreDraw(ref Color lightColor)
		{
			if (Projectile.velocity == Vector2.Zero)
				return false;

			Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantSphereGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			int rect1 = glow.Height;
			int rect2 = 0;
			Rectangle glowrectangle = new(0, rect2, glow.Width, rect1);
			Vector2 gloworigin2 = glowrectangle.Size() / 2f;

			for (int i = 0; i < MUTANT_SWORD_MAX; i++)
			{
				Color glowcolor = Color.Lerp(new Color(196, 247, 255, 0), Color.Transparent, 0.9f);
				glowcolor *= Projectile.Opacity;
				float increment = MathHelper.Lerp(1f, 0.05f, (float)i / MUTANT_SWORD_MAX);
				for (float j = 0; j < ProjectileID.Sets.TrailCacheLength[Projectile.type]; j += increment) //reused betsy fireball scaling trail thing
				{
					Color color27 = glowcolor;
					color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - j) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
					float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - j) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
					int max0 = (int)j - 1;
					if (max0 < 0)
						continue;
					Vector2 oldPos = Vector2.Lerp(Projectile.oldPos[(int)j], Projectile.oldPos[max0], 1 - j % 1);
					float oldRot = MathHelper.Lerp(Projectile.oldRot[(int)j], Projectile.oldRot[max0], 1 - j % 1);
					Vector2 trailOffset = Vector2.Normalize(oldRot.ToRotationVector2()) * MUTANT_SWORD_SPACING * Projectile.scale * i;
					Main.EntitySpriteDraw(glow, oldPos + Projectile.Size / 2f + trailOffset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
						Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.5f, SpriteEffects.None, 0);
				}
				glowcolor = Color.Lerp(new Color(255, 255, 255, 0), Color.Transparent, 0.85f);
				Vector2 offset = Vector2.Normalize(Projectile.velocity) * MUTANT_SWORD_SPACING * Projectile.scale * i;
				Main.EntitySpriteDraw(glow, Projectile.position + Projectile.Size / 2f + offset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), glowcolor,
						Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, Projectile.scale * 1.5f, SpriteEffects.None, 0);
			}

			for (int i = -1; i <= 1; i += 2)
			{
				Color glowcolor = Color.Lerp(new Color(196, 247, 255, 0), Color.Transparent, 0.9f);
				glowcolor *= Projectile.Opacity;
				float increment = MathHelper.Lerp(1f, 0.05f, (float)i / MUTANT_SWORD_MAX);
				for (float j = 0; j < ProjectileID.Sets.TrailCacheLength[Projectile.type]; j += increment) //reused betsy fireball scaling trail thing
				{
					Color color27 = glowcolor;
					color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - j) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
					float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - j) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
					int max0 = (int)j - 1;
					if (max0 < 0)
						continue;
					Vector2 oldPos = Vector2.Lerp(Projectile.oldPos[(int)j], Projectile.oldPos[max0], 1 - j % 1);
					float oldRot = MathHelper.Lerp(Projectile.oldRot[(int)j], Projectile.oldRot[max0], 1 - j % 1);
					Vector2 trailOffset = Vector2.Normalize(oldRot.ToRotationVector2()).RotatedBy(MathHelper.ToRadians(45 * i)) * MUTANT_SWORD_SPACING * 1.5f * Projectile.scale;
					Main.EntitySpriteDraw(glow, oldPos + Projectile.Size / 2f + trailOffset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
						Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.5f, SpriteEffects.None, 0);
				}
				glowcolor = Color.Lerp(new Color(255, 255, 255, 0), Color.Transparent, 0.85f);
				Vector2 offset = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.ToRadians(45 * i)) * MUTANT_SWORD_SPACING * 1.5f * Projectile.scale;
				Main.EntitySpriteDraw(glow, Projectile.position + Projectile.Size / 2f + offset - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), glowcolor,
						Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, Projectile.scale * 1.5f, SpriteEffects.None, 0);
			}

			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			if (Projectile.velocity == Vector2.Zero)
				return;

			for (int i = 0; i < MUTANT_SWORD_MAX; i++)
			{
				Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
				int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
				int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
				Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
				Vector2 origin2 = rectangle.Size() / 2f;
				Vector2 offset = Vector2.Normalize(Projectile.velocity) * MUTANT_SWORD_SPACING * Projectile.scale * i;
				Main.EntitySpriteDraw(texture2D13, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
			}

			for (int i = -1; i <= 1; i += 2)
			{
				Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
				int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
				int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
				Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
				Vector2 origin2 = rectangle.Size() / 2f;
				Vector2 offset = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.ToRadians(45 * i)) * MUTANT_SWORD_SPACING * 1.5f * Projectile.scale;
				Main.EntitySpriteDraw(texture2D13, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
			}
		}
	}
}