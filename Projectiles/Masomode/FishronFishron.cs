using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FishronFishron : MutantBoss.MutantFishron
    {
        bool firstTick = false;
        public override string Texture => "FargowiltasSouls/NPCs/Vanilla/NPC_370";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.scale *= 0.75f;
        }

        public override bool PreAI()
        {
            if (!firstTick)
            {
                projectile.timeLeft = 150 + Main.rand.Next(10); //make them all die at slightly different times so no big audio pop on death
                projectile.netUpdate = true; //sync timeleft on first tick
                firstTick = true;
            }
            if (projectile.localAI[0] > 85) //dust during dash
            {
                int num22 = 7;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(projectile.velocity) * new Vector2((projectile.width + 50) / 2f, projectile.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + projectile.Center;
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - 1.570796f).ToRotationVector2() * Main.rand.Next(3, 8);
                    Vector2 vector2_3 = vector2_2;
                    int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= projectile.velocity;
                }
            }
            return true;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(mod.BuffType("Defenseless"), 600);
            //target.AddBuff(BuffID.WitheredWeapon, 600);
            target.AddBuff(mod.BuffType("OceanicMaul"), 1800);
            target.GetModPlayer<FargoPlayer>().MaxLifeReduction += EModeGlobalNPC.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
        }

        public override void Kill(int timeLeft)
        {
            for (int num249 = 0; num249 < 150; num249++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 5, 2 * projectile.direction, -2f);
            }

            int soundtype = (Main.rand.NextBool()) ? 17 : 30;
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.Center.X, (int)projectile.Center.Y, soundtype, 0.75f, 0.2f);

            Gore.NewGore(projectile.Center - Vector2.UnitX * 20f * projectile.direction, projectile.velocity, mod.GetGoreSlot("Gores/Fishron/Gore_576"), projectile.scale);
            Gore.NewGore(projectile.Center - Vector2.UnitY * 30f, projectile.velocity, mod.GetGoreSlot("Gores/Fishron/Gore_574"), projectile.scale);
            Gore.NewGore(projectile.Center, projectile.velocity, mod.GetGoreSlot("Gores/Fishron/Gore_575"), projectile.scale);
            Gore.NewGore(projectile.Center + Vector2.UnitX * 20f * projectile.direction, projectile.velocity, mod.GetGoreSlot("Gores/Fishron/Gore_573"), projectile.scale);
            Gore.NewGore(projectile.Center - Vector2.UnitY * 30f, projectile.velocity, mod.GetGoreSlot("Gores/Fishron/Gore_574"), projectile.scale);
            Gore.NewGore(projectile.Center, projectile.velocity, mod.GetGoreSlot("Gores/Fishron/Gore_575"), projectile.scale);
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

            SpriteEffects spriteEffects = projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i += 2)
            {
                Color color27 = Color.Lerp(color26, Color.Blue, 0.5f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                if (projectile.spriteDirection < 0)
                    num165 += (float)Math.PI;
                Main.spriteBatch.Draw(texture2D13, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, spriteEffects, 0f);
            }

            float drawRotation = projectile.rotation;
            if (projectile.spriteDirection < 0)
                drawRotation += (float)Math.PI;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), drawRotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            float ratio = (255 - projectile.alpha) / 255f;
            float blue = MathHelper.Lerp(ratio, 1f, 0.25f);
            if (blue > 1f)
                blue = 1f;
            return new Color((int)(lightColor.R * ratio), (int)(lightColor.G * ratio), (int)(lightColor.B * blue), (int)(lightColor.A * ratio));
        }
    }
}