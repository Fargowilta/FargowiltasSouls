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
            DisplayName.SetDefault("Champion of Terra");
            DisplayName.AddTranslation(GameCulture.Chinese, "泰拉英灵");
        }

        public override void SetDefaults()
        {
            npc.width = 80;
            npc.height = 80;
            npc.damage = 160;
            npc.defense = 80;
            npc.lifeMax = 690000;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;
            /*npc.value = Item.buyPrice(0, 10);

            npc.boss = true;
            music = MusicID.Boss1;
            musicPriority = MusicPriority.BossMedium;*/

            for (int i = 0; i < npc.buffImmune.Length; i++)
                npc.buffImmune[i] = true;
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().SpecialEnchantImmune = true;

            npc.behindTiles = true;
            npc.chaseable = false;

            npc.scale *= 1.25f;
            npc.trapImmune = true;
            npc.dontCountMe = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return npc.Distance(target.Center) < 30 * npc.scale;
        }

        public override void AI()
        {
            int ai1 = (int)npc.ai[1];
            if (!(ai1 > -1 && ai1 < Main.maxNPCs && Main.npc[ai1].active && Main.npc[ai1].ai[0] == npc.whoAmI
                && (Main.npc[ai1].type == ModContent.NPCType<TerraChampion>() || Main.npc[ai1].type == ModContent.NPCType<TerraChampionBody>()))
                || (FargoSoulsWorld.MasochistMode && Main.npc[ai1].life < Main.npc[ai1].lifeMax / 10))
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
            
            Vector2 offset = Main.npc[ai1].Center - npc.Center;
            npc.rotation = offset.ToRotation();
            float num1 = offset.Length();
            int num2 = (int)(44 * npc.scale);
            float num3 = (num1 - num2) / num1;
            float num4 = offset.X * num3;
            float num5 = offset.Y * num3;
            npc.velocity = Vector2.Zero;
            npc.position.X += num4;
            npc.position.Y += num5;

            npc.timeLeft = Main.npc[ai1].timeLeft;

            if (Main.npc[(int)npc.ai[3]].ai[1] == 11)
            {
                Vector2 pivot = Main.npc[(int)npc.ai[3]].Center;
                pivot += Vector2.Normalize(Main.npc[(int)npc.ai[3]].velocity.RotatedBy(Math.PI / 2)) * 600;
                if (npc.Distance(pivot) < 600) //make sure body doesnt coil into the circling zone
                    npc.Center = pivot + npc.DirectionFrom(pivot) * 600;
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            /*damage *= 0.01;
            return true;*/

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
            if (FargoSoulsWorld.MasochistMode)
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
