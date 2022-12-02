using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class RainbowSlime : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_266";

        public int counter;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rainbow Slime");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.alpha = 75;
            Projectile.width = 24;
            Projectile.height = 16;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = 26;
            AIType = ProjectileID.BabySlime;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;

            if (player.whoAmI == Main.myPlayer && (!player.active || player.dead || player.ghost || !player.GetModPlayer<FargoSoulsPlayer>().RainbowSlime))
            {
                Projectile.Kill();
                return;
            }

            /*if (Projectile.damage == 0)
            {
                Projectile.damage = (int)(35 * player.GetDamage(DamageClass.Summon));
                if (player.GetModPlayer<FargoSoulsPlayer>().MasochistSoul)
                    Projectile.damage *= 2;
            }*/

            if (++counter > 150) //periodically do bonus attack
            {
                counter = 0;
                if (Projectile.owner == Main.myPlayer && FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 600, true) != -1)
                {
                    for (int j = 0; j < 15; j++) //spray spikes
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-8, -5)), ModContent.ProjectileType<RainbowSlimeSpikeFriendly>(), Projectile.damage / 10, Projectile.knockBack, Main.myPlayer);
                    }
                }
            }

            //Main.NewText(Projectile.ai[0].ToString() + " " + Projectile.ai[1].ToString() + " " + Projectile.localAI[0].ToString() + " " + Projectile.localAI[1].ToString());
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Projectile.owner].Center.Y > Projectile.position.Y + Projectile.height;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.FlamesoftheUniverse>(), 120);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY - 4),
                new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2,
                Projectile.scale, Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }
    }
}