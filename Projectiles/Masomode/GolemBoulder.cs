using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Masomode
{
    public class GolemBoulder : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_261";

        public bool spawned;
        public float vel;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boulder");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BoulderStaffOfEarth);
            AIType = ProjectileID.BoulderStaffOfEarth;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Default;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(vel);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            vel = reader.ReadSingle();
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, 31, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, 6, 0f, 0f, 100, default(Color), 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(Projectile.position, Projectile.width,
                        Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                float scaleFactor9 = 0.5f;
                for (int j = 0; j < 3; j++)
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[gore].velocity *= scaleFactor9;
                    Main.gore[gore].velocity.X += 1f;
                    Main.gore[gore].velocity.Y += 1f;
                }
            }

            if (!Projectile.tileCollide)
            {
                Tile tile = Framing.GetTileSafely(Projectile.Center - Vector2.UnitY * 26);
                if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType]))
                    Projectile.tileCollide = true;
            }

            if (Projectile.velocity.Y < 0 && Projectile.velocity.X == 0 && vel == 0) //on first bounce, roll at nearby player
            {
                int p = Player.FindClosest(Projectile.Center, 0, 0);
                if (p != -1)
                {
                    Projectile.velocity.X = vel = Projectile.Center.X < Main.player[p].Center.X ? 4f : -4f;
                    Projectile.velocity.Y *= Main.rand.NextFloat(1.9f, 2.1f);
                }
                else
                {
                    Projectile.timeLeft = 0;
                }
            }

            if (Math.Sign(Projectile.velocity.X) == Math.Sign(vel))
                Projectile.velocity.X = vel;
            else
                Projectile.timeLeft = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (vel == 0) //use the first bounce block code above, doesn't seem to work otherwise
                return true;

            if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 1) //bouncy
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 26;
            height = 26;

            Tile tile = Framing.GetTileSafely(Projectile.Center);
            bool inTemple = tile != null && tile.WallType == WallID.LihzahrdBrickUnsafe;
            if (!inTemple)
                fallThrough = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem) == null || Main.player[Main.npc[NPC.golemBoss].target].Bottom.Y > Projectile.Bottom.Y;

            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int index = 0; index < 5; ++index)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Lihzahrd, 0.0f, 0.0f, 0, new Color(), 1f);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BrokenArmor, 600);
            target.AddBuff(ModContent.BuffType<Defenseless>(), 600);
            target.AddBuff(BuffID.WitheredArmor, 600);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}