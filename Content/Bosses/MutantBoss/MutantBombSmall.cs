using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.MutantBoss
{
    public class MutantBombSmall : MutantBomb
    {
        public override string Texture => $"Terraria/Images/Projectile_{(FargoSoulsUtil.AprilFools ? "687" : "645")}";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 275;
            Projectile.height = 275;
            Projectile.scale = 0.75f;
            Projectile.FargoSouls().TimeFreezeImmune = false;
        }

        public override bool? CanDamage()
        {
            if (Projectile.frame > 2 && Projectile.frame <= 4)
            {
                Projectile.FargoSouls().GrazeCD = 1;
                return false;
            }
            return true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                }
            }
        }
    }
}