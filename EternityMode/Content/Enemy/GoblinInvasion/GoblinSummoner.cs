using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.GoblinInvasion
{
    public class GoblinSummoner : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.GoblinSummoner);

        public int AttackTimer;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            //EModeGlobalNPC.Aura(npc, 200, ModContent.BuffType<Shadowflame>(), false, DustID.Shadowflame);
            if (++AttackTimer > 180)
            {
                AttackTimer = 0;
                SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    Vector2 spawnPos = npc.Center + new Vector2(200f, 0f).RotatedBy(Math.PI / 2 * (i + 0.5));
                    //    //Vector2 speed = Vector2.Normalize(Main.player[npc.target].Center - spawnPos) * 10f;
                    //    int n = NPC.NewNPC(npc.GetSpawnSourceForProjectileNPC(), (int)spawnPos.X, (int)spawnPos.Y, NPCID.ChaosBall);
                    //    if (n != Main.maxNPCs && Main.netMode == NetmodeID.Server)
                    //        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                    //    for (int j = 0; j < 20; j++)
                    //    {
                    //        int d = Dust.NewDust(spawnPos, 0, 0, DustID.Shadowflame);
                    //        Main.dust[d].noGravity = true;
                    //        Main.dust[d].scale += 0.5f;
                    //        Main.dust[d].velocity *= 6f;
                    //    }
                    //}

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 spawnPos = Main.player[npc.target].Center + 180f * Vector2.UnitX.RotatedBy(MathHelper.TwoPi / 3 * i).RotatedByRandom(MathHelper.TwoPi / 3 / 2 * 0.75f);
                        float ai0 = Main.player[npc.target].DirectionFrom(spawnPos).ToRotation();
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<ShadowflamePortal>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, ai0);
                    }
                }
            }
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

            if (Main.rand.NextBool(3) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, new Vector2(Main.rand.NextFloat(-2f, 2f), -5), ModContent.ProjectileType<GoblinSpikyBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);

                //Vector2 vel = new Vector2(9f, 0f).RotatedByRandom(2 * Math.PI);
                //for (int i = 0; i < 6; i++)
                //{
                //    Vector2 speed = vel.RotatedBy(2 * Math.PI / 6 * (i + Main.rand.NextDouble() - 0.5));
                //    float ai1 = Main.rand.Next(10, 80) * (1f / 1000f);
                //    if (Main.rand.NextBool())
                //        ai1 *= -1f;
                //    float ai0 = Main.rand.Next(10, 80) * (1f / 1000f);
                //    if (Main.rand.NextBool())
                //        ai0 *= -1f;
                //    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, speed, ModContent.ProjectileType<ShadowflameTentacleHostile>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, ai0, ai1);
                //}
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<WretchedPouch>(), 5));
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 50; i++)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, new Vector2(Main.rand.Next(-500, 501) / 100f, Main.rand.Next(-1000, 1) / 100f),
                        ModContent.ProjectileType<GoblinSpikyBall>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage, 4f / 8), 0, Main.myPlayer);
                }
            }
        }
    }
}
