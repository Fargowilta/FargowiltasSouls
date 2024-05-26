using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class RainLightning : LightningArc
    {
        public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Default;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = true;
        }
        int telegraphTimer;
        public int TelegraphTime => (WorldSavingSystem.MasochistModeReal ? 110 : 190) * (1 + Projectile.extraUpdates);

        public bool Telegraphing => (telegraphTimer >= 0 && telegraphTimer < TelegraphTime);

        public override bool PreAI()
        {
            if (Telegraphing)
            {
                Projectile.timeLeft = 300 * (Projectile.extraUpdates + 1);
                telegraphTimer++;
                if (Main.rand.NextBool(30))
                {
                    Particle spark = new SparkParticle(Projectile.Center + Vector2.UnitY * (900 - Main.rand.Next(30, 300)), -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2 * 0.2f) * Main.rand.NextFloat(3, 13), Color.Cyan, Main.rand.NextFloat(0.3f, 0.7f), Main.rand.Next(10, 25));
                    spark.Spawn();
                }

                return false;
            }
            if (telegraphTimer >= TelegraphTime)
            {
                /*
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 4, Projectile.height / 2);
                spawnPos -= Main.rand.NextFloat(900f, 1800f) * Vector2.UnitY;
                float ai1 = Projectile.Center.Y + Main.rand.NextFloat(-Projectile.height / 4, Projectile.height / 4);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, 12f * Vector2.UnitY, ModContent.ProjectileType<RainLightning>(),
                    Projectile.damage, Projectile.knockBack / 2, Projectile.owner, Vector2.UnitY.ToRotation(), ai1);
                */
                telegraphTimer = -1000;
                Projectile.velocity = -Vector2.UnitY * 14;
                Projectile.tileCollide = true;
                return false;
            }


            return base.PreAI();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= 10;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.hardMode)
            {
                SoundEngine.PlaySound(SoundID.Item62);
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<RainExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
            return base.OnTileCollide(oldVelocity);
        }
        public override bool PreDraw(ref Color lightColor) //Prevent drawing while telegraphing
        {
            if (Telegraphing)
                return false;

            return base.PreDraw(ref lightColor);
        }
        public override bool CanHitPlayer(Player target) => Telegraphing ? false : base.CanHitPlayer(target);
    }
}