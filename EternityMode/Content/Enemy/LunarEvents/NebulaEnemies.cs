using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.EternityMode.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.EternityMode.Content.Enemy.LunarEvents
{
    public class NebulaEnemies : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            NPCID.NebulaBeast,
            NPCID.NebulaHeadcrab,
            NPCID.NebulaBrain,
            NPCID.NebulaSoldier
        );

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.buffImmune[BuffID.Suffocation] = true;
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            base.OnHitPlayer(npc, target, damage, crit);

            target.AddBuff(ModContent.BuffType<Berserked>(), 300);
            target.AddBuff(ModContent.BuffType<Lethargic>(), 300);
        }
    }

    public class NebulaBrain : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.NebulaBrain);

        public override void AI(NPC npc)
        {
            base.AI(npc);
        }

        public override bool CheckDead(NPC npc)
        {
            if (npc.HasValidTarget)
            {
                Player target = Main.player[npc.target];
                Vector2 boltVel = target.Center - npc.Center;
                boltVel.Normalize();
                boltVel *= 4.5f;

                for (int i = 0; i < (int)npc.localAI[2] / 60; i++)
                {
                    Vector2 spawnPos = npc.position;
                    spawnPos.X += Main.rand.Next(npc.width);
                    spawnPos.Y += Main.rand.Next(npc.height);

                    Vector2 boltVel2 = boltVel.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 21)));
                    boltVel2 *= Main.rand.NextFloat(0.8f, 1.2f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromThis(), spawnPos, boltVel2, ProjectileID.NebulaLaser, 48, 0f, Main.myPlayer);
                }
            }

            return base.CheckDead(npc);
        }
    }

    public class NebulaHeadcrab : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.NebulaHeadcrab);

        public int Counter;

        public override void AI(NPC npc)
        {
            base.AI(npc);

            if (++Counter >= 300)
            {
                if (npc.ai[0] != 5f && npc.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient) //if not latched on player
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, 6 * npc.DirectionTo(Main.player[npc.target].Center), ProjectileID.NebulaLaser, FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0, Main.myPlayer);
                Counter = (short)Main.rand.Next(120);
            }
        }
    }
}
