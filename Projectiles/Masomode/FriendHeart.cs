using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FriendHeart : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Masomode/FakeHeart";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Friend Heart");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.timeLeft = 900;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.aiStyle = -1;

            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);

            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.ai[0] = -1;
            }

            if (projectile.ai[0] >= 0 && projectile.ai[0] < 200)
            {
                int ai0 = (int)projectile.ai[0];
                if (Main.npc[ai0].CanBeChasedBy())
                {
                    if (projectile.localAI[1] < 0.16f)
                    {
                        float rotation = projectile.velocity.ToRotation();
                        Vector2 vel = Main.npc[ai0].Center - projectile.Center;
                        float targetAngle = vel.ToRotation();
                        projectile.localAI[1] += 1f / 1500f;
                        projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, projectile.localAI[1]));
                    }
                    else //eventually just switch to direct aim
                    {
                        projectile.velocity = projectile.DirectionTo(Main.npc[ai0].Center) * projectile.velocity.Length();
                    }
                }
                else
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                if (--projectile.ai[1] < 0f)
                {
                    projectile.ai[1] = 6f;
                    float maxDistance = 1700f;
                    int possibleTarget = -1;
                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy())
                        {
                            float npcDistance = projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                possibleTarget = i;
                            }
                        }
                    }

                    projectile.ai[0] = possibleTarget;
                    projectile.netUpdate = true;
                }

                projectile.localAI[1] = 0;
            }

            projectile.rotation = projectile.velocity.ToRotation() - (float)Math.PI / 2;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Lovestruck, 600);

            if (projectile.owner == Main.myPlayer)
            {
                int healAmount = 2;
                Main.player[Main.myPlayer].HealEffect(healAmount);
                Main.player[Main.myPlayer].statLife += healAmount;

                if (Main.player[Main.myPlayer].statLife > Main.player[Main.myPlayer].statLifeMax2)
                    Main.player[Main.myPlayer].statLife = Main.player[Main.myPlayer].statLifeMax2;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; // ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; // ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}