using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Pets
{
    public class MutantSpawn : ModProjectile
    {
        public bool yFlip; //used to suppress y velocity (pet fastfalls with an extra update per tick otherwise)
        public float notlocalai1 = 0f;

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Mutant Spawn");
            Main.projFrames[projectile.type] = 12;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 36;
            projectile.ignoreWater = true;
            projectile.aiStyle = 26;
            aiType = ProjectileID.BlackCat;
            projectile.netImportant = true;
            projectile.friendly = true;

            projectile.extraUpdates = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(notlocalai1);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            notlocalai1 = reader.ReadSingle();
        }

        public override bool PreAI()
        {
            Main.player[projectile.owner].blackCat = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            if (player.dead)
            {
                modPlayer.MutantSpawn = false;
            }
            if (modPlayer.MutantSpawn)
            {
                projectile.timeLeft = 2;
            }

            if (projectile.tileCollide && projectile.velocity.Y > 0) //pet updates twice per tick, this is called every tick; effectively gives it normal gravity when tangible
            {
                yFlip = !yFlip;
                if (yFlip)
                    projectile.position.Y -= projectile.velocity.Y;
            }

            if (player.velocity == Vector2.Zero) //run code when not moving
                BeCompanionCube();
        }

        public void BeCompanionCube()
        {
            Player player = Main.player[projectile.owner];
            Color color;
            color = Lighting.GetColor((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16);
            Vector3 vector3_1 = color.ToVector3();
            color = Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16);
            Vector3 vector3_2 = color.ToVector3();

            if (vector3_1.Length() < 0.15f && vector3_2.Length() < 0.15)
            {
                notlocalai1 += 1;
            }
            else if (notlocalai1 > 0)
            {
                notlocalai1 -= 1;
            }

            notlocalai1 = MathHelper.Clamp(notlocalai1, -3600f, 600);

            if (notlocalai1 > Main.rand.Next(300, 600) && !player.immune)
            {
                notlocalai1 = Main.rand.Next(30) * -10 - 300;

                switch (Main.rand.Next(3))
                {
                    case 0: //stab
                        if (projectile.owner == Main.myPlayer)
                        {
                            Main.PlaySound(SoundID.Item1, projectile.Center);
                            player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByOther(6), 777, 0, false, false, false, -1);
                            player.immune = false;
                            player.immuneTime = 0;
                        }
                        break;

                    case 1: //spawn mutant
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int n = NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y, ModContent.NPCType<NPCs.MutantBoss.MutantBoss>());
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("Mutant has awoken!"), new Color(175, 75, 255));
                                if (n != Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, n);
                            }
                            else
                            {
                                Main.NewText("Mutant has awoken!", 175, 75, 255);
                            }
                        }
                        break;

                    default:
                        if (projectile.owner == Main.myPlayer)
                        {
                            CombatText.NewText(projectile.Hitbox, Color.LimeGreen, "You think you're safe?");
                        }
                        break;
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = Main.player[projectile.owner].position.Y > projectile.Center.Y;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D texture2D14 = ModContent.GetTexture("FargowiltasSouls/Projectiles/Pets/MutantSpawn_Glow");
            /*float scale = ((Main.mouseTextColor / 200f - 0.35f) * 0.3f + 0.9f) * projectile.scale;
            Main.spriteBatch.Draw(texture2D14, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, scale, spriteEffects, 0f);*/
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = Color.White * projectile.Opacity * 0.6f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D14, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, projectile.scale, spriteEffects, 0f);
            }

            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(lightColor), projectile.rotation, origin2, projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}