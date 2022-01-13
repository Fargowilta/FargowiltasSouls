using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class BrainIllusionAttack : ModNPC
    {
        public override string Texture => "Terraria/NPC_266";

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Brain of Cthulhu");
            //DisplayName.AddTranslation(GameCulture.Chinese, "克苏鲁之脑");
            Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.BrainofCthulhu];
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 120;
            npc.height = 80;
            npc.scale += 0.25f;
            npc.damage = 30;
            npc.defense = 14;
            npc.lifeMax = 1;
            npc.HitSound = SoundID.NPCHit9;
            npc.DeathSound = SoundID.NPCDeath11;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.aiStyle = -1;

            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.Confused] = true;
            npc.buffImmune[BuffID.Suffocation] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Burning] = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.alpha == 0;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return npc.alpha == 0;
        }

        private const int attackDelay = 120;

        public override void AI()
        {
            if (npc.localAI[1] == 0)
            {
                npc.localAI[1] = 1;
                npc.localAI[2] = npc.Center.X;
                npc.localAI[3] = npc.Center.Y;
                Main.npcTexture[npc.type] = Main.npcTexture[NPCID.BrainofCthulhu];
            }

            NPC brain = FargoSoulsUtil.NPCExists(npc.ai[0], NPCID.BrainofCthulhu);
            if (brain == null)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                npc.HitEffect();
                npc.active = false;
                return;
            }

            npc.target = brain.target;
            npc.damage = npc.defDamage = (int)(brain.defDamage * 4.0 / 3.0);
            npc.defense = npc.defDefense = brain.defDefense;
            //npc.life = brain.life;
            //npc.lifeMax = brain.lifeMax;
            //npc.knockBackResist = brain.knockBackResist;

            npc.alpha = 0;
            npc.dontTakeDamage = true;
            if (++npc.localAI[0] < attackDelay)
            {
                npc.ai[1] = MathHelper.Lerp(npc.ai[1], 0, 0.03f);
                npc.alpha = Math.Min(255, (int)npc.ai[1] + 1);

                npc.position += 0.5f * (Main.player[npc.target].position - Main.player[npc.target].oldPosition) * (1f - npc.localAI[0] / attackDelay);

                float radius = 16f * npc.localAI[0] / attackDelay;
                npc.Center = new Vector2(npc.localAI[2], npc.localAI[3]) + Main.rand.NextVector2Circular(radius, radius);
            }
            else if (npc.localAI[0] == attackDelay)
            {
                npc.Center = new Vector2(npc.localAI[2], npc.localAI[3]);
                npc.velocity = 18f * npc.DirectionTo(Main.player[npc.target].Center);
                npc.ai[2] = Main.player[npc.target].Center.X;
                npc.ai[3] = Main.player[npc.target].Center.Y;
                npc.netUpdate = true;
            }
            else if (npc.localAI[0] > attackDelay + 180)// || npc.Distance(new Vector2(npc.ai[2], npc.ai[3])) < npc.velocity.Length() + 1f)
            {
                npc.StrikeNPCNoInteraction(9999, 0f, 0);
                npc.HitEffect();
                npc.active = false;
            }
            else //while dashing
            {
                npc.velocity *= 1.015f;
                npc.dontTakeDamage = false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC brain = FargoSoulsUtil.NPCExists(npc.ai[0], NPCID.BrainofCthulhu);
            if (brain != null)
            {
                npc.frame.Y = Main.npc[EModeGlobalNPC.brainBoss].frame.Y;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 120);
            target.AddBuff(BuffID.Darkness, 120);
            target.AddBuff(BuffID.Bleeding, 120);
            target.AddBuff(BuffID.Slow, 120);
            target.AddBuff(BuffID.Weak, 120);
            target.AddBuff(BuffID.BrokenArmor, 120);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                //Main.PlaySound(npc.DeathSound, npc.Center);
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 5);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 2f;
                }
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

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.npcTexture[npc.type];
            Rectangle rectangle = npc.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = npc.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            if (npc.alpha == 0)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    Color color27 = color26 * 0.5f;
                    color27 *= (float)(NPCID.Sets.TrailCacheLength[npc.type] - i) / NPCID.Sets.TrailCacheLength[npc.type];
                    Vector2 value4 = npc.oldPos[i];
                    float num165 = npc.rotation; //npc.oldRot[i];
                    Main.spriteBatch.Draw(texture2D13, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, npc.scale, effects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture2D13, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, npc.rotation, origin2, npc.scale, effects, 0f);
            return false;
        }
    }
}