using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class Bee : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_566";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bee");
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.velocity.X * .1f;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }

            if ((Projectile.wet || Projectile.lavaWet) && !Projectile.honeyWet) //die in liquids
            {
                Projectile.Kill();
            }

            if (++Projectile.localAI[0] > 30 && Projectile.localAI[0] < 90)
            {
                float rotation = Projectile.velocity.ToRotation();
                Vector2 vel = Main.player[(int)Projectile.ai[0]].Center - Projectile.Center;
                float targetAngle = vel.ToRotation();
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, Projectile.ai[1]));
            }

            Projectile.tileCollide = Projectile.localAI[0] > 180;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(ModContent.BuffType<InfestedBuff>(), 300);
            target.AddBuff(ModContent.BuffType<SwarmingBuff>(), 600);
        }
    }
}