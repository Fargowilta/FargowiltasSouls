using FargowiltasSouls.Content.Bosses.Champions.Terra;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosLightningOrb : TerraLightningOrb
    {
        public override string Texture => "Terraria/Images/Projectile_465";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.timeLeft = 600;
        }

        public override bool? CanDamage()
        {
            return Projectile.alpha == 0;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 30)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            else
            {
                Projectile.alpha += 10;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }

            if (++Projectile.localAI[1] > 120 && Projectile.localAI[1] < 240)
                Projectile.velocity *= 1.05f;

            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            /*if (Main.rand.NextBool(3))
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust :echbegone:
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-Projectile.width * 0.2f * Projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = Projectile.Center + vector21 * Projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - Projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-Projectile.width * 0.6f * Projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2 * Projectile.scale;
                Main.dust[index2].noGravity = true;
            }*/
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 360);
            //if (WorldSavingSystem.MasochistMode) target.AddBuff(ModContent.BuffType<LightningRod>(), 360);
        }
    }
}