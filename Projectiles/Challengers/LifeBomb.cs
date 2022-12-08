using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeBomb : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Life Mine");
		}
		public override void SetDefaults()
		{
			Projectile.width = 25;
			Projectile.height = 25;
			Projectile.aiStyle = 0;
			Projectile.hostile = true;
			AIType = 14;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override bool? CanDamage() => false;

		public override void AI()
		{
			Projectile.ai[0] += 2f;
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87, Projectile.velocity.X, Projectile.velocity.Y, 0, default(Color), 0.25f);
			Projectile.rotation = Projectile.rotation + (float)Math.PI / 16f;
			if (Projectile.ai[0] >= 60f)
			{
				Projectile.velocity = Projectile.velocity * 0.96f;
			}
			if (Projectile.ai[0] >= 100f)
			{
				Projectile.Kill();
			}
		}
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }

        public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
			int damage = Projectile.damage;
			if (Main.netMode != NetmodeID.MultiplayerClient)
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2), 0f, 0f, ModContent.ProjectileType<LifeBombExplosion>(), damage, 0f, Main.myPlayer);
		}

		public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
    }
}
