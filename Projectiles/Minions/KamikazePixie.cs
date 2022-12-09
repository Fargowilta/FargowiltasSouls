using FargowiltasSouls.Projectiles.ChallengerItems;
using FargowiltasSouls.Projectiles.Challengers;
using FargowiltasSouls.Projectiles.Masomode;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class KamikazePixie : ModProjectile
    {

        public int counter;

        //public override string Texture => "Terraria/Images/NPC_75";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pixie");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 36;
            Projectile.height = 30;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;

            Projectile.minionSlots = 1f / 3f;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanDamage() => Projectile.timeLeft <= 0;
        private bool foundTarget = false;
        private float speed = 22f; //22

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Pixie, Projectile.Center);
            }

            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[0] = -1;
                Projectile.velocity = Vector2.Normalize(player.Center - Projectile.Center);
            }
            Projectile.rotation = 0;
            Projectile.spriteDirection = Projectile.direction;
            #region Targeting

            if (!foundTarget && Projectile.ai[0] == -1)
            {
                // find target
                Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 2000, true);
                foundTarget = Projectile.ai[0] == -1 ? false : true;
                Projectile.netUpdate = true;
            }
            Projectile.friendly = foundTarget;
            #endregion
            if (Projectile.ai[1] >= 60 * 3)
            {
                if (Projectile.ai[1] == 60 * 3)
                {
                    speed *= 1.2f;
                }
                #region Charge at Target
                if (foundTarget)
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0]];
                    if (npc.active && npc.CanBeChasedBy()) //target is still valid
                    {
                        Vector2 targetCenter = npc.Center;
                        FlyToward(targetCenter);
                    }
                    else //we lost em boys
                    {
                        Projectile.ai[0] = -1;
                        foundTarget = false;
                        Projectile.netUpdate = true;
                    }
                    if (Projectile.ai[1] > 60 * 8 || Vector2.Distance(npc.Center, Projectile.Center) <= 25)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    FlyToward(player.position);
                }
                #endregion
            }
            else
            {
                #region Shoot at Target
                if (foundTarget)
                {
                    NPC npc = Main.npc[(int)Projectile.ai[0]];
                    if (npc.active && npc.CanBeChasedBy()) //target is still valid
                    {
                        //hard predictive shot
                        Vector2 prediction = npc.Center + (npc.velocity * 10);
                        Vector2 targetCenter = npc.Center - new Vector2((float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] * (360 / 60))), 250);
                        FlyToward(targetCenter);
                        if (Projectile.ai[1] % 30 == 0 && Vector2.Distance(npc.Center, Projectile.Center) <= 450)
                        {
                            SoundEngine.PlaySound(SoundID.Item12, Projectile.position);
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (prediction - Projectile.Center) / 10,
                                            ModContent.ProjectileType<LightslingerBombshot>(), Projectile.damage / 3, 0f, Main.myPlayer, 0, Projectile.whoAmI);
                        }
                    }
                    else //we lost em boys
                    {
                        Projectile.ai[0] = -1;
                        foundTarget = false;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    FlyToward(player.position);
                }
                #endregion
            }

            Projectile.timeLeft = 2;

            if (player.whoAmI == Main.myPlayer && (!player.active || player.dead || player.ghost))
            {
                Projectile.Kill();
                return;
            }

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            Projectile.ai[1]++;
        }

        public void FlyToward(Vector2 v)
        {
            float inertia = 15f; 
            float deadzone = 25f;
            Vector2 vectorToIdlePosition = v - Projectile.Center;
            float num = vectorToIdlePosition.Length();
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

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            //fallThrough = Main.player[Projectile.owner].Center.Y > Projectile.Bottom.Y;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Pixie, Projectile.Center);

            if (timeLeft == 1)
            {
                for (int k = 0; k < 20; k++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, -1f);
                    Main.dust[d].scale += 0.5f;
                }

                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 128;
                Projectile.Center = Projectile.position;

                if (Projectile.owner == Main.myPlayer)
                    Projectile.Damage();

                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                SoundEngine.PlaySound(SoundID.NPCDeath7, Projectile.Center);

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Pixie, 0f, 0f, 100, default(Color), 2.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 5f;
                    dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, DustID.Pixie, 0f, 0f, 100, default(Color), 1f);
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

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity;

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