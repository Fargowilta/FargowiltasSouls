using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.PHM;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class RoyalSubject : ModNPC
    {
        public override string Texture => "Terraria/NPC_222";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Royal Subject");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.QueenBee];
            //DisplayName.AddTranslation(GameCulture.Chinese, "皇家工蜂");
        }

        public override void SetDefaults()
        {
            npc.width = 66;
            npc.height = 66;
            npc.aiStyle = 43;
            aiType = NPCID.QueenBee;
            npc.damage = 25;
            npc.defense = 8;
            npc.lifeMax = 750;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.timeLeft = NPC.activeTime * 30;
            npc.npcSlots = 7f;
            npc.scale = 0.5f;
            npc.buffImmune[BuffID.Poisoned] = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.7 * System.Math.Max(1.0, bossLifeScale / 2));
            npc.damage = (int)(npc.damage * 0.9);
        }

        public override void AI()
        {
            if (!npc.GetGlobalNPC<EModeGlobalNPC>().masoBool[0])
            {
                npc.GetGlobalNPC<EModeGlobalNPC>().masoBool[0] = true;
                Main.npcTexture[npc.type] = Main.npcTexture[NPCID.QueenBee];
            }

            if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee)
                && !NPC.AnyNPCs(NPCID.QueenBee))
            {
                npc.life = 0;
                npc.HitEffect();
                npc.checkDead();
                return;
            }

            //tries to stinger, force into dash
            if (npc.ai[0] != 0)
            {
                npc.ai[0] = 0f;
                npc.netUpdate = true;
            }

            if (npc.ai[1] != 2f && npc.ai[1] != 3f)
            {
                npc.ai[1] = 2f;
                npc.netUpdate = true;
            }

            npc.position -= npc.velocity / 3;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, Main.rand.Next(60, 180));
            target.AddBuff(mod.BuffType("Infested"), 300);
            target.AddBuff(mod.BuffType("Swarming"), 600);
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override bool CheckDead()
        {
            NPC queenBee = FargoSoulsUtil.NPCExists(EModeGlobalNPC.beeBoss, NPCID.QueenBee);
            if (queenBee != null && Main.netMode != NetmodeID.MultiplayerClient
                && queenBee.GetEModeNPCMod<QueenBee>().BeeSwarmTimer < 600) //dont change qb ai during bee swarm attack
            {
                queenBee.ai[0] = 0f;
                queenBee.ai[1] = 4f; //trigger dashes, but skip the first one
                queenBee.ai[2] = -44f;
                queenBee.ai[3] = 0f;
                queenBee.netUpdate = true;
            }

            return base.CheckDead();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                //Main.PlaySound(npc.DeathSound, npc.Center);
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 5);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 0.75f;
                }

                for (int i = 303; i <= 308; i++)
                    Gore.NewGore(npc.position + new Vector2(Main.rand.Next(npc.width), Main.rand.Next(npc.height)), npc.velocity / 2, i, npc.scale);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.localAI[0] == 1)
            {
                if (npc.frameCounter > 4)
                {
                    npc.frame.Y += frameHeight;
                    npc.frameCounter = 0;
                }
                if (npc.frame.Y >= 4 * frameHeight)
                    npc.frame.Y = 0;
            }
            else
            {
                if (npc.frameCounter > 4)
                {
                    npc.frame.Y += frameHeight;
                    npc.frameCounter = 0;
                }
                if (npc.frame.Y < 4 * frameHeight)
                    npc.frame.Y = 4 * frameHeight;
                if (npc.frame.Y >= 12 * frameHeight)
                    npc.frame.Y = 4 * frameHeight;
            }
        }
    }
}