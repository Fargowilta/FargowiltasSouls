using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Deathrays
{
    public class PhantasmalDeathrayWOF : BaseDeathray
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Deathrays/PhantasmalDeathrayWOF";
        public PhantasmalDeathrayWOF() : base(90) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Divine Deathray");
        }

        public override void AI()
        {
            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.WallofFleshEye);
            if (npc == null)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = npc.Center + (npc.width - 36) * Vector2.UnitX.RotatedBy(npc.rotation + (npc.direction > 0 ? 0 : MathHelper.Pi));
            }
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/Zombie_104"), Projectile.Center);
            }
            float maxScale = 1f;
            if (WorldSavingSystem.MasochistModeReal)
            {
                maxScale = Main.rand.NextFloat(2.5f, 5f);
                FargoSoulsUtil.ScreenshakeRumble(6);
            }
            else
                FargoSoulsUtil.ScreenshakeRumble(3);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 5f * maxScale;
            if (Projectile.scale > maxScale)
            {
                Projectile.scale = maxScale;
            }
            //float num804 = Projectile.velocity.ToRotation();
            //num804 += Projectile.ai[0];
            //Projectile.rotation = num804 - 1.57079637f;

            Projectile.velocity = (npc.rotation + (npc.direction > 0 ? 0 : MathHelper.Pi)).ToRotationVector2();
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            float num805 = 3f;
            float num806 = Projectile.width;
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
                float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1f : 1f) * 1.57079637f;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new((float)Math.Cos((double)num810) * num811, (float)Math.Sin((double)num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, DustID.CopperCoin, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
                num3 = num809;
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, DustID.CopperCoin, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }
            //DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            //Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

            /*if (FargoSoulsUtil.HostCheck)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(array3[0]), Main.rand.NextFloat(-22 * Projectile.scale, 22 * Projectile.scale));
                Projectile.NewProjectile(Projectile.Center + offset, Vector2.Zero, 
                    ModContent.ProjectileType<WOFBlast2>(), 0, 0f, Main.myPlayer, 0f, Projectile.whoAmI);
            }*/

            const int increment = 100;
            for (int i = 0; i < array3[0]; i += increment)
            {
                if (!Main.rand.NextBool(5))
                    continue;
                float offset = i + Main.rand.NextFloat(-increment, increment);
                if (offset < 0)
                    offset = 0;
                if (offset > array3[0])
                    offset = array3[0];
                int d = Dust.NewDust(Projectile.position + Projectile.velocity * offset,
                    Projectile.width, Projectile.height, DustID.Frost, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
            if (WorldSavingSystem.MasochistModeReal && Projectile.Colliding(Projectile.Hitbox, Main.LocalPlayer.Hitbox))
            {
                Main.LocalPlayer.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 1.2f;
            }
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (WorldSavingSystem.MasochistModeReal)
                modifiers.Knockback *= 0;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SmiteBuff>(), 60 * 30);
            if (WorldSavingSystem.MasochistModeReal && Main.getGoodWorld)
                target.AddBuff(ModContent.BuffType<UnstableBuff>(), 300);
        }
    }
}