using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.TrojanSquirrel
{
    public class TrojanSquirrelHead : TrojanSquirrelLimb
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.lifeMax = 600;

            NPC.width = baseWidth = 80;
            NPC.height = baseHeight = 76;
        }

        public override void AI()
        {
            base.AI();

            if (body == null)
                return;

            NPC.velocity = Vector2.Zero;
            NPC.target = body.target;
            NPC.direction = NPC.spriteDirection = body.direction;
            NPC.Center = body.Bottom + new Vector2(42f * NPC.direction, -153f) * body.scale;

            switch ((int)NPC.ai[0])
            {
                case 0:
                    if (body.ai[0] == 0 && body.localAI[0] <= 0)
                    {
                        NPC.ai[1] += WorldSavingSystem.EternityMode ? 1.5f : 1f;

                        if (body.dontTakeDamage)
                            NPC.ai[1] += 1f;

                        int threshold = 240;

                        //structured like this so body gets priority first
                        int stallPoint = threshold - 30;
                        if (NPC.ai[1] > stallPoint)
                        {
                            TrojanSquirrel squirrel = body.As<TrojanSquirrel>();
                            if (squirrel.arms != null && squirrel.arms.ai[0] != 0f) //wait if other part is attacking
                                NPC.ai[1] = stallPoint;
                        }

                        if (NPC.ai[1] > threshold && Math.Abs(body.velocity.Y) < 0.05f)
                        {
                            NPC.ai[0] = 1 + NPC.ai[2];
                            NPC.ai[1] = 0;
                            if (Main.expertMode)
                                NPC.ai[2] = NPC.ai[2] == 0 ? 1 : 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case 1: //acorn spray
                    if (NPC.ai[1] == 0 && !WorldSavingSystem.MasochistModeReal)
                    {
                        //telegraph
                        SoundEngine.PlaySound(SoundID.Item11, NPC.Center);

                        Vector2 pos = NPC.Center;
                        pos.X += 22 * NPC.direction; //FUCKING LAUGH
                        pos.Y += 22;

                        for (int j = 0; j < 20; j++)
                        {
                            int d = Dust.NewDust(pos, 0, 0, DustID.GrassBlades, Scale: 3f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity *= 4f;
                            Main.dust[d].velocity.X += NPC.direction * Main.rand.NextFloat(6f, 18f);
                        }
                    }

                    if (++NPC.ai[1] % (body.dontTakeDamage || WorldSavingSystem.MasochistModeReal ? 30 : 45) == 0)
                    {
                        bool doAttack = true;
                        if (!WorldSavingSystem.MasochistModeReal && NPC.localAI[1] == 0)
                        {
                            NPC.localAI[1] = 1;
                            doAttack = false; //skip the first normally
                        }

                        if (doAttack)
                        {
                            Vector2 pos = NPC.Center;
                            pos.X += 22 * NPC.direction; //FUCKING LAUGH
                            pos.Y += 22;

                            const float gravity = 0.2f;
                            float time = 80f;
                            if (body.dontTakeDamage)
                                time = 60f;
                            if (WorldSavingSystem.MasochistModeReal)
                                time = 45f;
                            Vector2 distance = Main.player[NPC.target].Center - pos;// + player.velocity * 30f;
                            distance.X /= time;
                            distance.Y = distance.Y / time - 0.5f * gravity * time;
                            for (int i = 0; i < 10; i++)
                            {
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, distance + Main.rand.NextVector2Square(-0.5f, 0.5f),
                                        ModContent.ProjectileType<TrojanAcorn>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    if (NPC.ai[1] > 210)
                    {
                        NPC.ai[0] = 0;
                        NPC.ai[1] = 0;
                        NPC.netUpdate = true;
                    }
                    break;

                case 2: //squirrel barrage
                    {
                        if (WorldSavingSystem.MasochistModeReal && NPC.ai[1] == 90)
                        {
                            NPC arms = (body.ModNPC as TrojanSquirrel).arms;
                            if (arms != null && arms.ai[0] != 2)
                            {
                                arms.ai[0] = 2;
                                arms.ai[1] = 0;
                                arms.netUpdate = true;
                            }
                        }

                        NPC.ai[1]++;

                        int start = 60 + 30;
                        int end = 240 + 30;
                        if (WorldSavingSystem.MasochistModeReal)
                        {
                            start -= 30 - 30;
                            end -= 90 - 30;
                        }

                        body.velocity.X *= 0.99f;

                        if (NPC.ai[1] % 4 == 0)
                        {
                            ShootSquirrelAt(body.Center + Main.rand.NextVector2Circular(200, 200));

                            if (NPC.ai[1] > start)
                            {
                                float ratio = (NPC.ai[1] - start) / (end - start);
                                Vector2 target = new(NPC.Center.X, Main.player[NPC.target].Center.Y);
                                target.X += Math.Sign(NPC.direction) * (550f + (WorldSavingSystem.EternityMode ? 1800f : 1200f) * (1f - ratio));

                                ShootSquirrelAt(target);
                            }
                        }

                        if (NPC.ai[1] > end)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
            }
        }

        private void ShootSquirrelAt(Vector2 target)
        {
            float gravity = 0.6f;
            const float origTime = 75;
            float time = origTime;
            if (body.dontTakeDamage)
                time -= 15;
            if (WorldSavingSystem.MasochistModeReal)
                time -= 15;

            gravity *= origTime / time;

            Vector2 distance = target - NPC.Center;// + player.velocity * 30f;
            distance.X += Main.rand.NextFloat(-128, 128);
            distance.X /= time;
            distance.Y = distance.Y / time - 0.5f * gravity * time;

            distance.X += Math.Min(4f, Math.Abs(NPC.velocity.X)) * Math.Sign(NPC.velocity.X);

            SoundEngine.PlaySound(SoundID.Item1, NPC.Center);

            if (FargoSoulsUtil.HostCheck)
            {
                float ai1 = time + Main.rand.Next(-10, 11) - 1;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, distance,
                    ModContent.ProjectileType<TrojanSquirrelProj>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0f, Main.myPlayer, gravity, ai1);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Vector2 pos = NPC.Center;
                if (!Main.dedServ)
                    Gore.NewGore(NPC.GetSource_FromThis(), pos, NPC.velocity, ModContent.Find<ModGore>(Mod.Name, $"TrojanSquirrelGore1").Type, NPC.scale);
            }
        }
    }
}
