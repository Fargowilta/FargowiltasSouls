using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.EternityMode
{
    public class GelatinSlime : ModNPC
    {
        public override string Texture => "Terraria/Images/NPC_658";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Slime");
            Main.npcFrameCount[NPC.type] = 2;
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
            NPC.CloneDefaults(NPCID.QueenSlimeMinionBlue);

            //because they will double dip on expert/master scaling otherwise
            NPC.lifeMax = 40;
            NPC.damage = 30;

            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.noTileCollide = true;

            NPC.scale *= 1.5f;
            NPC.lifeMax *= 3;
        }

        public override bool? CanHitNPC(NPC target) => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void AI()
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss))
            {
                if (Main.npc[EModeGlobalNPC.queenSlimeBoss].GetEModeNPCMod<QueenSlime>().RainTimer > -90)
                    Main.npc[EModeGlobalNPC.queenSlimeBoss].GetEModeNPCMod<QueenSlime>().RainTimer = -90;

                if (!FargoSoulsWorld.MasochistModeReal)
                {
                    if (Main.npc[EModeGlobalNPC.queenSlimeBoss].GetEModeNPCMod<QueenSlime>().StompTimer > -30)
                        Main.npc[EModeGlobalNPC.queenSlimeBoss].GetEModeNPCMod<QueenSlime>().StompTimer = -30;
                }
            }

            if (--NPC.ai[0] > 0)
            {
                NPC.position.X += NPC.ai[2];
                NPC.position.Y += NPC.ai[3];

                NPC.ai[3] += NPC.ai[1];

                NPC.velocity = Vector2.Zero;
            }
            else
            {
                NPC.noTileCollide = false;

                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    NPC.position.Y -= 16;

                if (NPC.ai[0] < -210)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float maxSpeed =
                            FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.queenSlimeBoss, NPCID.QueenSlimeBoss)
                            && Main.npc[EModeGlobalNPC.queenSlimeBoss].life < Main.npc[EModeGlobalNPC.queenSlimeBoss].lifeMax / 2
                            ? 12 : 8;

                        for (int i = 0; i < 20; i++)
                        {
                            Projectile.NewProjectile(
                                NPC.GetSource_FromThis(),
                                NPC.Center,
                                new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-maxSpeed, -4)),
                                ProjectileID.QueenSlimeMinionBlueSpike,
                                FargoSoulsUtil.ScaledProjectileDamage(NPC.damage, 1.5f),
                                0f,
                                Main.myPlayer);
                        }
                    }

                    NPC.life = 0;
                    NPC.HitEffect();
                    NPC.checkDead();
                }
            }

            if (NPC.ai[0] == 0)
                NPC.velocity.Y = 12f;
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
                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueTorch);
                    Main.dust[d].velocity *= 3f;
                    Main.dust[d].scale += 0.75f;
                }

                //for (int i = 0; i < 2 ; i++)
                //    if (!Main.dedServ)
                //            Gore.NewGore(NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity / 2, 1260, NPC.scale);
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
            if (!Terraria.GameContent.TextureAssets.Npc[NPCID.QueenSlimeMinionBlue].IsLoaded)
                return false;

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Npc[NPCID.QueenSlimeMinionBlue].Value;
            Rectangle rectangle = NPC.frame;
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = drawColor;
            color26 = NPC.GetAlpha(color26);

            SpriteEffects effects = NPC.spriteDirection < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;


            //spriteBatch.End(); spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            //GameShaders.Misc["HallowBoss"].Apply(new Terraria.DataStructures.DrawData?());

            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Color color27 = color26 * 0.5f;
                color27 *= (float)(NPCID.Sets.TrailCacheLength[NPC.type] - i) / NPCID.Sets.TrailCacheLength[NPC.type];
                Vector2 value4 = NPC.oldPos[i];
                float num165 = NPC.rotation; //NPC.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, NPC.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, NPC.rotation, origin2, NPC.scale, effects, 0);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}