using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies
{
    public class Bats : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.JungleBat,
            NPCID.IceBat,
            NPCID.Vampire,
            NPCID.VampireBat,
            NPCID.GiantFlyingFox,
            NPCID.Hellbat,
            NPCID.Lavabat,
            NPCID.IlluminantBat,
            NPCID.CaveBat,
            NPCID.GiantBat,
            NPCID.SporeBat
        );

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            //if (Main.rand.NextBool(4)) Horde(npc, Main.rand.Next(5) + 1);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            switch (npc.type)
            {
                case NPCID.CaveBat:
                    target.AddBuff(BuffID.Bleeding, 300);
                    break;

                case NPCID.Hellbat:
                    target.AddBuff(BuffID.OnFire, 240);
                    break;

                case NPCID.JungleBat:
                    target.AddBuff(BuffID.Poisoned, 240);
                    break;

                case NPCID.IceBat:
                    target.AddBuff(ModContent.BuffType<HypothermiaBuff>(), 600);
                    break;

                case NPCID.Lavabat:
                    target.AddBuff(BuffID.Burning, 240);
                    break;

                case NPCID.GiantBat:
                    target.AddBuff(BuffID.Confused, 240);
                    break;

                case NPCID.GiantFlyingFox:
                    target.AddBuff(ModContent.BuffType<BloodthirstyBuff>(), 300);
                    break;

                default:
                    break;
            }
        }
    }
}
