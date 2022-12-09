using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace FargowiltasSouls.Projectiles.ChallengerItems
{

    public class EnchantedLifebladeProjectile : ModProjectile
    {
        public bool PlayedSound = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Enchanted Lifeblade");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 54; //sqrt(sprite width^2 / 2) since diagonal
            Projectile.height = 54;
            Projectile.aiStyle = 0;
            AIType = 14;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //Projectile.light = 2f;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
        }
        private Vector2 Aim = Vector2.Zero;
        private Vector2 AimDir = Vector2.Zero;
        private Vector2 Position = Vector2.Zero;
        const int ProjSpriteWidth = 74;
        const int ProjSpriteHeight = 74;
        public override void AI()
        {
            
            
            const int SpinTime = 15 + 1;
            const int ChargeTime = 15;
            const int ChargeSpeed = 30;

            if (Projectile.frameCounter > 4)
            {
                Projectile.frame %= 3;
                Projectile.frameCounter = 0;
            }
            Projectile.frameCounter++;

            //for a duration, stays in place and spins around
            //after timer, dash towards mouse and then reset timer and spin again

            Player player = Main.player[Projectile.owner];
            Vector2 vector = Vector2.Normalize(Main.MouseWorld - player.Center);
            
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.ai[1] < 5) //number of charges        //if (player.channel && !player.noItems && !player.CCed) for if only one sword
                {
                    
                    //if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    //{
                        if (Projectile.ai[0] == 0)
                        {
                            //Position = vector * Math.Min((Main.MouseWorld - player.Center).Length(), 450); move dependent on player variant
                            Projectile.position = player.Center + (vector * Math.Min((Main.MouseWorld - player.Center).Length(), 450)); //move independently variant
                            for (int i = 0; i < 30; i++)
                            {
                                int index3 = Dust.NewDust(Projectile.Center - new Vector2(ProjSpriteWidth / 2, ProjSpriteWidth / 2), ProjSpriteWidth, ProjSpriteWidth, DustID.PurpleCrystalShard,
                                    Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f);
                                Main.dust[index3].noGravity = true;
                            }

                            SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                            Projectile.friendly = false;
                        }
                        if (Projectile.ai[0] == SpinTime)
                        {
                            Aim = vector * Math.Min((Main.MouseWorld - player.Center).Length(), 450);
                            //AimDir = Vector2.Normalize(Aim - Position); move dependent on player variant
                            AimDir = Vector2.Normalize((player.Center + Aim) - Projectile.Center); //move independently variant
                            if (AimDir.HasNaNs())
                            {
                                AimDir = Vector2.UnitX * (float)player.direction;
                            }
                            SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, player.Center + Position);
                        }
                        if (Projectile.ai[0] >= SpinTime)
                        {
                            //Position = Position + (AimDir * ChargeSpeed); move dependent on player variant
                            Projectile.velocity = (AimDir * ChargeSpeed); //move independently variant
                            Projectile.friendly = true;
                        }
                        if (Projectile.ai[0] == SpinTime + ChargeTime)
                        {
                            if (Projectile.ai[1] != 4)
                                Projectile.velocity = Vector2.Zero; //move independently
                            for (int i = 0; i < 30; i++)
                            {
                                int index4 = Dust.NewDust(Projectile.Center - new Vector2(ProjSpriteWidth / 2, ProjSpriteWidth / 2), ProjSpriteWidth, ProjSpriteWidth, DustID.PurpleCrystalShard,
                                    Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f);
                                Main.dust[index4].noGravity = true;
                            }
                            Projectile.ai[0] = 1;
                            Projectile.ai[1]++; //number of charges
                        }
                        //dust marker at aim position so you know the max range
                        int index2 = Dust.NewDust(player.Center + vector * Math.Min((Main.MouseWorld - player.Center).Length(), 450), 0, 0, DustID.PurpleCrystalShard,
                            0, 0, 100, default, 1f);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].velocity.X *= 0f;
                        Main.dust[index2].velocity.Y *= 0f;
                    //}


                }
                else //if no longer holding, die
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath3, Projectile.Center);
                    
                    Projectile.Kill();
                }
            }
            //Projectile.velocity = Position + new Vector2(-Projectile.width / 2, -Projectile.height / 2); move dependent on player variant
            //Projectile.position = player.Center; move dependent on player variant
            DrawOffsetX = -(ProjSpriteWidth - Projectile.width);
            

            //rotation
            if (Projectile.ai[0] < SpinTime)
            {
                Projectile.rotation += MathHelper.ToRadians(360 / 10);
            }
            if (Projectile.ai[0] == SpinTime)
            {
                Projectile.rotation = AimDir.ToRotation() + MathHelper.PiOver4;
            }

            player.ChangeDir(Aim.X < 0 ? -1 : 1);
            //player.heldProj = Projectile.whoAmI; for if only one sword
            //player.itemTime = 20; for if only one sword
            //player.itemAnimation = 20; for if only one sword
            //player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * (float)Projectile.direction), (double)(Projectile.velocity.X * (float)Projectile.direction));

            Tile tile = Framing.GetTileSafely(Projectile.Center);
            if (!(tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
                Lighting.AddLight(Projectile.Center, torchID: TorchID.Pink);

            Projectile.ai[0]++;


        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 610 - Main.mouseTextColor * 2) * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.5f)
            {
                Color color27 = new Color(255, 51, 153) * Projectile.Opacity * 0.5f;
                color27.A = (byte)(color26.A / 2);
                float fade = (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                color27 *= fade * fade;
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                float num165 = Projectile.oldRot[max0];
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                center += Projectile.Size / 2;
                Main.EntitySpriteDraw(texture2D13, center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);

            return false;
        }
    }
}