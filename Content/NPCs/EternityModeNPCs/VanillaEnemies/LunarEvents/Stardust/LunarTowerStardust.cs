using FargowiltasSouls.Content.BossBars;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust.StardustMinion;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Stardust
{
	public class LunarTowerStardust : LunarTowers
	{
		public override int ShieldStrength
		{
			get => NPC.ShieldStrengthTowerStardust;
			set => NPC.ShieldStrengthTowerStardust = value;
		}

		public override NPCMatcher CreateMatcher() =>
			new NPCMatcher().MatchType(NPCID.LunarTowerStardust);

		public LunarTowerStardust() : base(ModContent.BuffType<AntisocialBuff>(), 20) { }
		public override int MaxHP => 20000;
		public override int Damage => 0;

		List<int> DragonParts = new()
		{
					NPCID.CultistDragonHead,
					NPCID.CultistDragonBody1,
					NPCID.CultistDragonBody2,
					NPCID.CultistDragonBody3,
					NPCID.CultistDragonBody4,
					NPCID.CultistDragonTail
				};

		private string DragonName => Language.GetTextValue("Mods.FargowiltasSouls.NPCs.StardustDragon.DisplayName");

		public override bool CheckDead(NPC npc)
		{
			foreach (NPC n in Main.npc.Where(n => n.active && DragonParts.Contains(n.type)))
			{
				n.StrikeInstantKill();
				n.checkDead();
			}
			return base.CheckDead(npc);
		}
		public enum Attacks
		{
			Idle,
			CellExpandContract,
			CellRush,
			CellCurves,
			CellScissor
		}
		public override List<int> RandomAttacks => new() //these are randomly chosen attacks in p1
        {
			(int)Attacks.CellExpandContract,
			(int)Attacks.CellRush,
			(int)Attacks.CellCurves,
			(int)Attacks.CellScissor
		};
		private bool gotBossBar = false;
		public const int CellAmount = 20;
		public float CellRotation = 0;
		int DragonTimer = 0;
		public override void ShieldsDownAI(NPC npc)
		{
			if (!gotBossBar)
			{
				npc.BossBar = ModContent.GetInstance<CompositeBossBar>();
				gotBossBar = true;
			}
			int cells = 0;
			int bigCells = 0;
			foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<StardustMinion>() && n.ai[2] == npc.whoAmI))
			{
				if (n.frame.Y == 0) //frame check is to check if big
				{
					bigCells++;
				}
				cells++;
			}
			foreach (NPC n in Main.npc.Where(n => n.active && DragonParts.Contains(n.type) && n.GivenName != DragonName))
			{
				n.GivenName = DragonName;
			}
			//cells are sorted by a unique key, stored in their NPC.ai[3], that determines their behavior during attacks, for example spot in a circle. 
			//here it only spawns new cells if there's missing keys, and missing cells.
			//yes this will probably lag some pcs a bit, it's a lot to check in one frame
			if (cells < CellAmount)
			{
				for (int i = 0; i < CellAmount; i++)
				{
					bool foundCell = false;
					foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<StardustMinion>() && n.ai[2] == npc.whoAmI && n.ai[3] == i)) //frame check is to check if big
					{
						foundCell = true;
						break;
					}
					if (!foundCell)
					{
						SpawnMinion(npc, i);
						bigCells++;
					}
				}

			}
			if (NPC.CountNPCS(NPCID.CultistDragonHead) <= 0 && WorldSavingSystem.MasochistModeReal && DragonTimer <= 0) //spawn james in maso
			{
				if (FargoSoulsUtil.HostCheck)
				{

					int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)(npc.Center.Y - npc.height * 0.45f), NPCID.CultistDragonHead);
					if (Main.npc[n].active)
					{
						Main.npc[n].GivenName = DragonName;

					}
				}
				SoundEngine.PlaySound(SoundID.Item119, npc.Center);
				DragonTimer = 0;
			}
			DragonTimer++;
			if (bigCells > 0)
			{
				//The pillar is purposefully not immune because it's meant to bait aggro, this is a mechanic you need to deal with. Same goes for the dragon.
				npc.defense = 99999999;
				npc.life = npc.lifeMax;
			}
			else
			{
				if (npc.defense != 0) //trigger shield going down animation
				{
					CellState((int)States.Idle);
					npc.defense = 0;
					npc.ai[3] = 1f;
					npc.netUpdate = true;
					NetSync(npc);
				}
				if (NPC.CountNPCS(NPCID.CultistDragonHead) <= 0) //spawn james
				{
					if (FargoSoulsUtil.HostCheck)
					{
						int n = NPC.NewNPC(npc.GetSource_FromThis(), (int)npc.Center.X, (int)(npc.Center.Y - npc.height * 0.45f), NPCID.CultistDragonHead);
						if (Main.npc[n].active)
						{
							Main.npc[n].GivenName = DragonName;
							Main.npc[n].dontTakeDamage = true;

						}
					}
					SoundEngine.PlaySound(SoundID.Item119, npc.Center);
				}
				return;
			}
			Player target = Main.player[npc.target];
			if (npc.HasPlayerTarget && target.active)
			{
				switch (Attack)
				{
					case (int)Attacks.CellExpandContract:
						CellExpandContract(npc, target);
						CellRotation++;
						break;
					case (int)Attacks.CellRush:
						CellRush(npc, target);
						CellRotation++;
						break;
					case (int)Attacks.CellCurves:
						CellCurves(npc, target);
						CellRotation++;
						break;
					case (int)Attacks.CellScissor:
						CellScissor(npc, target);
						break;
					case (int)Attacks.Idle:
						Idle(npc, target);
						CellRotation++;
						break;
				}
			}
		}
		#region Attacks
		private void CellExpandContract(NPC npc, Player player)
		{
			const int WindupDuration = 90;
			const int AttackDuration = 60 * 6;
			const int EndlagDuration = 60 * 1;
			void Windup()
			{
				if (AttackTimer == 1)
				{
					SoundEngine.PlaySound(SoundID.Item77, npc.Center);
					CellState((int)States.PrepareExpand);
				}
			}
			void Attack()
			{
				if (AttackTimer - WindupDuration == 1)
				{
					SoundEngine.PlaySound(SoundID.Item92, npc.Center);
					CellState((int)States.Expand);
				}
			}
			void Endlag()
			{

			}
			if (AttackTimer <= WindupDuration)
			{
				Windup();
			}
			else if (AttackTimer <= WindupDuration + AttackDuration)
			{
				Attack();
			}
			else
			{
				Endlag();
			}
			if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
			{
				EndAttack(npc);
			}
		}
		private void CellRush(NPC npc, Player player)
		{
			const int AttackDuration = 60 * 9;
			const int ForceSendTime = 60 * 4;
			void Attack()
			{
				if (AttackTimer == 1)
				{
					SoundEngine.PlaySound(SoundID.Item44, npc.Center);
					CellState((int)States.PrepareRush);
				}
				if (AttackTimer == ForceSendTime)
				{
					SoundEngine.PlaySound(SoundID.Item96, npc.Center);
					foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<StardustMinion>() && n.ai[2] == npc.whoAmI && n.ai[1] != (float)States.Rush && n.ai[1] != (float)States.Contract))
					{
						n.ai[1] = (float)States.Rush;
					}
				}
			}
			Attack();
			if (AttackTimer > AttackDuration)
			{
				EndAttack(npc);
			}
		}
		private void CellCurves(NPC npc, Player player)
		{
			const int WindupDuration = 90;
			const int AttackDuration = 10 * CellAmount;
			const int EndlagDuration = 60 * 6;
			void Windup()
			{
				if (AttackTimer == 1)
				{
					SoundEngine.PlaySound(SoundID.Item88, npc.Center);
					CellState((int)States.Idle);
				}
			}
			void Attack()
			{
				const int AttackCD = 10;
				if (AttackTimer % AttackCD == AttackCD - 1)
				{
					SoundEngine.PlaySound(SoundID.Item115, npc.Center);
					foreach (NPC n in Main.npc.Where(n => n.active && n.type == ModContent.NPCType<StardustMinion>() && n.ai[2] == npc.whoAmI && n.ai[1] != (float)States.Curve))
					{
						n.ai[1] = (float)States.Curve;
						break;
					}
				}
			}
			void Endlag()
			{
			}
			if (AttackTimer <= WindupDuration)
			{
				Windup();
			}
			else if (AttackTimer <= WindupDuration + AttackDuration)
			{
				Attack();
			}
			else
			{
				Endlag();
			}
			if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
			{
				EndAttack(npc);
			}
		}
		private void CellScissor(NPC npc, Player player)
		{
			const int WindupDuration = 90;
			const int AttackDuration = 60 * 4 + 20;
			const int EndlagDuration = 60 * 2;
			void Windup()
			{
				if (AttackTimer == 1)
				{
					SoundEngine.PlaySound(SoundID.Item113, npc.Center);
					CellState((int)States.PrepareScissor);
				}
				CellRotation = 45; //degrees
			}
			void Attack()
			{
				if (AttackTimer - WindupDuration == 1)
				{
					SoundEngine.PlaySound(SoundID.Item113, npc.Center);
					CellState((int)States.Scissor);
				}
			}
			void Endlag()
			{
				if (AttackTimer - WindupDuration - AttackDuration == 1)
				{
					CellState((int)States.ScissorContract);
					CellRotation = 0;
				}
			}
			if (AttackTimer <= WindupDuration)
			{
				Windup();
			}
			else if (AttackTimer <= WindupDuration + AttackDuration)
			{
				Attack();
			}
			else
			{
				Endlag();
			}
			if (AttackTimer > WindupDuration + AttackDuration + EndlagDuration)
			{
				EndAttack(npc);
			}
		}
		private void CellState(int state)
		{
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<StardustMinion>())
				{
					Main.npc[i].ai[1] = state;
				}
			}
		}
		private void SpawnMinion(NPC npc, int cell)
		{
			if (FargoSoulsUtil.HostCheck)
			{
				NPC spawn = NPC.NewNPCDirect(npc.GetSource_FromThis(), npc.Center + Main.rand.Next(-20, 20) * Vector2.UnitX + Main.rand.Next(-20, 20) * Vector2.UnitY, ModContent.NPCType<StardustMinion>());
				spawn.ai[1] = 1;
				spawn.ai[2] = npc.whoAmI;
				spawn.ai[3] = cell;
			}
			return;
		}
		const int IdleTime = 60;
		private void Idle(NPC npc, Player player)
		{
			if (AttackTimer == 1)
			{
				CellState((int)States.Idle);
			}
			if (AttackTimer > IdleTime)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{

				}
				RandomAttack(npc);
			}
		}
		#endregion
	}
}
