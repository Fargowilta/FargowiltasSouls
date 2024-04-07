using FargowiltasSouls.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class CreeperHitbox : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Creeper");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override bool? CanDamage()
        {
            Projectile.maxPenetrate = 1;
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Main.player[Projectile.owner].Center.X > target.Center.X ? -1 : 1;
            if (Projectile.ai[0] == 0) //gutted
                target.AddBuff(BuffID.Ichor, 60 * 10);
            else
                target.AddBuff(ModContent.BuffType<SublimationBuff>(), 60 * 10);
        }
    }
}