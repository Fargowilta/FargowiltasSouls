using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2LightningBug : Shooters
    {
        public DD2LightningBug() : base(240, ModContent.ProjectileType<LightningVortexHostile>(), 0.5f, 1, DustID.Vortex) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DD2LightningBugT3);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 400, ModContent.BuffType<LightningRodBuff>(), false, DustID.Vortex);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Electrified, 300);
            target.FargoSouls().AddBuffNoStack(BuffID.Webbed, 60);
        }
    }
}
