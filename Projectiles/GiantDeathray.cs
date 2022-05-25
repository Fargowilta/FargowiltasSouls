using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GiantDeathray : MutantBoss.MutantGiantDeathray2
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Phantasmal Deathray");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            maxTime = 180;

            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;

            CooldownSlot = -1;
        }

        public override void AI()
        {
            base.AI();

            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 2;

            Vector2? vector78 = null;
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }

            Projectile.Center = Main.player[Projectile.owner].Center;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (Projectile.localAI[0] == 0f)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Sounds/Zombie_104") { Volume = 0.5f }, Projectile.Center);
            }
            float num801 = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= maxTime)
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * 3.14159274f / maxTime) * 1.5f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;
            float num804 = Projectile.velocity.ToRotation();
            float oldRot = Projectile.rotation;
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

            //if (--dustTimer < -1)
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
            //        int index2 = Dust.NewDust(Main.player[Projectile.owner].Center, 0, 0, 111, 0.0f, 0.0f, 0, new Color(), 1f);
            //        Main.dust[index2].scale = 2.5f;
            //        Main.dust[index2].noGravity = true;
            //        Main.dust[index2].position = Main.player[Projectile.owner].Center;
            //        Main.dust[index2].velocity = vector2 * 2.5f + speed;

            //        index2 = Dust.NewDust(Main.player[Projectile.owner].Center, 0, 0, 111, 0.0f, 0.0f, 0, new Color(), 1f);
            //        Main.dust[index2].scale = 2.5f;
            //        Main.dust[index2].noGravity = true;
            //        Main.dust[index2].position = Main.player[Projectile.owner].Center;
            //        Main.dust[index2].velocity = vector2 * 1.75f + speed * 2;
            //    }
            //}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 300);
        }
    }
}