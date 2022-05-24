using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Desert
{
    public class Antlion : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Antlion);

        public int AttackTimer;
        public int VacuumTimer;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            VacuumTimer++;
            if (VacuumTimer >= 30)
            {
                foreach (Player p in Main.player.Where(x => x.active && !x.dead))
                {
                    if (p.HasBuff(ModContent.BuffType<Stunned>()) && npc.Distance(p.Center) < 250)
                    {
                        Vector2 velocity = Vector2.Normalize(npc.Center - p.Center) * 5f;
                        p.velocity += velocity;
                    }
                }
                VacuumTimer = 0;
            }

            //sand balls
            if (AttackTimer > 0)
            {
                if (AttackTimer == 75)
                {
                    SoundEngine.PlaySound(SoundID.Item5, npc.position);
                }

                AttackTimer--;
            }

            if (AttackTimer <= 0)
            {
                float num265 = 12f;
                Vector2 pos = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float velocityX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - pos.X;
                float velocityY = Main.player[npc.target].position.Y - pos.Y;
                float num268 = (float)Math.Sqrt((double)(velocityX * velocityX + velocityY * velocityY));
                num268 = num265 / num268;
                velocityX *= num268 * 1.5f;
                velocityY *= num268 * 1.5f;

                if (Main.netMode != NetmodeID.MultiplayerClient && Main.player[npc.target].Center.Y <= npc.Center.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    int num269 = 10;
                    int num270 = 31;
                    int proj = Projectile.NewProjectile(npc.GetSource_FromThis(), pos.X, pos.Y, velocityX, velocityY, num270, num269, 0f, Main.myPlayer, 0f, 0);
                    if (proj != Main.maxProjectiles)
                    {
                        Main.projectile[proj].ai[0] = 2f;
                        Main.projectile[proj].timeLeft = 300;
                        Main.projectile[proj].friendly = false;
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                    }
                    npc.netUpdate = true;

                    AttackTimer = 75;
                }
            }

            //never fire sand balls from vanilla
            npc.ai[0] = 10;
        }
    }
}
