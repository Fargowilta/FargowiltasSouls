using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Hallow
{
    public class IlluminantBat : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.IlluminantBat);

        public int Counter;
        public bool IsFakeBat;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            bitWriter.WriteBit(IsFakeBat);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            IsFakeBat = bitReader.ReadBit();
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            Counter++;

            if (IsFakeBat)
            {
                if (Main.netMode == NetmodeID.Server && Counter <= 65 && Counter % 15 == 5) //mp sync
                {
                    npc.netUpdate = true;
                    NetSync(npc);
                }

                if (npc.alpha > 200)
                    npc.alpha = 200;

                if (npc.lifeMax > 100)
                    npc.lifeMax = 100;

                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;
            }
            else if (Counter > 600)
            {
                Counter = 0;

                if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 1000
                    && Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(NPCID.IlluminantBat) < 10)
                {
                    int bat = FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, npc.type,
                        velocity: new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5)));
                    if (bat != Main.maxNPCs)
                        Main.npc[bat].GetGlobalNPC<IlluminantBat>().IsFakeBat = true;
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<MutantNibble>(), 600);
        }

        public override bool CheckDead(NPC npc)
        {
            if (IsFakeBat)
            {
                npc.active = false;
                if (npc.DeathSound != null)
                    SoundEngine.PlaySound(npc.DeathSound.Value, npc.Center);
                return false;
            }

            return base.CheckDead(npc);
        }
    }
}
