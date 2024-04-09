using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class PrismaRegaliaProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Prisma Regalia");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 244;
            Projectile.height = 244;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 150;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;

            Projectile.FargoSouls().NinjaCanSpeedup = false;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 HitboxSize = new(88 * Projectile.scale, 88 * Projectile.scale);
            Vector2 HitboxCenter = Projectile.Center + Projectile.velocity * (Projectile.Size.Length() / 2f - HitboxSize.Length() / 2f);
            hitbox = new Rectangle((int)(HitboxCenter.X - HitboxSize.X / 2f), (int)(HitboxCenter.Y - HitboxSize.Y / 2f), (int)HitboxSize.X, (int)HitboxSize.Y);
        }
        public float maxCharge = 60 * 2.5f;
        public int SwingDirection = 1;
        public float Extension = 0;
        int OrigAnimMax = 30;
        bool Charged;
        Vector2 ChargeVector = Vector2.Zero;
        public override void AI()
        {
            ref float chargeLevel = ref Projectile.ai[0];
            ref float timer = ref Projectile.ai[1];

            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            if (Projectile.localAI[0] == 0)
            {
                OrigAnimMax = player.itemAnimationMax;
                Projectile.localAI[0] = 1;
            }
            float HoldoutRangeMax = (float)Projectile.Size.Length() / 2; //since sprite is diagonal
            float HoldoutRangeMin = (float)-Projectile.Size.Length() / 6;

            if (player.channel)
            {
                Projectile.velocity = player.SafeDirectionTo(Main.MouseWorld);
                Projectile.Center = player.MountedCenter + Projectile.velocity * HoldoutRangeMin;
                Projectile.friendly = false;
                if (chargeLevel < maxCharge)
                    chargeLevel++;
                if (chargeLevel == (int)maxCharge - 1 && player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ChargeSound"), Projectile.Center + Projectile.velocity * Projectile.Size.Length() / 2);
                }
                Projectile.localAI[1] = chargeLevel; //store the charge amount
                                                     //int d = Dust.NewDust(player.MountedCenter + Projectile.velocity * Projectile.Size.Length() * 0.95f, 0, 0, DustID.CrystalPulse);
                                                     //Main.dust[d].noGravity = true;
                bool charged = chargeLevel >= maxCharge - 1;
                FargoSoulsUtil.AuraParticles(player, Projectile.Size.Length() * 0.95f, color: charged ? Color.HotPink : Color.DeepPink, particleType: charged ? 1 : 0);
            }
            else
            {
                Projectile.friendly = true;
                if (chargeLevel > -1) //check once
                {
                    Projectile.damage = (int)(Projectile.damage * (1.15f + chargeLevel / 60f)); //modify this to change damage charge
                    Charged = chargeLevel == maxCharge;
                    chargeLevel = -1;

                }
                int duration = (int)(OrigAnimMax / 1.5f);
                int WaitTime = OrigAnimMax / 5;


                if (timer == 0)
                    SwingDirection = Main.rand.NextBool(2) ? 1 : -1;
                float Swing = 13; //higher value = less swing
                Projectile.localNPCHitCooldown = OrigAnimMax;   //only hit once per swing
                                                                //projectile.ai[1] is time from spawn
                                                                //Extension is between 0 and 1
                if (Projectile.timeLeft > OrigAnimMax)
                {
                    Projectile.timeLeft = OrigAnimMax;
                }
                if (timer <= duration / 2)
                {
                    Extension = timer / (duration / 2);
                    Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * -Math.PI / (Swing * OrigAnimMax));
                }
                else if (timer <= duration / 2 + WaitTime)
                {
                    Extension = 1;
                    Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * (1.5 * duration / WaitTime) * Math.PI / (Swing * OrigAnimMax)); //i know how wacky this looks
                }
                else
                {
                    Projectile.friendly = false; //no hit on backswing
                    Extension = (duration + WaitTime - timer) / (duration / 2);
                    Projectile.velocity = Projectile.velocity.RotatedBy(SwingDirection * Projectile.spriteDirection * -Math.PI / (Swing * OrigAnimMax));
                }

                if (timer == duration / 2)
                {
                    float pitch = Charged ? -1 : 0;
                    SoundEngine.PlaySound(SoundID.Item1 with { Pitch = pitch }, player.Center);
                }

                timer++;
                Projectile.velocity = Vector2.Normalize(Projectile.velocity); //store direction
                Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, Extension);
                //}
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction);
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.ToRadians(-45f) + (float)Math.PI;
            }
            else
            {
                Projectile.rotation += MathHelper.ToRadians(-135f) + (float)Math.PI;
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 pos = Projectile.Center + Projectile.velocity * (Projectile.Size.Length() / 2f);
            int count = 0;
            if (Charged)
            {
                count = 4;
            }
            if (Extension > 0.675f)
            {
                SoundEngine.PlaySound(SoundID.Item68, pos);
                count += 3;
            }
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 10,
                    ProjectileID.FairyQueenMagicItemShot, Projectile.damage / 6, Projectile.knockBack, Projectile.owner, -1, Main.rand.NextFloat(1)); //random ai1 decides color completely randomly
                if (Main.projectile[p] != null && p != Main.maxProjectiles)
                {
                    Main.projectile[p].DamageType = DamageClass.MeleeNoSpeed;
                    //Main.projectile[p].
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);
            Vector2 pos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);


            if (!Main.player[Projectile.owner].channel)
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color27 = color26;
                    color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    Vector2 value4 = Projectile.oldPos[i];
                    float num165 = Projectile.rotation;//Projectile.oldRot[i];
                    Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
                }
            }

            //offset glow
            for (int j = 0; j < 16; j++)
            {
                float offsetDistance = 4f * (Projectile.localAI[1] / maxCharge);
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * offsetDistance;
                Color glowColor = Main.DiscoColor * 0.3f;
                Main.spriteBatch.Draw(texture2D13, pos + afterimageOffset, null, glowColor, Projectile.rotation, origin2, Projectile.scale, effects, 0f);
            }

            Main.EntitySpriteDraw(texture2D13, pos, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}