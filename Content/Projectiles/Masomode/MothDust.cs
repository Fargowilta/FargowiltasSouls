using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class MothDust : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Moth Dust");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            //Projectile.hide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;

            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            Projectile.velocity *= .96f;

            if (Main.rand.NextBool())
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 2.5f;
            }

            Lighting.AddLight(Projectile.position, .3f, .1f, .3f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (FargoSoulsUtil.BossIsAlive(ref EModeGlobalNPC.deviBoss, ModContent.NPCType<DeviBoss>()))
            {
                target.AddBuff(ModContent.BuffType<BerserkedBuff>(), 240);
                target.AddBuff(ModContent.BuffType<MutantNibbleBuff>(), 240);
                target.AddBuff(ModContent.BuffType<GuiltyBuff>(), 240);
                target.AddBuff(ModContent.BuffType<LovestruckBuff>(), 240);
                target.AddBuff(ModContent.BuffType<RottingBuff>(), 240);
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    int d = Main.rand.Next(FargowiltasSouls.DebuffIDs.Count);
                    target.AddBuff(FargowiltasSouls.DebuffIDs[d], 240);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), new Color(255, 255, 255, 0), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}