using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class BetsyElectrosphere : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_443";

        public int p = -1;
        public int timer;
        public float rotation;
        public Vector2 spawn;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sky Dragon's Fury");
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.Electrosphere];
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = 77;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 1;
        }

        public override bool PreAI()
        {
            if (Projectile.timeLeft == 240)
            {
                spawn.X = Projectile.ai[0];
                spawn.Y = Projectile.ai[1];
                rotation = Projectile.velocity.ToRotation();
                timer = Main.rand.Next(60);
            }
            return true;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            if (p == -1)
            {
                p = Player.FindClosest(Projectile.Center, 0, 0);
            }
            else
            {
                if (Projectile.Distance(Main.player[p].Center) < 600)
                    Projectile.velocity = 2f * Projectile.SafeDirectionTo(Main.player[p].Center);

                if (++timer > 60)
                {
                    timer = 0;
                    if (Main.player[p].Distance(spawn) > Projectile.Distance(spawn) && FargoSoulsUtil.HostCheck)
                    {
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, rotation.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(30)),
                            ModContent.ProjectileType<BetsyFury2>(), Projectile.damage, 0f, Main.myPlayer, p);
                    }
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            //target.AddBuff(BuffID.OnFire, 600);
            //target.AddBuff(BuffID.Ichor, 600);
            target.AddBuff(BuffID.WitheredArmor, Main.rand.Next(60, 300));
            target.AddBuff(BuffID.WitheredWeapon, Main.rand.Next(60, 300));
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 128) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}