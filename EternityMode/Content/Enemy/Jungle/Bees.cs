using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class Bees : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Bee,
            NPCID.BeeSmall
        );

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
        }

        //public override void OnSpawn(NPC npc)
        //{
        //    base.OnSpawn(npc);

        //    if (Main.rand.NextBool(5))
        //        switch ((Main.hardMode && !FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee)) ? Main.rand.Next(16, 21) : Main.rand.Next(16))
        //        {
        //            case 0: npc.Transform(NPCID.Hornet); break;
        //            case 1: npc.Transform(NPCID.HornetFatty); break;
        //            case 2: npc.Transform(NPCID.HornetHoney); break;
        //            case 3: npc.Transform(NPCID.HornetLeafy); break;
        //            case 4: npc.Transform(NPCID.HornetSpikey); break;
        //            case 5: npc.Transform(NPCID.HornetStingy); break;
        //            case 6: npc.Transform(NPCID.LittleHornetFatty); break;
        //            case 7: npc.Transform(NPCID.LittleHornetHoney); break;
        //            case 8: npc.Transform(NPCID.LittleHornetLeafy); break;
        //            case 9: npc.Transform(NPCID.LittleHornetSpikey); break;
        //            case 10: npc.Transform(NPCID.LittleHornetStingy); break;
        //            case 11: npc.Transform(NPCID.BigHornetFatty); break;
        //            case 12: npc.Transform(NPCID.BigHornetHoney); break;
        //            case 13: npc.Transform(NPCID.BigHornetLeafy); break;
        //            case 14: npc.Transform(NPCID.BigHornetSpikey); break;
        //            case 15: npc.Transform(NPCID.BigHornetStingy); break;
        //            case 16: npc.Transform(NPCID.MossHornet); break;
        //            case 17: npc.Transform(NPCID.BigMossHornet); break;
        //            case 18: npc.Transform(NPCID.GiantMossHornet); break;
        //            case 19: npc.Transform(NPCID.LittleMossHornet); break;
        //            case 20: npc.Transform(NPCID.TinyMossHornet); break;
        //        }
        //}

        public override bool CheckDead(NPC npc)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.beeBoss, NPCID.QueenBee))
            {
                npc.life = 0;
                npc.HitEffect();
                return false;
            }

            return base.CheckDead(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Infested>(), 300);
            target.AddBuff(ModContent.BuffType<Swarming>(), 600);
        }
    }
}
