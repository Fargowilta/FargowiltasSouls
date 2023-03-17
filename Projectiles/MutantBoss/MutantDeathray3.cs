using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantDeathray3 : Deathrays.BaseDeathray , IPixelPrimitiveDrawer
    {
        public PrimDrawer LaserDrawer { get; private set; } = null;

        public override string Texture => "FargowiltasSouls/Projectiles/Deathrays/PhantasmalDeathray";
        public MutantDeathray3() : base(270, grazeCD: 30) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Blazing Deathray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CooldownSlot = -1; //iframe interaction with prime lol
        }

        float displayMaxTime;

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            Projectile.position -= Projectile.velocity;

            float DECELERATION = FargoSoulsWorld.MasochistModeReal ? 0.9716f : 0.9712f;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
            if (npc != null)
            {
                //float minTime = npc.ai[3] - 60;
                //if (npc.ai[0] == 27 && minTime > Projectile.localAI[0])
                //{
                //    Projectile.velocity *= (float)Math.Pow(DECELERATION, minTime - Projectile.localAI[0]);
                //    Projectile.localAI[0] = minTime;
                //}
            }
            /*else
            {
                Projectile.Kill();
                return;
            }*/
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Zombie_104") { Volume = 0.5f }, Projectile.Center);

                displayMaxTime = Math.Min(maxTime, Projectile.timeLeft + 2);
            }
            float num801 = 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                //float angle = MathHelper.WrapAngle(Projectile.velocity.ToRotation());
                //float compare = MathHelper.PiOver2 * Math.Sign(angle) - angle;
                //Main.NewText(MathHelper.ToDegrees(compare));
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / displayMaxTime) * 6f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();

            if (Projectile.localAI[0] > 45 && Projectile.localAI[0] < maxTime - 120)
                Projectile.ai[0] *= DECELERATION;
            num804 += Projectile.ai[0];

            Projectile.rotation = num804 - 1.57079637f;
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
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
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
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            //if (Projectile.localAI[0] == maxTime - 1) Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().GrazeCD = 0;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
                target.AddBuff(BuffID.Burning, 300);
            }
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public float WidthFunction(float trailInterpolant) => Projectile.width * Projectile.scale * 1.3f;

        public Color ColorFunction(float trailInterpolant) => new(192, 36, 31);//Color.Lerp(new(31, 187, 192), new(51, 255, 191), trailInterpolant) * Projectile.Opacity;

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            LaserDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["FargowiltasSouls:GenericDeathray"]);

            // Get the laser end position.
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * drawDistance;

            // Create 8 points that span across the draw distance from the projectile center.

            // This allows the drawing to be pushed back, which is needed due to the shader fading in at the start to avoid
            // sharp lines.
            Vector2 initialDrawPoint = Projectile.Center - Projectile.velocity * 50f;
            Vector2[] baseDrawPoints = new Vector2[8];
            for (int i = 0; i < baseDrawPoints.Length; i++)
                baseDrawPoints[i] = Vector2.Lerp(initialDrawPoint, laserEnd, i / (float)(baseDrawPoints.Length - 1f));

            // Set shader parameters. This one takes a fademap and a color.

            // GameShaders.Misc["FargoswiltasSouls:MutantDeathray"].UseImage1(); cannot be used due to only accepting vanilla paths.
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].SetShaderTexture(FargosTextureRegistry.MutantStreak);
            // The laser should fade to this in the middle.
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].UseColor(new Color(255, 108, 151));
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["stretchAmount"].SetValue(1);
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["scrollSpeed"].SetValue(3f);
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["uColorFadeScaler"].SetValue(1f);
            GameShaders.Misc["FargowiltasSouls:GenericDeathray"].Shader.Parameters["useFadeIn"].SetValue(true);

            LaserDrawer.DrawPixelPrims(baseDrawPoints, -Main.screenPosition, 30);
        }
    }
}