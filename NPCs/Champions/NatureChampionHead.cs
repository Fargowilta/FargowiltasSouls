using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.Champions;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.NPCs.Champions
{
    [AutoloadBossHead]
    public class NatureChampionHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Nature");
            DisplayName.AddTranslation(GameCulture.Chinese, "自然英灵");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 80;
            npc.height = 80;
            npc.damage = 110;
            npc.defense = 100;
            npc.lifeMax = 600000;
            npc.HitSound = SoundID.NPCHit6;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;

            npc.buffImmune[BuffID.Chilled] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[mod.BuffType("Lethargic")] = true;
            npc.buffImmune[mod.BuffType("ClippedWings")] = true;
            npc.buffImmune[mod.BuffType("LightningRod")] = true;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (npc.ai[0] == -3) //crimson head does no contact damage
                return false;
            cooldownSlot = 1;
            return true;
        }

        public override void AI()
        {
            if (!(npc.ai[1] > -1 && npc.ai[1] < Main.maxNPCs && Main.npc[(int)npc.ai[1]].active
                && Main.npc[(int)npc.ai[1]].type == ModContent.NPCType<NatureChampion>()))
            {
                npc.life = 0;
                npc.checkDead();
                npc.active = false;
                return;
            }
            
            NPC body = Main.npc[(int)npc.ai[1]];
            
            npc.target = body.target;
            npc.realLife = body.whoAmI;
            npc.position += body.velocity * 0.75f;

            Player player = Main.player[npc.target];
            Vector2 targetPos;

            if (player.Center.X < npc.position.X)
                npc.direction = npc.spriteDirection = -1;
            else if (player.Center.X > npc.position.X + npc.width)
                npc.direction = npc.spriteDirection = 1;
            npc.rotation = 0;
            
            switch ((int)npc.ai[0])
            {
                case -3: //crimson
                    targetPos = player.Center - Vector2.UnitY * 250f;
                    Movement(targetPos, 0.3f, 24f);

                    if (++npc.ai[2] > 75) //ichor periodically
                    {
                        if (npc.ai[2] > 105)
                        {
                            npc.ai[2] = 0;
                        }

                        npc.velocity *= 0.99f;

                        if (++npc.localAI[1] > 2) //rain piss
                        {
                            npc.localAI[1] = 0;
                            if (npc.localAI[0] > 60 && Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center + Main.rand.NextVector2Circular(npc.width / 2, npc.height / 2),
                                    Vector2.UnitY * Main.rand.NextFloat(-4f, 0), ProjectileID.GoldenShowerHostile, npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (++npc.localAI[0] > 300)
                    {
                        npc.ai[0] = 0;
                        npc.localAI[0] = 0;
                        npc.ai[2] = 0;
                        npc.localAI[1] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case -2: //molten
                    /*if (++npc.ai[2] > 60)
                    {
                        npc.ai[2] = 0;

                        Main.PlaySound(SoundID.Item, npc.Center, 14);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 12;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 speed = 20f * npc.DirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i);
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<NatureFireball>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }*/

                    if (++npc.localAI[0] < 240) //stay near
                    {
                        targetPos = player.Center;
                        Movement(targetPos, 0.10f, 24f);

                        for (int i = 0; i < 20; i++) //warning ring
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * 400);
                            offset.Y += (float)(Math.Cos(angle) * 400);
                            Dust dust = Main.dust[Dust.NewDust(npc.Center + offset - new Vector2(4, 4), 0, 0, DustID.Fire, 0, 0, 100, Color.White, 2f)];
                            dust.velocity = npc.velocity;
                            if (Main.rand.Next(3) == 0)
                                dust.velocity += Vector2.Normalize(offset) * -5f;
                            dust.noGravity = true;
                        }
                    }
                    else if (npc.localAI[0] == 240) //explode into attacks
                    {
                        npc.velocity = Vector2.Zero;
                        npc.netUpdate = true;

                        Main.PlaySound(SoundID.Roar, npc.Center, 0);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<NatureExplosion>(), npc.damage / 4, 0f, Main.myPlayer);

                            const int max = 12;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 speed = 24f * npc.DirectionTo(player.Center).RotatedBy(2 * Math.PI / max * i);
                                Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<NatureFireball>(), npc.damage / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    else if (npc.localAI[0] > 300)
                    {
                        npc.ai[0] = 0;
                        npc.ai[2] = 0;
                        npc.localAI[0] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case -1: //rain
                    targetPos = player.Center + npc.DirectionFrom(player.Center) * 300;
                    Movement(targetPos, 0.25f, 24f);

                    if (++npc.localAI[1] > 45)
                    {
                        npc.localAI[1] = 0;

                        Main.PlaySound(SoundID.Item66, npc.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            /*Vector2 dir = Main.player[npc.target].Center - npc.Center;
                            float ai1New = Main.rand.Next(100);
                            Vector2 vel = Vector2.Normalize(dir.RotatedByRandom(Math.PI / 4)) * 6f;
                            Projectile.NewProjectile(npc.Center, vel, ProjectileID.CultistBossLightningOrbArc,
                                npc.damage / 4, 0, Main.myPlayer, dir.ToRotation(), ai1New);*/

                            Vector2 speed = player.Center - npc.Center;
                            speed.Y -= 300;
                            speed /= 40;
                            Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<NatureCloudMoving>(), npc.damage / 4, 0f, Main.myPlayer);
                        }
                    }

                    if (++npc.localAI[0] > 300)
                    {
                        npc.ai[0] = 0;
                        npc.localAI[0] = 0;
                        npc.ai[2] = 0;
                        npc.localAI[1] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 0: //float near body
                    {
                        Vector2 offset;
                        offset.X = 100f * npc.ai[3] - 50 * Math.Sign(npc.ai[3]);
                        offset.Y = -350 + 75f * Math.Abs(npc.ai[3]);
                        targetPos = body.Center + offset;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 24f);
                    }
                    break;

                case 1: //frost
                    {
                        Vector2 offset;
                        offset.X = 100f * npc.ai[3] - 50 * Math.Sign(npc.ai[3]);
                        offset.Y = -350 + 75f * Math.Abs(npc.ai[3]);
                        targetPos = body.Center + offset;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 24f);

                        if (++npc.ai[2] > 60)
                        {
                            npc.ai[2] = 0;
                            //npc.localAI[1] = npc.localAI[1] == 1 ? -1 : 1;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                const int max = 25;
                                for (int i = 0; i < max; i++)
                                {
                                    Vector2 speed = Main.rand.NextFloat(1f, 3f) * Vector2.UnitX.RotatedBy(2 * Math.PI / max * (i + Main.rand.NextDouble()));
                                    Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<NatureIcicle>(),
                                        npc.damage / 4, 0f, Main.myPlayer, 60 + Main.rand.Next(20), 1f);// npc.localAI[1]);
                                }
                            }
                        }

                        if (++npc.localAI[0] > 300)
                        {
                            npc.ai[0] = 0;
                            npc.localAI[0] = 0;
                            npc.ai[2] = 0;
                            npc.localAI[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                case 2: //chlorophyte
                    targetPos = player.Center;
                    Movement(targetPos, 0.12f, 24f);

                    if (npc.ai[2] == 0)
                    {
                        npc.ai[2] = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            const int max = 5;
                            const float distance = 125f;
                            float rotation = 2f * (float)Math.PI / max;
                            for (int i = 0; i < max; i++)
                            {
                                Vector2 spawnPos = npc.Center + new Vector2(distance, 0f).RotatedBy(rotation * i);
                                Projectile.NewProjectile(spawnPos, Vector2.Zero, ModContent.ProjectileType<NatureCrystalLeaf>(), npc.damage / 4, 0f, Main.myPlayer, npc.whoAmI, rotation * i);
                            }
                        }
                    }

                    if (++npc.localAI[0] > 300)
                    {
                        npc.ai[0] = 0;
                        npc.localAI[0] = 0;
                        npc.ai[2] = 0;
                        npc.localAI[1] = 0;
                        npc.netUpdate = true;
                    }
                    break;

                case 3: //shroomite
                    {
                        Vector2 offset;
                        offset.X = 100f * npc.ai[3] - 50 * Math.Sign(npc.ai[3]);
                        offset.Y = -350 + 75f * Math.Abs(npc.ai[3]);
                        targetPos = body.Center + offset;
                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.8f, 24f);

                        if (++npc.ai[2] < 20)
                        {
                            if (npc.localAI[0] > 60 && npc.ai[2] % 2 == 0)
                            {
                                Vector2 speed = player.Center - npc.Center;
                                speed.X += Main.rand.Next(-40, 41);
                                speed.Y += Main.rand.Next(-40, 41);
                                speed.Normalize();
                                speed *= 12.5f;
                                int delay = (int)(npc.Distance(player.Center) - 100) / 14;
                                if (delay < 0)
                                    delay = 0;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center + Vector2.UnitY * 10, speed,
                                        ModContent.ProjectileType<NatureBullet>(), npc.damage / 4, 0f, Main.myPlayer, delay);
                                }
                            }
                        }

                        if (npc.ai[2] > 60)
                            npc.ai[2] = 0;

                        if (++npc.localAI[0] > 300)
                        {
                            npc.ai[0] = 0;
                            npc.localAI[0] = 0;
                            npc.ai[2] = 0;
                            npc.localAI[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                case 4: //deathrays
                    {
                        Vector2 offset = -600 * Vector2.UnitY.RotatedBy(MathHelper.ToRadians(60 / 3) * npc.ai[3]);
                        targetPos = body.Center + offset;
                        Movement(targetPos, 0.8f, 24f);

                        npc.direction = npc.spriteDirection = npc.Center.X < body.Center.X ? 1 : -1;

                        if (++npc.ai[2] == 90)
                        {
                            npc.netUpdate = true;
                            npc.localAI[1] = npc.DirectionTo(body.Center - Vector2.UnitY * 300).ToRotation();

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(npc.localAI[1]), 
                                    ModContent.ProjectileType<NatureDeathraySmall>(), npc.damage / 3, 0f, Main.myPlayer, 0f, npc.whoAmI);
                            }
                        }
                        else if (npc.ai[2] == 150)
                        {
                            float ai0 = 2f * (float)Math.PI / 120 * Math.Sign(npc.ai[3]);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.UnitX.RotatedBy(npc.localAI[1]),
                                    ModContent.ProjectileType<NatureDeathray>(), npc.damage / 3, 0f, Main.myPlayer, ai0, npc.whoAmI);
                            }
                        }

                        if (++npc.localAI[0] > 330)
                        {
                            npc.ai[0] = 0;
                            npc.localAI[0] = 0;
                            npc.ai[2] = 0;
                            npc.localAI[1] = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }

            if (npc.Distance(body.Center) > 1400) //try to prevent going too far from body, will cause neck to disappear
            {
                npc.Center = body.Center + body.DirectionTo(npc.Center) * 1400f;
            }
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 3;
            return true;
        }

        private void Movement(Vector2 targetPos, float speedModifier, float cap = 12f)
        {
            if (npc.Center.X < targetPos.X)
            {
                npc.velocity.X += speedModifier;
                if (npc.velocity.X < 0)
                    npc.velocity.X += speedModifier * 2;
            }
            else
            {
                npc.velocity.X -= speedModifier;
                if (npc.velocity.X > 0)
                    npc.velocity.X -= speedModifier * 2;
            }
            if (npc.Center.Y < targetPos.Y)
            {
                npc.velocity.Y += speedModifier;
                if (npc.velocity.Y < 0)
                    npc.velocity.Y += speedModifier * 2;
            }
            else
            {
                npc.velocity.Y -= speedModifier;
                if (npc.velocity.Y > 0)
                    npc.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(npc.velocity.X) > cap)
                npc.velocity.X = cap * Math.Sign(npc.velocity.X);
            if (Math.Abs(npc.velocity.Y) > cap)
                npc.velocity.Y = cap * Math.Sign(npc.velocity.Y);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.MasochistMode)
            {
                target.AddBuff(BuffID.Frostburn, 300);
                target.AddBuff(BuffID.OnFire, 300);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            /*int frameModifier = (int)npc.ai[3];
            if (frameModifier > 0)
                frameModifier--;
            frameModifier += 3;*/

            npc.frame.Y = 0;
            if (!npc.HasValidTarget)
                npc.frame.Y = frameHeight * 3;

            switch ((int)npc.ai[0])
            {
                case -3: //crimson
                    if (npc.ai[2] > 60) //ichor periodically
                        npc.frame.Y = frameHeight;
                    break;

                case -2: //molten
                    if (npc.localAI[0] > 240) //stay near
                        npc.frame.Y = frameHeight * 2;
                    break;

                case -1: //rain
                    if (npc.localAI[1] < 20)
                        npc.frame.Y = frameHeight;
                    break;

                case 1: //frost
                    if (npc.ai[2] > 30)
                        npc.frame.Y = frameHeight;
                    break;

                case 2: //chlorophyte
                    npc.frame.Y = frameHeight * 2;
                    break;

                case 3: //shroomite
                    if (npc.ai[2] < 20 && npc.localAI[0] > 60)
                        npc.frame.Y = frameHeight;
                    break;

                case 4: //deathrays
                    if (npc.ai[2] > 90)
                        npc.frame.Y = frameHeight * 2;
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
            if (!(npc.ai[1] > -1 && npc.ai[1] < Main.maxNPCs && Main.npc[(int)npc.ai[1]].active
                && Main.npc[(int)npc.ai[1]].type == ModContent.NPCType<NatureChampion>()))
            {
                return;
            }

            NPC body = Main.npc[(int)npc.ai[1]];

            if (Main.LocalPlayer.Distance(body.Center) > 1200)
            {
                string neckTex = "NPCs/Champions/NatureChampion_Neck";
                Texture2D neckTex2D = mod.GetTexture(neckTex);
                Vector2 connector = npc.Center;
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
                    new Vector2(neckTex2D.Width * 0.5f, neckTex2D.Height * 0.5f), 1f, connector.X < neckOrigin.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CheckDrawNeck(spriteBatch);

            Texture2D texture2D13 = Main.npcTexture[npc.type];
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            int glow = (int)npc.ai[3];
            if (glow > 0)
                glow--;
            glow += 3;
            Texture2D texture2D14 = mod.GetTexture("NPCs/Champions/NatureChampionHead_Glow" + glow.ToString());

            float scale = (Main.mouseTextColor / 200f - 0.35f) * 0.4f + 0.8f;
            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor) * 0.5f, npc.rotation, origin2, npc.scale * scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            Main.spriteBatch.Draw(texture2D14, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}
