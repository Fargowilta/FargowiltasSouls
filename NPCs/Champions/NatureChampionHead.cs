using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class NatureChampionHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Nature");
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "自然英灵");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    BuffID.Chilled,
                    BuffID.OnFire,
                    BuffID.Suffocation,
                    ModContent.BuffType<Lethargic>(),
                    ModContent.BuffType<ClippedWings>(),
                    ModContent.BuffType<LightningRod>()
                }
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 80;
            NPC.damage = 110;
            NPC.defense = 100;
            NPC.lifeMax = 900000;
            NPC.HitSound = SoundID.NPCHit6;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            if (NPC.ai[0] == -3) //crimson head does no contact damage
                return false;
            CooldownSlot = 1;
            return true;
        }

        public override void AI()
        {
            NPC body = FargoSoulsUtil.NPCExists(NPC.ai[1], ModContent.NPCType<NatureChampion>());
            if (body == null)
            {
                NPC.life = 0;
                NPC.checkDead();
                NPC.active = false;
                return;
            }

            NPC.target = body.target;
            NPC.realLife = body.whoAmI;
            NPC.position += body.velocity * 0.75f;

            Player player = Main.player[NPC.target];
            Vector2 targetPos;

            if (player.Center.X < NPC.position.X)
                NPC.direction = NPC.spriteDirection = -1;
            else if (player.Center.X > NPC.position.X + NPC.width)
                NPC.direction = NPC.spriteDirection = 1;
            NPC.rotation = 0;

            switch ((int)NPC.ai[0])
            {
                case -3: //crimson
                    targetPos = player.Center - Vector2.UnitY * 250f;
                    Movement(targetPos, 0.3f, 24f);

                    if (++NPC.ai[2] > 75) //ichor periodically
                    {
                        if (NPC.ai[2] > 105)
                        {
                            NPC.ai[2] = 0;
                        }

                        NPC.velocity *= 0.99f;

                        if (++NPC.localAI[1] > 2) //rain piss
                        {
                            NPC.localAI[1] = 0;
                            if (NPC.localAI[0] > 60 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2),
                                    Vector2.UnitY * Main.rand.NextFloat(-4f, 0), ProjectileID.GoldenShowerHostile, FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++NPC.localAI[0] > 300)
                    {
                        NPC.ai[0] = 0;
                        NPC.localAI[0] = 0;
                        NPC.ai[2] = 0;
                        NPC.localAI[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case -2: //molten
                    /*if (++NPC.ai[2] > 60)
                    {
                        NPC.ai[2] = 0;

                        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 12;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 speed = 20f * NPC.DirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<NatureFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }*/

                    if (++NPC.localAI[0] < 240) //stay near
                    {
                        targetPos = player.Center;
                        Movement(targetPos, 0.10f, 24f);

                        for (int i = 0; i < 20; i++) //warning ring
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 400);
                            offset.Y += (float)(Math.Cos(angle) * 400);
                            Dust dust = Main.dust[Dust.NewDust(NPC.Center + offset - new Vector2(4, 4), 0, 0, DustID.Torch, 0, 0, 100, Color.White, 2f)];
                            dust.velocity = NPC.velocity;
                            if (Main.rand.NextBool(3))
                                dust.velocity += Vector2.Normalize(offset) * -5f;
                            dust.noGravity = true;
                        }
                    }
                    else if (NPC.localAI[0] == 240) //explode into attacks
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.netUpdate = true;

                        SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<NatureExplosion>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);

                            const int max = 12;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 speed = 24f * NPC.DirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<NatureFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            }
                        }
                    }
                    else if (NPC.localAI[0] > 300)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[2] = 0;
                        NPC.localAI[0] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case -1: //rain
                    targetPos = player.Center + NPC.DirectionFrom(player.Center) * 300;
                    Movement(targetPos, 0.25f, 24f);

                    if (++NPC.localAI[1] > 45)
                    {
                        NPC.localAI[1] = 0;

                        SoundEngine.PlaySound(SoundID.Item66, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            /*Vector2 dir = Main.player[NPC.target].Center - NPC.Center;
                            float ai1New = Main.rand.Next(100);
                            Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 6f;
                            Projectile.NewProjectile(npc.GetSource_FromThis(), NPC.Center, vel, ProjectileID.CultistBossLightningOrbArc,
                                FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0, Main.myPlayer, dir.ToRotation(), ai1New);*/

                            Vector2 speed = player.Center - NPC.Center;
                            speed.Y -= 300;
                            speed /= 40;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<NatureCloudMoving>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }
                    }

                    if (++NPC.localAI[0] > 300)
                    {
                        NPC.ai[0] = 0;
                        NPC.localAI[0] = 0;
                        NPC.ai[2] = 0;
                        NPC.localAI[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 0: //float near body
                    {
                        Vector2 offset;
                        offset.X = 100f * NPC.ai[3] - 50 * Math.Sign(NPC.ai[3]);
                        offset.Y = -350 + 75f * Math.Abs(NPC.ai[3]);
                        targetPos = body.Center + offset;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 24f);
                    }
                    break;

                case 1: //frost
                    {
                        Vector2 offset;
                        offset.X = 100f * NPC.ai[3] - 50 * Math.Sign(NPC.ai[3]);
                        offset.Y = -350 + 75f * Math.Abs(NPC.ai[3]);
                        targetPos = body.Center + offset;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 24f);

                        if (++NPC.ai[2] > 60)
                        {
                            NPC.ai[2] = 0;
                            //NPC.localAI[1] = NPC.localAI[1] == 1 ? -1 : 1;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 25;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 speed = Main.rand.NextFloat(1f, 3f) * Vector2.UnitX.RotatedBy(2 * Math.PI / max * (i + Main.rand.NextDouble()));
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<NatureIcicle>(),
                                        FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, 60 + Main.rand.Next(20), 1f);// NPC.localAI[1]);
                                }
                            }
                        }

                        if (++NPC.localAI[0] > 300)
                        {
                            NPC.ai[0] = 0;
                            NPC.localAI[0] = 0;
                            NPC.ai[2] = 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 2: //chlorophyte
                    targetPos = player.Center;
                    Movement(targetPos, 0.12f, 24f);

                    if (NPC.ai[2] == 0)
                    {
                        NPC.ai[2] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 5;
                            const float distance = 125f;
                            float rotation = 2f * (float)Math.PI / max;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 spawnPos = NPC.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<NatureCrystalLeaf>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.whoAmI, rotation * i);
                            }
                        }
                    }

                    if (++NPC.localAI[0] > 300)
                    {
                        NPC.ai[0] = 0;
                        NPC.localAI[0] = 0;
                        NPC.ai[2] = 0;
                        NPC.localAI[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 3: //shroomite
                    {
                        Vector2 offset;
                        offset.X = 100f * NPC.ai[3] - 50 * Math.Sign(NPC.ai[3]);
                        offset.Y = -350 + 75f * Math.Abs(NPC.ai[3]);
                        targetPos = body.Center + offset;
                        if (NPC.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 24f);

                        if (++NPC.ai[2] < 20)
                        {
                            if (NPC.localAI[0] > 60 && NPC.ai[2] % 2 == 0)
                            {
                                Vector2 speed = player.Center - NPC.Center;
                                speed.X += Main.rand.Next(-40, 41);
                                speed.Y += Main.rand.Next(-40, 41);
                                speed.Normalize();
                                speed *= 12.5f;
                                int delay = (int)(NPC.Distance(player.Center) - 100) / 14;
                                if (delay < 0)
                                    delay = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.UnitY * 10, speed,
                                        ModContent.ProjectileType<NatureBullet>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, delay);
                                }
                            }
                        }

                        if (NPC.ai[2] > 60)
                            NPC.ai[2] = 0;

                        if (++NPC.localAI[0] > 300)
                        {
                            NPC.ai[0] = 0;
                            NPC.localAI[0] = 0;
                            NPC.ai[2] = 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 4: //deathrays
                    {
                        Vector2 offset = -600 * Vector2.UnitY.RotatedBy(MathHelper.ToRadians(60 / 3) * NPC.ai[3]);
                        targetPos = body.Center + offset;
                        Movement(targetPos, 0.8f, 24f);

                        NPC.direction = NPC.spriteDirection = NPC.Center.X < body.Center.X ? 1 : -1;

                        if (++NPC.ai[2] == 90)
                        {
                            NPC.netUpdate = true;
                            NPC.localAI[1] = NPC.DirectionTo(body.Center - Vector2.UnitY * 300).ToRotation();

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.localAI[1]),
                                    ModContent.ProjectileType<NatureDeathraySmall>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            }
                        }
                        else if (NPC.ai[2] == 150)
                        {
                            float ai0 = 2f * (float)Math.PI / 120 * Math.Sign(NPC.ai[3]);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX.RotatedBy(NPC.localAI[1]),
                                    ModContent.ProjectileType<NatureDeathray>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 4f / 3), 0f, Main.myPlayer, ai0, NPC.whoAmI);
                            }
                        }

                        if (++NPC.localAI[0] > 330)
                        {
                            NPC.ai[0] = 0;
                            NPC.localAI[0] = 0;
                            NPC.ai[2] = 0;
                            NPC.localAI[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                default:
                    NPC.ai[0] = 0;
                    goto case 0;
            }

            if (NPC.Distance(body.Center) > 1400) //try to prevent going too far from body, will cause neck to disappear
            {
                NPC.Center = body.Center + body.DirectionTo(NPC.Center) * 1400f;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 3;
            return true;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (NPC.Center.X < targetPos.X)
            {
                NPC.velocity.X += speedModifier;
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += speedModifier * 2;
            }
            else
            {
                NPC.velocity.X -= speedModifier;
                if (NPC.velocity.X > 0)
                    NPC.velocity.X -= speedModifier * 2;
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > cap)
                NPC.velocity.X = cap * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > cap)
                NPC.velocity.Y = cap * Math.Sign(NPC.velocity.Y);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(BuffID.Frostburn, 300);
                target.AddBuff(BuffID.OnFire, 300);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                NPC body = FargoSoulsUtil.NPCExists(NPC.ai[1], ModContent.NPCType<NatureChampion>());
                if (body != null) //make body handle gibs
                {
                    body.HitEffect(hitDirection, damage);
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            /*int frameModifier = (int)NPC.ai[3];
            if (frameModifier > 0)
                frameModifier--;
            frameModifier += 3;*/

            NPC.frame.Y = 0;
            if (!NPC.HasValidTarget)
                NPC.frame.Y = frameHeight * 3;

            switch ((int)NPC.ai[0])
            {
                case -3: //crimson
                    if (NPC.ai[2] > 60) //ichor periodically
                        NPC.frame.Y = frameHeight;
                    break;

                case -2: //molten
                    if (NPC.localAI[0] > 240) //stay near
                        NPC.frame.Y = frameHeight * 2;
                    break;

                case -1: //rain
                    if (NPC.localAI[1] < 20)
                        NPC.frame.Y = frameHeight;
                    break;

                case 1: //frost
                    if (NPC.ai[2] > 30)
                        NPC.frame.Y = frameHeight;
                    break;

                case 2: //chlorophyte
                    NPC.frame.Y = frameHeight * 2;
                    break;

                case 3: //shroomite
                    if (NPC.ai[2] < 20 && NPC.localAI[0] > 60)
                        NPC.frame.Y = frameHeight;
                    break;

                case 4: //deathrays
                    if (NPC.ai[2] > 90)
                        NPC.frame.Y = frameHeight * 2;
                    break;

                default:
                    break;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public Vector2 position, oldPosition;
        private static float X(float t, float x0, float x1, float x2)
        {
            return (float)(
                x0 * Math.Pow((1 - t), 2) +
                x1 * 2 * t * Math.Pow((1 - t), 1) +
                x2 * Math.Pow(t, 2)
            );
        }
        private static float Y(float t, float y0, float y1, float y2)
        {
            return (float)(
                 y0 * Math.Pow((1 - t), 2) +
                 y1 * 2 * t * Math.Pow((1 - t), 1) +
                 y2 * Math.Pow(t, 2)
             );
        }

        public void CheckDrawNeck(SpriteBatch spriteBatch)
        {
            if (!(NPC.ai[1] > -1 && NPC.ai[1] < Main.maxNPCs && Main.npc[(int)NPC.ai[1]].active
                && Main.npc[(int)NPC.ai[1]].type == ModContent.NPCType<NatureChampion>()))
            {
                return;
            }

            NPC body = Main.npc[(int)NPC.ai[1]];

            if (Main.LocalPlayer.Distance(body.Center) > 1200)
            {
                string neckTex = "NPCs/Champions/NatureChampion_Neck";
                Texture2D neckTex2D = FargowiltasSouls.Instance.Assets.Request<Texture2D>(neckTex, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Vector2 connector = NPC.Center;
                Vector2 neckOrigin = body.Center + new Vector2(54 * body.spriteDirection, -10);
                float chainsPerUse = 0.05f;
                for (float j = 0; j <= 1; j += chainsPerUse)
                {
                    if (j == 0)
                        continue;
                    Vector2 distBetween = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) -
                    X(j - chainsPerUse, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X),
                    Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) -
                    Y(j - chainsPerUse, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                    if (distBetween.Length() > 36 && chainsPerUse > 0.01f)
                    {
                        chainsPerUse -= 0.01f;
                        j -= chainsPerUse;
                        continue;
                    }
                    float projTrueRotation = distBetween.ToRotation() - (float)Math.PI / 2;
                    Vector2 lightPos = new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X), Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y));
                    spriteBatch.Draw(neckTex2D, new Vector2(X(j, neckOrigin.X, (neckOrigin.X + connector.X) / 2, connector.X) - Main.screenPosition.X, Y(j, neckOrigin.Y, (neckOrigin.Y + 50), connector.Y) - Main.screenPosition.Y),
                    new Rectangle(0, 0, neckTex2D.Width, neckTex2D.Height), body.GetAlpha(Lighting.GetColor((int)lightPos.X / 16, (int)lightPos.Y / 16)), projTrueRotation,
                    new Vector2(neckTex2D.Width * 0.5f, neckTex2D.Height * 0.5f), 1f, connector.X < neckOrigin.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            CheckDrawNeck(spriteBatch);

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int glow = (int)NPC.ai[3];
            if (glow > 0)
                glow--;
            glow += 3;
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("NPCs/Champions/NatureChampionHead_Glow" + glow.ToString(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 0.8f;
            Main.EntitySpriteDraw(texture2D13, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor) * 0.5f, NPC.rotation, origin2, NPC.scale * scale, effects, 0);
            Main.EntitySpriteDraw(texture2D13, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(texture2D14, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
