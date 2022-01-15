using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CosmosReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Cosmic Reticle");
        }

        public override void SetDefaults()
        {
            projectile.width = 110;
            projectile.height = 110;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.hostile = true;
            
            //cooldownSlot = 1;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            NPC npc = FargoSoulsUtil.NPCExists(projectile.ai[0], ModContent.NPCType<NPCs.Champions.CosmosChampion>());
            if (npc == null || npc.ai[0] != 11)
            {
                projectile.Kill();
                return;
            }

            Player player = Main.player[npc.target];

            projectile.velocity = Vector2.Zero;

            if (++projectile.ai[1] > 45)
            {
                if (projectile.ai[1] % 5 == 0)
                {
                    Main.PlaySound(SoundID.Item89, projectile.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient) //rain meteors
                    {
                        Vector2 spawnPos = projectile.Center;
                        spawnPos.X += Main.rand.Next(-200, 201);
                        spawnPos.Y -= 700;
                        Vector2 vel = Main.rand.NextFloat(10, 15f) * Vector2.Normalize(projectile.Center - spawnPos);
                        Projectile.NewProjectile(spawnPos, vel, ModContent.ProjectileType<CosmosMeteor>(), npc.damage / 4, 0f, Main.myPlayer, 0f, Main.rand.NextFloat(1f, 1.5f));
                    }
                }

                if (projectile.ai[1] > 90)
                {
                    projectile.ai[1] = 0;
                    projectile.netUpdate = true;
                }

                projectile.rotation = 0;
                projectile.alpha = 0;
                projectile.scale = 1;
            }
            else
            {
                projectile.Center = player.Center;
                projectile.position.X += player.velocity.X * 30;

                if (projectile.ai[1] == 45)
                {
                    projectile.netUpdate = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -5);
                }

                float spindown = 1f - projectile.ai[1] / 45f;
                projectile.rotation = MathHelper.TwoPi * 1.5f * spindown;
                projectile.alpha = (int)(255 * spindown);
                projectile.scale = 1 + 2 * spindown;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * (1f - projectile.alpha / 255f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}