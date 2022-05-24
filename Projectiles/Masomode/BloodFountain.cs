using FargowiltasSouls.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class BloodFountain : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Empty";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }

        public override void AI()
        {
            if (++Projectile.localAI[0] == 4)
            {
                if (--Projectile.ai[0] > 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center - 16 * Vector2.UnitY, Vector2.Zero, Projectile.type, Projectile.damage, 0f, Main.myPlayer, Projectile.ai[0]);
                }
            }

            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<BloodDust>(), Main.rand.Next(-50, 50) * 1f, -3f, 0, default(Color), 1f);
                Main.dust[d].scale = 2f;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.1f;
                Main.dust[d].velocity += Projectile.velocity * 0.1f;
            }
        }

        //public override void OnHitPlayer(Player target, int damage, bool crit)
        //{
        //    target.AddBuff(ModContent.BuffType<Anticoagulation>(), 600);
        //}
    }
}