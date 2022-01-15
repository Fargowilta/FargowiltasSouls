using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class CultistRitual : BaseArena
    {
        public override string Texture => "Terraria/Projectile_454";

        public CultistRitual() : base(MathHelper.Pi / -140f, 1600f, NPCID.CultistBoss) { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Cultist Ritual");
            Main.projFrames[projectile.type] = 2;
        }

        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] == 5)
            {
                int ritual = (int)npc.ai[2];
                if (ritual > -1 && ritual < Main.maxProjectiles && Main.projectile[ritual].active && Main.projectile[ritual].type == ProjectileID.CultistRitual)
                {
                    projectile.Center = Main.projectile[ritual].Center;
                }
            }
        }

        public override void AI()
        {
            base.AI();

            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 1)
                    projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);

            target.AddBuff(mod.BuffType("CurseoftheMoon"), 300);
        }
    }
}