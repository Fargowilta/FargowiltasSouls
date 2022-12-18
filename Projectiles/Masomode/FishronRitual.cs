using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FishronRitual : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Oceanic Ritual");
        }

        public override void SetDefaults()
        {
            Projectile.width = 320;
            Projectile.height = 320;
            //Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            if (FargoSoulsWorld.MasochistModeReal) //Fargowiltas.Instance.MasomodeEXLoaded)
                Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            NPC fishron = FargoSoulsUtil.NPCExists(Projectile.ai[1], NPCID.DukeFishron);
            if (fishron == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = fishron.Center;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                if (EModeGlobalNPC.fishBossEX != fishron.whoAmI)
                {
                    fishron.GetGlobalNPC<DukeFishron>().IsEX = true;
                    fishron.GivenName = "Duke Fishron EX";
                    fishron.defDamage = (int)(fishron.defDamage * 1.5);
                    fishron.defDefense *= 2;
                    fishron.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
                    fishron.buffImmune[ModContent.BuffType<LightningRod>()] = true;
                }
                Projectile.netUpdate = true;
            }

            if (Projectile.localAI[1] == 0f)
            {
                Projectile.alpha -= 17;
                if (fishron.ai[0] % 5 == 1f)
                    Projectile.localAI[1] = 1f;
            }
            else
            {
                Projectile.alpha += 9;
            }

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            if (Projectile.alpha > 255)
            {
                if (fishron.ai[0] < 4f && Main.netMode == NetmodeID.Server) //ensure synchronized max life(?)
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)FargowiltasSouls.PacketID.SyncFishronEXLife);
                    netMessage.Write((int)Projectile.ai[1]);
                    netMessage.Write((int)Projectile.ai[0] * 25);
                    netMessage.Send();
                }
                Projectile.Kill();
                return;
            }
            Projectile.scale = 1f - Projectile.alpha / 255f;
            Projectile.rotation += (float)Math.PI / 70f;

            if (Projectile.alpha == 0)
            {
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    float num = Main.rand.Next(2, 4);
                    float scale = Projectile.scale * 0.6f;
                    if (index1 == 1)
                    {
                        scale *= 0.42f;
                        num *= -0.75f;
                    }
                    Vector2 vector21 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                    vector21.Normalize();
                    int index21 = Dust.NewDust(Projectile.Center, 0, 0, 135, 0f, 0f, 100, new Color(), 2f);
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
            if (fishron.ai[0] < 4f && Projectile.timeLeft <= 240 && Projectile.timeLeft >= 180)// && Projectile.timeLeft % 6 == 0)
            {
                fishron.GetGlobalNPC<FargoSoulsGlobalNPC>().MutantNibble = false;
                fishron.GetGlobalNPC<FargoSoulsGlobalNPC>().LifePrevious = int.MaxValue;
                while (fishron.buffType[0] != 0)
                    fishron.DelBuff(0);

                fishron.lifeMax = (int)Projectile.ai[0] * 5000; //10;
                if (fishron.lifeMax <= 0)
                    fishron.lifeMax = int.MaxValue;
                int heal = /*9*/ /*49*/ /*499999*/ (int)(fishron.lifeMax / 30 /*10*/ * Main.rand.NextFloat(1f, 1.1f));
                fishron.life += heal;
                if (fishron.life > fishron.lifeMax)
                    fishron.life = fishron.lifeMax;
                CombatText.NewText(fishron.Hitbox, CombatText.HealLife, heal);
                fishron.netUpdate = true;
            }

            int num1 = (300 - Projectile.timeLeft) / 60;
            float num2 = Projectile.scale * 0.4f;
            float num3 = Main.rand.Next(1, 3);
            Vector2 vector2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
            vector2.Normalize();
            int index2 = Dust.NewDust(Projectile.Center, 0, 0, 135, 0f, 0f, 100, new Color(), 2f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].noLight = true;
            Main.dust[index2].velocity = vector2 * num3;
            if (Main.rand.NextBool())
            {
                Main.dust[index2].velocity *= 2f;
                Main.dust[index2].scale += 0.5f;
            }
            Main.dust[index2].fadeIn = 2f;

            Lighting.AddLight(Projectile.Center, 0.4f, 0.9f, 1.1f);
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}