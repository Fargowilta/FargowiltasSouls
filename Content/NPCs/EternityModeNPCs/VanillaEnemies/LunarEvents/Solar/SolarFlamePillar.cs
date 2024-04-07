using FargowiltasSouls.Common.Graphics.Particles;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Solar
{
    public class SolarFlamePillar : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Meteor");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 1;
            Projectile.DamageType = DamageClass.Default;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60 * 30;
            Projectile.scale = 1;
            AIType = 0;
            Projectile.aiStyle = 0;
        }
        int child = -1;
        public override void AI()
        {
            const int height = 400;
            ref float Timer = ref Projectile.ai[0];
            if (Timer == 0)
            {
                Projectile.Center -= Vector2.UnitY * height;
                Projectile.height = height;
            }
            if (Timer == 60 * 2)
            {
                SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);
                if (FargoSoulsUtil.HostCheck)
                {
                    Vector2 aim = Vector2.UnitY * -30;
                    child = FargoSoulsUtil.NewNPCEasy(Projectile.GetSource_FromThis(), Projectile.Center + (Vector2.UnitY * height / 2), NPCID.SolarGoop, velocity: aim);
                    if (Main.npc[child].active && child != Main.maxNPCs)
                    {
                        Main.npc[child].damage = Projectile.damage;
                    }
                }
            }
            if (Timer < 60 * 2)
            {
                //int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SolarFlare);
                //Main.dust[d].velocity.X = 0;
                Particle p = new ExpandingBloomParticle(
                    position: Main.rand.NextVector2FromRectangle(Projectile.Hitbox),
                    velocity: Vector2.UnitY * Main.rand.NextFloat(-20, 0),
                    drawColor: Microsoft.Xna.Framework.Color.Goldenrod,
                    startScale: Vector2.One * 1,
                    endScale: Vector2.One * 0,
                    lifetime: 60);
                p.Spawn();
            }
            else
            {
                Main.npc[child].velocity.Y = -30;
            }

            if (Timer > 60 * 2 + 30)
            {
                Projectile.Kill();
            }
            Timer++;
        }
    }
}