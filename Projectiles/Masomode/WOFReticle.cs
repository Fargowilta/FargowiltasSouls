using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class WOFReticle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wall of Flesh Reticle");
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 300;
            //CooldownSlot = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        int additive = 130;

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;

            if (++Projectile.ai[0] < 130)
            {
                Projectile.alpha -= 2;
                if (Projectile.alpha < 0) //fade in
                    Projectile.alpha = 0;

                int modifier = Math.Min(110, (int)Projectile.ai[0]);
                Projectile.scale = 4f - 3f / 110 * modifier; //start big, shrink down

                /*Projectile.Center = Main.npc[ai0].Center;
                Projectile.velocity = Main.player[Main.npc[ai0].target].Center - Projectile.Center;
                Projectile.velocity = Projectile.velocity / 60 * modifier; //move from npc to player*/
                Projectile.rotation = (float)Math.PI * 2f / 55 * modifier * Projectile.localAI[0];
            }
            else //if (Projectile.ai[0] < 145)
            {
                additive -= 7;
                if (additive < 0)
                    additive = 0;

                Projectile.alpha += 15;
                if (Projectile.alpha > 255) //fade out
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }

                Projectile.scale = 4f - 3f * Projectile.Opacity; //scale back up

                //if (Projectile.ai[0] == 130 && Main.netMode != NetmodeID.MultiplayerClient) Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -13);

                if (Projectile.ai[0] % 6 == 0 && Projectile.localAI[1]++ < 3)
                {
                    //Vector2 spawnPos = Projectile.Center;
                    //spawnPos.X += Main.rand.NextFloat(-250, 250);
                    //spawnPos.Y += Main.rand.NextFloat(700, 800) * Projectile.localAI[0];
                    float angle = MathHelper.ToRadians(15);
                    Vector2 spawnPos;
                    spawnPos.Y = (Projectile.localAI[0] > 0 ? Main.maxTilesY * 16 - 16 : Main.maxTilesY * 16 - 200 * 16) - Projectile.Center.Y;
                    spawnPos.X = spawnPos.Y * (float)Math.Tan(Main.rand.NextFloat(-angle, angle));
                    spawnPos += Projectile.Center;

                    Vector2 vel = Main.rand.NextFloat(0.8f, 1.2f) * (Projectile.Center - spawnPos) / 90;
                    if (vel.Length() < 10f)
                        vel = Vector2.Normalize(vel) * Main.rand.NextFloat(10f, 15f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, vel, ModContent.ProjectileType<WOFChain>(), Projectile.damage, 0f, Main.myPlayer);
                    
                    SoundEngine.PlaySound(SoundID.NPCDeath13 with { Volume = 0.5f }, Projectile.Center);

                    Projectile.localAI[0] *= -1;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, additive) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}