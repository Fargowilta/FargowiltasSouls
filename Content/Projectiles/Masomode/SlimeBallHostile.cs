using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SlimeBallHostile : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BossWeapons/SlimeBall";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slime Ball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 14;
            Projectile.aiStyle = 14;
            Projectile.hostile = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.BlueTorch, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default, 2f);
            Main.dust[dust].noGravity = true;
        }

        public override void OnKill(int timeleft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.BlueTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default, 2f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 2f;
                num469 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.BlueTorch, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[num469].velocity *= 2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 60);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Slimed, 60);
        }
    }
}