using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs.Champions;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class LifeRitual : BaseArena
    {
        public override string Texture => "Terraria/Projectile_467";

        public LifeRitual() : base(MathHelper.Pi / 140f, 1000f, ModContent.NPCType<LifeChampion>(), 87) { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Life Seal");
            Main.projFrames[projectile.type] = 4;
        }

        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] != 2f && npc.ai[0] != 8f)
            {
                projectile.velocity = (npc.Center - projectile.Center) / 30;
            }
            else
            {
                projectile.velocity *= 0.95f;
            }
        }

        public override void AI()
        {
            base.AI();

            projectile.rotation += 0.77f;
            if (++projectile.frameCounter > 6)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame > 3)
                    projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);

            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(mod.BuffType("Purified"), 300);
        }
    }
}