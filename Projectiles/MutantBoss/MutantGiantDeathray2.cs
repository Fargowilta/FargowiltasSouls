using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Souls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantGiantDeathray2 : Deathrays.MutantSpecialDeathray
    {
        public MutantGiantDeathray2() : base(600) { }

        public int dustTimer;
        public bool stall;
        public bool BeBrighter => Projectile.ai[0] > 0f;

        public PrimDrawer LaserDrawer { get; private set; } = null;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Phantasmal Deathray");
            ProjectileID.Sets.DismountsPlayersOnHit[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.netImportant = true;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;

            if (FargoSoulsWorld.MasochistModeReal)
                maxTime += 180;
        }

        public override bool? CanDamage()
        {
            return Projectile.scale >= 5f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            base.AI();

            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

            Projectile.timeLeft = 2;

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
            if (npc != null)
            {
                Projectile.Center = npc.Center + Main.rand.NextVector2Circular(5, 5) + Vector2.UnitX.RotatedBy(npc.ai[3]) * (npc.ai[0] == -7 ? 100 : 175) * Projectile.scale / 10f;
                if (npc.ai[0] == -7) //death animation, not actual attack
                {
                    maxTime = 255;
                }
                else if (npc.ai[0] == -5) //final spark
                {
                    if (npc.localAI[2] > 30) //mutant is forcing a despawn
                    {
                        //so this should disappear too
                        if (Projectile.localAI[0] < maxTime - 90)
                            Projectile.localAI[0] = maxTime - 90;
                    }
                    else if (stall)
                    {
                        stall = false;

                        Projectile.localAI[0] -= 1;
                        Projectile.netUpdate = true;

                        npc.ai[2] -= 1;
                        npc.netUpdate = true;
                    }
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Zombie_104") with { Volume = 1.5f }, Main.player[Main.myPlayer].Center);
            }
            float num801 = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 7f * num801;
            if (FargoSoulsWorld.MasochistModeReal)
                Projectile.scale *= 5f;

            if (Projectile.scale > num801)
                Projectile.scale = num801;
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;
            float num804 = npc.ai[3] - 1.57079637f;
            //if (Projectile.ai[0] != 0f) num804 -= (float)Math.PI;
            float oldRot = Projectile.rotation;
            Projectile.rotation = num804;
            num804 += 1.57079637f;
            Projectile.velocity = num804.ToRotationVector2();
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

            //if (--dustTimer < -1 && npc.ai[0] != -7)
            //{
            //    dustTimer = 50;

            //    float diff = MathHelper.WrapAngle(Projectile.rotation - oldRot);
            //    //if (npc.HasPlayerTarget && Math.Abs(MathHelper.WrapAngle(npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - Projectile.velocity.ToRotation())) < Math.Abs(diff)) diff = 0;
            //    diff *= 15f;

            //    const int ring = 220; //LAUGH
            //    for (int i = 0; i < ring; ++i)
            //    {
            //        Vector2 speed = Projectile.velocity.RotatedBy(diff) * 24f;

            //        Vector2 vector2 = (-Vector2.UnitY.RotatedBy(i * 3.14159274101257 * 2 / ring) * new Vector2(8f, 16f)).RotatedBy(Projectile.velocity.ToRotation() + diff);
            //        int index2 = Dust.NewDust(npc.Center, 0, 0, 111, 0.0f, 0.0f, 0, new Color(), 1f);
            //        Main.dust[index2].scale = 2.5f;
            //        Main.dust[index2].noGravity = true;
            //        Main.dust[index2].position = npc.Center;
            //        Main.dust[index2].velocity = vector2 * 2.5f + speed;

            //        index2 = Dust.NewDust(npc.Center, 0, 0, 111, 0.0f, 0.0f, 0, new Color(), 1f);
            //        Main.dust[index2].scale = 2.5f;
            //        Main.dust[index2].noGravity = true;
            //        Main.dust[index2].position = npc.Center;
            //        Main.dust[index2].velocity = vector2 * 1.75f + speed * 2;
            //    }
            //}

            if (Projectile.damage > 0 && Main.LocalPlayer.active && Projectile.Colliding(Projectile.Hitbox, Main.LocalPlayer.Hitbox))
            {
                Main.LocalPlayer.immune = false;
                Main.LocalPlayer.immuneTime = 0;
                Main.LocalPlayer.hurtCooldowns[0] = 0;
                Main.LocalPlayer.hurtCooldowns[1] = 0;
                Main.LocalPlayer.ClearBuff(ModContent.BuffType<GoldenStasis>());
            }
        }

        private int hits;

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            base.ModifyHitPlayer(target, ref damage, ref crit);
            DamageRampup(ref damage);
            if (hits > 180)
                target.endurance = 0;
        }

        private void DamageRampup(ref int damage)
        {
            int tempHits = hits - 90;
            if (tempHits > 0)
            {
                double modifier = Math.Min(1.0 + tempHits / 6.0, 100.0);
                damage = (int)(damage * modifier);
            }
            else
            {
                damage = (int)(hits / 90.0 * damage);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            hits++;

            stall = true;

            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);

            //if (FargowiltasSouls.Instance.MasomodeEXLoaded) target.AddBuff(ModLoader.GetMod("MasomodeEX").BuffType("MutantJudgement"), 3600);

            target.immune = false;
            target.immuneTime = 0;
            target.hurtCooldowns[0] = 0;
            target.hurtCooldowns[1] = 0;

            target.velocity = -0.4f * Vector2.UnitY;
        }

        public float WidthFunction(float trailInterpolant)
        {
            // Grow rapidly from the start to full length. Any more than this notably distorts the texture.
            float baseWidth = Projectile.scale * Projectile.width;
            //if (trailInterpolant < 0.05f)
                return baseWidth;

            // Grow to 2x width by the end. Any more than this distorts the texture too much.
            return MathHelper.Lerp(baseWidth, baseWidth * 2, trailInterpolant);
        }

        public Color ColorFunction(float trailInterpolant) => Color.Lerp(new(31, 187, 192), new(51, 255, 191), trailInterpolant) * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case.
            if (Projectile.velocity == Vector2.Zero)
                return false;

            // If it isnt set, set the prim instance.
            LaserDrawer ??= new(WidthFunction, ColorFunction, GameShaders.Misc["FargowiltasSouls:MutantDeathray"]);

            // Get the laser end position.
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

            // Create 8 points that span across the draw distance from the projectile center.

            // This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
            // sharp lines.
            Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity * 650;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Set shader parameters. This one takes a fademap and a color.

            // The laser should fade to white in the middle.
            Color brightColor = new(194, 255, 242);
            if (BeBrighter)
                brightColor *= 2;
            GameShaders.Misc["FargowiltasSouls:MutantDeathray"].UseColor(brightColor);
            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            GameShaders.Misc["FargowiltasSouls:MutantDeathray"].SetShaderTexture(FargosTextureRegistry.MutantStreak);
            // Draw a big glow above the start of the laser, to help mask the intial fade in due to the immense width.

            Texture2D glowTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Projectiles/GlowRing").Value;

            Vector2 glowDrawPosition = Projectile.Center - Projectile.velocity * (BeBrighter ? 90f : 180f);

            Main.EntitySpriteDraw(glowTexture, glowDrawPosition - Main.screenPosition, null, brightColor, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None, 0);
            LaserDrawer.DrawPrims(baseDrawPoints, -Main.screenPosition, 60);
            return false;
        }
    }
}