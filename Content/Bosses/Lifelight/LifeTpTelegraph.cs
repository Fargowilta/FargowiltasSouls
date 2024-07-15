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
    public class LifeTpTelegraph : ModProjectile
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
            float ratio = 1f - Math.Abs(Timer) / Projectile.localAI[0];
            float rampupVfx = (float)Math.Sin(MathHelper.PiOver2 * ratio);
            Projectile.scale = 0.1f + 1.4f * rampupVfx;
            Projectile.scale *= Main.rand.NextFloat(0.8f, 1.2f);
            Projectile.rotation = 2f * MathHelper.TwoPi * rampupVfx;

            NPC parent = FargoSoulsUtil.NPCExists(ParentIndex);
            // Stick to a position set by lifelight.
            if (parent != null)
                Projectile.Center = new Vector2(parent.localAI[0], parent.localAI[1]);

            Timer++;
        }

        // Telegraphs should not deal damage.
        public override bool? CanDamage() => false;

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.HotPink;
            color.A = 50;
            return color;
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
