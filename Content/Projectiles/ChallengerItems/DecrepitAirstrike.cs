using FargowiltasSouls.Content.Bosses.TrojanSquirrel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.ChallengerItems
{
    public class DecrepitAirstrike : TrojanAcorn
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/Masomode/TargetingReticle";

        const int maxTime = 60 * 3;
        private SoundStyle Beep = new("FargowiltasSouls/Assets/Sounds/NukeBeep");
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 82;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = maxTime;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float slotsConsumed => ref Projectile.ai[2];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (State == 0)
            {
                Projectile.netUpdate = true;
                Projectile.alpha -= 4;
                if (Projectile.alpha < 0) //fade in
                    Projectile.alpha = 0;

                int modifier = Math.Min(60, maxTime - Projectile.timeLeft);

                Projectile.scale = 4f - 3f / 60f * modifier; //start big, shrink down

                Projectile.rotation = (float)Math.PI * 2 / 30 * modifier;
                if (Projectile.timeLeft % 60 == 0 && Projectile.timeLeft > 30)
                {
                    SoundEngine.PlaySound(Beep, Projectile.Center);
                    CombatText.NewText(Projectile.Hitbox, Color.Red, Projectile.timeLeft / 60, true);
                }
                if (Projectile.timeLeft == 30)
                {
                    Vector2 offset = Vector2.UnitY * -700 + Vector2.UnitX * Main.rand.NextFloat(-300, 300);
                    int nukeDamage = Projectile.damage + (Projectile.damage / 2 * ((int)slotsConsumed - 1));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + offset, Vector2.Zero, ModContent.ProjectileType<DecrepitAirstrikeNuke>(), nukeDamage, 2, Projectile.owner, Projectile.Center.X, Projectile.Center.Y, Projectile.timeLeft);
                }
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 0)
            {
                Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
                int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
                Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
                Vector2 origin2 = rectangle.Size() / 2f;
                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}