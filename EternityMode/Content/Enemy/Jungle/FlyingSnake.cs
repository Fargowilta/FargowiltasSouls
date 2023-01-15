using System.IO;
using Terraria.ModLoader.IO;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class FlyingSnake : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.FlyingSnake);

        public bool Phase2;

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(npc, bitWriter, binaryWriter);

            bitWriter.WriteBit(Phase2);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(npc, bitReader, binaryReader);

            Phase2 = bitReader.ReadBit();
        }

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;
            npc.lifeMax *= 2;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (!Phase2 && npc.life < npc.lifeMax / 2)
            {
                Phase2 = true;
                FargoSoulsUtil.DustRing(npc.Center, 32, DustID.Torch, 10f, default, 3f);
                NetSync(npc);
            }

            if (Phase2)
            {
                npc.position += npc.velocity;
                npc.knockBackResist = 0f;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Infested>(), 300);
            target.AddBuff(ModContent.BuffType<ClippedWings>(), 300);
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (Phase2)
            {
                drawColor.R = 255;
                drawColor.G /= 2;
                drawColor.B /= 2;
                return drawColor;
            }

            return base.GetAlpha(npc, drawColor);
        }
    }
}
