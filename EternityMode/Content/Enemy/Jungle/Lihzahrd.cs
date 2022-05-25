using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.Jungle
{
    public class Lihzahrd : Shooters
    {
        public Lihzahrd() : base(210, ProjectileID.PoisonDartTrap, 12f) { }

        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.Lihzahrd,
            NPCID.LihzahrdCrawler
        );

        public int FireballCounter;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            npc.trapImmune = true;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);

            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.Venom] = true;
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (npc.type == NPCID.LihzahrdCrawler)
            {
                if (++FireballCounter > 30)
                {
                    FireballCounter = -90;
                    if (npc.HasPlayerTarget && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vel = npc.DirectionTo(Main.player[npc.target].Center) * 12f;
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, vel, ProjectileID.Fireball, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0f, Main.myPlayer);
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Infested>(), 300);
            //target.AddBuff(ModContent.BuffType<Bloodthirsty>(), 120);
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);

            /*if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(npc.Center, Vector2.UnitY * -6, ProjectileID.SpikyBallTrap, 30, 0f, Main.myPlayer);*/
        }
    }
}
