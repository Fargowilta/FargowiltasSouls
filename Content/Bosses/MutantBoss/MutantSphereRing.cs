using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
	public class MutantSphereRing : ModProjectile
	{
		public override string Texture => FargoSoulsUtil.AprilFools ?
			"FargowiltasSouls/Content/Bosses/MutantBoss/MutantSphere_April" :
			"Terraria/Images/Projectile_454";

		protected bool DieOutsideArena;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Phantasmal Sphere");
			Main.projFrames[Projectile.type] = 2;

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 480;
			Projectile.alpha = 200;
			CooldownSlot = 1;

			//dont let others inherit this behaviour
			if (Projectile.type == ModContent.ProjectileType<MutantSphereRing>())
			{
				DieOutsideArena = true;

				Projectile.FargoSouls().TimeFreezeImmune =
					WorldSavingSystem.MasochistModeReal
					&& FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>())
					&& Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == -5;
			}
		}

		public override bool CanHitPlayer(Player target)
		{
			return target.hurtCooldowns[1] == 0 || WorldSavingSystem.MasochistModeReal;
		}

		private int ritualID = -1;

		float originalSpeed;
		bool spawned;

		public override void AI()
		{
			if (!spawned)
			{
				spawned = true;
				originalSpeed = Projectile.velocity.Length();
			}

			Projectile.velocity = originalSpeed * Vector2.Normalize(Projectile.velocity).RotatedBy(Projectile.ai[1] / (2 * Math.PI * Projectile.ai[0] * ++Projectile.localAI[0]));

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 20;
				if (Projectile.alpha < 0)
					Projectile.alpha = 0;
			}
			Projectile.scale = 1f - Projectile.alpha / 255f;

			if (++Projectile.frameCounter >= 6)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame > 1)
					Projectile.frame = 0;
			}

			if (DieOutsideArena)
			{
				if (ritualID == -1) //identify the ritual CLIENT SIDE
				{
					ritualID = -2; //if cant find it, give up and dont try every tick

					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<MutantRitual>())
						{
							ritualID = i;
							break;
						}
					}
				}

				Projectile ritual = FargoSoulsUtil.ProjectileExists(ritualID, ModContent.ProjectileType<MutantRitual>());
				if (ritual != null && Projectile.Distance(ritual.Center) > 1200f) //despawn faster
					Projectile.timeLeft = 0;
			}

			TryTimeStop();
		}

		void TryTimeStop()
		{
			if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost
				&& FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()))
			{
				//final spark spheres
				if (WorldSavingSystem.MasochistModeReal && Main.npc[EModeGlobalNPC.mutantBoss].ai[0] == -5
					&& Projectile.Colliding(Projectile.Hitbox, Main.LocalPlayer.FargoSouls().GetPrecisionHurtbox()))
				{
					if (!Main.LocalPlayer.HasBuff(ModContent.BuffType<TimeFrozenBuff>()))
						SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ZaWarudo"), Main.LocalPlayer.Center);
					Main.LocalPlayer.AddBuff(ModContent.BuffType<TimeFrozenBuff>(), 300);
				}
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.mutantBoss, ModContent.NPCType<MutantBoss>()))
			{
				if (WorldSavingSystem.EternityMode)
				{
					target.FargoSouls().MaxLifeReduction += 100;
					target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 5400);
					target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
				}
			}
			target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 360);

			TryTimeStop();
		}

		public override void OnKill(int timeleft)
		{
			if (Main.rand.NextBool(Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] / 10 + 1))
				SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
			Projectile.position = Projectile.Center;
			Projectile.width = Projectile.height = 208;
			Projectile.Center = Projectile.position;
			for (int index1 = 0; index1 < 2; ++index1)
			{
				int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
				Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
			}
			for (int index1 = 0; index1 < 4; ++index1)
			{
				int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 0, new Color(), 2.5f);
				Main.dust[index2].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
				Main.dust[index2].noGravity = true;
				Dust dust1 = Main.dust[index2];
				dust1.velocity *= 1f;
				int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0.0f, 0.0f, 100, new Color(), 1.5f);
				Main.dust[index3].position = new Vector2(Projectile.width / 2, 0.0f).RotatedBy(6.28318548202515 * Main.rand.NextDouble(), new Vector2()) * (float)Main.rand.NextDouble() + Projectile.Center;
				Dust dust2 = Main.dust[index3];
				dust2.velocity *= 1f;
				Main.dust[index3].noGravity = true;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * Projectile.Opacity;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/MutantBoss/MutantSphereGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			int rect1 = glow.Height;
			int rect2 = 0;
			Rectangle glowrectangle = new(0, rect2, glow.Width, rect1);
			Vector2 gloworigin2 = glowrectangle.Size() / 2f;
			Color glowcolor = Color.Lerp(FargoSoulsUtil.AprilFools ? Color.Red : new Color(196, 247, 255, 0), Color.Transparent, 0.9f);
			glowcolor *= Projectile.Opacity;
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++) //reused betsy fireball scaling trail thing
			{

				Color color27 = glowcolor;
				color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
				float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
				Vector2 value4 = Projectile.oldPos[i] - Vector2.Normalize(Projectile.velocity) * i * 6;
				Main.EntitySpriteDraw(glow, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), color27,
					Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, scale * 1.5f, SpriteEffects.None, 0);
			}
			glowcolor = Color.Lerp(new Color(255, 255, 255, 0), Color.Transparent, 0.85f);
			Main.EntitySpriteDraw(glow, Projectile.position + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glowrectangle), glowcolor,
					Projectile.velocity.ToRotation() + MathHelper.PiOver2, gloworigin2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
		}
	}
}