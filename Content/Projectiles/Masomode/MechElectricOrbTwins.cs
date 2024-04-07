using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class MechElectricOrbTwins : MechElectricOrb
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/MechElectricOrb";
        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();

            if (++Projectile.ai[1] < 75) //straight accel
                Projectile.velocity *= 1.06f;

            Player target = FargoSoulsUtil.PlayerExists(Projectile.ai[0]);
            if (target != null)
            {
                float rotation = Projectile.velocity.ToRotation();
                Vector2 vel = target.Center - Projectile.Center;
                float targetAngle = vel.ToRotation();

                //if reti alive and player isnt behind projectile
                if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.retiBoss, NPCID.Retinazer)
                    && System.Math.Abs(MathHelper.WrapAngle(targetAngle - rotation)) < MathHelper.PiOver2)
                {
                    const float deadZone = 600;
                    const float maxHomingRampupDistance = 1800;
                    const float maxLerp = 0.5f;

                    float ratio = (Main.npc[EModeGlobalNPC.retiBoss].Distance(target.Center) - deadZone) / (maxHomingRampupDistance - deadZone);
                    ratio *= ratio; //so the lerp effect ramps up much more violently at distance
                    if (ratio < 0)
                        ratio = 0;
                    if (ratio > 1)
                        ratio = 1;

                    float lerp = maxLerp * ratio;
                    Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, lerp));
                }
            }
        }
    }
}