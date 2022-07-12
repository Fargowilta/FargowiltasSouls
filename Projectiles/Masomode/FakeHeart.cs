using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class FakeHeart : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fake Heart");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 600;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            CooldownSlot = 0;
        }

        public override void AI()
        {
            float gravity = .1f;
            float yMax = 7f;

            Projectile.velocity.Y += gravity;
            if (Projectile.velocity.Y > yMax)
                Projectile.velocity.Y = yMax;
            Projectile.velocity.X *= .95f;
            if (Projectile.velocity.X < .1f && Projectile.velocity.X > -.1f)
                Projectile.velocity.X = 0f;

            float rand = Main.rand.Next(90, 111) * 0.01f * (Main.essScale * 0.5f);
            Lighting.AddLight(Projectile.Center, 0.5f * rand, 0.1f * rand, 0.1f * rand);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.Colliding(Projectile.Hitbox, target.Hitbox))
            {
                if (target.GetModPlayer<FargoSoulsPlayer>().DevianttHeartItem == null)
                {
                    target.hurtCooldowns[0] = 0;
                    int defense = target.statDefense;
                    float endurance = target.endurance;
                    target.statDefense = 0;
                    target.endurance = 0;
                    if (FargoSoulsUtil.IsChinese())
                    {
                        target.Hurt(PlayerDeathReason.ByCustomReason(target.name + "感到了心碎。"), Projectile.damage, 0, false, false, false, 0);
                    }
                    else
                    {
                        target.Hurt(PlayerDeathReason.ByCustomReason(target.name + " felt heartbroken."), Projectile.damage, 0, false, false, false, 0);
                    }
                    target.statDefense = defense;
                    target.endurance = endurance;

                    if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<NPCs.DeviBoss.DeviBoss>()))
                        target.AddBuff(ModContent.BuffType<Lovestruck>(), 240);
                }
                else
                {
                    target.statLife += 1;
                    target.HealEffect(1);
                }

                Projectile.timeLeft = 0;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, lightColor.G, lightColor.B, lightColor.A);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}