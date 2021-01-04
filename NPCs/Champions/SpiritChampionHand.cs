using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.NPCs.Champions
{
    public class SpiritChampionHand : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Spirit");
            DisplayName.AddTranslation(GameCulture.Chinese, "魂灵英灵");
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 100;
            npc.height = 100;
            npc.damage = 125;
            npc.defense = 140;
            npc.lifeMax = 550000;
            npc.HitSound = SoundID.NPCHit54;
            npc.DeathSound = SoundID.NPCDeath52;
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.localAI[3] == 0;
        }

        public override void AI()
        {
            if (!(npc.ai[1] > -1 && npc.ai[1] < Main.maxNPCs && Main.npc[(int)npc.ai[1]].active
                && Main.npc[(int)npc.ai[1]].type == ModContent.NPCType<SpiritChampion>()))
            {
                npc.life = 0;
                npc.checkDead();
                npc.active = false;
                return;
            }
            
            NPC head = Main.npc[(int)npc.ai[1]];
            
            npc.target = head.target;
            npc.realLife = head.whoAmI;
            npc.position += head.velocity * 0.75f;

            Player player = Main.player[npc.target];
            Vector2 targetPos;
            
            void Heal()
            {
                if (++npc.localAI[1] > 10) //heal 6 times per second
                {
                    npc.localAI[1] = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 speed = Main.rand.NextFloat(1, 2) * Vector2.UnitX.RotatedByRandom(Math.PI * 2);
                        int heal = (int)(head.lifeMax / 100f * Main.rand.NextFloat(0.95f, 1.05f)); //heal back roughly 1 percent per heal
                        float ai1 = 30 + Main.rand.Next(30);
                        Projectile.NewProjectile(npc.Center, speed, ModContent.ProjectileType<SpiritHeal>(), heal, 0f, Main.myPlayer, head.whoAmI, ai1);
                    }
                }
            }

            npc.localAI[3] = 0;

            switch ((int)npc.ai[0])
            {
                case 0: //float near head
                    {
                        targetPos = head.Center;
                        float offset = head.ai[0] % 2 == 0 ? 50 : 150;
                        float distance = head.ai[0] % 2 == 0 ? 50 : 100;
                        if (head.ai[0] % 2 == 0)
                            npc.localAI[3] = 1;
                        targetPos.X += offset * npc.ai[2];
                        targetPos.Y += offset * npc.ai[3];
                        if (npc.Distance(targetPos) > distance)
                            Movement(targetPos, 0.8f, 24f);
                    }
                    break;

                case 1: //you think you're safe?
                    {
                        if (head.ai[0] != 3 && head.ai[0] != -3) //return to normal when head no longer wants to grab
                        {
                            npc.ai[0] = 0;
                            npc.netUpdate = true;
                        }

                        bool targetPlayer = Math.Sign(player.Center.X - head.Center.X) * Math.Sign(npc.Center.X - head.Center.X) == 1
                            && Math.Sign(player.Center.Y - head.Center.Y) * Math.Sign(npc.Center.Y - head.Center.Y) == 1; //in same quadrant as you
                        if (head.ai[0] == -3) //four hands never target you during last stand
                            targetPlayer = false;
                        if (npc.ai[0] == 3) //FIFTH hand always targets you
                            targetPlayer = true;

                        if (targetPlayer)
                        {
                            targetPos = player.Center;
                        }
                        else //wave around
                        {
                            targetPos = head.Center + head.DirectionTo(npc.Center) * head.Distance(player.Center);
                        }

                        if (npc.Distance(targetPos) > 50)
                            Movement(targetPos, 0.15f, 7f);

                        if (npc.Hitbox.Intersects(player.Hitbox) && !player.HasBuff(ModContent.BuffType<Buffs.Boss.Grabbed>())
                            && player.GetModPlayer<FargoPlayer>().MashCounter <= 0) //GOTCHA
                        {
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);
                            
                            npc.ai[0] = 2;
                            npc.netUpdate = true;

                            if (head.ai[0] != -3) //don't change head state in last stand
                            {
                                head.ai[0] = -1;
                                head.ai[1] = 0;
                                head.netUpdate = true;
                            }
                        }
                    }
                    break;

                case 2: //successful grab
                    if ((head.ai[0] != -1 && head.ai[0] != -3) || !player.active || player.dead || player.GetModPlayer<FargoPlayer>().MashCounter > 30)
                    {
                        if (npc.Hitbox.Intersects(player.Hitbox)) //throw aside
                        {
                            player.GetModPlayer<FargoPlayer>().MashCounter += 30;
                            player.velocity.X = player.Center.X < head.Center.X ? -15f : 15f;
                            player.velocity.Y = -10f;
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        }

                        npc.ai[0] = head.ai[0] == -3 ? 1 : 0;
                        npc.netUpdate = true;
                    }
                    else //keep trying to grab
                    {
                        if (npc.Hitbox.Intersects(player.Hitbox))
                        {
                            Heal();

                            player.Center = npc.Center;
                            player.velocity.X = 0;
                            player.velocity.Y = -0.4f;

                            Movement(head.Center, 0.8f, 24f);

                            player.AddBuff(ModContent.BuffType<Buffs.Boss.Grabbed>(), 2);
                        }
                        else
                        {
                            Movement(player.Center, 2.4f, 48f);
                        }
                    }
                    break;

                case 3:
                    goto case 1;

                case 4: //enrage grab
                    if (npc.Hitbox.Intersects(player.Hitbox))
                    {
                        Heal();

                        player.Center = npc.Center;
                        player.velocity.X = 0;
                        player.velocity.Y = -0.4f;

                        Movement(head.Center, 0.8f, 24f);
                    }
                    else
                    {
                        Movement(player.Center, 2.4f, 48f);
                    }

                    if (++npc.localAI[0] > 120)
                    {
                        if (head.Hitbox.Intersects(player.Hitbox)) //throw aside
                        {
                            player.velocity.X = player.Center.X < head.Center.X ? -15f : 15f;
                            player.velocity.Y = -10f;
                            Main.PlaySound(SoundID.Roar, npc.Center, 0);
                        }

                        npc.life = 0;
                        npc.StrikeNPCNoInteraction(npc.lifeMax, 0f, 0);
                        npc.active = false;
                    }
                    break;

                default:
                    npc.ai[0] = 0;
                    goto case 0;
            }

            npc.direction = npc.spriteDirection = -(int)npc.ai[2];
            npc.rotation = npc.DirectionFrom(head.Center).ToRotation();
            if (npc.spriteDirection < 0)
                npc.rotation += (float)Math.PI;

            //dust tendrils connecting hands to base
            Vector2 dustHead = head.Center + head.DirectionTo(npc.Center) * 50;
            Vector2 headOffset = npc.Center - dustHead;
            for (int i = 0; i < headOffset.Length(); i+= 16)
            {
                if (Main.rand.Next(2) == 0)
                    continue;

                int d = Dust.NewDust(dustHead+ Vector2.Normalize(headOffset) * i, 0, 0, 54,
                    head.velocity.X * 0.4f, head.velocity.Y * 0.4f, 0, default(Color), 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
            Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 54, 0f, 0f, 0, default(Color), 2f)].noGravity = true;

            npc.dontTakeDamage = head.ai[0] < 0;
        }

        public override bool CheckDead()
        {
            if (!(npc.ai[1] > -1 && npc.ai[1] < Main.maxNPCs && Main.npc[(int)npc.ai[1]].active
                && Main.npc[(int)npc.ai[1]].type == ModContent.NPCType<SpiritChampion>()))
            {
                return true;
            }

            NPC head = Main.npc[(int)npc.ai[1]];
            if (head.ai[0] != -3)
            {
                npc.active = true;
                npc.life = 1;
                return false;
            }

            return true;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage /= 2;
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
                target.AddBuff(ModContent.BuffType<Infested>(), 360);
                target.AddBuff(ModContent.BuffType<ClippedWings>(), 180);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                Vector2 value4 = npc.oldPos[i];
                float num165 = npc.rotation; //npc.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}
