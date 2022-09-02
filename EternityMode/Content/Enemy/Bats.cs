using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy
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

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

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
                    target.AddBuff(ModContent.BuffType<Hypothermia>(), 600);
                    break;

                case NPCID.Lavabat:
                    target.AddBuff(BuffID.Burning, 240);
                    break;

                case NPCID.GiantBat:
                    target.AddBuff(BuffID.Confused, 240);
                    break;

                case NPCID.GiantFlyingFox:
                    target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 300);
                    break;

                default:
                    break;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<Items.Consumables.RabiesShot>(), 5));
        }
    }
}
