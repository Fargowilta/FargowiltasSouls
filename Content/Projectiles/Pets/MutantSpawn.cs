using FargowiltasSouls.Content.Bosses.MutantBoss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Pets
{
    public class MutantSpawn : ModProjectile
    {
        public bool yFlip; //used to suppress y velocity (pet fastfalls with an extra update per tick otherwise)
        public float notlocalai1 = 0f;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mutant Spawn");
            Main.projFrames[Projectile.type] = 12;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 26;
            AIType = ProjectileID.BlackCat;
            Projectile.netImportant = true;
            Projectile.friendly = true;

            Projectile.extraUpdates = 1;
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
            Main.player[Projectile.owner].blackCat = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.dead)
            {
                modPlayer.MutantSpawn = false;
            }
            if (modPlayer.MutantSpawn)
            {
                Projectile.timeLeft = 2;
            }

            if (Projectile.tileCollide && Projectile.velocity.Y > 0) //pet updates twice per tick, this is called every tick; effectively gives it normal gravity when tangible
            {
                yFlip = !yFlip;
                if (yFlip)
                    Projectile.position.Y -= Projectile.velocity.Y;
            }

            if (player.velocity == Vector2.Zero) //run code when not moving
                BeCompanionCube();
        }

        public void BeCompanionCube()
        {
            Player player = Main.player[Projectile.owner];
            Color color;
            color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);
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
                        if (Projectile.owner == Main.myPlayer)
                        {
                            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                            // TODO: figure out old args
                            player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByOther(6), 777, 0, false, false, -1, false);
                            player.immune = false;
                            player.immuneTime = 0;
                        }
                        break;

                    case 1: //spawn mutant
                        if (FargoSoulsUtil.HostCheck)
                        {
                            FargoSoulsUtil.NewNPCEasy(Projectile.GetSource_FromThis(), Projectile.Center, ModContent.NPCType<MutantBoss>());
                            FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue($"Mods.{Mod.Name}.NPCs.MutantBoss.DisplayName"));
                        }
                        break;

                    default:
                        if (Projectile.owner == Main.myPlayer)
                        {
                            CombatText.NewText(Projectile.Hitbox, Color.LimeGreen, Language.GetTextValue($"Mods.{Mod.Name}.Items.SpawnSack.NotSafe"));
                        }
                        break;
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = Main.player[Projectile.owner].position.Y > Projectile.Center.Y;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D texture2D14 = ModContent.Request<Texture2D>($"FargowiltasSouls/Content/Projectiles/Pets/{Name}_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            /*float scale = ((Main.mouseTextColor / 200f - 0.35f) * 0.3f + 0.9f) * Projectile.scale;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, scale, spriteEffects, 0);*/
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = Color.White * Projectile.Opacity * 0.6f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D14, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}