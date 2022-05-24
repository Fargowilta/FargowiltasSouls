using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class QueenFlocko : AbomBoss.AbomFlocko
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 150;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], NPCID.IceQueen);
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            Player player = Main.player[npc.target];

            Vector2 target = player.Center;
            target.X += 700 * Projectile.ai[1];

            Vector2 distance = target - Projectile.Center;
            float length = distance.Length();
            if (length > 100f)
            {
                distance /= 8f;
                Projectile.velocity = (Projectile.velocity * 23f + distance) / 24f;
            }
            else
            {
                if (Projectile.velocity.Length() < 12f)
                    Projectile.velocity *= 1.05f;
            }

            if (++Projectile.localAI[1] > 120) //fire frost wave
            {
                Projectile.localAI[1] = 0f;
                SoundEngine.PlaySound(SoundID.Item120, Projectile.position);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vel = Projectile.DirectionTo(player.Center) * 7f;
                    float xDistance = Math.Abs(player.Center.X - Projectile.Center.X);
                    for (int i = -1; i <= 1; i++)
                    {
                        Vector2 velocity = vel.RotatedBy(MathHelper.ToRadians(4) * i);
                        velocity.X = (player.Center.X - Projectile.Center.X) / 100f;
                        int p = Projectile.NewProjectile(npc.GetSource_FromThis(), Projectile.Center,
                            velocity, ProjectileID.FrostWave, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        if (p != Main.maxProjectiles)
                            Main.projectile[p].timeLeft = 101;
                    }
                }
            }

            Projectile.rotation += Projectile.velocity.Length() / 12f * (Projectile.velocity.X > 0 ? -0.2f : 0.2f);
            if (++Projectile.frameCounter > 3)
            {
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }
    }
}