using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeRuneExplosion : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1;
            Projectile.timeLeft = 6000;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
            return LumUtils.CircularHitboxCollision(Projectile.Center, projHitbox.Width / 2, targetHitbox);
        }
        private Color GetColor()
        {
            int i = (int)Projectile.ai[1];
            if (i % 3 == 0) //cyan
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.UltraBrightTorch);
                return Color.Cyan;
            }
            else if (i % 3 == 1) //yellow
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.YellowTorch);
                return Color.Goldenrod;
            }
            else //pink
            {
                //Dust.NewDust(Projectile.Center, 0, 0, DustID.PinkTorch);
                return Color.DeepPink;
            }
        }
        public int Timer = 0;
        public const int ExplosionSize = 150;
        public override void AI()
        {

            int i = (int)Projectile.ai[1];
            float explosionTime = Projectile.ai[2];

            if (Timer < explosionTime)
            {
                Projectile.Opacity = Timer / explosionTime;
            }
            if (Timer == explosionTime)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = ExplosionSize;
                Projectile.Center = Projectile.position;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                SoundEngine.PlaySound(LifeChallenger.RuneSound1 with { PitchRange = (-0.6f, -0.4f) }, Projectile.Center);

                for (int j = 0; j < 32; j++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Particle p = new SmallSparkle(
                        worldPosition: Projectile.Center + offset,
                        velocity: Vector2.Zero,
                        drawColor: GetColor(),
                        scale: 1f,
                        lifetime: Main.rand.Next(20, 80),
                        rotation: 0,
                        rotationSpeed: Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)
                        );
                    p.Spawn();
                }
                
            }

            if (Timer > explosionTime + 10)
                Projectile.Kill();

            Timer++;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.SmiteBuff>(), 60 * 3);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer < Projectile.ai[2])
            {
                //draw glow ring
                Color RingColor = GetColor() * Projectile.Opacity * 0.5f;
                Texture2D ringTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                float RingScale = Projectile.scale * 2 * ExplosionSize / ringTexture.Height;
                Rectangle ringrect = new(0, 0, ringTexture.Width, ringTexture.Height);
                Vector2 ringorigin = ringrect.Size() / 2f;
                Main.EntitySpriteDraw(ringTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(ringrect), RingColor, Projectile.rotation, ringorigin, RingScale, SpriteEffects.None, 0);

            }
            return false;
        }

    }
}
