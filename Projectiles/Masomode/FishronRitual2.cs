using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FishronRitual2 : BaseArena
    {
        public override string Texture => "Terraria/Images/Projectile_409";

        public FishronRitual2() : base(MathHelper.Pi / 140f, 1600f, NPCID.DukeFishron) { }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Oceanic Ritual");
            Main.projFrames[Projectile.type] = 3;
        }

        protected override void Movement(NPC npc)
        {
            Projectile.velocity = npc.Center - Projectile.Center;
            Projectile.velocity /= 20f;
        }

        public override void AI()
        {
            base.AI();
            Projectile.rotation += 0.2f;
            Projectile.frame++;
            if (Projectile.frame > 2)
                Projectile.frame = 0;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);

            target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 50;
            target.AddBuff(ModContent.BuffType<OceanicMaul>(), 20 * 60);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 50 + (int)(100.0 * Main.DiscoG / 255.0), 255, 150) * (targetPlayer == Main.myPlayer ? 1f : 0.2f);
        }
    }
}