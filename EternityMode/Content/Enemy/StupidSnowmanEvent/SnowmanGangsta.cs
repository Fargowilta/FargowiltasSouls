using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.StupidSnowmanEvent
{
    public class SnowmanGangsta : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.SnowmanGangsta);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter > 300)
            {
                Counter = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget)
                {
                    for (int index = 0; index < 6; ++index)
                    {
                        Vector2 Speed = Vector2.UnitX * (Main.player[npc.target].Center.X - npc.Center.X);
                        Speed.X += Main.rand.Next(-40, 41);
                        Speed.Y += Main.rand.Next(-40, 41);
                        Speed.Normalize();
                        Speed *= 11f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center.X, npc.Center.Y, Speed.X, Speed.Y, ProjectileID.BulletSnowman, 20, 0f, Main.myPlayer);
                    }
                }
                SoundEngine.PlaySound(SoundID.Item38, npc.Center);
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
