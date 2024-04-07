using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class AccessorySourceProjectileHack : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool needsStatUpdate;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemSource && itemSource.Item != null && itemSource.Item.type != Main.player[projectile.owner].HeldItem.type)
                needsStatUpdate = true;
        }

        public override bool PreAI(Projectile projectile)
        {
            if (needsStatUpdate)
            {
                needsStatUpdate = false;

                projectile.CritChance += (int)Main.player[projectile.owner].GetTotalCritChance(projectile.DamageType);
                projectile.ArmorPenetration += (int)Main.player[projectile.owner].GetTotalArmorPenetration(projectile.DamageType);
            }

            return base.PreAI(projectile);
        }
    }
}
