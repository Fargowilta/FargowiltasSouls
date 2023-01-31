using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordMoon : Champions.CosmosMoon
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/CosmosMoon";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Moon");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.alpha = 200;
        }

        public override bool? CanDamage()
        {
            if (base.CanDamage() == false)
                return false;

            return Projectile.localAI[0] > 120;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
                Projectile.rotation = Projectile.velocity.ToRotation();

                SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

                for (int num615 = 0; num615 < 20; num615++)
                {
                    int num616 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[num616].velocity *= 1.4f;
                }

                for (int num617 = 0; num617 < 40; num617++)
                {
                    int num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[num618].noGravity = true;
                    Main.dust[num618].velocity *= 7f;
                    num618 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[num618].velocity *= 3f;
                }

                for (int num619 = 0; num619 < 2; num619++)
                {
                    float scaleFactor9 = 0.4f;
                    if (num619 == 1) scaleFactor9 = 0.8f;
                    int num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore97 = Main.gore[num620];
                    gore97.velocity.X = gore97.velocity.X + 1f;
                    Gore gore98 = Main.gore[num620];
                    gore98.velocity.Y = gore98.velocity.Y + 1f;
                    num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore99 = Main.gore[num620];
                    gore99.velocity.X = gore99.velocity.X - 1f;
                    Gore gore100 = Main.gore[num620];
                    gore100.velocity.Y = gore100.velocity.Y + 1f;
                    num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore101 = Main.gore[num620];
                    gore101.velocity.X = gore101.velocity.X + 1f;
                    Gore gore102 = Main.gore[num620];
                    gore102.velocity.Y = gore102.velocity.Y - 1f;
                    num620 = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore103 = Main.gore[num620];
                    gore103.velocity.X = gore103.velocity.X - 1f;
                    Gore gore104 = Main.gore[num620];
                    gore104.velocity.Y = gore104.velocity.Y - 1f;
                }


                for (int k = 0; k < 20; k++) //make visual dust
                {
                    Vector2 dustPos = Projectile.position;
                    dustPos.X += Main.rand.Next(Projectile.width);
                    dustPos.Y += Main.rand.Next(Projectile.height);

                    for (int i = 0; i < 15; i++)
                    {
                        int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[dust].velocity *= 1.4f;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default(Color), 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 7f;
                        dust = Dust.NewDust(dustPos, 32, 32, DustID.Torch, 0f, 0f, 100, default(Color), 1.5f);
                        Main.dust[dust].velocity *= 3f;
                    }

                    float scaleFactor9 = 0.5f;
                    for (int j = 0; j < 4; j++)
                    {
                        int gore = Gore.NewGore(Projectile.GetSource_FromThis(), dustPos, default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[gore].velocity *= scaleFactor9;
                        Main.gore[gore].velocity.X += 1f;
                        Main.gore[gore].velocity.Y += 1f;
                    }
                }

                const int num226 = 30;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.UnitX * 40f;
                    vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Torch, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
            }

            NPC moonLord = FargoSoulsUtil.NPCExists(NPCs.EModeGlobalNPC.moonBoss, NPCID.MoonLordCore);
            Projectile arena = FargoSoulsUtil.ProjectileExists(FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, Projectile.ai[0], ModContent.ProjectileType<LunarRitual>()));
            if (moonLord != null && arena != null && moonLord.ai[0] != 2f)
            {
                if (moonLord.GetGlobalNPC<MoonLordCore>().VulnerabilityState == 4)
                    Projectile.timeLeft = 60;

                float orbitRange = Math.Abs(Projectile.ai[1]) + 400f * moonLord.life / moonLord.lifeMax;

                if (++Projectile.localAI[0] < 60)
                {
                    Vector2 desiredPosition = arena.Center + Projectile.velocity * orbitRange;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, desiredPosition, 0.05f);

                    Projectile.position += arena.velocity;
                    Projectile.position -= Projectile.velocity;

                    Projectile.alpha -= 10;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }
                else
                {
                    Projectile.alpha = 0;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.rotation += MathHelper.ToRadians(3.5f) * Math.Min(1f, (Projectile.localAI[0] - 60) / 180) * Math.Sign(Projectile.ai[1]);
                    Projectile.Center = arena.Center + orbitRange * Projectile.rotation.ToRotationVector2();
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.Kill();
            }
        }
    }
}