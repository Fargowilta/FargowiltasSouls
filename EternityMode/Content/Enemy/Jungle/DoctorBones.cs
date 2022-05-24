using FargowiltasSouls.EternityMode.NPCMatching;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class DoctorBones : Shooters
    {
        public DoctorBones() : base(480, ProjectileID.Boulder, 14, 4) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DoctorBones);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<Items.Accessories.Masomode.SkullCharm>(), 10));
        }
    }
}
