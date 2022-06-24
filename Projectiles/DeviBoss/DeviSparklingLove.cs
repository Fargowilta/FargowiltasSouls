using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.DeviBoss
{
    public class DeviSparklingLove : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/SparklingLove";

        public int scaleCounter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Love");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.alpha = 250;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            //the important part
            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc != null)
            {
                if (Projectile.localAI[0] == 0)
                {
                    Projectile.localAI[0] = 1;
                    Projectile.localAI[1] = Projectile.DirectionFrom(npc.Center).ToRotation();
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -17);
                }

                //not important part
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 4;
                    if (Projectile.alpha < 0)
                        Projectile.alpha = 0;
                }

                //important again
                if (++Projectile.localAI[0] > 31)
                {
                    Projectile.localAI[0] = 1;
                    if (++scaleCounter < 3)
                    {
                        Projectile.position = Projectile.Center;

                        Projectile.width *= 2;
                        Projectile.height *= 2;
                        Projectile.scale *= 2;

                        Projectile.Center = Projectile.position;

                        MakeDust();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -16 + scaleCounter);

                        SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
                    }
                }

                Vector2 offset = new Vector2(Projectile.ai[1], 0).RotatedBy(npc.ai[3] + Projectile.localAI[1]);
                Projectile.Center = npc.Center + offset * Projectile.scale;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.timeLeft == 8)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -14);

                if (!Main.dedServ && Main.LocalPlayer.active)
                    Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 30;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float baseRotation = FargoSoulsWorld.EternityMode ? Main.rand.NextFloat(MathHelper.TwoPi) : 0;

                    int max = 8;
                    if (FargoSoulsWorld.EternityMode)
                        max = 12;
                    if (FargoSoulsWorld.MasochistModeReal)
                        max = 8; //lowered because maso doubles down on it

                    for (int i = 0; i < max; i++)
                    {
                        Vector2 target = 600 * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / max * i + baseRotation);
                        Vector2 speed = 2 * target / 90;
                        float acceleration = -speed.Length() / 90;
                        float rotation = speed.ToRotation();
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                            (int)(Projectile.damage * 0.75), 0f, Main.myPlayer, rotation + MathHelper.PiOver2, acceleration);

                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation + MathHelper.PiOver2);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation - MathHelper.PiOver2);
                    }

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 target = 300 * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / max * (i + 0.5f) + baseRotation);
                            Vector2 speed = 2 * target / 90;
                            float acceleration = -speed.Length() / 90;
                            float rotation = speed.ToRotation();
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<DeviEnergyHeart>(),
                                (int)(Projectile.damage * 0.75), 0f, Main.myPlayer, rotation + MathHelper.PiOver2, acceleration);

                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation);
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation + MathHelper.PiOver2);
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, speed, ModContent.ProjectileType<GlowLine>(), Projectile.damage, 0f, Main.myPlayer, 2, rotation - MathHelper.PiOver2);
                        }
                    }
                }
            }

            Projectile.direction = Projectile.spriteDirection = npc.direction;
            Projectile.rotation = npc.ai[3] + Projectile.localAI[1] + (float)Math.PI / 2 + (float)Math.PI / 4;
            if (Projectile.spriteDirection >= 0)
                Projectile.rotation -= (float)Math.PI / 2;
        }

        public override void Kill(int timeLeft)
        {
            MakeDust();
        }

        private void MakeDust()
        {
            Vector2 start = Projectile.width * Vector2.UnitX.RotatedBy(Projectile.rotation - (float)Math.PI / 4);
            if (Math.Abs(start.X) > Projectile.width / 2) //bound it so its always inside projectile's hitbox
                start.X = Projectile.width / 2 * Math.Sign(start.X);
            if (Math.Abs(start.Y) > Projectile.height / 2)
                start.Y = Projectile.height / 2 * Math.Sign(start.Y);
            int length = (int)start.Length();
            start = Vector2.Normalize(start);
            float scaleModifier = scaleCounter / 3f + 0.5f;
            for (int j = -length; j <= length; j += 80)
            {
                Vector2 dustPoint = Projectile.Center + start * j;
                dustPoint.X -= 23;
                dustPoint.Y -= 23;

                /*for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier * 2f);
                    Main.dust[dust].velocity *= 1.4f * scaleModifier;
                }*/

                for (int index1 = 0; index1 < 15; ++index1)
                {
                    int index2 = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier * 2.5f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 16f * scaleModifier;
                    int index3 = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier);
                    Main.dust[index3].velocity *= 8f * scaleModifier;
                    Main.dust[index3].noGravity = true;
                }

                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), Main.rand.NextFloat(1f, 2f) * scaleModifier);
                    Main.dust[d].velocity *= Main.rand.NextFloat(1f, 4f) * scaleModifier;
                }
            }
        }

        /*public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.velocity.X = target.Center.X < Main.npc[(int)Projectile.ai[0]].Center.X ? -15f : 15f;
            target.velocity.Y = -10f;
            target.AddBuff(ModContent.BuffType<Lovestruck>(), 240);
        }*/

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.DeviBoss.DeviBoss>());
            if (npc != null && npc.velocity != Vector2.Zero)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color26 * 0.5f;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Items/Weapons/FinalUpgrades/SparklingLove_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}