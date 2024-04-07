using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosRitual : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_454";

        private const float maxSize = 1200f;
        private const float minSize = 600f;

        public CosmosRitual() : base(MathHelper.Pi / 140f, 1000f, ModContent.NPCType<CosmosChampion>()) { }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Seal");
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 2;
        }

        protected override void Movement(NPC npc)
        {
            Projectile.Center = npc.Center;

            float scaleModifier = npc.life / (npc.lifeMax * 0.2f);
            if (scaleModifier > 1f)
                scaleModifier = 1f;
            if (scaleModifier < 0f)
                scaleModifier = 0f;

            float targetSize = minSize + (maxSize - minSize) * scaleModifier;
            if (threshold > targetSize)
            {
                threshold -= 4;
                if (threshold < targetSize)
                    threshold = targetSize;
            }
            if (threshold < targetSize)
            {
                threshold += 4;
                if (threshold > targetSize)
                    threshold = targetSize;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            if (WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.OnFire, 300);
                target.AddBuff(BuffID.Electrified, 300);
                target.AddBuff(ModContent.BuffType<HexedBuff>(), 300);
                target.AddBuff(BuffID.Frostburn, 300);
            }
        }
    }
}