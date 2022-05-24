using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs.Champions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class WillRitual : BaseArena
    {
        public override string Texture => "FargowiltasSouls/Projectiles/Champions/WillTyphoon";

        public WillRitual() : base(MathHelper.Pi / 140f, 1200f, ModContent.NPCType<WillChampion>(), 87, 5) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Will Seal");
            Main.projFrames[Projectile.type] = 22;
        }

        protected override void Movement(NPC npc)
        {
            if ((npc.ai[0] == 2 && npc.ai[1] < 30) || (npc.ai[0] == -1 && npc.ai[1] < 10))
            {
                Projectile.Kill();
            }
        }

        public override void AI()
        {
            base.AI();

            Projectile.rotation -= MathHelper.ToRadians(1.5f);
            if (++Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
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