using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class PrismaRegaliaStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_79";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Star");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[1] > 20)
            {
                Projectile.friendly = true;
                //homing
                Projectile.tileCollide = true;

                if (Projectile.ai[0] == -1) //no target atm
                {
                    if (Projectile.ai[1] % 6 == 0)
                    {
                        Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2000, true);
                        Projectile.netUpdate = true;
                        if (Projectile.ai[0] == -1)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                }
                else //currently have target
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0]];

                    if (npc.active && npc.CanBeChasedBy()) //target is still valid
                    {
                        Vector2 distance = npc.Center - Projectile.Center;
                        double angle = distance.ToRotation() - Projectile.velocity.ToRotation();
                        if (angle > Math.PI)
                            angle -= 2.0 * Math.PI;
                        if (angle < -Math.PI)
                            angle += 2.0 * Math.PI;

                        float modifier = Math.Min(Projectile.velocity.Length() / 100f, 1f);
                        Projectile.velocity = Projectile.velocity.RotatedBy(angle * modifier);
                    }
                    else //target lost, reset
                    {
                        Projectile.ai[0] = -1;
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * MathHelper.Lerp(Projectile.velocity.Length(), 24f, 0.02f);


            }
            Projectile.ai[1]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D star = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, star.Width, star.Height);
            float scale = Main.rand.NextFloat(0.9f, 1.1f);
            Vector2 origin = new(star.Width / 2 + scale, star.Height / 2 + scale);

            float of1 = 6;
            Vector2 offset1 = new(Main.rand.NextFloat(-of1, of1), Main.rand.NextFloat(-of1, of1));
            Main.spriteBatch.Draw(star, Projectile.Center + offset1 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rect), Color.White, 0, origin, scale, SpriteEffects.None, 0);

            float of2 = 3;
            Vector2 offset2 = new(Main.rand.NextFloat(-of2, of2), Main.rand.NextFloat(-of2, of2));
            Main.spriteBatch.Draw(star, Projectile.Center + offset2 - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rect), Color.Pink, 0, origin, scale, SpriteEffects.None, 0);
            DrawData starDraw = new(star, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rect), Color.Pink, 0, origin, scale, SpriteEffects.None, 0);
            GameShaders.Misc["LCWingShader"].UseColor(Color.Pink).UseSecondaryColor(Color.LightPink);
            GameShaders.Misc["LCWingShader"].Apply(new DrawData?());
            starDraw.Draw(Main.spriteBatch);

            /*
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            */
            return false;
        }
    }
}