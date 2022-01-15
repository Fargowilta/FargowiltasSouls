using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class MoonLordMoon : Champions.CosmosMoon
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/CosmosMoon";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //DisplayName.SetDefault("Moon");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.alpha = 200;
        }

        public override bool CanDamage()
        {
            return base.CanDamage() && projectile.localAI[0] > 120;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                Main.PlaySound(SoundID.Item92, projectile.Center);
                projectile.rotation = projectile.velocity.ToRotation();

                Main.PlaySound(SoundID.Item89, projectile.position);

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoPlayer>().Screenshake = 30;

                for (int num615 = 0; num615 < 20; num615++)
                {
                    int num616 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[num616].velocity *= 1.4f;
                }

                for (int num617 = 0; num617 < 40; num617++)
                {
                    int num618 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[num618].noGravity = true;
                    Main.dust[num618].velocity *= 7f;
                    num618 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[num618].velocity *= 3f;
                }

                for (int num619 = 0; num619 < 2; num619++)
                {
                    float scaleFactor9 = 0.4f;
                    if (num619 == 1) scaleFactor9 = 0.8f;
                    int num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore97 = Main.gore[num620];
                    gore97.velocity.X = gore97.velocity.X + 1f;
                    Gore gore98 = Main.gore[num620];
                    gore98.velocity.Y = gore98.velocity.Y + 1f;
                    num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore99 = Main.gore[num620];
                    gore99.velocity.X = gore99.velocity.X - 1f;
                    Gore gore100 = Main.gore[num620];
                    gore100.velocity.Y = gore100.velocity.Y + 1f;
                    num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore101 = Main.gore[num620];
                    gore101.velocity.X = gore101.velocity.X + 1f;
                    Gore gore102 = Main.gore[num620];
                    gore102.velocity.Y = gore102.velocity.Y - 1f;
                    num620 = Gore.NewGore(projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[num620].velocity *= scaleFactor9;
                    Gore gore103 = Main.gore[num620];
                    gore103.velocity.X = gore103.velocity.X - 1f;
                    Gore gore104 = Main.gore[num620];
                    gore104.velocity.Y = gore104.velocity.Y - 1f;
                }


                for (int k = 0; k < 20; k++) //make visual dust
                {
                    Vector2 dustPos = projectile.position;
                    dustPos.X += Main.rand.Next(projectile.width);
                    dustPos.Y += Main.rand.Next(projectile.height);

                    for (int i = 0; i < 15; i++)
                    {
                        int dust = Dust.NewDust(dustPos, 32, 32, 31, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[dust].velocity *= 1.4f;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        int dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 7f;
                        dust = Dust.NewDust(dustPos, 32, 32, DustID.Fire, 0f, 0f, 100, default(Color), 1.5f);
                        Main.dust[dust].velocity *= 3f;
                    }

                    float scaleFactor9 = 0.5f;
                    for (int j = 0; j < 4; j++)
                    {
                        int gore = Gore.NewGore(dustPos, default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[gore].velocity *= scaleFactor9;
                        Main.gore[gore].velocity.X += 1f;
                        Main.gore[gore].velocity.Y += 1f;
                    }
                }

                const int num226 = 30;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.UnitX * 40f;
                    vector6 = vector6.RotatedBy(((num227 - (num226 / 2 - 1)) * 6.28318548f / num226), default(Vector2)) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Fire, 0f, 0f, 0, default(Color), 3f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
            }

            NPC moonLord = FargoSoulsUtil.NPCExists(NPCs.EModeGlobalNPC.moonBoss, NPCID.MoonLordCore);
            Projectile arena = FargoSoulsUtil.ProjectileExists(FargoSoulsUtil.GetByUUIDReal(projectile.owner, projectile.ai[0], ModContent.ProjectileType<LunarRitual>()));
            if (moonLord != null && arena != null && moonLord.ai[0] != 2f)
            {
                if (moonLord.GetEModeNPCMod<MoonLordCore>().VulnerabilityState == 4)
                    projectile.timeLeft = 60;

                float orbitRange = Math.Abs(projectile.ai[1]) + 400f * moonLord.life / moonLord.lifeMax;

                if (++projectile.localAI[0] < 60)
                {
                    Vector2 desiredPosition = arena.Center + projectile.velocity * orbitRange;
                    projectile.Center = Vector2.Lerp(projectile.Center, desiredPosition, 0.05f);

                    projectile.position += arena.velocity;
                    projectile.position -= projectile.velocity;

                    projectile.alpha -= 10;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;
                }
                else
                {
                    projectile.alpha = 0;
                    projectile.velocity = Vector2.Zero;
                    projectile.rotation += MathHelper.ToRadians(3.5f) * Math.Min(1f, (projectile.localAI[0] - 60) / 180) * Math.Sign(projectile.ai[1]);
                    projectile.Center = arena.Center + orbitRange * projectile.rotation.ToRotationVector2();
                }
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                projectile.Kill();
            }
        }
    }
}