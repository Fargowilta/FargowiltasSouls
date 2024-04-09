using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse
{
    public class Mothron : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Mothron);

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.knockBackResist *= 0.1f;
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center + 100 * Vector2.UnitX, NPCID.MothronSpawn);
            FargoSoulsUtil.NewNPCEasy(npc.GetSource_FromAI(), npc.Center - 100 * Vector2.UnitX, NPCID.MothronSpawn);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            npc.defense = npc.defDefense;

            if (npc.ai[0] == 0f) //stupid idiot slow movement
            {
                npc.ai[1] += 10; //do something else faster
            }
            else if (npc.ai[0] == 1f) //getting back in range
            {
                npc.position += npc.velocity / 2f;
            }
            else if (npc.ai[0] == 2f) //just charging straight at you
            {
                npc.ai[1] += 1; //do something else faster
            }
            else if (npc.ai[0] == 3.1f && npc.ai[1] < 1) //just began dash at player
            {
                const int max = 5;
                for (int i = -max; i <= max; i++)
                {
                    if (i == 0)
                        continue;

                    int direction = Math.Sign(Main.player[npc.target].Center.X - npc.Center.X);

                    Vector2 targetPos = npc.Center;
                    targetPos.X += (500f / max * Math.Abs(i) - 250f) * -direction;
                    targetPos.Y += 800f / max * i;

                    const int zenithStartup = 100; //this should match MothronZenith
                    Vector2 vel = 2f * (targetPos - npc.Center) / zenithStartup;

                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ModContent.ProjectileType<MothronZenith>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, -1f, direction);
                }
            }
            else if ((npc.ai[0] == 3f || npc.ai[0] == 4f) && npc.ai[1] == 0f)
            {
                if (FargoSoulsUtil.HostCheck) //clear old zeniths
                {
                    int type = ModContent.ProjectileType<MothronZenith>();
                    for (int p = 0; p < Main.maxProjectiles; p++)
                    {
                        if (Main.projectile[p].active && Main.projectile[p].type == type && Main.projectile[p].ai[0] == npc.whoAmI)
                            Main.projectile[p].Kill();
                    }
                }

                const int max = 6;
                for (int i = -max; i <= max; i++) //ring zeniths
                {
                    if (i < 0 && npc.ai[0] == 3f) //dont do outer ring on dash
                        continue;

                    if (i == 0)
                        continue;

                    float rotation = MathHelper.TwoPi / max * i;

                    if (FargoSoulsUtil.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<MothronZenith>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer, npc.whoAmI, rotation);
                }
            }
            else if (npc.ai[0] == 4.1f || npc.ai[0] == 4.2f) //egg laying
            {
                npc.position += npc.velocity / 2f; //get to egg pos faster
                npc.defDefense *= 2;
            }

            //FargoSoulsUtil.PrintAI(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Rabies, 3600);
            target.FargoSouls().AddBuffNoStack(ModContent.BuffType<StunnedBuff>(), 60);
        }
    }
}
