using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantScythe2 : MutantScythe1
    {
        public override string Texture => "FargowiltasSouls/Projectiles/AbomBoss/AbomSickle";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Sickle");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            projectile.hide = false;
        }

        public override void PostAI()
        {
            if (projectile.timeLeft == 180) //draw attention to myself
            {
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 87, 0, 0, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 6f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<Souls.IronParry>(), 0, 0f, Main.myPlayer);
            }
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            
        }
    }
}