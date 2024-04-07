using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class SaucerRocket : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_448";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rocket");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 0) //when first spawned just move straight
            {
                Projectile.timeLeft++; //don't expire while counting down

                if (--Projectile.ai[1] == 0) //do for one tick right before homing
                {
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() + 6f);
                    Projectile.netUpdate = true;
                    for (int index1 = 0; index1 < 8; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy(index1 * 3.14159274101257 / 4.0, new Vector2()) * new Vector2(2f, 8f)).RotatedBy(Projectile.rotation - (float)Math.PI / 2, new Vector2());
                        int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1.5f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = Projectile.Center + vector2;
                        Main.dust[index2].velocity = Projectile.velocity * 0.0f;
                    }
                }
            }
            else //start homing
            {
                if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs && Main.npc[(int)Projectile.ai[0]].CanBeChasedBy()) //have target
                {
                    double num4 = (double)(Main.npc[(int)Projectile.ai[0]].Center - Projectile.Center).ToRotation() - (double)Projectile.velocity.ToRotation();
                    if (num4 > Math.PI)
                        num4 -= 2.0 * Math.PI;
                    if (num4 < -1.0 * Math.PI)
                        num4 += 2.0 * Math.PI;
                    Projectile.velocity = Projectile.velocity.RotatedBy(num4 * 0.2f, new Vector2());
                }
                else //retarget
                {
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000, true);
                    Projectile.netUpdate = true;
                    if (Projectile.ai[0] == -1) //no valid targets, selfdestruct
                        Projectile.Kill();
                }

                Projectile.tileCollide = true;
                if (++Projectile.localAI[0] > 5)
                {
                    Projectile.localAI[0] = 0f;
                    for (int index1 = 0; index1 < 4; ++index1)
                    {
                        Vector2 vector2 = (Vector2.UnitX * -8f + -Vector2.UnitY.RotatedBy(index1 * 3.14159274101257 / 4.0, new Vector2()) * new Vector2(2f, 4f)).RotatedBy(Projectile.rotation - (float)Math.PI / 2, new Vector2());
                        int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
                        Main.dust[index2].scale = 1.5f;
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].position = Projectile.Center + vector2;
                        Main.dust[index2].velocity = Projectile.velocity * 0.0f;
                    }
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;

            Vector2 vector21 = Vector2.UnitY.RotatedBy(Projectile.rotation, new Vector2()) * 8f * 2;
            int index21 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
            Main.dust[index21].position = Projectile.Center + vector21;
            Main.dust[index21].scale = 1f;
            Main.dust[index21].noGravity = true;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate < 0)
                target.immune[Projectile.owner] = 0;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.penetrate > -1)
            {
                Projectile.penetrate = -1;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 112;
                Projectile.position.X -= Projectile.width / 2;
                Projectile.position.Y -= Projectile.height / 2;
                for (int index = 0; index < 4; ++index)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                for (int index1 = 0; index1 < 40; ++index1)
                {
                    int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 2.5f);
                    Main.dust[index2].noGravity = true;
                    Dust dust1 = Main.dust[index2];
                    dust1.velocity *= 3f;
                    int index3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, 0.0f, 0.0f, 100, new Color(), 1.5f);
                    Dust dust2 = Main.dust[index3];
                    dust2.velocity *= 2f;
                    Main.dust[index3].noGravity = true;
                }
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    int index2 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Projectile.width * Main.rand.Next(100) / 100f, Projectile.height * Main.rand.Next(100) / 100f) - Vector2.One * 10f, new Vector2(), Main.rand.Next(61, 64), 1f);
                    Gore gore = Main.gore[index2];
                    gore.velocity *= 0.3f;
                    Main.gore[index2].velocity.X += Main.rand.Next(-10, 11) * 0.05f;
                    Main.gore[index2].velocity.Y += Main.rand.Next(-10, 11) * 0.05f;
                }
                Projectile.Damage();
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
    }
}