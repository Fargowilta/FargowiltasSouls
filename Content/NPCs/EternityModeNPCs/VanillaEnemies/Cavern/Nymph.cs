using FargowiltasSouls.Content.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using FargowiltasSouls.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern
{
    public class Nymph : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(NPCID.LostGirl, NPCID.Nymph);

        public int Counter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.lavaImmune = true;

            if (Main.hardMode)
            {
                npc.lifeMax *= 4;
                npc.damage *= 2;
                npc.defense *= 2;
            }
        }

        public override void OnFirstTick(NPC npc)
        {
            base.OnFirstTick(npc);

            npc.buffImmune[BuffID.Confused] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.Nymph)
            {
                npc.knockBackResist = 0f;

                EModeGlobalNPC.Aura(npc, 250, ModContent.BuffType<LovestruckBuff>(), true, DustID.PinkTorch);

                if (--Counter < 0)
                {
                    Counter = 300;

                    if (FargoSoulsUtil.HostCheck && npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 1000)
                    {
                        Vector2 spawnVel = npc.DirectionFrom(Main.player[npc.target].Center) * 10f;
                        for (int i = -3; i < 3; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromThis(),
                                  npc.Center, spawnVel.RotatedBy(Math.PI / 7 * i),
                                  ModContent.ProjectileType<FakeHeart2>(),
                                  20, 0f, Main.myPlayer, 30, 90 + 10 * i);
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            //target.AddBuff(ModContent.BuffType<LovestruckBuff>(), 240);

            npc.life += hurtInfo.Damage * 2;
            if (npc.life > npc.lifeMax)
                npc.life = npc.lifeMax;
            CombatText.NewText(npc.Hitbox, CombatText.HealLife, hurtInfo.Damage * 2);
            npc.netUpdate = true;
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByAnything(npc, player, ref modifiers);

            if (player.loveStruck)
            {
                modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
                {
                    Vector2 speed = Main.rand.NextFloat(1, 2) * Vector2.UnitX.RotatedByRandom(Math.PI * 2);
                    float ai1 = 30 + Main.rand.Next(30);
                    Projectile.NewProjectile(npc.GetSource_FromThis(), player.Center, speed, ModContent.ProjectileType<HostileHealingHeart>(), hitInfo.Damage, 0f, Main.myPlayer, npc.whoAmI, ai1);
                    hitInfo.Null();
                };
            }
        }

    }
}
