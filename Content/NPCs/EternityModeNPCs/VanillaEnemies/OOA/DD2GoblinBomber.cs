﻿using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.OOA
{
    public class DD2GoblinBomber : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.DD2GoblinBomberT1,
            NPCID.DD2GoblinBomberT2,
            NPCID.DD2GoblinBomberT3
        );

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (FargoSoulsUtil.HostCheck)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center,
                        new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-9f, -6f)),
                        ProjectileID.DD2GoblinBomb, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0, Main.myPlayer);
                }
            }
        }
    }
}
