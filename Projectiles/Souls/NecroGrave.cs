using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Souls
{
    public class NecroGrave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necro Grave");
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //move it up when spawned, synchronizes with necro ench spawning at npc.Bottom
            Projectile.Bottom = Projectile.Center;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.5f);

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item2, Projectile.Center);
            }

            if (player.GetModPlayer<FargoSoulsPlayer>().WizardEnchantActive || player.GetModPlayer<FargoSoulsPlayer>().ShadowForce)
            {
                Projectile.velocity.Y = 0;

                for (int i = 0; i < 4; i++) //smoke to make the floating convincing
                {
                    int d = Dust.NewDust(Projectile.BottomLeft, Projectile.width, 0, DustID.Wraith);
                    Main.dust[d].position.Y -= 4;
                    Main.dust[d].velocity *= 0.5f;
                    Main.dust[d].noGravity = true;
                }
            }
            else
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;
            }

            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost && Main.LocalPlayer.Hitbox.Intersects(Projectile.Hitbox)
                && player.GetModPlayer<FargoSoulsPlayer>().NecroEnchantActive && player.GetToggleValue("Necro"))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -12), ModContent.ProjectileType<DungeonGuardianNecro>(), (int)Projectile.ai[0], 1, Projectile.owner);

                SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);

                for (int i = 0; i < 36; i++)
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Blood, 0, -3, Scale: 2f);
                    Main.dust[d].velocity *= 2;
                    Main.dust[d].noGravity = Main.rand.NextBool();
                }

                Projectile.Kill();
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position += Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            if (Projectile.timeLeft % 10 < 5)
                color26.A = 0;

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
