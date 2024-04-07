using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantEyeWavy : MutantEye
    {
        public override string Texture => FargoSoulsUtil.AprilFools ?
            "FargowiltasSouls/Content/Bosses/MutantBoss/MutantEye_April" :
            "Terraria/Images/Projectile_452";

        public override int TrailAdditive => 150;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 180;
            Projectile.FargoSouls().TimeFreezeImmune = true;
            CooldownSlot = 0;
        }

        private float Amplitude => Projectile.ai[0];
        private float Period => Projectile.ai[1];
        private float Counter => Projectile.localAI[1] * 4;

        public float oldRot;

        public override void AI()
        {
            NPC mutant = FargoSoulsUtil.NPCExists(EModeGlobalNPC.mutantBoss);
            if (mutant != null && (mutant.ai[0] == -5f || mutant.ai[0] == -7f))
            {
                float targetRotation = mutant.ai[3];

                float speed = Projectile.velocity.Length();
                float rotation = targetRotation + (float)Math.PI / 4 * (float)Math.Sin(2 * (float)Math.PI * Counter / Period) * Amplitude;
                Projectile.velocity = speed * rotation.ToRotationVector2();

                if (oldRot != 0)
                {
                    Vector2 oldCenter = Projectile.Center;
                    Projectile.Center = mutant.Center + (Projectile.Center - mutant.Center).RotatedBy(targetRotation - oldRot);

                    Vector2 diff = Projectile.Center - oldCenter;
                    for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                    {
                        Projectile.oldPos[i] += diff;
                    }
                }

                oldRot = targetRotation;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            Projectile.localAI[0] += 0.1f;

            base.AI();
        }

        public override void OnKill(int timeleft)
        {
            //prevents base dust from forming
        }
    }
}