using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.AbomBoss
{
    public class AbomRitual : BaseArena
    {
        public override string Texture => "Terraria/Projectile_274";

        private const float realRotation = MathHelper.Pi / 180f;

        public AbomRitual() : base(realRotation, 1400f, ModContent.NPCType<NPCs.AbomBoss.AbomBoss>(), 87) { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abominationn Seal");
        }

        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] < 9)
            {
                projectile.velocity = npc.Center - projectile.Center;
                if (npc.ai[0] != 8) //snaps directly to abom when preparing for p2 attack
                    projectile.velocity /= 40f;

                rotationPerTick = realRotation;
            }
            else //remains still in higher AIs
            {
                projectile.velocity = Vector2.Zero;

                rotationPerTick -= realRotation / 10f; //denote arena isn't moving
            }
        }

        public override void AI()
        {
            base.AI();
            projectile.rotation += 1f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            base.OnHitPlayer(player, damage, crit);

            if (FargoSoulsWorld.MasochistMode)
            {
                player.AddBuff(mod.BuffType("AbomFang"), 300);
                player.AddBuff(mod.BuffType("Unstable"), 240);
                player.AddBuff(mod.BuffType("Berserked"), 120);
            }
            player.AddBuff(BuffID.Bleeding, 600);
        }
    }
}