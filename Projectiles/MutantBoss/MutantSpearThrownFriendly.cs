using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpearThrownFriendly : BossWeapons.HentaiSpearThrown
    {
        public override string Texture => "FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.ranged = false;
        }

        /*public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("GodEater"), 600);
            target.AddBuff(mod.BuffType("Sadism"), 600);
            base.OnHitNPC(target, damage, knockback, crit);
        }*/
    }
}