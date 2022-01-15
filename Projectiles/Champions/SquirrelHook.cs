using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class SquirrelHook : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_13";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Squirrel Hook");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = -1;
            projectile.timeLeft = 240;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.GetGlobalProjectile<FargoGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampion>());
            if (npc == null)
            {
                projectile.Kill();
                return;
            }
            Player player = Main.player[npc.target];

            const float speed = 24f;

            if (!npc.HasValidTarget)
            {
                projectile.velocity = projectile.DirectionTo(npc.Center) * speed;
                projectile.ai[1] = 0;
                return;
            }

            if (projectile.ai[1] == 0)
            {
                projectile.velocity = projectile.DirectionTo(player.Center) * speed + player.velocity / 4f;
                projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2;

                if (projectile.Distance(player.Center) < speed) //in range
                {
                    projectile.ai[1] = 1;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                projectile.rotation = npc.DirectionTo(player.Center).ToRotation() + (float)Math.PI / 2;

                if (projectile.Distance(player.Center) > 64) //out of range somehow
                {
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    float dragSpeed = projectile.Distance(npc.Center) / 50;
                    
                    player.position += projectile.DirectionTo(npc.Center) * dragSpeed;
                    projectile.Center = player.Center - player.velocity;

                    if (projectile.timeLeft == 1)
                        player.velocity /= 3;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.Champions.TimberChampion>());
            if (npc != null)
            {
                Texture2D texture = Main.chainTexture;
                Vector2 position = projectile.Center;
                Vector2 mountedCenter = Main.npc[(int)projectile.ai[0]].Center;
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = projectile.GetAlpha(color2);
                        Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                    }
            }

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}