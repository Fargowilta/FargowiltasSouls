using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantHeal : ModProjectile, IPixelPrimitiveDrawer
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mutant Heal");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
            //Projectile.hostile = true;
            Projectile.scale = 0.8f;

            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                if (Projectile.localAI[1] == 0)
                {
                    Projectile.localAI[1] = Main.rand.NextFloat(MathHelper.ToRadians(1f)) * (Main.rand.NextBool() ? 1 : -1);
                    Projectile.netUpdate = true;
                }

                Projectile.velocity = Vector2.Normalize(Projectile.velocity).RotatedBy(Projectile.localAI[1]) * (Projectile.velocity.Length() - Projectile.ai[1]);

                if (Projectile.velocity.Length() < 0.01f)
                {
                    Projectile.localAI[0] = 1;
                    Projectile.netUpdate = true;
                }
            }
            else if (Projectile.localAI[0] == 1)
            {
                for (int i = 0; i < 2; i++) //make up for real spectre bolt having 3 extraUpdates
                {
                    Projectile.position += Projectile.velocity;

                    Vector2 change = Vector2.Normalize(Projectile.velocity) * 5f;
                    Projectile.velocity = (Projectile.velocity * 29f + change).RotatedBy(Projectile.localAI[1] * 3) / 30f;
                }

                if (Projectile.velocity.Length() > 4.5f)
                {
                    Projectile.localAI[0] = 2;
                    Projectile.netUpdate = true;

                    Projectile.timeLeft = 180 * 2; //compensating for extraUpdates
                }
            }
            else
            {
                Projectile.extraUpdates = 1;

                int ai0 = (int)Math.Abs(Projectile.ai[0]);
                bool feedPlayer = Projectile.ai[0] < 0;
                if (feedPlayer)
                {
                    ai0 -= 1;

                    if (FargoSoulsWorld.MasochistModeReal)
                    {
                        Projectile.Kill();
                        return;
                    }
                }

                if (ai0 < 0 || (feedPlayer ? ai0 >= Main.maxPlayers || !Main.player[ai0].active || Main.player[ai0].ghost || Main.player[ai0].dead : ai0 >= Main.maxNPCs || !Main.npc[ai0].active))
                {
                    Projectile.Kill();
                    return;
                }

                Entity target = feedPlayer ? Main.player[ai0] : Main.npc[ai0];

                if (Projectile.Distance(target.Center) < 5f)
                {
                    if (feedPlayer) //die and feed player
                    {
                        if (Main.player[ai0].whoAmI == Main.myPlayer)
                        {
                            Main.player[ai0].ClearBuff(ModContent.BuffType<MutantFang>());
                            Main.player[ai0].statLife += Projectile.damage;
                            Main.player[ai0].HealEffect(Projectile.damage);
                            if (Main.player[ai0].statLife > Main.player[ai0].statLifeMax2)
                                Main.player[ai0].statLife = Main.player[ai0].statLifeMax2;
                            Projectile.Kill();
                        }
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Main.npc[ai0].life += Projectile.damage;
                            Main.npc[ai0].HealEffect(Projectile.damage);
                            if (Main.npc[ai0].life > Main.npc[ai0].lifeMax)
                                Main.npc[ai0].life = Main.npc[ai0].lifeMax;
                            Main.npc[ai0].netUpdate = true;
                            Projectile.Kill();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++) //make up for real spectre bolt having 3 extraUpdates
                    {
                        Vector2 change = Projectile.DirectionTo(target.Center) * 5f;
                        Projectile.velocity = (Projectile.velocity * 29f + change) / 30f;
                    }

                    Projectile.position += (target.position - target.oldPosition) / 2;
                }

                for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                {
                    Projectile.position += Projectile.velocity;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            /*int ai0 = (int)Projectile.ai[0];
            if (ai0 > -1 && ai0 < Main.maxNPCs && Main.npc[ai0].active && Main.npc[ai0].type == ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
            {
                CombatText.NewText(Main.npc[ai0].Hitbox, CombatText.HealLife, Projectile.damage);
            }*/

            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(51, 255, 191, 210) * Projectile.Opacity * 0.8f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            #region Old drawcode
            //int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            //Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            //Vector2 origin2 = rectangle.Size() / 2f;

            //Color color26 = lightColor;
            //color26 = Projectile.GetAlpha(color26);

            //for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.2f)
            //{
            //    Texture2D glow = texture2D13; //FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/BossWeapons/HentaiSpearSpinGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //    Color color27 = color26; //Color.Lerp(new Color(255, 255, 0, 210), Color.Transparent, 0.4f);
            //    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            //    float scale = Projectile.scale;
            //    scale *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            //    int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
            //    if (max0 < 0)
            //        continue;
            //    Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
            //    float smoothtrail = i % 1 * MathHelper.Pi / 6.85f;

            //    center += Projectile.Size / 2;

            //    Main.EntitySpriteDraw(
            //        glow,
            //        center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
            //        null,
            //        color27,
            //        Projectile.rotation,
            //        glow.Size() / 2,
            //        scale,
            //        SpriteEffects.None,
            //        0);
            //}
            #endregion
            // Draw the base sprite.
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Cyan, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.9f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Cyan, Color.Transparent, completionRatio) * 0.7f;
        }

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["FargowiltasSouls:BlobTrail"]);
            GameShaders.Misc["FargowiltasSouls:BlobTrail"].SetShaderTexture(FargosTextureRegistry.FadedStreak);
            TrailDrawer.DrawPixelPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 25);
        }
    }
}