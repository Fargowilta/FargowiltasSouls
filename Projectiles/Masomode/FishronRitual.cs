using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FishronRitual : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Oceanic Ritual");
        }

        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 320;
            //projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            if (Fargowiltas.Instance.MasomodeEXLoaded)
                projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            NPC fishron = FargoSoulsUtil.NPCExists(projectile.ai[1], NPCID.DukeFishron);
            if (fishron == null)
            {
                projectile.Kill();
                return;
            }

            projectile.Center = fishron.Center;

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                if (EModeGlobalNPC.fishBossEX != fishron.whoAmI)
                {
                    fishron.GetGlobalNPC<EModeGlobalNPC>().masoBool[3] = true;
                    fishron.GivenName = "Duke Fishron EX";
                    fishron.defDamage = (int)(fishron.defDamage * 1.5);
                    fishron.defDefense *= 2;
                    fishron.buffImmune[mod.BuffType("FlamesoftheUniverse")] = true;
                    fishron.buffImmune[mod.BuffType("LightningRod")] = true;
                }
                projectile.netUpdate = true;
            }

            if (projectile.localAI[1] == 0f)
            {
                projectile.alpha -= 17;
                if (fishron.ai[0] % 5 == 1f)
                    projectile.localAI[1] = 1f;
            }
            else
            {
                projectile.alpha += 9;
            }

            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
            {
                if (fishron.ai[0] < 4f && Main.netMode == NetmodeID.Server) //ensure synchronized max life(?)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)78);
                    netMessage.Write((int)projectile.ai[1]);
                    netMessage.Write((int)projectile.ai[0] * 25);
                    netMessage.Send();
                }
                projectile.Kill();
                return;
            }
            projectile.scale = 1f - projectile.alpha / 255f;
            projectile.rotation += (float)Math.PI / 70f;

            if (projectile.alpha == 0)
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    float num = Main.rand.Next(2, 4);
                    float scale = projectile.scale * 0.6f;
                    if (index1 == 1)
                    {
                        scale *= 0.42f;
                        num *= -0.75f;
                    }
                    Vector2 vector21 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                    vector21.Normalize();
                    int index21 = Dust.NewDust(projectile.Center, 0, 0, 135, 0f, 0f, 100, new Color(), 2f);
                    Main.dust[index21].noGravity = true;
                    Main.dust[index21].noLight = true;
                    Main.dust[index21].position += vector21 * 204f * scale;
                    Main.dust[index21].velocity = vector21 * -num;
                    if (Main.rand.NextBool(8))
                    {
                        Main.dust[index21].velocity *= 2f;
                        Main.dust[index21].scale += 0.5f;
                    }
                }
            }

            //while fishron is first spawning, has made the noise, and every 6 ticks
            if (fishron.ai[0] < 4f && projectile.timeLeft <= 240 && projectile.timeLeft >= 180)// && projectile.timeLeft % 6 == 0)
            {
                fishron.GetGlobalNPC<FargoSoulsGlobalNPC>().MutantNibble = false;
                fishron.GetGlobalNPC<FargoSoulsGlobalNPC>().LifePrevious = int.MaxValue;
                while (fishron.buffType[0] != 0)
                    fishron.DelBuff(0);

                fishron.lifeMax = (int)projectile.ai[0] * 5000; //10;
                if (fishron.lifeMax <= 0)
                    fishron.lifeMax = int.MaxValue;
                int heal = /*9*/ /*49*/ /*499999*/ (int)(fishron.lifeMax / 30 /*10*/ * Main.rand.NextFloat(1f, 1.1f));
                fishron.life += heal;
                if (fishron.life > fishron.lifeMax)
                    fishron.life = fishron.lifeMax;
                CombatText.NewText(fishron.Hitbox, CombatText.HealLife, heal);
                fishron.netUpdate = true;
            }

            int num1 = (300 - projectile.timeLeft) / 60;
            float num2 = projectile.scale * 0.4f;
            float num3 = Main.rand.Next(1, 3);
            Vector2 vector2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
            vector2.Normalize();
            int index2 = Dust.NewDust(projectile.Center, 0, 0, 135, 0f, 0f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].noLight = true;
            Main.dust[index2].velocity = vector2 * num3;
            if (Main.rand.NextBool())
            {
                Main.dust[index2].velocity *= 2f;
                Main.dust[index2].scale += 0.5f;
            }
            Main.dust[index2].fadeIn = 2f;

            Lighting.AddLight(projectile.Center, 0.4f, 0.9f, 1.1f);
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}