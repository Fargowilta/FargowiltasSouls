using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using FargowiltasSouls.NPCs;
using FargowiltasSouls.Projectiles;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Cavern
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

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            npc.buffImmune[BuffID.Confused] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.Nymph)
            {
                npc.knockBackResist = 0f;

                EModeGlobalNPC.Aura(npc, 250, ModContent.BuffType<Lovestruck>(), true, DustID.PinkTorch);

                if (--Counter < 0)
                {
                    Counter = 300;

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < 1000)
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

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Lovestruck>(), 240);

            npc.life += damage * 2;
            if (npc.life > npc.lifeMax)
                npc.life = npc.lifeMax;
            CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage * 2);
            npc.netUpdate = true;
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref int damage, ref float knockback, ref bool crit)
        {
            base.ModifyHitByAnything(npc, player, ref damage, ref knockback, ref crit);

            if (player.loveStruck)
            {
                /*npc.life += damage;
                if (npc.life > npc.lifeMax)
                    npc.life = npc.lifeMax;
                CombatText.NewText(npc.Hitbox, CombatText.HealLife, damage);*/

                Vector2 speed = Main.rand.NextFloat(1, 2) * Vector2.UnitX.RotatedByRandom(Math.PI * 2);
                float ai1 = 30 + Main.rand.Next(30);
                Projectile.NewProjectile(npc.GetSource_FromThis(), player.Center, speed, ModContent.ProjectileType<HostileHealingHeart>(), damage, 0f, Main.myPlayer, npc.whoAmI, ai1);

                damage = 0;
                crit = false;
                npc.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<Items.Accessories.Masomode.NymphsPerfume>(), 5));
        }
    }
}
