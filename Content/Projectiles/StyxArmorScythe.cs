using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles
{
    public class StyxArmorScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Styx Scythe");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;

            Projectile.timeLeft = 300;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.FargoSouls().DeletionImmuneRank = 1;
            Projectile.FargoSouls().CanSplit = false;
            Projectile.FargoSouls().TimeFreezeImmune = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            const int baseDamage = 100;

            if (Projectile.velocity == Vector2.Zero || Projectile.velocity.HasNaNs())
                Projectile.velocity = -Vector2.UnitY;

            Player player = Main.player[Projectile.owner];

            if (!Projectile.friendly || Projectile.hostile || !player.active || player.dead || player.ghost || !player.FargoSouls().StyxSet)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 240;
            Projectile.damage = (int)(baseDamage * player.GetDamage(DamageClass.Melee).Additive);

            Projectile.Center = player.Center;

            if (player.ownedProjectileCounts[Projectile.type] > 0)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    int threshold = Projectile.localAI[1] == 0 ? 300 : 5; //check more often when something seems off, check slower when everything seems normal
                    if (++Projectile.localAI[0] > threshold)
                    {
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 0;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner && Projectile.whoAmI != i)
                            {
                                if (Projectile.localAI[0] == Main.projectile[i].localAI[0])
                                    Projectile.localAI[0] += 5; //deliberately desync
                                if (Projectile.ai[0] == Main.projectile[i].ai[0])
                                {
                                    Projectile.ai[0]++;
                                    Projectile.localAI[1] = 1;
                                }
                            }
                        }
                        Projectile.netUpdate = true;
                    }

                    if (Projectile.ai[0] >= player.ownedProjectileCounts[Projectile.type])
                    {
                        Projectile.ai[0] = 0;
                        Projectile.localAI[1] = 1;
                    }
                }

                Vector2 target = -150f * Vector2.UnitY.RotatedBy(MathHelper.TwoPi / player.ownedProjectileCounts[Projectile.type] * Projectile.ai[0]);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, target, 0.1f);
            }

            Projectile.rotation += 1f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleCrystalShard, Scale: 2f);
                Main.dust[d].velocity *= 6f;
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 300);
            target.AddBuff(ModContent.BuffType<Buffs.Masomode.MutantNibbleBuff>(), 300);
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

            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = (Projectile.ai[0] < 0 || Main.player[Projectile.owner].ownedProjectileCounts[Projectile.type] >= Items.Armor.StyxCrown.MAX_SCYTHES ? Color.Yellow : Color.Purple) * Projectile.Opacity;
            color.A = 0;
            return color;
        }
    }
}