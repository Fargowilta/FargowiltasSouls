using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class TurtleShield : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Turtle Shield");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if (Projectile.frame != 6)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = (Projectile.frame + 1) % 7;
                }
            }

            if (modPlayer.TurtleShellHP <= 3)
            {
                Projectile.localAI[0] = 1;
            }

            if (!modPlayer.ShellHide)
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = 1.5f }, Projectile.Center);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.localAI[0] == 1)
            {
                return new Color(255, 132, 105);
            }

            return base.GetAlpha(lightColor);
        }
    }
}