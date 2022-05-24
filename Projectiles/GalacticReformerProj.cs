using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class GalacticReformerProj : ModProjectile
    {
        public int countdown = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuke");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = 16; //explosives AI
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 200 == 0)
            {
                CombatText.NewText(Projectile.Hitbox, new Color(51, 102, 0), countdown, true);
                countdown--;
            }

            Projectile.frameCounter++;   //Making the timer go up.
            if (Projectile.frameCounter >= 200)  //how fast animation is
            {
                Projectile.frame++; //Making the frame go up...
                Projectile.frameCounter = 0; //Resetting the timer.
                if (Projectile.frame > 4) //amt of frames - 1
                {
                    Projectile.frame = 0;
                }
            }
        }

        private const int radius = 300; //bigger = boomer
        private bool die;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (die)
                return (targetHitbox.Center.ToVector2() - projHitbox.Center.ToVector2()).Length() < Projectile.width / 2;
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
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (radius * 16 + 8) * 2;
                Projectile.Center = Projectile.position;
                Projectile.hostile = true;
                Projectile.damage = 2000;
                Projectile.Damage();
            }

            Vector2 position = Projectile.Center;

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

                        if (WorldGen.InWorld(xPosition, yPosition))
                        {
                            tile.ClearEverything();
                            Main.Map.Update(xPosition, yPosition, 255);
                        }
                    }

                    //NetMessage.SendTileSquare(-1, xPosition, yPosition, 1);
                }
            }

            Main.refreshMap = true;
            // Play explosion sound
            if (!Main.dedServ)
            {
                SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
                SoundEngine.PlaySound(SoundID.Item14, position);
            }
        }
    }
}