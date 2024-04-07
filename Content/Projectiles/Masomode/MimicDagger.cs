using FargowiltasSouls.Content.Buffs.Masomode;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class MimicDagger : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_93";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Magic Dagger");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MagicDagger);
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
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            }
            if (Projectile.ai[0] < 60)
            {
                Projectile.hostile = false;
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2;
                Projectile.velocity *= 0.95f;
            }
            else
            {
                Projectile.hostile = true;
                if (Projectile.ai[0] == 60)
                    Projectile.velocity = (Projectile.rotation - (float)Math.PI / 2).ToRotationVector2() * 9;
                if (Projectile.ai[0] > 80)
                    Projectile.rotation += 0.6f;
                if (Projectile.ai[0] > 180)
                {
                    Projectile.velocity.X *= 0.95f;
                    Projectile.velocity.Y += 0.3f;
                }
            }
            if (Main.rand.NextBool(12))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldCoin, Scale: 0.5f);


            if (Projectile.timeLeft < 180)
                Projectile.tileCollide = true;
            Projectile.ai[0]++;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MidasBuff>(), 600);
        }
    }
}
