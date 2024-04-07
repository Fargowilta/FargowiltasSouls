using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Gittle
{
    public class RoombaPetProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Turtle);
            AIType = ProjectileID.Turtle;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.turtle = false; // Relic from AIType
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();
            if (player.dead)
            {
                modPlayer.RoombaPet = false;
            }
            if (modPlayer.RoombaPet)
            {
                Projectile.timeLeft = 2;
            }

            int num113 = Dust.NewDust(new Vector2(Projectile.Center.X - Projectile.direction * (Projectile.width / 2), Projectile.Center.Y + Projectile.height / 2), Projectile.width, 6, DustID.Snow, 0f, 0f, 0, default, 1f);
            Main.dust[num113].noGravity = true;
            Main.dust[num113].velocity *= 0.3f;
            Main.dust[num113].noLight = true;
        }
    }
}