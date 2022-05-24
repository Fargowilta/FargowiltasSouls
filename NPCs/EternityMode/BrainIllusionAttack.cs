using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class BrainIllusionAttack : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_266";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brain of Cthulhu");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "克苏鲁之脑");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BrainofCthulhu];
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 80;
            NPC.scale += 0.25f;
            NPC.damage = 30;
            NPC.defense = 14;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.aiStyle = -1;

            NPC.buffImmune[BuffID.OnFire] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.buffImmune[BuffID.Suffocation] = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.Burning] = true;
        }

        public override bool CanHitPlayer(Player target, ref int CooldownSlot)
        {
            return NPC.alpha == 0;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return NPC.alpha == 0;
        }

        private const int attackDelay = 120;

        public override void AI()
        {
            if (NPC.localAI[1] == 0)
            {
                NPC.localAI[1] = 1;
                NPC.localAI[2] = NPC.Center.X;
                NPC.localAI[3] = NPC.Center.Y;
            }

            NPC brain = FargoSoulsUtil.NPCExists(NPC.ai[0], NPCID.BrainofCthulhu);
            if (brain == null)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                NPC.HitEffect();
                NPC.active = false;
                return;
            }

            NPC.target = brain.target;
            NPC.damage = NPC.defDamage = (int)(brain.defDamage * 4.0 / 3.0);
            NPC.defense = NPC.defDefense = brain.defDefense;
            //NPC.life = brain.life;
            //NPC.lifeMax = brain.lifeMax;
            //NPC.knockBackResist = brain.knockBackResist;

            NPC.alpha = 0;
            NPC.dontTakeDamage = true;
            if (++NPC.localAI[0] < attackDelay)
            {
                NPC.ai[1] = MathHelper.Lerp(NPC.ai[1], 0, 0.03f);
                NPC.alpha = Math.Min(255, (int)NPC.ai[1] + 1);

                NPC.position += 0.5f * (Main.player[NPC.target].position - Main.player[NPC.target].oldPosition) * (1f - NPC.localAI[0] / attackDelay);

                float radius = 16f * NPC.localAI[0] / attackDelay;
                NPC.Center = new Vector2(NPC.localAI[2], NPC.localAI[3]) + Main.rand.NextVector2Circular(radius, radius);
            }
            else if (NPC.localAI[0] == attackDelay)
            {
                NPC.Center = new Vector2(NPC.localAI[2], NPC.localAI[3]);
                NPC.velocity = 18f * NPC.DirectionTo(Main.player[NPC.target].Center);
                NPC.ai[2] = Main.player[NPC.target].Center.X;
                NPC.ai[3] = Main.player[NPC.target].Center.Y;
                NPC.netUpdate = true;
            }
            else if (NPC.localAI[0] > attackDelay + 180)// || NPC.Distance(new Vector2(NPC.ai[2], NPC.ai[3])) < NPC.velocity.Length() + 1f)
            {
                NPC.StrikeNPCNoInteraction(9999, 0f, 0);
                NPC.HitEffect();
                NPC.active = false;
            }
            else //while dashing
            {
                NPC.velocity *= 1.015f;
                NPC.dontTakeDamage = false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC brain = FargoSoulsUtil.NPCExists(NPC.ai[0], NPCID.BrainofCthulhu);
            if (brain != null)
            {
                NPC.frame.Y = Main.npc[EModeGlobalNPC.brainBoss].frame.Y;
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
            if (NPC.life <= 0)
            {
                //SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                for (int i = 0; i < 30; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 5);
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

        public override bool CheckDead()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!Terraria.GameContent.TextureAssets.Npc[NPCID.BrainofCthulhu].IsLoaded)
                return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.BrainofCthulhu].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            if (NPC.alpha == 0)
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Color color27 = color26 * 0.5f;
                    color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                    Vector2 value4 = NPC.oldPos[i];
                    float num165 = NPC.rotation; //NPC.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - Main.screenPosition + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
                }
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);
            return false;
        }
    }
}