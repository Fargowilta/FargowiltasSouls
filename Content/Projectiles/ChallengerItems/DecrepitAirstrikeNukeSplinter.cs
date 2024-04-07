using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{

    public class DecrepitAirstrikeNukeSplinter : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/BanishedBaron/BaronScrap";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Banished Baron Scrap");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            if (Projectile.ai[2] == 0)
            {
                Projectile.ai[2] = Main.rand.NextBool() ? 1 : -1;
            }
            Projectile.rotation += Projectile.ai[2] * MathHelper.TwoPi / 90;

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = -1;
            }
            if (Projectile.ai[0] > 10f)
            {
                if (Projectile.ai[0] % 30 == 11f)
                    Projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 600, true);

                if (Projectile.ai[1] != -1)
                {
                    if (Main.npc[(int)Projectile.ai[1]].active) //nestled for no index error
                    {
                        Vector2 vectorToIdlePosition = Main.npc[(int)Projectile.ai[1]].Center - Projectile.Center;
                        float num = vectorToIdlePosition.Length();
                        float speed = 15f;
                        float inertia = 15f;
                        float deadzone = 2f;
                        if (num > deadzone)
                        {
                            vectorToIdlePosition.Normalize();
                            vectorToIdlePosition *= speed;
                            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + vectorToIdlePosition) / inertia;
                        }
                        else if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.velocity.X = -0.15f;
                            Projectile.velocity.Y = -0.05f;
                        }
                    }
                }
            }
            Projectile.ai[0]++;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Iron, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
            }
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

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
