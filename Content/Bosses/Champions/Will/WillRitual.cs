using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Will
{
    public class WillRitual : BaseArena
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Will/WillTyphoon";

        public WillRitual() : base(MathHelper.Pi / 140f, 1200f, ModContent.NPCType<WillChampion>(), 87, 5) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Will Seal");
            Main.projFrames[Projectile.type] = 22;
        }

        protected override void Movement(NPC npc)
        {
            if (npc.ai[0] == 2 && npc.ai[1] < 30 || npc.ai[0] == -1 && npc.ai[1] < 10)
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

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 300);
                target.AddBuff(ModContent.BuffType<MidasBuff>(), 300);
            }
            target.AddBuff(BuffID.Bleeding, 300);
        }
    }
}