using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class GelatinSubject : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_660";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gelatin Subject");
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.QueenSlimeMinionPurple];
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = NPCID.Sets.DebuffImmunitySets[NPCID.QueenSlimeBoss].SpecificallyImmuneTo
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            });
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.QueenSlimeMinionPurple);
            AIType = NPCID.QueenSlimeMinionPurple;

            //because they will double dip on expert/master scaling otherwise
            NPC.lifeMax = 120;
            NPC.damage = 50;

            NPC.lifeMax *= 15;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.scale *= 1.5f;
            NPC.width = NPC.height = (int)(NPC.height * 0.9);
            if (FargoSoulsWorld.MasochistModeReal)
                NPC.knockBackResist *= 0.1f;
        }

        public override void AI()
        {
            if (!FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss)
                && !NPC.AnyNPCs(NPCID.QueenSlimeBoss))
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                return;
            }

            const float IdleAccel = 0.025f;
            foreach (NPC n in Main.npc.Where(n => n.active && n.type == NPC.type && n.whoAmI != NPC.whoAmI && NPC.Distance(n.Center) < NPC.width))
            {
                NPC.velocity.X += IdleAccel * (NPC.Center.X < n.Center.X ? -1 : 1);
                NPC.velocity.Y += IdleAccel * (NPC.Center.Y < n.Center.Y ? -1 : 1);
                n.velocity.X += IdleAccel * (n.Center.X < NPC.Center.X ? -1 : 1);
                n.velocity.Y += IdleAccel * (n.Center.Y < NPC.Center.Y ? -1 : 1);
            }

            //if (NPC.HasValidTarget && NPC.Distance(Main.player[NPC.target].Center) > 300)
            //    NPC.velocity += NPC.DirectionTo(Main.player[NPC.target].Center) * 0.05f;

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = Math.Abs(NPC.velocity.X * .1f) * NPC.direction;

            const int cooldown = 90;

            //move slower during rain attack
            if (NPC.Distance(Main.player[NPC.target].Center) < 600 &&
                (Main.npc[EModeGlobalNPC.queenSlimeBoss].GetEModeNPCMod<QueenSlime>().RainTimer > 0
                || NPC.AnyNPCs(ModContent.NPCType<GelatinSlime>())))
            {
                NPC.localAI[0] = cooldown;
            }

            if (NPC.Distance(Main.npc[EModeGlobalNPC.queenSlimeBoss].Center) > 2000)
                NPC.Center = Main.npc[EModeGlobalNPC.queenSlimeBoss].Center;

            if (NPC.localAI[0] > 0)
            {
                NPC.localAI[0]--;
            }

            //if moving towards you, slow down
            if (NPC.HasValidTarget && Math.Abs(MathHelper.WrapAngle(NPC.velocity.ToRotation() - NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation())) < MathHelper.PiOver2)
            {
                if (NPC.Distance(Main.player[NPC.target].Center) < 16 * 5)
                {
                    NPC.position -= NPC.velocity * 0.33f;
                }
                else if (NPC.localAI[0] > 0)
                {
                    float ratio = NPC.localAI[0] / cooldown;
                    NPC.position -= NPC.velocity * 0.66f * ratio;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            if (NPC.DeathSound != null)
                SoundEngine.PlaySound(NPC.DeathSound.Value, NPC.Center);

            NPC.active = false;

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                //SoundEngine.PlaySound(NPC.DeathSound, NPC.Center);
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 5);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 0.75f;
                }

                for (int i = 0; i < 2; i++)
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity / 2, 1260, NPC.scale);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y += frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= Main.npcFrameCount[NPC.type] * frameHeight)
                NPC.frame.Y = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!Terraria.GameContent.TextureAssets.Npc[NPCID.QueenSlimeMinionPurple].IsLoaded)
                return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.QueenSlimeMinionPurple].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            GameShaders.Misc["HallowBoss"].Apply(new Terraria.DataStructures.DrawData?());

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}