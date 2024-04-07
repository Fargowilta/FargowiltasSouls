using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2Ogre : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2OgreT2,
            NPCID.DD2OgreT3
        );

        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);

            entity.scale *= 2;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center, NPCID.DD2DarkMageT1, target: npc.target);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 500, BuffID.Stinky, false, 188);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.FargoSouls().AddBuffNoStack(ModContent.BuffType<StunnedBuff>(), 60);
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
            target.AddBuff(BuffID.BrokenArmor, 300);
        }
    }
}
