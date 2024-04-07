using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BloodScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Moon Sickle");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DemonSickle);
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
            }
            Projectile.rotation += 0.8f;
            if (++Projectile.localAI[1] > 30 && Projectile.localAI[1] < 120)
                Projectile.velocity *= 1.03f;

            //for (int i = 0; i < 3; i++)
            //{
            Vector2 offset = new Vector2(0, -20).RotatedBy(Projectile.rotation);
            offset = offset.RotatedByRandom(MathHelper.Pi / 6);
            int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Vortex, 0f, 0f, 150);
            Main.dust[d].position += offset;
            float velrando = Main.rand.Next(20, 31) / 10;
            Main.dust[d].velocity = Projectile.velocity / velrando;
            Main.dust[d].noGravity = true;
            //}

            if (Projectile.timeLeft < 180)
                Projectile.tileCollide = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(ModContent.BuffType<ShadowflameBuff>(), 300);
                target.AddBuff(BuffID.Bleeding, 600);
                target.AddBuff(BuffID.Obstructed, 15);
            }

            target.AddBuff(ModContent.BuffType<BerserkedBuff>(), 300);
            target.AddBuff(ModContent.BuffType<CurseoftheMoonBuff>(), 120);
        }
    }
}
