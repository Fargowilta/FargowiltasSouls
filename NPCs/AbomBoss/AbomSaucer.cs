using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.AbomBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.AbomBoss
{
    public class AbomSaucer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mini Saucer");
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(NPC.type);
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
                    ModContent.BuffType<MutantNibble>(),
                    ModContent.BuffType<OceanicMaul>(),
                    ModContent.BuffType<LightningRod>(),
        }
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitScale = 1f
            });
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(
                   ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<AbomBoss>()],
                   quickUnlock: true
               );
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement($"Mods.FargowiltasSouls.Bestiary.{Name}")
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 25;
            NPC.height = 25;
            NPC.defense = 90;
            NPC.lifeMax = 600;
            NPC.scale = 2f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;

            NPC.dontTakeDamage = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax /** 0.5f*/ * bossLifeScale);
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            NPC abom = FargoSoulsUtil.NPCExists(NPC.ai[0], ModContent.NPCType<AbomBoss>());
            if (abom == null || abom.dontTakeDamage)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.checkDead();
                    NPC.active = false;
                }
                return;
            }
            NPC.target = abom.target;

            NPC.dontTakeDamage = abom.ai[0] == 0 && abom.ai[2] < 3;

            if (++NPC.ai[1] > 90) //pause before attacking
            {
                NPC.velocity = Vector2.Zero;

                if (NPC.ai[3] == 0) //store angle for attack
                {
                    NPC.localAI[2] = NPC.Distance(Main.player[NPC.target].Center);
                    NPC.ai[3] = NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation();

                    if (NPC.whoAmI == NPC.FindFirstNPC(NPC.type) && Main.netMode != NetmodeID.MultiplayerClient) //reticle telegraph
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), Main.player[NPC.target].Center, Vector2.Zero, ModContent.ProjectileType<AbomReticle>(), 0, 0f, Main.myPlayer);
                    }
                }

                if (NPC.ai[1] > 120) //attack and reset
                {
                    SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 speed = 16f * NPC.ai[3].ToRotationVector2().RotatedBy((Main.rand.NextDouble() - 0.5) * 0.785398185253143 / 12.0);
                            speed *= Main.rand.NextFloat(0.9f, 1.1f);
                            int p = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, ModContent.ProjectileType<AbomLaser>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].timeLeft = (int)(NPC.localAI[2] / speed.Length()) + 1;
                        }
                    }
                    NPC.netUpdate = true;
                    NPC.ai[1] = 0;
                    NPC.ai[3] = 0;
                }
            }
            else
            {
                Vector2 target = Main.player[NPC.target].Center; //targeting
                target += Vector2.UnitX.RotatedBy(NPC.ai[2]) * (NPC.ai[1] < 45 ? 200 : 500);

                Vector2 distance = target - NPC.Center;
                distance /= 8f;
                NPC.velocity = (NPC.velocity * 19f + distance) / 20f;
            }

            NPC.ai[2] -= 0.045f; //spin around target
            if (NPC.ai[2] < (float)-Math.PI)
                NPC.ai[2] += 2 * (float)Math.PI;

            if (NPC.localAI[1] == 0) //visuals
                NPC.localAI[1] = Main.rand.NextBool() ? 1 : -1;
            NPC.rotation = (float)Math.Sin(2 * Math.PI * NPC.localAI[0]++ / 90) * (float)Math.PI / 8f * NPC.localAI[1];
            if (NPC.localAI[0] > 180)
                NPC.localAI[0] = 0;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 87, 0f, 0f, 0, default(Color), 1f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 3f;
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 87, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 12f;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            //int num156 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * NPC.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}