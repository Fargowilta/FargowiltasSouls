using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class CosmosNebulaBlaze : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_634";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Meteor");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = -1;
            projectile.timeLeft = 270;
            projectile.hostile = true;

            projectile.extraUpdates = 2;
            projectile.scale = 1.5f;
            cooldownSlot = 1;
        }

        public override void AI() //vanilla code echprimebegone
        {
            if (++projectile.localAI[1] < 45 * projectile.MaxUpdates
                && EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.championBoss, ModContent.NPCType<NPCs.Champions.CosmosChampion>())
                && Main.npc[EModeGlobalNPC.championBoss].HasValidTarget) //home
            {
                float rotation = projectile.velocity.ToRotation();
                Vector2 vel = Main.player[Main.npc[EModeGlobalNPC.championBoss].target].Center - projectile.Center;
                float targetAngle = vel.ToRotation();
                projectile.velocity = new Vector2(projectile.velocity.Length(), 0f).RotatedBy(rotation.AngleLerp(targetAngle, projectile.ai[0]));
            }

            float num1 = 5f;
            float num2 = 250f;
            float num3 = 6f;
            Vector2 vector2_1 = new Vector2(8f, 10f);
            float num4 = 1.2f;
            Vector3 rgb = new Vector3(0.7f, 0.1f, 0.5f);
            int num5 = 4 * projectile.MaxUpdates;
            int Type1 = Utils.SelectRandom<int>(Main.rand, new int[5] { 242, 73, 72, 71, (int)byte.MaxValue });
            int Type2 = (int)byte.MaxValue;
            if ((double)projectile.ai[1] == 0.0)
            {
                projectile.ai[1] = 1f;
                projectile.localAI[0] = (float)-Main.rand.Next(48);
                Main.PlaySound(SoundID.Item34, projectile.position);
            }
            else if ((double)projectile.ai[1] == 1.0 && projectile.owner == Main.myPlayer)
            {
                
            }
            else if ((double)projectile.ai[1] > (double)num1)
            {
                
            }
            if ((double)projectile.ai[1] >= 1.0 && (double)projectile.ai[1] < (double)num1)
            {
                ++projectile.ai[1];
                if ((double)projectile.ai[1] == (double)num1)
                    projectile.ai[1] = 1f;
            }
            projectile.alpha = projectile.alpha - 40;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            projectile.spriteDirection = projectile.direction;
            projectile.frameCounter = projectile.frameCounter + 1;
            if (projectile.frameCounter >= num5)
            {
                projectile.frame = projectile.frame + 1;
                projectile.frameCounter = 0;
                if (projectile.frame >= 4)
                    projectile.frame = 0;
            }
            Lighting.AddLight(projectile.Center, rgb);
            projectile.rotation = projectile.velocity.ToRotation();
            ++projectile.localAI[0];
            if ((double)projectile.localAI[0] == 48.0)
                projectile.localAI[0] = 0.0f;
            else if (projectile.alpha == 0)
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    Vector2 vector2_2 = Vector2.UnitX * -30f;
                    Vector2 vector2_3 = -Vector2.UnitY.RotatedBy((double)projectile.localAI[0] * 0.130899697542191 + (double)index1 * 3.14159274101257, new Vector2()) * vector2_1 - projectile.rotation.ToRotationVector2() * 10f;
                    int index2 = Dust.NewDust(projectile.Center, 0, 0, Type2, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = num4;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + vector2_3 + projectile.velocity * 2f;
                    Main.dust[index2].velocity = Vector2.Normalize(projectile.Center + projectile.velocity * 2f * 8f - Main.dust[index2].position) * 2f + projectile.velocity * 2f;
                }
            }
            if (Main.rand.Next(12) == 0)
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.196349546313286).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 100, new Color(), 1f);
                    Main.dust[index2].velocity *= 0.1f;
                    Main.dust[index2].position = projectile.Center + vector2_2 * (float)projectile.width / 2f + projectile.velocity * 2f;
                    Main.dust[index2].fadeIn = 0.9f;
                }
            }
            if (Main.rand.Next(64) == 0)
            {
                for (int index1 = 0; index1 < 1; ++index1)
                {
                    Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.392699092626572).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, 0.0f, 0.0f, 155, new Color(), 0.8f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].position = projectile.Center + vector2_2 * (float)projectile.width / 2f;
                    if (Main.rand.Next(2) == 0)
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.785398185253143).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, Type1, 0.0f, 0.0f, 0, new Color(), 1.2f);
                    Main.dust[index2].velocity *= 0.3f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = projectile.Center + vector2_2 * (float)projectile.width / 2f;
                    if (Main.rand.Next(2) == 0)
                        Main.dust[index2].fadeIn = 1.4f;
                }
            }
            if (Main.rand.Next(12) == 0 && projectile.type == 634)
            {
                Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.196349546313286).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, Type2, 0.0f, 0.0f, 100, new Color(), 1f);
                Main.dust[index].velocity *= 0.3f;
                Main.dust[index].position = projectile.Center + vector2_2 * (float)projectile.width / 2f;
                Main.dust[index].fadeIn = 0.9f;
                Main.dust[index].noGravity = true;
            }
            if (Main.rand.Next(3) == 0 && projectile.type == 635)
            {
                Vector2 vector2_2 = -Vector2.UnitX.RotatedByRandom(0.196349546313286).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2());
                int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, Type2, 0.0f, 0.0f, 100, new Color(), 1f);
                Main.dust[index].velocity *= 0.3f;
                Main.dust[index].position = projectile.Center + vector2_2 * (float)projectile.width / 2f;
                Main.dust[index].fadeIn = 1.2f;
                Main.dust[index].scale = 1.5f;
                Main.dust[index].noGravity = true;
            }
        }

        public override void Kill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            int num1 = Utils.SelectRandom<int>(Main.rand, new int[5] { 242, 73, 72, 71, (int)byte.MaxValue });
            int Type1 = (int)byte.MaxValue;
            int Type2 = (int)byte.MaxValue;
            int num2 = 50;
            float Scale1 = 1.7f;
            float Scale2 = 0.8f;
            float Scale3 = 2f;
            Vector2 vector2 = (projectile.rotation - 1.570796f).ToRotationVector2() * projectile.velocity.Length() * (float)projectile.MaxUpdates;
            if (projectile.type == 635)
            {
                Type1 = 88;
                Type2 = 88;
                num1 = Utils.SelectRandom<int>(Main.rand, new int[3]
                {
            242,
            59,
            88
                });
                Scale1 = 3.7f;
                Scale2 = 1.5f;
                Scale3 = 2.2f;
                vector2 *= 0.5f;
            }
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = num2;
            projectile.Center = projectile.position;
            for (int index1 = 0; index1 < 40; ++index1)
            {
                int Type3 = Utils.SelectRandom<int>(Main.rand, new int[5] { 242, 73, 72, 71, (int)byte.MaxValue });
                if (projectile.type == 635)
                    Type3 = Utils.SelectRandom<int>(Main.rand, new int[3]
                    {
              242,
              59,
              88
                    });
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Type3, 0.0f, 0.0f, 200, new Color(), Scale1);
                Main.dust[index2].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 3f;
                Dust dust2 = Main.dust[index2];
                dust2.velocity = dust2.velocity + vector2 * Main.rand.NextFloat();
                int index3 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Type1, 0.0f, 0.0f, 100, new Color(), Scale2);
                Main.dust[index3].position = projectile.Center + Vector2.UnitY.RotatedByRandom(3.14159274101257) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                Dust dust3 = Main.dust[index3];
                dust3.velocity = dust3.velocity * 2f;
                Main.dust[index3].noGravity = true;
                Main.dust[index3].fadeIn = 1f;
                Main.dust[index3].color = Color.Crimson * 0.5f;
                Dust dust4 = Main.dust[index3];
                dust4.velocity = dust4.velocity + vector2 * Main.rand.NextFloat();
            }
            for (int index1 = 0; index1 < 20; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, Type2, 0.0f, 0.0f, 0, new Color(), Scale3);
                Main.dust[index2].position = projectile.Center + Vector2.UnitX.RotatedByRandom(3.14159274101257).RotatedBy((double)projectile.velocity.ToRotation(), new Vector2()) * (float)projectile.width / 3f;
                Main.dust[index2].noGravity = true;
                Dust dust1 = Main.dust[index2];
                dust1.velocity = dust1.velocity * 0.5f;
                Dust dust2 = Main.dust[index2];
                dust2.velocity = dust2.velocity + vector2 * (float)(0.600000023841858 + 0.600000023841858 * (double)Main.rand.NextFloat());
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(ModContent.BuffType<Berserked>(), 300);
                target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = projectile.GetAlpha(color26);

            SpriteEffects effects = projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = Color.HotPink * projectile.Opacity;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, effects, 0f);
            return false;
        }
    }
}