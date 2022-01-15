using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs.Champions;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class WillRitual : BaseArena
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/WillTyphoon";

        public WillRitual() : base(MathHelper.Pi / 140f, 1200f, ModContent.NPCType<WillChampion>(), 87, 5) { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Will Seal");
            Main.projFrames[projectile.type] = 22;
        }

        protected override void Movement(NPC npc)
        {
            if ((npc.ai[0] == 2 && npc.ai[1] < 30) || (npc.ai[0] == -1 && npc.ai[1] < 10))
            {
                projectile.Kill();
            }
        }

        public override void AI()
        {
            base.AI();

            projectile.rotation -= MathHelper.ToRadians(1.5f);
            if (++projectile.frameCounter > 2)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);

            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Defenseless>(), 300);
                target.AddBuff(ModContent.BuffType<Midas>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}