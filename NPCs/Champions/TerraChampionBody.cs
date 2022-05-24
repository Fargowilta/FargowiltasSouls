using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.Champions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Champions
{
    public class TerraChampionBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Champion of Terra");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "泰拉英灵");

            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.NoMultiplayerSmoothingByType[NPC.type] = true;

            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true
            });

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 45;
            NPC.height = 45;
            NPC.damage = 140;
            NPC.defense = 80;
            NPC.lifeMax = 170000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;

            NPC.behindTiles = true;
            NPC.chaseable = false;

            NPC.scale *= 1.25f;
            NPC.trapImmune = true;
            NPC.dontCountMe = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //NPC.damage = (int)(NPC.damage * 0.5f);
            NPC.lifeMax = (int)(NPC.lifeMax * Math.Sqrt(bossLifeScale));
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            CooldownSlot = 1;
            return NPC.Distance(target.Center) < 30 * NPC.scale;
        }

        public override void AI()
        {
            NPC segment = FargoSoulsUtil.NPCExists(NPC.ai[1], ModContent.NPCType<TerraChampion>(), ModContent.NPCType<TerraChampionBody>());
            NPC head = FargoSoulsUtil.NPCExists(NPC.ai[3], ModContent.NPCType<TerraChampion>());
            if (segment == null || head == null || (FargoSoulsWorld.EternityMode && segment.life < segment.lifeMax / 10))
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, 31, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[dust].velocity *= 1.4f;
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 7f;
                        dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                        Main.dust[dust].velocity *= 3f;
                    }

                    float scaleFactor9 = 0.5f;
                    for (int j = 0; j < 4; j++)
                    {
                        int gore = Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[gore].velocity *= scaleFactor9;
                        Main.gore[gore].velocity.X += 1f;
                        Main.gore[gore].velocity.Y += 1f;
                    }

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TerraLightningOrb>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, NPC.ai[3]);

                    NPC.active = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                }
                return;
            }

            NPC.velocity = Vector2.Zero;

            int pastPos = NPCID.Sets.TrailCacheLength[NPC.type] - (int)head.ai[3] - 1; //ai3 check is to trace better and coil tightly

            if (NPC.localAI[0] == 0)
            {
                NPC.localAI[0] = 1;
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    NPC.oldPos[i] = NPC.position;
            }

            NPC.Center = segment.oldPos[pastPos] + segment.Size / 2;
            NPC.rotation = NPC.DirectionTo(segment.Center).ToRotation();
            if (NPC.Distance(NPC.oldPos[pastPos - 1] + NPC.Size / 2) > 45 * NPC.scale)
            {
                NPC.oldPos[pastPos - 1] = NPC.position + Vector2.Normalize(NPC.oldPos[pastPos - 1] - NPC.position) * 45 * NPC.scale;
            }

            NPC.timeLeft = segment.timeLeft;

            /*if (head.ai[1] == 11)
            {
                Vector2 pivot = head.Center;
                pivot += Vector2.Normalize(head.velocity.RotatedBy(Math.PI / 2)) * 600;
                if (NPC.Distance(pivot) < 600) //make sure body doesnt coil into the circling zone
                    NPC.Center = pivot + NPC.DirectionFrom(pivot) * 600;
            }*/
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage = 1;
            crit = false;
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<LivingWasteland>(), 600);
                target.AddBuff(ModContent.BuffType<LightningRod>(), 600);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Rectangle rectangle = NPC.frame;//new Rectangle(0, y3, texture2D13.Width, turnSpeed6);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), NPC.GetAlpha(drawColor), NPC.rotation, origin2, NPC.scale, effects, 0);
            Texture2D glowmask = FargowiltasSouls.Instance.Assets.Request<Texture2D>($"NPCs/Champions/{Name}_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(glowmask, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}
