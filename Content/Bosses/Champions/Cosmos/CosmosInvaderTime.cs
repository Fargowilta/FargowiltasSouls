using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosInvaderTime : CosmosInvader
    {
        public override string Texture => "Terraria/Images/Projectile_539";

        public override void SetDefaults()
        {
            base.SetDefaults();

            if (WorldSavingSystem.MasochistModeReal)
                Projectile.FargoSouls().GrazeCheck = Projectile => false;
        }

        public override bool PreAI()
        {
            if (!spawned)
            {
                Projectile.localAI[1] = Projectile.velocity.Length();
                Projectile.velocity = Vector2.Zero;

                SoundEngine.PlaySound(SoundID.Item25 with { Volume = 0.5f, Pitch = 0 }, Projectile.Center);

                for (int index1 = 0; index1 < 4; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                }
                for (int index1 = 0; index1 < 20; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBurst_Blue, 0.0f, 0.0f, 200, new Color(), 3.7f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 3f;
                }
                for (int index1 = 0; index1 < 20; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0.0f, 0.0f, 0, new Color(), 2.7f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 3f;
                }
                for (int index1 = 0; index1 < 10; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 0, new Color(), 1.5f);
                    Main.dust[index2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2()) * Projectile.width / 2f;
                    Main.dust[index2].noGravity = true;
                    Dust dust = Main.dust[index2];
                    dust.velocity *= 3f;
                }
            }
            return base.PreAI();
        }

        public override void AI()
        {
            const int startup = 60;
            if (Projectile.localAI[0] < startup)
                Projectile.velocity += Projectile.ai[1].ToRotationVector2() * Projectile.localAI[1] / startup;

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            Projectile.localAI[0]++;
        }
    }
}