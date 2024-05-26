﻿using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse
{
    public class Eyezor : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Eyezor);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 8)
            {
                Counter = 0;
                int p = npc.HasPlayerTarget ? npc.target : npc.FindClosestPlayer();
                if (p != -1 && Main.player[p].active && !Main.player[p].ghost && !Main.player[p].dead && npc.Distance(Main.player[p].Center) < 600)
                {
                    Vector2 velocity = Main.player[p].Center - npc.Center;
                    velocity.Normalize();
                    velocity *= 4f;
                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, velocity, ProjectileID.EyeFire, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage, 0.8f), 0f, Main.myPlayer);
                }
            }
        }
    }
}
