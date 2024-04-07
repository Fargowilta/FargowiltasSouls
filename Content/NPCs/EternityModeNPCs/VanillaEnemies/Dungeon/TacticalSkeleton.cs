using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Dungeon
{
    public class TacticalSkeleton : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.TacticalSkeleton);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //num3 = 120, damage = 40/50, num8 = 0
            if (npc.ai[2] > 0f && npc.ai[1] <= 65f)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    for (int index = 0; index < 6; ++index)
                    {
                        float num6 = Main.player[npc.target].Center.X - npc.Center.X;
                        float num10 = Main.player[npc.target].Center.Y - npc.Center.Y;
                        float num11 = 11f / (float)Math.Sqrt(num6 * num6 + num10 * num10);
                        float num18 = num6 + Main.rand.Next(-40, 41);
                        float num20 = num10 + Main.rand.Next(-40, 41);
                        float SpeedX = num18 * num11;
                        float SpeedY = num20 * num11;
                        int damage = Main.expertMode ? 40 : 50;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center.X, npc.Center.Y, SpeedX, SpeedY, ProjectileID.MeteorShot, damage, 0f, Main.myPlayer);
                    }
                }
                SoundEngine.PlaySound(SoundID.Item38, npc.Center);
                npc.ai[2] = 0f;
                npc.ai[1] = 0f;
                npc.ai[3] = 0f; //specific to me
                npc.netUpdate = true;
            }
        }
    }
}
