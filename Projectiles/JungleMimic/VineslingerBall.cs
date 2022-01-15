using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.JungleMimic
{
    public class VineslingerBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Vineslinger Ball");
        }
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = -1; 
            projectile.melee = true; 
            projectile.aiStyle = 15;
        }

        public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("FargowiltasSouls/Projectiles/JungleMimic/VineslingerChain");

            Vector2 position = projectile.Center;
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?();
            Vector2 origin = new Vector2((float)texture.Width * 0.5f, (float)texture.Height * 0.5f);
            float num1 = (float)texture.Height;
            Vector2 vector2_4 = mountedCenter - position;
            float rotation = (float)Math.Atan2((double)vector2_4.Y, (double)vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if ((double)vector2_4.Length() < (double)num1 + 1.0)
                {
                    flag = false;
                }
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    position += vector2_1 * num1;
                    vector2_4 = mountedCenter - position;
                    Microsoft.Xna.Framework.Color color2 = Lighting.GetColor((int)position.X / 16, (int)((double)position.Y / 16.0));
                    color2 = projectile.GetAlpha(color2);
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                }
            }

            return true;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) 
        {
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 2; i++)
                {
                    float speedX = -projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
                    float speedY = -projectile.velocity.Y * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
                    Projectile.NewProjectile(projectile.position.X + speedX, projectile.position.Y + speedY, speedX, speedY, mod.ProjectileType("VineslingerProjectileFriendly"), (int)(projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
                    Main.PlaySound(SoundID.Grass, projectile.position);
                }
            }
        }
    }
}