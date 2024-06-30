using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class TerraLightning : LightningArc
    {
        public override string Texture => "Terraria/Images/Projectile_466";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Generic;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 60 * 5);
            target.AddBuff(ModContent.BuffType<LeadPoisonBuff>(), 60 * 5);
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62);
            if (FargoSoulsUtil.HostCheck)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<ObsidianExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }
    }
}