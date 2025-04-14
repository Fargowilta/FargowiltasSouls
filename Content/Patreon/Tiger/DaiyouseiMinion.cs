﻿using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Fargowiltas.Content.Projectiles;

namespace FargowiltasSouls.Content.Patreon.Tiger
{
    public class DaiyouseiMinion : TouhouMinionBase
    {
        public override int frameCount => 4;
        public override int attackSpeed => 15;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 41;
            Projectile.height = 31;
        }

        public override void MinionAttack(Vector2 target)
        {
            int leaf = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                Projectile.SafeDirectionTo(target) * 12, ModContent.ProjectileType<DaiyoLeaf>(),
                (int)(Projectile.damage * 0.3f), Projectile.knockBack, Projectile.owner);

            if (leaf > 0)
            {
                Projectile leafProj = Main.projectile[leaf];
                FargoGlobalProjectile.SplitProj(leafProj, 3);
            }
        }
    }
}
