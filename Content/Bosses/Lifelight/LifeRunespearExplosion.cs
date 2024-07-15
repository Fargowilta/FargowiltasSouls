using FargowiltasSouls.Assets.ExtraTextures;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Lifelight
{

    public class LifeRunespearExplosion : ModProjectile
    {
        // Kills the projectile above 0, so set it to a negative value.
        public ref float Timer => ref Projectile.ai[0];

        // The .whoAmI of the parent npc.
        public ref float ParentIndex => ref Projectile.ai[1];
        public override string Texture => "FargowiltasSouls/Assets/Effects/LifeStar";

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;

            Projectile.hostile = true;
            Projectile.scale = 1;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.Read();
            base.ReceiveExtraAI(reader);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return LumUtils.CircularHitboxCollision(Projectile.Center, projHitbox.Width / 2, targetHitbox);
        }
        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Math.Abs(Timer);
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 1.5f }, Projectile.Center);
                Projectile.netUpdate = true;
            }

            if (Timer > 0f)
                Projectile.Kill();

            // Ramp up the scale and rotation over time
            float ratio = 1f - Math.Abs(Timer + 5) / Projectile.localAI[0];
            float rampupVfx = (float)Math.Sin(MathHelper.PiOver2 * ratio);
            Projectile.scale = 0.1f + 1.4f * rampupVfx;

            // rescale hitbox
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = (int)(150 * Projectile.scale);
            Projectile.Center = Projectile.position;

            if (Timer == -5f)
            {
                Projectile.Opacity = 0;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                SoundEngine.PlaySound(LifeChallenger.RuneSound1 with { PitchRange = (-0.6f, -0.4f) }, Projectile.Center);

                int damage = Projectile.damage;
                if (FargoSoulsUtil.HostCheck)
                    for (int i = 0; i < 4; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation + MathHelper.PiOver2 * i).ToRotationVector2() * 20f, ModContent.ProjectileType<LifeWave>(), damage, 0f, Main.myPlayer);

                for (int j = 0; j < 32; j++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2);
                    Particle p = new SmallSparkle(
                        worldPosition: Projectile.Center + offset,
                        velocity: Vector2.Zero,
                        drawColor: Color.Cyan,
                        scale: 1f,
                        lifetime: Main.rand.Next(20, 80),
                        rotation: 0,
                        rotationSpeed: Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)
                        );
                    p.Spawn();
                }
            }
            

            NPC parent = FargoSoulsUtil.NPCExists(ParentIndex);
            // Stick to a position set by lifelight.
            if (parent != null && parent.ModNPC is LifeChallenger lifelight)
            {
                Projectile.Center = parent.Center + lifelight.LockVector1 + lifelight.LockVector2 * (LifeRunespearHitbox.Length + 30);
                if (parent.HasPlayerTarget)
                    Projectile.rotation = 1f * MathHelper.TwoPi * rampupVfx + Projectile.DirectionTo(Main.player[parent.target].Center).ToRotation();
                else
                    Projectile.Kill();
            }
            else
                Projectile.Kill();

            Timer++;
        }
        public override bool CanHitPlayer(Player target) => Timer > -5f;
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Buffs.Masomode.SmiteBuff>(), 60 * 3);
        }
        public override void OnKill(int timeLeft)
        {
            
        }
        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.Cyan;
            color.A = 50;
            return color * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = texture.Size() / 2f;

            for (int i = 0; i < 3; i++)
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
    }
}
