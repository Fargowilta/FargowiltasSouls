using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.PirateInvasion
{
    public class PirateCaptain : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.PirateCaptain);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //60 delay for cannonball, 8 for bullets
            if (npc.ai[2] > 0f && npc.localAI[2] >= 20f && npc.ai[1] <= 30)
            {
                //npc.localAI[2]++;
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 speed = Main.player[npc.target].Center - npc.Center;
                    speed.Y -= Math.Abs(speed.X) * 0.2f; //account for gravity
                    speed.X += Main.rand.Next(-20, 21);
                    speed.Y += Main.rand.Next(-20, 21);
                    speed.Normalize();
                    speed *= 12f;
                    npc.localAI[2] = 0f;
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 cannonSpeed = speed;
                        cannonSpeed.X += Main.rand.Next(-10, 11) * 0.3f;
                        cannonSpeed.Y += Main.rand.Next(-10, 11) * 0.3f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, cannonSpeed, ProjectileID.CannonballHostile, Main.expertMode ? 80 : 100, 0f, Main.myPlayer);
                    }
                }
                //npc.ai[2] = 0f;
                //npc.ai[1] = 0f;
                npc.netUpdate = true;
            }
        }
    }
}
