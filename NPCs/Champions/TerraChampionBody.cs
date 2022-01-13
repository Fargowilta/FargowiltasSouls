using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.Champions;

namespace FargowiltasSouls.NPCs.Champions
{
    public class TerraChampionBody : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Champion of Terra");
            //DisplayName.AddTranslation(GameCulture.Chinese, "泰拉英灵");

            NPCID.Sets.TrailCacheLength[npc.type] = 5;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 45;
            npc.height = 45;
            npc.damage = 140;
            npc.defense = 80;
            npc.lifeMax = 170000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;

            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;

            npc.behindTiles = true;
            npc.chaseable = false;

            npc.scale *= 1.25f;
            npc.trapImmune = true;
            npc.dontCountMe = true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            //npc.damage = (int)(npc.damage * 0.5f);
            npc.lifeMax = (int)(npc.lifeMax * Math.Sqrt(bossLifeScale));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.Distance(target.Center) < 30 * npc.scale;
        }

        public override void AI()
        {
            NPC segment = FargoSoulsUtil.NPCExists(npc.ai[1], ModContent.NPCType<TerraChampion>(), ModContent.NPCType<TerraChampionBody>());
            NPC head = FargoSoulsUtil.NPCExists(npc.ai[3], ModContent.NPCType<TerraChampion>());
            if (segment == null || head == null || (FargoSoulsWorld.EternityMode && segment.life < segment.lifeMax / 10))
            {
                Main.PlaySound(SoundID.Item, npc.Center, 14);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        int dust = Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[dust].velocity *= 1.4f;
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 7f;
                        dust = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                        Main.dust[dust].velocity *= 3f;
                    }

                    float scaleFactor9 = 0.5f;
                    for (int j = 0; j < 4; j++)
                    {
                        int gore = Gore.NewGore(npc.Center, default(Vector2), Main.rand.Next(61, 64));
                        Main.gore[gore].velocity *= scaleFactor9;
                        Main.gore[gore].velocity.X += 1f;
                        Main.gore[gore].velocity.Y += 1f;
                    }

                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<TerraLightningOrb>(), npc.damage / 4, 0f, Main.myPlayer, npc.ai[3]);

                    npc.active = false;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
                }
                return;
            }

            npc.velocity = Vector2.Zero;

            int pastPos = NPCID.Sets.TrailCacheLength[npc.type] - (int)head.ai[3] - 1; //ai3 check is to trace better and coil tightly

            if (npc.localAI[0] == 0)
            {
                npc.localAI[0] = 1;
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                    npc.oldPos[i] = npc.position;
            }

            npc.Center = segment.oldPos[pastPos] + segment.Size / 2;
            npc.rotation = npc.DirectionTo(segment.Center).ToRotation();
            if (npc.Distance(npc.oldPos[pastPos - 1] + npc.Size / 2) > 45 * npc.scale)
            {
                npc.oldPos[pastPos - 1] = npc.position + Vector2.Normalize(npc.oldPos[pastPos - 1] - npc.position) * 45 * npc.scale;
            }

            npc.timeLeft = segment.timeLeft;

            /*if (head.ai[1] == 11)
            {
                Vector2 pivot = head.Center;
                pivot += Vector2.Normalize(head.velocity.RotatedBy(Math.PI / 2)) * 600;
                if (npc.Distance(pivot) < 600) //make sure body doesnt coil into the circling zone
                    npc.Center = pivot + npc.DirectionFrom(pivot) * 600;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            //int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * npc.frame.Y; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = npc.frame;//new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), npc.GetAlpha(lightColor), npc.rotation, origin2, npc.scale, effects, 0f);
            Texture2D glowmask = npc.type == ModContent.NPCType<TerraChampionBody>() ? ModContent.GetTexture("FargowiltasSouls/NPCs/Champions/TerraChampionBody_Glow") : ModContent.GetTexture("FargowiltasSouls/NPCs/Champions/TerraChampionTail_Glow");
            Main.spriteBatch.Draw(glowmask, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}
