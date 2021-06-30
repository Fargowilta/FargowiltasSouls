using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomSickleSplit2 : AbomSickleSplit1
    {
        public override string Texture => "FargowiltasSouls/Projectiles/AbomBoss/AbomSickle";

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                Main.PlaySound(SoundID.Item8, projectile.Center);
            }
            projectile.rotation += 0.8f;
            if (++projectile.localAI[1] > 30 && projectile.localAI[1] < 100)
                projectile.velocity *= 1.06f;
            for (int i = 0; i < 6; i++)
            {
                Vector2 offset = new Vector2(0, -20).RotatedBy(projectile.rotation);
                offset = offset.RotatedByRandom(MathHelper.Pi / 6);
                int d = Dust.NewDust(projectile.Center, 0, 0, 87, 0f, 0f, 150);
                Main.dust[d].position += offset;
                float velrando = Main.rand.Next(20, 31) / 10;
                Main.dust[d].velocity = projectile.velocity / velrando;
                Main.dust[d].noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            //dont split
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(mod.BuffType("AbomFang"), 300);
                target.AddBuff(mod.BuffType("Berserked"), 120);
            }
            target.AddBuff(BuffID.Bleeding, 600);
        }
    }
}