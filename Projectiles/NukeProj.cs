using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class NukeProj : ModProjectile
    {
        public int countdown = 5;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Nuke");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = 16; //explosives AI
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1000;
        }

        public override void AI()
        {
            if (projectile.timeLeft % 200 == 0)
            {
                CombatText.NewText(projectile.Hitbox, new Color(51, 102, 0), countdown, true);
                countdown--;
            }
        }

        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            projectile.frameCounter++;   //Making the timer go up.
            if (projectile.frameCounter >= 200)  //how fast animation is
            {
                projectile.frame++; //Making the frame go up...
                projectile.frameCounter = 0; //Resetting the timer.
                if (projectile.frame > 4) //amt of frames - 1
                {
                    projectile.frame = 0;
                }
            }

            return true;
        }

        private const int radius = 300; //bigger = boomer
        private bool die;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (die)
                return (targetHitbox.Center.ToVector2() - projHitbox.Center.ToVector2()).Length() < projectile.width / 2;
            return null;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.boss)
                return false;
            return null;
        }

        public override void Kill(int timeLeft)
        {
            if (!die)
            {
                die = true;
                projectile.position = projectile.Center;
                projectile.width = projectile.height = (radius * 16 + 8) * 2;
                projectile.Center = projectile.position;
                projectile.hostile = true;
                projectile.damage = 2000;
                projectile.Damage();
            }

            Vector2 position = projectile.Center;

            for (int x = -radius; x <= (radius); x++)
            {
                for (int y = -radius; y <= (radius); y++)
                {
                    if (Math.Sqrt(x * x + y * y) <= radius)   //circle
                    {
                        int xPosition = (int)(x + position.X / 16.0f);
                        int yPosition = (int)(y + position.Y / 16.0f);
                        if (xPosition < 0 || xPosition >= Main.maxTilesX || yPosition < 0 || yPosition >= Main.maxTilesY)
                            continue;

                        Tile tile = Main.tile[xPosition, yPosition];

                        if (tile == null) continue;

                        FargoGlobalTile.ClearTileWithNet(xPosition, yPosition);
                        FargoGlobalTile.ClearWallWithNet(xPosition, yPosition);
                        FargoGlobalTile.ClearLiquid(xPosition, yPosition);
                        FargoGlobalTile.SquareUpdate(xPosition, yPosition);


                        if (WorldGen.InWorld(xPosition, yPosition))
                            Main.Map.Update(xPosition, yPosition, 255);
                    }

                    //NetMessage.SendTileSquare(-1, xPosition, yPosition, 1);
                }
            }

            Main.refreshMap = true;
            // Play explosion sound
            Main.PlaySound(SoundID.Item15, projectile.position);
            Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
        }
    }
}