using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantTrueEyeR : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_650";

        private float localAI0;
        private float localAI1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Eye of Mutant");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 42;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            CooldownSlot = 1;
            Projectile.penetrate = -1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            Projectile.localAI[0]++;
            switch ((int)Projectile.ai[1])
            {
                case 0: //true eye movement code
                    Vector2 newVel = target.Center - Projectile.Center + new Vector2(300f * Projectile.localAI[1], -300f);
                    if (newVel != Vector2.Zero)
                    {
                        newVel.Normalize();
                        newVel *= 24f;
                        Projectile.velocity.X = (Projectile.velocity.X * 29 + newVel.X) / 30;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 29 + newVel.Y) / 30;
                    }
                    if (Projectile.Distance(target.Center) < 150f)
                    {
                        if (Projectile.Center.X < target.Center.X)
                            Projectile.velocity.X -= 0.25f;
                        else
                            Projectile.velocity.X += 0.25f;

                        if (Projectile.Center.Y < target.Center.Y)
                            Projectile.velocity.Y -= 0.25f;
                        else
                            Projectile.velocity.Y += 0.25f;
                    }

                    if (Projectile.localAI[0] > 90f)
                    {
                        Projectile.localAI[0] = 0f;
                        Projectile.ai[1]++;
                        Projectile.netUpdate = true;
                    }

                    if (Projectile.rotation > 3.14159274101257)
                        Projectile.rotation = Projectile.rotation - 6.283185f;
                    Projectile.rotation = Projectile.rotation <= -0.005 || Projectile.rotation >= 0.005 ? Projectile.rotation * 0.96f : 0.0f;
                    break;

                case 1: //slow down
                    if (Projectile.localAI[0] == 1f) //spawn orb ring
                    {
                        const int max = 6;
                        const float distance = 100f;
                        const float rotation = 2f * (float)Math.PI / max;
                        for (int i = 0; i < max; i++)
                        {
                            Vector2 spawnPos = Projectile.Center - Vector2.UnitY * 6f + new Vector2(distance, 0f).RotatedBy(rotation * i);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, Vector2.Zero, ModContent.ProjectileType<MutantTrueEyeSphere>(),
                                    Projectile.damage, 0f, Projectile.owner, Projectile.identity, i);
                        }
                    }
                    Projectile.velocity *= 0.95f;
                    if (Projectile.localAI[0] > 60f)
                    {
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localAI[0] = 0f;
                        Projectile.ai[1]++;
                        Projectile.netUpdate = true;
                    }

                    if (Projectile.rotation > 3.14159274101257)
                        Projectile.rotation = Projectile.rotation - 6.283185f;
                    Projectile.rotation = Projectile.rotation <= -0.005 || Projectile.rotation >= 0.005 ? Projectile.rotation * 0.96f : 0.0f;
                    break;

                case 2: //ramming
                    if (Projectile.localAI[0] == 1f)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie102, Projectile.Center);
                        Projectile.velocity = target.Center - Projectile.Center;
                        if (Projectile.velocity != Vector2.Zero)
                        {
                            Projectile.velocity.Normalize();
                            Projectile.velocity *= 24f;
                        }
                        Projectile.netUpdate = true;
                    }
                    else if (Projectile.localAI[0] > 10f)
                    {
                        Projectile.localAI[0] = 0f;
                        Projectile.ai[1]++;
                    }

                    float num3 = Projectile.velocity.ToRotation() + 1.570796f;
                    if (Math.Abs(Projectile.rotation - num3) >= 3.14159274101257)
                        Projectile.rotation = num3 >= Projectile.rotation ? Projectile.rotation + 6.283185f : Projectile.rotation - 6.283185f;
                    float num4 = 12f;
                    Projectile.rotation = (Projectile.rotation * (num4 - 1f) + num3) / num4;
                    break;

                default:
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 135, 0f, 0f, 0, default(Color), 3f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 8f;
                    }
                    SoundEngine.PlaySound(SoundID.Zombie102, Projectile.Center);
                    Projectile.Kill();
                    break;
            }

            if (Projectile.rotation > 3.14159274101257)
                Projectile.rotation = Projectile.rotation - 6.283185f;
            Projectile.rotation = Projectile.rotation <= -0.005 || Projectile.rotation >= 0.005 ? Projectile.rotation * 0.96f : 0.0f;
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            if (Projectile.ai[1] != 2f) //custom pupil when attacking
                UpdatePupil();
        }

        private void UpdatePupil()
        {
            float f1 = (float)(localAI0 % 6.28318548202515 - 3.14159274101257);
            float num13 = (float)Math.IEEERemainder(localAI1, 1.0);
            if (num13 < 0.0)
                ++num13;
            float num14 = (float)Math.Floor(localAI1);
            float max = 0.999f;
            int num15 = 0;
            float amount = 0.1f;
            float f2;
            float num18;
            float num19;
            f2 = Projectile.AngleTo(Main.player[(int)Projectile.ai[0]].Center);
            num15 = 2;
            num18 = MathHelper.Clamp(num13 + 0.05f, 0.0f, max);
            num19 = num14 + Math.Sign(-12f - num14);
            Vector2 rotationVector2 = f2.ToRotationVector2();
            localAI0 = (float)(Vector2.Lerp(f1.ToRotationVector2(), rotationVector2, amount).ToRotation() + num15 * 6.28318548202515 + 3.14159274101257);
            localAI1 = num19 + num18;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 360);
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = Projectile.GetAlpha(lightColor);

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 1f;
            scale *= Projectile.scale;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27.A = 0;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Texture2D pupil = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Minions/TrueEyePupil", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 pupilOffset = new Vector2(localAI1 / 2f, 0f).RotatedBy(localAI0);
            pupilOffset += new Vector2(0f, -6f).RotatedBy(Projectile.rotation);
            Vector2 pupilOrigin = pupil.Size() / 2f;
            //Main.EntitySpriteDraw(pupil, pupilOffset + Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(pupil.Bounds), color26, 0f, pupilOrigin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}