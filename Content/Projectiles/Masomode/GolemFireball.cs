using FargowiltasSouls.Content.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class GolemFireball : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_258";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fireball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 100;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            }
            for (int index1 = 0; index1 < 2; ++index1)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity.X *= 0.3f;
                Main.dust[index2].velocity.Y *= 0.3f;
            }
            Projectile.rotation += 0.3f * Projectile.direction;
            Projectile.velocity.Y += Projectile.ai[0];
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(BuffID.BrokenArmor, 600);
            target.AddBuff(ModContent.BuffType<DefenselessBuff>(), 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
            NPC npc = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);
            if (npc != null)
            {
                if (Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16] == null || //outside temple
                    Main.tile[(int)npc.Center.X / 16, (int)npc.Center.Y / 16].WallType != WallID.LihzahrdBrickUnsafe)
                {
                    target.AddBuff(BuffID.Burning, 300);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 25);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}