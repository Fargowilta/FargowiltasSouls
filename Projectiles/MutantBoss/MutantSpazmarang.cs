using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpazmarang : MutantRetirang
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/Spazmarang";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Spazmarang");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            timeLeftModifier = 0.96f;
            accelModifier = 0.25f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 120);
            if (FargoSoulsWorld.MasochistMode)
                target.AddBuff(mod.BuffType("MutantFang"), 180);
        }
    }
}