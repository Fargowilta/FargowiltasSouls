using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FishronRitual2 : BaseArena
    {
        public override string Texture => "Terraria/Projectile_409";

        public FishronRitual2() : base(MathHelper.Pi / 140f, 1600f, NPCID.DukeFishron) { }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Oceanic Ritual");
            Main.projFrames[projectile.type] = 3;
        }

        protected override void Movement(NPC npc)
        {
            projectile.velocity = npc.Center - projectile.Center;
            projectile.velocity /= 20f;
        }

        public override void AI()
        {
            base.AI();
            projectile.rotation += 0.2f;
            projectile.frame++;
            if (projectile.frame > 2)
                projectile.frame = 0;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            base.OnHitPlayer(target, damage, crit);

            target.GetModPlayer<FargoPlayer>().MaxLifeReduction += 50;
            target.AddBuff(mod.BuffType("OceanicMaul"), Main.rand.Next(300, 600));
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(150, 50 + (int)(100.0 * Main.DiscoG / 255.0), 255, 150) * (targetPlayer == Main.myPlayer ? 1f : 0.2f);
        }
    }
}