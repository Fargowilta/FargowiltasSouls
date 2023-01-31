using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using FargowiltasSouls.NPCs.Challengers;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class LifeBlaster : ModProjectile
    {
        float glowIntensity = 1f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Blaster");
            Main.projFrames[Projectile.type] = 11;
        }
        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 56;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile.rotation = Projectile.ai[0] + MathHelper.PiOver2;

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] >= 5) //to line up to fully charge at 60 frames (when it fires)
            {
                if (++Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    if (++Projectile.frame >= Main.projFrames[Projectile.type])
                        Projectile.frame = Main.projFrames[Projectile.type] - 1;
                }
            }
            if (Projectile.localAI[0] < 60)
                glowIntensity += 2/60f;

            if ((Projectile.localAI[0] >= 60 && Projectile.ai[1] <= 1) || (Projectile.localAI[0] >= 120 && Projectile.ai[1] == 2))
            {
                glowIntensity = 4f;
                //Projectile.frame = 1;
                if ((Projectile.localAI[0] == 60 && Projectile.ai[1] <= 1) || (Projectile.localAI[0] == 120 && Projectile.ai[1] == 2))
                {
                    SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item66, Projectile.Center);

                    //fire beam
                    Vector2 rot = Projectile.ai[0].ToRotationVector2();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //visual    center - height/2 + 32 + beamheight/2
                        //Vector2 visualpos = Projectile.Center + (rot * (1005));
                        //Projectile.NewProjectile(Projectile.GetSource_FromThis(), visualpos, rot * 0.000000001f, ModContent.ProjectileType<LifeBlasterLaser>(), 0, 0, Main.myPlayer);

                        //real deathray lol
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, rot,
                            ModContent.ProjectileType<LifeChalBlasterDeathray>(), Projectile.damage, 0f, Main.myPlayer);
                    }
                }
                //damage projectiles
                /*if ((Projectile.localAI[0] >= 60 && Projectile.localAI[0] % 11 == 10 && Projectile.ai[1] <= 1) || (Projectile.localAI[0] >= 120 && Projectile.localAI[0] % 11 == 10 && Projectile.ai[1] == 2))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 rot = Projectile.ai[0].ToRotationVector2();
                        for (int i = 0; i < 2; i++)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + (rot * (i * 450)), rot * 35f, ModContent.ProjectileType<LifeBlasterLaserHitbox>(), damage, 3f, Main.myPlayer, Projectile.localAI[0]);
                    }
                }*/
            }
            //else
            //Projectile.frame = 0;

            int endTime = Projectile.ai[1] == 2 ? 155 : 95;
            if (Projectile.localAI[0] >= endTime)
                Projectile.Kill();
            if (Projectile.localAI[0] > endTime - 15)
            {
                Projectile.alpha += 15;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }
            else
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255 - Projectile.alpha) * Projectile.Opacity;
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.Smite>(), 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTexture = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Challengers/LifeBlasterGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * glowIntensity;
                Color glowColor = Color.White;
                glowColor.A = 0;

                Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + afterimageOffset, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(glowColor), Projectile.rotation, origin2, Projectile.scale, effects, 0f);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
