using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class RazorbladeTyphoon2 : RazorbladeTyphoon
    {
        public override string Texture => "Terraria/Images/Projectile_409";

        public override void AI()
        {
            if (++Projectile.localAI[1] > 30 && Projectile.localAI[1] < 120)
                Projectile.velocity *= 1 + Projectile.ai[0];

            //vanilla typhoon dust (ech)
            int cap = Main.rand.Next(3);
            for (int index1 = 0; index1 < cap; ++index1)
            {
                Vector2 vector2_2 = (Main.rand.NextFloat() * (float)Math.PI - (float)Math.PI / 2f).ToRotationVector2();
                vector2_2 *= Main.rand.Next(3, 8);
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonWater, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity /= 4f;
                Main.dust[index2].velocity -= Projectile.velocity;
            }
            Projectile.rotation += 0.2f * (Projectile.velocity.X > 0f ? 1f : -1f);
            Projectile.frame++;
            if (Projectile.frame > 2)
                Projectile.frame = 0;
        }
    }
}