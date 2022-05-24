using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.OOA
{
    public class DD2LightningBug : Shooters
    {
        public DD2LightningBug() : base(240, ModContent.ProjectileType<LightningVortexHostile>(), 0.5f, 1, DustID.Vortex) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.DD2LightningBugT3);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            EModeGlobalNPC.Aura(npc, 400, ModContent.BuffType<LightningRod>(), false, DustID.Vortex);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(BuffID.Electrified, 300);
            target.GetModPlayer<FargoSoulsPlayer>().AddBuffNoStack(BuffID.Webbed, 60);
        }
    }
}
