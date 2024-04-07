using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Life
{
    public class LifeRitual : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_467";

        public LifeRitual() : base(MathHelper.Pi / 140f, 1000f, ModContent.NPCType<LifeChampion>(), 87) { }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Life Seal");
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 4;
        }

        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] != 2f && npc.ai[0] != 8f)
            {
                Projectile.velocity = (npc.Center - Projectile.Center) / 30;
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
        }

        public override void AI()
        {
            base.AI();

            Projectile.rotation += 0.77f;
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<PurifiedBuff>(), 300);
        }
    }
}