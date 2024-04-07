using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs
{
	public class DetonatingBubbleEX : ModNPC
	{
		public override string Texture => "Terraria/Images/NPC_371";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Detonating Bubble");
			//DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "爆炸泡泡");
			Main.npcFrameCount[NPC.type] = 2;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

			this.ExcludeFromBestiary();
		}

		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 36;
			NPC.damage = 100;
			NPC.lifeMax = 5000;//500;
			NPC.HitSound = SoundID.NPCHit3;
			NPC.DeathSound = SoundID.NPCDeath3;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.knockBackResist = 0f;
			NPC.alpha = 255;
			NPC.lavaImmune = true;
			NPC.aiStyle = -1;
			NPC.chaseable = false;
		}

		public override void AI()
		{
			if (NPC.buffTime[0] != 0)
			{
				NPC.buffImmune[NPC.buffType[0]] = true;
				NPC.DelBuff(0);
			}

			if (NPC.alpha > 50)
				NPC.alpha -= 30;
			else
				NPC.alpha = 50;

			NPC.velocity *= 1.04f;

			NPC.ai[0]++;
			if (NPC.ai[0] >= 120f)
			{
				NPC.life = 0;
				NPC.checkDead();
				NPC.active = false;
			}
		}

		public override bool CanHitPlayer(Player target, ref int CooldownSlot)
		{
			CooldownSlot = 1;
			return true;
		}

		public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
		{
		}

		public override bool CheckDead()
		{
			NPC.FargoSouls().Needled = false;
			return true;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			if (target.hurtCooldowns[1] == 0)
			{
				target.AddBuff(BuffID.Wet, 420);
				if (WorldSavingSystem.MasochistModeReal)
					target.AddBuff(ModContent.BuffType<SqueakyToyBuff>(), 120);
				target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 600);
				target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 20 * 60);
				target.FargoSouls().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (TextureAssets.Npc[NPC.type].IsLoaded)
				NPC.frame.Y = TextureAssets.Npc[NPC.type].Value.Height / 2;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
	}
}