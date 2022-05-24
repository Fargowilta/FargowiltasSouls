using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.StupidSnowmanEvent
{
    public class SnowBalla : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SnowBalla);

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.ai[2] == 8f)
            {
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                float num3 = 10f;
                Vector2 vector2 = new Vector2(npc.position.X + npc.width * 0.5f - npc.direction * 12, npc.position.Y + npc.height * 0.25f);
                float num4 = Main.player[npc.target].position.X + Main.player[npc.target].width / 2f - vector2.X;
                float num5 = Main.player[npc.target].position.Y - vector2.Y;
                float num6 = (float)Math.Sqrt(num4 * num4 + num5 * num5);
                float num7 = num3 / num6;
                float SpeedX = num4 * num7;
                float SpeedY = num5 * num7;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int Damage = 35;
                    int Type = 109;
                    int p = Projectile.NewProjectile(npc.GetSource_FromThis(), vector2.X, vector2.Y, SpeedX, SpeedY, Type, Damage, 0f, Main.myPlayer);
                    Main.projectile[p].ai[0] = 2f;
                    Main.projectile[p].timeLeft = 300;
                    Main.projectile[p].friendly = false;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                    npc.netUpdate = true;
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Hypothermia>(), 300);
            target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}
