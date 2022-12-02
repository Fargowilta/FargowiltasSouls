using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class LifeScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Ray");
        }
        public override void SetDefaults()
        {
            Projectile.width = 720;
            Projectile.height = 2400;
            Projectile.aiStyle = 0;
            Projectile.hostile = false;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }

            /*if (Projectile.ai[1] <= 0)
            {
                Projectile.alpha += 17;
            }*/
            if ((Projectile.ai[0] > 0 && Projectile.ai[1] <= 0) || (Projectile.ai[0] > 1 && Projectile.ai[1] > 0))
            {
                Projectile.Kill();
            }
            if (Projectile.ai[1] > 0)
            {
                Projectile.alpha = 250 - ((int)Projectile.ai[1] * 3);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
            Projectile.ai[0] += 1f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.ai[1] <= 0 && Projectile.ai[0] > 0)
                behindProjectiles.Add(index);
        }
    }
}
