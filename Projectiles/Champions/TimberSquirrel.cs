using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    internal class TimberSquirrel : ModProjectile
    {
        public int Counter;

        public override string Texture => "FargowiltasSouls/Items/Weapons/Misc/TophatSquirrelWeapon";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        const int baseTimeleft = 120;

        public override void SetDefaults()
        {
            Projectile.width = 19;
            Projectile.height = 19;
            Projectile.hostile = true;
            Projectile.timeLeft = baseTimeleft;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? CanDamage() => false;

        bool EvilSqurrl => Projectile.ai[0] != 0;

        NPC npc;

        public override void OnSpawn(IEntitySource source)
        {
            npc = FargoSoulsUtil.NPCExists(Projectile.ai[1]);
        }

        bool spawned;

        public override void AI()
        {
            //NOTE: AI[1] CONTAINS NPC.WHOAMI (so boss knows to wait until this proj is gone)

            if (!spawned)
            {
                spawned = true;
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, Scale: 3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].velocity += Projectile.velocity * Main.rand.NextFloat(9f);
                }
            }

            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
            Projectile.rotation += 0.2f * Projectile.spriteDirection;

            if (++Counter >= 45)
            {
                Projectile.scale += .1f;
            }

            if (EvilSqurrl)
            {
                Projectile.timeLeft++;

                const float maxRange = 1000f;
                const int maxAttacks = 20;

                FargoSoulsUtil.AuraDust(Projectile, maxRange, DustID.UltraBrightTorch, Color.White, true, 1f - Projectile.localAI[1] / maxAttacks);

                if (Counter == baseTimeleft)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(npc is NPC ? npc : Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TimberLightningOrb>(), Projectile.damage, 0f, Main.myPlayer);
                }

                if (Counter > baseTimeleft)
                {
                    Projectile.hide = true;
                    Projectile.position -= Projectile.velocity;

                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                    Projectile.rotation += 0.1349578f * Math.Sign(Projectile.velocity.X);

                    if (--Projectile.localAI[0] < 0)
                    {
                        Projectile.localAI[0] = 15;

                        SoundEngine.PlaySound(SoundID.Item157, Projectile.Center);

                        int lasersToAddPerAttack = 3;

                        int modifier = (int)Projectile.localAI[1]; //scale up to halfway
                        if (modifier >= maxAttacks / 2) //scale back down after that
                            modifier = maxAttacks - modifier;

                        int max = lasersToAddPerAttack * modifier;
                        for (int i = 0; i < max; i++)
                        {
                            float ai0 = npc is NPC ? npc.whoAmI : -1;

                            const float speed = 20f / 5f;

                            float rotationBaseOffset = MathHelper.TwoPi / max;
                            Vector2 vel = speed * (Projectile.rotation + rotationBaseOffset * i).ToRotationVector2();

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(Terraria.Entity.InheritSource(npc is NPC ? npc : Projectile), Projectile.Center, vel, ModContent.ProjectileType<TimberLaser>(), Projectile.damage, 0f, Main.myPlayer, ai0);

                            if (npc is NPC)
                            {
                                float edgeRotation = Projectile.rotation + rotationBaseOffset * (i + 0.5f);
                                Vector2 spawnPos = Projectile.Center + maxRange * edgeRotation.ToRotationVector2();
                                Vector2 target = Main.player[npc.target].Center + Main.player[npc.target].velocity * Main.rand.NextFloat(30f) + Main.rand.NextVector2Circular(128, 128);
                                Vector2 edgeVel = (target - spawnPos).SafeNormalize(Vector2.UnitY);

                                float angleDifference = MathHelper.WrapAngle(edgeVel.ToRotation() - edgeRotation);
                                if (Math.Abs(angleDifference) > MathHelper.PiOver2)
                                    edgeVel = (edgeRotation + MathHelper.PiOver2 * Math.Sign(angleDifference)).ToRotationVector2();

                                edgeVel *= speed * 3f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(Terraria.Entity.InheritSource(npc), spawnPos, edgeVel, ModContent.ProjectileType<TimberLaser>(), Projectile.damage, 0f, Main.myPlayer, ai0);
                            }
                        }

                        if (++Projectile.localAI[1] >= maxAttacks)
                            Projectile.Kill();
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (EvilSqurrl)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 3)
                {
                    Color color27 = new Color(93, 255, 241, 0);
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale * 1.1f, effects, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}