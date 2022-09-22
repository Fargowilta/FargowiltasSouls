using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.Volknet.Projectiles
{
    class NanoProbe : ModProjectile
    {
        public bool OldChannel = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nano Probe");
            //DisplayName.AddTranslation(GameCulture.Chinese, "纳米探针");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.7f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D tex2 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Patreon/Volknet/Projectiles/NanoProbeGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            
            Color color = Projectile.GetAlpha(lightColor);
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.25f)
            {
                Color color27 = Color.LimeGreen * Projectile.Opacity;
                if (i > 1f)
                    color27 *= 0.3f;
                color27.A = 0;
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                if (i > 1f)
                    color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(tex, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, color27, num165, tex.Size() / 2, Projectile.scale * Main.rand.NextFloat(1f, 1.6f), SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, color, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Color.White, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Main.player[Projectile.owner].active)
            {
                Player owner = Main.player[Projectile.owner];
                if (!owner.dead && owner.HeldItem.type == ModContent.ItemType<NanoCore>())
                {
                    Projectile.timeLeft = 2;



                    if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 0)         //blade phase
                    {
                        Vector2 TM = Main.MouseWorld - owner.Center;
                        if (TM == Vector2.Zero) TM = new Vector2(0, -1);
                        TM.Normalize();
                        if (Projectile.ai[0] < 5)
                        {
                            Vector2 TargetPos = owner.Center + TM * 30 * (Projectile.ai[0] + 1);
                            if (Projectile.ai[1] == 0)
                            {
                                Movement(TargetPos, 0.5f);
                                Projectile.rotation = Projectile.velocity.ToRotation();
                                if (Projectile.Distance(TargetPos) < 30)
                                {
                                    Projectile.ai[1] = 1;
                                }
                            }
                            else
                            {
                                Projectile.velocity = Vector2.Zero;
                                Projectile.Center = TargetPos;
                                Projectile.rotation = TM.ToRotation();
                            }

                        }
                        else
                        {
                            int t = Math.Sign(Projectile.ai[0] - 5.5);
                            Vector2 TargetPos = owner.Center + (TM.ToRotation() + MathHelper.Pi / 2 * t).ToRotationVector2() * 20 + TM * 30;
                            if (Projectile.ai[1] == 0)
                            {
                                Movement(TargetPos, 0.5f);
                                Projectile.rotation = Projectile.velocity.ToRotation();
                                if (Projectile.Distance(TargetPos) < 30)
                                {
                                    Projectile.ai[1] = 1;
                                }
                            }
                            else
                            {
                                Projectile.velocity = Vector2.Zero;
                                Projectile.Center = TargetPos;
                                Projectile.rotation = (TM.ToRotation() + MathHelper.Pi / 2 * t);
                            }

                        }
                    }
                    else if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 1)       //bow phase
                    {
                        Vector2 TM = Main.MouseWorld - owner.Center;
                        if (TM == Vector2.Zero) TM = new Vector2(0, -1);
                        TM.Normalize();
                        Vector2 TargetPos = Vector2.Zero;
                        float TargetRot = 0;
                        switch (Projectile.ai[0])
                        {
                            case 0:
                                TargetPos = owner.Center + TM * 40;
                                TargetRot = TM.ToRotation();
                                break;
                            case 1:
                                TargetPos = owner.Center + TM * 40 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 30;
                                TargetRot = TM.ToRotation() + MathHelper.Pi / 2;
                                break;
                            case 2:
                                TargetPos = owner.Center + TM * 40 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 30;
                                TargetRot = TM.ToRotation() - MathHelper.Pi / 2;
                                break;
                            case 3:
                                TargetPos = owner.Center + TM * 40 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 30 + (TM.ToRotation() + MathHelper.Pi / 4 * 3).ToRotationVector2() * 30;
                                TargetRot = TM.ToRotation() + MathHelper.Pi / 4 * 3;
                                break;
                            case 4:
                                TargetPos = owner.Center + TM * 40 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 30 + (TM.ToRotation() - MathHelper.Pi / 4 * 3).ToRotationVector2() * 30;
                                TargetRot = TM.ToRotation() - MathHelper.Pi / 4 * 3;
                                break;
                            case 5:
                                TargetPos = owner.Center + TM * 40 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 30 + (TM.ToRotation() + MathHelper.Pi / 4 * 3).ToRotationVector2() * 60;
                                TargetRot = TM.ToRotation() + MathHelper.Pi / 4 * 3;
                                break;
                            case 6:
                                TargetPos = owner.Center + TM * 40 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 30 + (TM.ToRotation() - MathHelper.Pi / 4 * 3).ToRotationVector2() * 60;
                                TargetRot = TM.ToRotation() - MathHelper.Pi / 4 * 3;
                                break;
                            default:
                                break;
                        }

                        if (Projectile.ai[1] == 0)
                        {
                            Movement(TargetPos, 0.5f);
                            Projectile.rotation = Projectile.velocity.ToRotation();
                            if (Projectile.Distance(TargetPos) < 30)
                            {
                                Projectile.ai[1] = 1;
                            }
                        }
                        else
                        {
                            Projectile.velocity = Vector2.Zero;
                            Projectile.Center = TargetPos;
                            Projectile.rotation = TargetRot;
                        }
                    }

                    else if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 2)       //laser cannon
                    {
                        Vector2 TM = Main.MouseWorld - owner.Center;
                        if (TM == Vector2.Zero) TM = new Vector2(0, -1);
                        TM.Normalize();
                        Vector2 TargetPos = Vector2.Zero;
                        float TargetRot = 0;
                        switch (Projectile.ai[0])
                        {
                            case 0:
                                TargetPos = owner.Center + TM * 40;
                                TargetRot = TM.ToRotation();
                                break;
                            case 1:
                                TargetPos = owner.Center + TM * 55 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 15;
                                TargetRot = TM.ToRotation();
                                break;
                            case 2:
                                TargetPos = owner.Center + TM * 55 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 15;
                                TargetRot = TM.ToRotation();
                                break;
                            case 3:
                                TargetPos = owner.Center + TM * 85 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 15;
                                TargetRot = TM.ToRotation();
                                break;
                            case 4:
                                TargetPos = owner.Center + TM * 85 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 15;
                                TargetRot = TM.ToRotation();
                                break;
                            case 5:
                                TargetPos = owner.Center + TM * 115 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 15;
                                TargetRot = TM.ToRotation();
                                break;
                            case 6:
                                TargetPos = owner.Center + TM * 115 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 15;
                                TargetRot = TM.ToRotation();
                                break;
                            default:
                                break;
                        }

                        if (Projectile.ai[1] == 0)
                        {
                            Movement(TargetPos, 0.5f);
                            Projectile.rotation = Projectile.velocity.ToRotation();
                            if (Projectile.Distance(TargetPos) < 30)
                            {
                                Projectile.ai[1] = 1;
                            }
                        }
                        else
                        {
                            Projectile.velocity = Vector2.Zero;
                            Projectile.Center = TargetPos;
                            Projectile.rotation = TargetRot;
                        }
                    }

                    else if (owner.GetModPlayer<NanoPlayer>().NanoCoreMode == 3)         //bombing phase
                    {
                        Vector2 TM = Main.MouseWorld - owner.Center;
                        if (TM == Vector2.Zero) TM = new Vector2(0, -1);
                        TM.Normalize();
                        Vector2 TargetPos = Vector2.Zero;
                        float TargetRot = Vector2.Normalize(Main.MouseWorld - Projectile.Center).ToRotation();

                        if (!owner.channel)
                        {
                            if (owner.direction <= 0)
                            {
                                TargetPos = owner.Center + (-MathHelper.Pi / 2 + MathHelper.Pi / 8 * (Projectile.ai[0] + 1)).ToRotationVector2() * 60;
                            }
                            else
                            {
                                TargetPos = owner.Center + (MathHelper.Pi / 2 + MathHelper.Pi / 8 * (Projectile.ai[0] + 1)).ToRotationVector2() * 60;
                            }
                            TargetRot = (Projectile.Center - owner.Center).ToRotation();
                        }
                        else
                        {
                            switch (Projectile.ai[0])
                            {
                                case 0:
                                    TargetPos = owner.Center + TM * -50 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 300;
                                    break;
                                case 1:
                                    TargetPos = owner.Center + TM * -50 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 300;
                                    break;
                                case 2:
                                    TargetPos = owner.Center + TM * 200 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 250;
                                    break;
                                case 3:
                                    TargetPos = owner.Center + TM * 200 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 250;
                                    break;
                                case 4:
                                    TargetPos = owner.Center + TM * 450 + (TM.ToRotation() + MathHelper.Pi / 2).ToRotationVector2() * 200;
                                    break;
                                case 5:
                                    TargetPos = owner.Center + TM * 450 + (TM.ToRotation() - MathHelper.Pi / 2).ToRotationVector2() * 200;
                                    break;
                                case 6:
                                    TargetPos = owner.Center + new Vector2(-30 * owner.direction, -40);
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (OldChannel && !owner.channel)
                        {
                            Projectile.ai[1] = 0;
                            OldChannel = false;
                        }
                        if (!OldChannel && owner.channel)
                        {
                            Projectile.ai[1] = 0;
                            OldChannel = true;
                        }

                        if (Projectile.ai[1] == 0)
                        {
                            Movement(TargetPos, 0.5f);
                            Projectile.rotation = Projectile.velocity.ToRotation();
                            if (Projectile.Distance(TargetPos) < 30)
                            {
                                Projectile.ai[1] = 1;
                            }
                        }
                        else
                        {
                            Projectile.velocity = Vector2.Zero;
                            Projectile.Center = TargetPos;
                            Projectile.rotation = TargetRot;
                        }
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.Kill();
            }
        }


        public void Movement(Vector2 targetPos, float speedModifier, float Limit = 24)
        {
            Projectile.position += (Main.player[Projectile.owner].position - Main.player[Projectile.owner].oldPosition) * 0.8f;

            if (Projectile.Center.X < targetPos.X)
            {
                Projectile.velocity.X += speedModifier;
                if (Projectile.velocity.X < 0)
                    Projectile.velocity.X += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.X -= speedModifier;
                if (Projectile.velocity.X > 0)
                    Projectile.velocity.X -= speedModifier * 2;
            }
            if (Projectile.Center.Y < targetPos.Y)
            {
                Projectile.velocity.Y += speedModifier;
                if (Projectile.velocity.Y < 0)
                    Projectile.velocity.Y += speedModifier * 2;
            }
            else
            {
                Projectile.velocity.Y -= speedModifier;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(Projectile.velocity.X) > Limit)
                Projectile.velocity.X = Limit * Math.Sign(Projectile.velocity.X);
            if (Math.Abs(Projectile.velocity.Y) > Limit)
                Projectile.velocity.Y = Limit * Math.Sign(Projectile.velocity.Y);


        }
    }
}