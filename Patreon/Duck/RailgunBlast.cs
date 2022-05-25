using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Duck
{
    public class RailgunBlast : Projectiles.Deathrays.MutantSpecialDeathray
    {
        public RailgunBlast() : base(20, 1.25f) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Railgun Blast");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.hide = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            Projectile.frameCounter += 60;

            Player player = Main.player[Projectile.owner];

            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 20;

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            float num801 = 10f;
            Projectile.scale = (float)Math.Cos(Projectile.localAI[0] * MathHelper.PiOver2 / maxTime) * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;

            if (player.active && !player.dead && !player.ghost)
            {
                Projectile.Center = player.Center + (18f * Projectile.scale + 50f) * Projectile.velocity + Main.rand.NextVector2Circular(5, 5);
            }
            else
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.frame = Main.rand.Next(15);
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Railgun"), Projectile.Center);
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Thunder"), player.Center + Projectile.velocity * Math.Min(Main.screenWidth / 2, 900f));
                }

                Vector2 dustPos = player.Center + Projectile.velocity * 50f;

                for (int i = 0; i < 40; i++)
                {
                    int dust = Dust.NewDust(dustPos - new Vector2(16, 16), 32, 32, 31, 0f, 0f, 100, default(Color), 4f);
                    Main.dust[dust].velocity -= Projectile.velocity * 2;
                    Main.dust[dust].velocity *= 3f;
                    Main.dust[dust].velocity += player.velocity / 2;
                }

                for (int i = 0; i < 50; i++)
                {
                    int dust = Dust.NewDust(dustPos - new Vector2(16, 16), 32, 32, 6, 0f, 0f, 100, default(Color), 4f);
                    Main.dust[dust].scale *= Main.rand.NextFloat(1, 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity -= Projectile.velocity * 2;
                    Main.dust[dust].velocity = Main.dust[dust].velocity.RotatedByRandom(MathHelper.ToRadians(40)) * 6f;
                    Main.dust[dust].velocity *= Main.rand.NextFloat(1f, 3f);
                    Main.dust[dust].velocity += player.velocity / 2;
                    dust = Dust.NewDust(dustPos - new Vector2(16, 16), 32, 32, 6, 0f, 0f, 100, default(Color), 4f);
                    Main.dust[dust].velocity -= Projectile.velocity * 2;
                    Main.dust[dust].velocity *= 5f;
                    Main.dust[dust].velocity *= Main.rand.NextFloat(1f, 2f);
                    Main.dust[dust].velocity += player.velocity / 2;
                }

                float scaleFactor9 = 2;
                for (int j = 0; j < 20; j++)
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), dustPos, -Projectile.velocity, Main.rand.Next(61, 64), scaleFactor9);
                    Main.gore[gore].velocity -= Projectile.velocity;
                    Main.gore[gore].velocity.Y += 2f;
                    Main.gore[gore].velocity *= 4f;
                    Main.gore[gore].velocity += player.velocity / 2;
                }
            }

            if (++Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            //Projectile.scale = num801;
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            //float num804 = Main.npc[(int)Projectile.ai[1]].ai[3] - 1.57079637f;
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            //Projectile.rotation = num804;
            //num804 += 1.57079637f;
            //Projectile.velocity = num804.ToRotationVector2();
            float num805 = 3f;
            float num806 = (float)Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num805];
            //Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 3000f, array3);
            for (int i = 0; i < array3.Length; i++)
                array3[i] = 3000f;
            float num807 = 0f;
            int num3;
            for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
            {
                num807 += array3[num808];
                num3 = num808;
            }
            num807 /= num805;
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount);
            /*Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809 = num3 + 1)
            {
                float num810 = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, 244, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 244, 0f, 0f, 100, default(Color), 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }*/
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            Projectile.position -= Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57079637f;

            const int increment = 75;
            for (int i = 0; i < array3[0]; i += increment)
            {
                float offset = i + Main.rand.NextFloat(-increment, increment);
                if (offset < 0)
                    offset = 0;
                if (offset > array3[0])
                    offset = array3[0];
                float spawnRange = Projectile.scale * 32f;
                int d = Dust.NewDust(Projectile.position + Projectile.velocity * offset + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-spawnRange, spawnRange),
                    Projectile.width, Projectile.height, 87, 0f, 0f, 0, new Color(255, 255, 255, 150), Main.rand.NextFloat(2f, 4f));
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity += Projectile.velocity * 2f;
                Main.dust[d].velocity *= Main.rand.NextFloat(12f, 24f) / 10f * Projectile.scale;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 12;
            target.AddBuff(BuffID.Electrified, 600);

            if (Projectile.owner == Main.myPlayer && Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Projectiles.LightningArc>()] < 60)
            {
                const int max = 3;
                int count = max;
                foreach (NPC n in Main.npc)
                {
                    if (n.CanBeChasedBy() && n.whoAmI != target.whoAmI && target.Distance(n.Center) < 1500 && Collision.CanHitLine(target.Center, 0, 0, n.Center, 0, 0))
                    {
                        if (--count < 0)
                            break;
                        Vector2 vel = Main.rand.NextFloat(10f, 20f) * target.DirectionTo(n.Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, vel, ModContent.ProjectileType<Projectiles.LightningArc>(),
                            Projectile.damage / 10, Projectile.knockBack / 10, Projectile.owner, vel.ToRotation(), Main.rand.Next(80));
                    }
                }
                int spray = (max + count) / 2;
                for (int i = -spray; i <= spray; i++)
                {
                    Vector2 vel = Main.rand.NextFloat(10f, 20f) * Projectile.velocity.RotatedBy(MathHelper.ToRadians(30) / spray * (i + Main.rand.NextFloat(-0.5f, 0.5f)));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, vel, ModContent.ProjectileType<Projectiles.LightningArc>(),
                        Projectile.damage / 10, Projectile.knockBack / 10, Projectile.owner, vel.ToRotation(), Main.rand.Next(80));
                }
                Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Projectiles.LightningArc>()] += max * 2;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ItemID.BrightYellowDye);
            shader.Apply(Projectile, new Terraria.DataStructures.DrawData?());

            bool retval = base.PreDraw(ref lightColor);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            return retval;
        }
    }
}
