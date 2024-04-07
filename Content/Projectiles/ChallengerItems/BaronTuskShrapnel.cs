using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class BaronTuskShrapnel : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/BanishedBaron/BaronShrapnel";
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60 * 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.FargoSouls().CanSplit = false;
        }

        public NPC EmbeddedNPC = null;
        public Vector2 embedOffset = Vector2.Zero;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ref float State = ref Projectile.ai[1];
            if (State == 0)
            {
                EmbeddedNPC = target;
                embedOffset = Projectile.Center - target.Center;
                Projectile.velocity = Vector2.Zero;
                State = 1;
            }

        }
        public override void AI()
        {
            ref float State = ref Projectile.ai[1];
            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.Pi).ToRotation();

            const float gravity = 0.25f;
            switch (State)
            {
                case 0:
                    {
                        Projectile.velocity.Y += gravity;
                        break;
                    }
                case 1:
                    {
                        Projectile.friendly = false;
                        Projectile.tileCollide = false;
                        if (EmbeddedNPC.active)
                        {
                            Projectile.Center = EmbeddedNPC.Center + embedOffset;
                            Projectile.rotation = (embedOffset).ToRotation();
                        }
                        else
                        {
                            State = 2;
                        }
                        break;
                    }
                case 2:
                    {
                        Projectile.velocity = Vector2.Normalize(embedOffset) * 20;
                        Projectile.friendly = true;
                        Projectile.tileCollide = true;
                        Projectile.penetrate = 2;
                        EmbeddedNPC = null;
                        embedOffset = Vector2.Zero;
                        State = 3;
                        break;
                    }
                case 3:
                    {
                        Projectile.velocity.Y += gravity;
                        break;
                    }
            }

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

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
