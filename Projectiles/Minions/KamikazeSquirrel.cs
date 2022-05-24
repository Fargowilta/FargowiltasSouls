using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class KamikazeSquirrel : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Critters/TophatSquirrelCritter";

        public int counter;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = Projectile.height = 30;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = ProjAIStyleID.Pet;
            AIType = ProjectileID.BabySlime;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;

            Projectile.minionSlots = 1f / 3f;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }

        public override bool? CanDamage() => Projectile.timeLeft <= 0;

        private float realFrameCounter;
        private int realFrame;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;

            if (player.whoAmI == Main.myPlayer && (!player.active || player.dead || player.ghost))
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.velocity.X == 0)
            {
                realFrame = 0;
            }
            else
            {
                realFrameCounter += System.Math.Abs(Projectile.velocity.X);

                if (++realFrameCounter > 6)
                {
                    realFrameCounter = 0;
                    realFrame += 1;
                }

                if (realFrame < 1 || realFrame >= Main.projFrames[Projectile.type])
                    realFrame = 1;
            }

            Projectile.frame = realFrame;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Projectile.owner].Center.Y > Projectile.Bottom.Y;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }

        public override void Kill(int timeLeft)
        {
            if (timeLeft == 1)
            {
                for (int k = 0; k < 20; k++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, -1f);
                    Main.dust[d].scale += 0.5f;
                }

                if (!Main.dedServ)
                {
                    int g = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, 0.5f * Projectile.velocity.RotatedByRandom(MathHelper.PiOver4), ModContent.Find<ModGore>(Mod.Name, Main.rand.NextBool() ? "TrojanSquirrelGore2" : "TrojanSquirrelGore2_2").Type, Projectile.scale);
                    Main.gore[g].rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    Main.gore[g].velocity.Y -= 6f;
                }

                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 128;
                Projectile.Center = Projectile.position;

                if (Projectile.owner == Main.myPlayer)
                    Projectile.Damage();

                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 5f;
                    dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 1f);
                    Main.dust[dust].velocity *= 3f;
                }

                for (int j = 0; j < 4; j++)
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[gore].velocity *= 0.4f;
                    Main.gore[gore].velocity += new Vector2(1f, 1f).RotatedBy(MathHelper.TwoPi / 4 * j);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            //int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            //int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            //Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            //Vector2 origin2 = rectangle.Size() / 2f;

            //Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY - 4),
            //    new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2,
            //    Projectile.scale, Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return true;
        }
    }
}