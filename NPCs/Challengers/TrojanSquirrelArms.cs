using FargowiltasSouls.Projectiles.Challengers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.NPCs.Challengers
{
    public class TrojanSquirrelArms : TrojanSquirrelLimb
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.lifeMax = 450;

            NPC.width = 114;
            NPC.height = 64;
        }

        public override void AI()
        {
            base.AI();

            if (body == null)
                return;

            NPC.velocity = Vector2.Zero;
            NPC.target = body.target;
            NPC.direction = NPC.spriteDirection = body.direction;
            NPC.Center = body.Bottom + new Vector2(18f * NPC.direction, -105f) * body.scale;

            switch ((int)NPC.ai[0])
            {
                case 0:
                    if (body.ai[0] == 0 && body.localAI[0] <= 0)
                    {
                        NPC.ai[1] += FargoSoulsWorld.EternityMode ? 1.5f : 1f;

                        if (body.dontTakeDamage)
                            NPC.ai[1] += 1f;

                        int threshold = 360;

                        //structured like this so body gets priority first
                        int stallPoint = threshold - 30;
                        if (NPC.ai[1] > stallPoint)
                        {
                            TrojanSquirrel squirrel = body.ModNPC as TrojanSquirrel;
                            if (squirrel.head != null && squirrel.head.ai[0] != 0f) //wait if other part is attacking
                                NPC.ai[1] = stallPoint;
                        }

                        if (NPC.ai[1] > threshold && Math.Abs(body.velocity.Y) < 0.05f)
                        {
                            //dont attack unless player is in 90 degree cone in front of squrrl
                            float baseAngle = NPC.direction > 0 ? 0f : MathHelper.Pi;
                            if (Math.Abs(MathHelper.WrapAngle(NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation() - baseAngle)) > MathHelper.PiOver4)
                            {
                                NPC.ai[1] = stallPoint;
                            }
                            else
                            {
                                NPC.ai[0] = 1 + NPC.ai[2];
                                NPC.ai[1] = 0;
                                if (Main.expertMode)
                                    NPC.ai[2] = NPC.ai[2] == 0 ? 1 : 0;
                                NPC.netUpdate = true;

                                body.localAI[3] = Math.Sign(body.DirectionTo(Main.player[body.target].Center).X);
                                body.netUpdate = true;
                            }
                        }
                    }
                    break;

                case 1: //chains
                    {
                        int start = 90;
                        if (FargoSoulsWorld.EternityMode)
                            start -= 30;
                        if (FargoSoulsWorld.MasochistModeReal)
                            start -= 30;
                        int end = 310;

                        int teabagInterval = start / (FargoSoulsWorld.MasochistModeReal ? 3 : 2);

                        if (NPC.ai[1] < start) //better for animation
                        {
                            body.velocity.X *= 0.9f;
                            if (NPC.ai[1] % teabagInterval == 0)
                                SoundEngine.PlaySound(SoundID.NPCHit41, NPC.Center);
                        }

                        NPC.ai[1]++;

                        //to help animate body
                        NPC.ai[3] = NPC.ai[1] < start && NPC.ai[1] % teabagInterval < teabagInterval / 2 ? 1 : 0;

                        if (NPC.ai[1] > start && NPC.ai[1] < end && NPC.ai[1] % (body.dontTakeDamage || FargoSoulsWorld.MasochistModeReal ? 40 : 70) == 0)
                        {
                            Vector2 pos = GetShootPos();

                            float baseAngle = NPC.direction > 0 ? 0f : MathHelper.Pi;
                            float angle = NPC.DirectionTo(Main.player[NPC.target].Center).ToRotation();
                            if (Math.Abs(MathHelper.WrapAngle(angle - baseAngle)) > MathHelper.PiOver2)
                                angle = MathHelper.PiOver2 * Math.Sign(angle);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, 8f * angle.ToRotationVector2(), ModContent.ProjectileType<TrojanHook>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer);
                        }

                        if (NPC.ai[1] > 300 && Main.netMode != NetmodeID.MultiplayerClient && Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<TrojanHook>()] <= 0)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;

                            body.localAI[3] = 0;
                            body.netUpdate = true;
                        }
                    }
                    break;

                case 2: //snowballs
                    {
                        NPC.ai[1]++;

                        int start = 90;
                        int end = 300;
                        if (FargoSoulsWorld.EternityMode)
                        {
                            start -= 30;
                            end -= 30;
                        }
                        if (FargoSoulsWorld.MasochistModeReal)
                            end -= 60;

                        body.velocity.X *= 0.98f;

                        if (NPC.ai[1] == 10)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 pos = GetShootPos();
                                SoundEngine.PlaySound(SoundID.Item11, pos);
                                for (int j = 0; j < 20; j++)
                                {
                                    int d = Dust.NewDust(pos, 0, 0, DustID.SnowBlock, Scale: 3f);
                                    Main.dust[d].noGravity = true;
                                    Main.dust[d].velocity *= 4f;
                                    Main.dust[d].velocity.X += NPC.direction * Main.rand.NextFloat(6f, 24f);
                                }
                            }
                        }

                        if (NPC.ai[1] > start && NPC.ai[1] % 4 == 0)
                        {
                            Vector2 pos = GetShootPos();

                            SoundEngine.PlaySound(SoundID.Item11, pos);

                            float ratio = (NPC.ai[1] - start) / (end - start);

                            Vector2 target = NPC.Center;
                            target.X += Math.Sign(NPC.direction) * (FargoSoulsWorld.EternityMode ? 1800f : 1200f) * ratio; //gradually targets further and further
                            //target.Y -= 8 * 16;
                            target += Main.rand.NextVector2Circular(16, 16);
                            const float gravity = 0.5f;
                            float time = 45f;
                            Vector2 distance = target - pos;
                            distance.X = distance.X / time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, distance, ModContent.ProjectileType<TrojanSnowball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.damage), 0f, Main.myPlayer, gravity);
                        }

                        if (NPC.ai[1] > end)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;

                            body.localAI[3] = 0;
                            body.netUpdate = true;
                        }
                    }
                    break;
            }
        }

        private Vector2 GetShootPos()
        {
            NPC.localAI[0] = NPC.localAI[0] == 0 ? 1 : 0;

            Vector2 pos = NPC.Bottom;
            pos.X += NPC.width / 2f * NPC.direction;
            pos.Y -= 16 * NPC.scale;

            pos.X -= (NPC.localAI[0] == 0 ? 10 : 48) * NPC.direction * NPC.scale;

            return pos;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 8; i <= 10; i++)
                {
                    Vector2 pos = Main.rand.NextVector2FromRectangle(NPC.Hitbox);
                    if (!Main.dedServ)
                        Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TrojanSquirrelGore{i}").Type, NPC.scale);
                }
            }
        }
    }
}
