using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class CrystalLeafShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_227";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Leaf");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.aiStyle = -1;//ProjAIStyleID.CrystalLeafShot;
            AIType = ProjectileID.CrystalLeafShot;
            Projectile.penetrate = -1;
        }
        public void VanillaAIStyleCrystalLeafShot()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 3.14f;

            float num347 = 1f - (float)Projectile.timeLeft / 180f;
            float num348 = ((num347 * -6f * 0.85f + 0.33f) % 1f + 1f) % 1f;

            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Color color = recolor ? Color.Blue : Color.LimeGreen;

            Color value3 = recolor ? color : Main.hslToRgb(num348, 1f, 0.5f);
            value3 = Color.Lerp(value3, recolor ? Color.DarkSlateGray : Color.Red, Utils.Remap(num348, 0.33f, 0.7f, 0f, 1f));
            value3 = Color.Lerp(value3, Color.Lerp(color, Color.Gold, 0.3f), (float)(int)value3.R / 255f * 1f);
            if (Projectile.frameCounter++ >= 1)
            {
                Projectile.frameCounter = 0;
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
                {
                    PositionInWorld = Projectile.Center,
                    MovementVector = Projectile.velocity,
                    UniqueInfoPiece = (byte)(Main.rgbToHsl(value3).X * 255f)
                });
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.2f, 0.1f) * 1.5f);
            if (Main.rand.NextBool(5))
            {
                Dust dust12 = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.WhiteTorch);
                dust12.noGravity = true;
                Dust dust2 = dust12;
                dust2.velocity *= 0.1f;
                dust12.scale = 1.5f;
                dust2 = dust12;
                dust2.velocity += Projectile.velocity * Main.rand.NextFloat();
                dust12.color = value3;
                dust12.color.A /= 4;
                dust12.alpha = 100;
                dust12.noLight = true;
            }
        }

        public override void AI()
        {
            if (!Collision.SolidCollision(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height))
            {
                bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
                if (recolor)
                    Lighting.AddLight(Projectile.Center, 25f / 255, 47f / 255, 64f / 255);
                else
                    Lighting.AddLight(Projectile.Center + Projectile.velocity, 0.1f, 0.4f, 0.2f);

            }

            if (Projectile.timeLeft < 900 - 120)
                Projectile.tileCollide = true;

            const int netWindow = 10; // extra leniency time to let variables net sync
            const float redirectTime = 60f + netWindow;
            if (Projectile.ai[1] > 0) // redirect
            {
                if (Projectile.ai[1] > netWindow)
                {

                    Player player = Main.player[(int)Projectile.ai[2]];
                    if (player.Alive())
                    {
                        Vector2 LV = Projectile.velocity;
                        Vector2 PV = Projectile.SafeDirectionTo(player.Center);
                        float anglediff = FargoSoulsUtil.RotationDifference(LV, PV);
                        //change rotation towards target
                        Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), MathHelper.Pi / redirectTime));
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        /*
                        float angledif = FargoSoulsUtil.RotationDifference(Projectile.rotation.ToRotationVector2(), Projectile.SafeDirectionTo(player.Center));
                        float amt = MathHelper.Min(Math.Abs(angledif), MathHelper.Pi / redirectTime);
                        Projectile.rotation += amt * Math.Sign(angledif);
                        Projectile.velocity = Projectile.rotation.ToRotationVector2() * Projectile.velocity
                        */
                    }

                }
                Projectile.position -= Projectile.velocity * 0.9f;
                Projectile.ai[1]++;
                if (Projectile.ai[1] > redirectTime)
                {
                    Projectile.ai[1] = 0;
                    Projectile.netUpdate = true;
                }
            }
            VanillaAIStyleCrystalLeafShot();
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.hurtCooldowns[1] == 0)
            {
                target.AddBuff(ModContent.BuffType<IvyVenomBuff>(), 240);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Texture2D texture2D13 = recolor ? ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/Masomode/CrystalLeafShot").Value : Terraria.GameContent.TextureAssets.Projectile[Type].Value;

            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            float scale = Projectile.scale * 1.5f;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, spriteEffects, 0);

            color26.A = 150;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, scale, spriteEffects, 0);
            return false;
        }
    }
}