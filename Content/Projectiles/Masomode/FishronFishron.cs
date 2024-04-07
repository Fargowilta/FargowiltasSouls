using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class FishronFishron : MutantFishron
    {
        bool firstTick = false;
        public override string Texture => FargoSoulsUtil.VanillaTextureNPC(NPCID.DukeFishron);

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale *= 0.75f;
            CooldownSlot = -1;
        }

        public override bool CanHitPlayer(Player target)
        {
            return true;
        }

        public override bool PreAI()
        {
            if (!firstTick)
            {
                Projectile.timeLeft = 150 + Main.rand.Next(10); //make them all die at slightly different times so no big audio pop on death
                Projectile.netUpdate = true; //sync timeleft on first tick
                firstTick = true;
            }
            if (Projectile.localAI[0] > 85) //dust during dash
            {
                int num22 = 7;
                for (int index1 = 0; index1 < num22; ++index1)
                {
                    Vector2 vector2_1 = (Vector2.Normalize(Projectile.velocity) * new Vector2((Projectile.width + 50) / 2f, Projectile.height) * 0.75f).RotatedBy((index1 - (num22 / 2 - 1)) * Math.PI / num22, new Vector2()) + Projectile.Center;
                    Vector2 vector2_2 = ((float)(Main.rand.NextDouble() * 3.14159274101257) - (float)Math.PI / 2).ToRotationVector2() * Main.rand.Next(3, 8);
                    Vector2 vector2_3 = vector2_2;
                    int index2 = Dust.NewDust(vector2_1 + vector2_3, 0, 0, DustID.DungeonWater, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].noLight = true;
                    Main.dust[index2].velocity /= 4f;
                    Main.dust[index2].velocity -= Projectile.velocity;
                }
            }
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 600);
            //target.AddBuff(BuffID.WitheredWeapon, 600);
            target.AddBuff(ModContent.BuffType<OceanicMaulBuff>(), 20 * 60);
            target.FargoSouls().MaxLifeReduction += FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.fishBossEX, NPCID.DukeFishron) ? 100 : 25;
        }

        public override void OnKill(int timeLeft)
        {
            for (int num249 = 0; num249 < 150; num249++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 2 * Projectile.direction, -2f);
            }

            SoundEngine.PlaySound((Main.rand.NextBool() ? SoundID.NPCDeath17 : SoundID.NPCDeath30) with { Volume = 0.75f, Pitch = 0.2f }, Projectile.Center);

            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitX * 20f * Projectile.direction, Projectile.velocity, 576, Projectile.scale);
            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitY * 30f, Projectile.velocity, 574, Projectile.scale);
            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, 575, Projectile.scale);
            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitX * 20f * Projectile.direction, Projectile.velocity, 573, Projectile.scale);
            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitY * 30f, Projectile.velocity, 574, Projectile.scale);
            if (!Main.dedServ)
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, 575, Projectile.scale);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (Projectile.localAI[0] > 85)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 2)
                {
                    Color color27 = Color.Lerp(color26, Color.Blue, 0.5f);
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    if (Projectile.spriteDirection < 0)
                        num165 += (float)Math.PI;
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, spriteEffects, 0);
                }
            }

            float drawRotation = Projectile.rotation;
            if (Projectile.spriteDirection < 0)
                drawRotation += (float)Math.PI;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), drawRotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            float ratio = (255 - Projectile.alpha) / 255f;
            float blue = MathHelper.Lerp(ratio, 1f, 0.25f);
            if (blue > 1f)
                blue = 1f;
            return new Color((int)(lightColor.R * ratio), (int)(lightColor.G * ratio), (int)(lightColor.B * blue), (int)(lightColor.A * ratio));
        }
    }
}