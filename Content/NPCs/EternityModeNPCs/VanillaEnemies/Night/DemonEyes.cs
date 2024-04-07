using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Night
{
	public class DemonEyes : EModeNPCBehaviour
	{
		public override NPCMatcher CreateMatcher() =>
			new NPCMatcher().MatchTypeRange(
				NPCID.DemonEye,
				NPCID.DemonEye2,
				NPCID.DemonEyeOwl,
				NPCID.DemonEyeSpaceship,
				NPCID.CataractEye,
				NPCID.CataractEye2,
				NPCID.SleepyEye,
				NPCID.SleepyEye2,
				NPCID.DialatedEye,
				NPCID.DialatedEye2,
				NPCID.GreenEye,
				NPCID.GreenEye2,
				NPCID.PurpleEye,
				NPCID.PurpleEye2
			);

		public int AttackTimer;

		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
		{
			base.SendExtraAI(npc, bitWriter, binaryWriter);

			binaryWriter.Write7BitEncodedInt(AttackTimer);
		}

		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
		{
			base.ReceiveExtraAI(npc, bitReader, binaryReader);

			AttackTimer = binaryReader.Read7BitEncodedInt();
		}

		public override void OnFirstTick(NPC npc)
		{
			base.OnFirstTick(npc);

			if (Main.hardMode && Main.rand.NextBool(4))
				npc.Transform(NPCID.WanderingEye);
		}

		public override void AI(NPC npc)
		{
			base.AI(npc);
			AttackTimer++;
			if (AttackTimer == 360) //warning dust
			{
				FargoSoulsUtil.DustRing(npc.Center, 32, DustID.Torch, 5f, scale: 1.5f);
				npc.netUpdate = true;
				NetSync(npc);
			}
			else if (AttackTimer >= 420)
			{
				npc.TargetClosest();

				Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * 10;
				npc.velocity = velocity;

				AttackTimer = Main.rand.Next(-300, 0);
				npc.netUpdate = true;
				NetSync(npc);
			}

			if (Math.Abs(npc.velocity.Y) > 5 || Math.Abs(npc.velocity.X) > 5)
			{
				int dustId = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height + 5, DustID.Stone, npc.velocity.X * 0.2f,
					npc.velocity.Y * 0.2f, 100, default, 1f);
				Main.dust[dustId].noGravity = true;
				int dustId3 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height + 5, DustID.Stone, npc.velocity.X * 0.2f,
					npc.velocity.Y * 0.2f, 100, default, 1f);
				Main.dust[dustId3].noGravity = true;
			}
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			//target.AddBuff(BuffID.Obstructed, 15);
			target.AddBuff(ModContent.BuffType<BerserkedBuff>(), 300);
		}
	}

	public class WanderingEye : DemonEyes
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.WanderingEye);

		public override void SetDefaults(NPC npc)
		{
			base.SetDefaults(npc);

			npc.lifeMax *= 2;
		}

		public override void OnFirstTick(NPC npc) { }

		public override void AI(NPC npc)
		{
			if (npc.life < npc.lifeMax / 2)
			{
				npc.knockBackResist = 0f;
				if (++AttackTimer > 20)
				{
					AttackTimer = 0;
					if (FargoSoulsUtil.HostCheck)
						Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Normalize(npc.velocity), ModContent.ProjectileType<BloodScythe>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
				}
			}
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 120);
		}
	}

	public class ServantofCthulhu : DemonEyes
	{
		public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ServantofCthulhu);

		public override void OnFirstTick(NPC npc) { }

		public override void SetDefaults(NPC npc)
		{
			base.SetDefaults(npc);

			npc.lifeMax *= 2;
		}

		public override void AI(NPC npc)
		{
			npc.position += npc.velocity;
		}

		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			base.OnHitPlayer(npc, target, hurtInfo);

			target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 120);
		}

		public override void LoadSprites(NPC npc, bool recolor)
		{
			base.LoadSprites(npc, recolor);

			LoadNPCSprite(recolor, npc.type);
		}
	}
}
