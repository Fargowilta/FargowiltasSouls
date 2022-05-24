using FargowiltasSouls.EternityMode.Net;
using FargowiltasSouls.EternityMode.Net.Strategies;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.EternityMode.Content.Enemy
{
    public abstract class Shooters : EModeNPCBehaviour
    {
        protected readonly int AttackThreshold;
        protected readonly int ProjectileType;
        protected readonly int DustType;
        protected readonly float Distance;
        protected readonly float Speed;
        protected readonly float DamageMultiplier;
        protected readonly int Telegraph;
        protected readonly bool NeedLineOfSight;

        protected Shooters(int attackThreshold, int projectileType, float speed, float damageMultiplier = 1f, int dustType = 159, float distance = 1000, int telegraph = 30, bool needLineOfSight = false)
        {
            AttackThreshold = attackThreshold;
            ProjectileType = projectileType;
            DustType = dustType;
            Distance = distance;
            Speed = speed;
            DamageMultiplier = damageMultiplier;
            Telegraph = telegraph;
            NeedLineOfSight = needLineOfSight;
        }

        public int AttackTimer;

        public override Dictionary<Ref<object>, CompoundStrategy> GetNetInfo() =>
            new Dictionary<Ref<object>, CompoundStrategy> {
                { new Ref<object>(AttackTimer), IntStrategies.CompoundStrategy },
            };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);

            AttackTimer = -Main.rand.Next(60);
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);

            AttackTimer++;

            if (AttackTimer == AttackThreshold - Telegraph)
            {
                if (!npc.HasPlayerTarget || npc.Distance(Main.player[npc.target].Center) > Distance
                    || (NeedLineOfSight && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0)))
                {
                    AttackTimer = 0;
                }
                else if (DustType != -1)
                {
                    FargoSoulsUtil.DustRing(npc.Center, 32, DustType, 5f, default, 2f);
                }

                npc.netUpdate = true;
                NetSync(npc);
            }

            if (AttackTimer > AttackThreshold - Telegraph)
            {
                npc.position -= npc.velocity;
            }

            if (AttackTimer > AttackThreshold)
            {
                AttackTimer = 0;

                npc.netUpdate = true;
                NetSync(npc);

                if (npc.HasPlayerTarget && npc.Distance(Main.player[npc.target].Center) < Distance
                    && (!NeedLineOfSight || NeedLineOfSight && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, Speed * npc.DirectionTo(Main.player[npc.target].Center), ProjectileType, FargoSoulsUtil.ScaledProjectileDamage(npc.damage, DamageMultiplier), 0, Main.myPlayer);
                }
            }
        }
    }
}
