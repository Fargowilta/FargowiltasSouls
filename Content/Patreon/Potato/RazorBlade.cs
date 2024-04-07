using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;

using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.Potato
{
    public class RazorBlade : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20; //
            Projectile.penetrate = -1;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.tileCollide = false;
        }

        int MaxDistance = 100;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.GetModPlayer<PatreonPlayer>().RazorContainer)
            {
                Projectile.Kill();
            }

            Projectile.timeLeft++;
            Projectile.rotation += 0.3f;

            switch (Projectile.ai[0])
            {
                //default, follow mouse, but limited radius around player
                case 0:

                    Projectile.Center = Main.MouseWorld;
                    Projectile.velocity = Vector2.Zero;

                    int distance = (int)Vector2.Distance(Projectile.Center, player.Center);

                    if (distance > MaxDistance)
                    {
                        Vector2 angle = Vector2.Normalize(Projectile.Center - player.Center);
                        Projectile.Center = player.Center + (angle * MaxDistance);
                    }
                    break;
                //after hit by sword, just fly straight 
                case 1:
                    if (Projectile.ai[1]++ > 10)
                    {
                        Projectile.ai[0] = 2;
                    }

                    break;
                //returning to player
                case 2:
                    Projectile.velocity = Vector2.Normalize(player.Center - Projectile.Center) * 25;
                    distance = (int)Vector2.Distance(Projectile.Center, player.Center);

                    if (distance <= MaxDistance)
                    {
                        Projectile.ai[0] = 0;
                        Projectile.Center = Main.MouseWorld;
                    }

                    break;
            }
        }

        public virtual Asset<Texture2D> ChainTexture => ModContent.Request<Texture2D>("FargowiltasSouls/Content/Patreon/Potato/RazorChain");

        public override bool PreDraw(ref Color lightColor)
        {
            //dont draw when right on the player
            Player player = Main.player[Projectile.owner];

            if (Vector2.Distance(player.Center, Projectile.Center) < 5)
            {
                return true;
            }

            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 origin = new(ChainTexture.Width() * 0.5f, ChainTexture.Height() * 0.5f);
            float textureHeight = ChainTexture.Height();
            Vector2 mountedPosition = mountedCenter - position;
            float rotation = (float)Math.Atan2(mountedPosition.Y, mountedPosition.X) - 1.57f;
            bool invalidPosition = !(float.IsNaN(position.X) && float.IsNaN(position.Y) ||
                                     float.IsNaN(mountedPosition.X) && float.IsNaN(mountedPosition.Y));

            int numLoops = 0;

            while (invalidPosition)
            {
                if (numLoops++ > 100)
                {
                    break;
                }

                if (mountedPosition.Length() < textureHeight + 1.0)
                    invalidPosition = false;
                else
                {
                    Vector2 mountedClone = mountedPosition;
                    mountedClone.Normalize();
                    position += mountedClone * textureHeight;
                    mountedPosition = mountedCenter - position;
                    Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));

                    Main.spriteBatch.Draw(((Texture2D)ChainTexture), position - Main.screenPosition, null, color2, rotation, origin,
                        1f, SpriteEffects.None, 0.0f);
                }
            }

            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //bonus dmg when being launched
            if (Projectile.ai[0] == 1)
            {
                modifiers.SetCrit();
                modifiers.ArmorPenetration += 10;
            }
        }
    }
}
