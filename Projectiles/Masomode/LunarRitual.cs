using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using FargowiltasSouls.EternityMode;
using FargowiltasSouls.EternityMode.Content.Boss.HM;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class LunarRitual : BaseArena
    {
        public override string Texture => "Terraria/Projectile_454";

        private const float maxSize = 1600f;

        public LunarRitual() : base(MathHelper.Pi / 140f, maxSize, NPCID.MoonLordCore) { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Lunar Ritual");
            Main.projFrames[projectile.type] = 2;
        }

        protected override void Movement(NPC npc)
        {
            Vector2 target = npc.Center;
            if (npc.HasValidTarget) //tracks halfway between player and boss
                target += (Main.player[npc.target].Center - npc.Center) / 2;

            if (projectile.Distance(target) <= 1)
                projectile.Center = target;
            else if (projectile.Distance(target) > threshold)
                projectile.velocity = (target - projectile.Center) / 30;
            else if (npc.GetEModeNPCMod<MoonLordCore>().VulnerabilityState == 4 && npc.GetEModeNPCMod<MoonLordCore>().VulnerabilityTimer < 60)
                projectile.velocity = (Main.player[npc.target].Center - projectile.Center) * 0.05f;
            else
                projectile.velocity = projectile.DirectionTo(target);

            threshold += 6;
            if (threshold > maxSize)
                threshold = maxSize;
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