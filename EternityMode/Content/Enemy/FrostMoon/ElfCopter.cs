using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Projectiles.Masomode;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.FrostMoon
{
    public class ElfCopter : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.ElfCopter);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.localAI[0] >= 14f)
            {
                npc.localAI[0] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float num8 = Main.player[npc.target].Center.X - npc.Center.X;
                    float num9 = Main.player[npc.target].Center.Y - npc.Center.Y;
                    float num10 = num8 + Main.rand.Next(-35, 36);
                    float num11 = num9 + Main.rand.Next(-35, 36);
                    float num12 = num10 * (1f + Main.rand.Next(-20, 21) * 0.015f);
                    float num13 = num11 * (1f + Main.rand.Next(-20, 21) * 0.015f);
                    float num14 = 10f / (float)Math.Sqrt(num12 * num12 + num13 * num13);
                    float num15 = num12 * num14;
                    float num16 = num13 * num14;
                    float SpeedX = num15 * (1f + Main.rand.Next(-20, 21) * 0.0125f);
                    float SpeedY = num16 * (1f + Main.rand.Next(-20, 21) * 0.0125f);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center.X, npc.Center.Y, SpeedX, SpeedY, ModContent.ProjectileType<ElfCopterBullet>(), 32, 0f, Main.myPlayer);
                }
            }
        }
    }
}
