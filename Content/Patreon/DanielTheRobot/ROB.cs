using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.DanielTheRobot
{
    public class ROB : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 40;
            Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = ProjAIStyleID.Pet;
        }

        //public override bool MinionContactDamage() => true;
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            PatreonPlayer modPlayer = player.GetModPlayer<PatreonPlayer>();

            if (!player.active || player.dead || player.ghost)
            {
                modPlayer.ROB = false;
            }

            if (modPlayer.ROB)
            {
                Projectile.timeLeft = 2;
            }
            HandleAnimation();
            return true;
        }
        private void HandleAnimation()
        {
            int startFrame = 0;
            int endFrame = 0;
            int animFrames = 5;
            switch (Projectile.ai[0])
            {
                case 1: //flying to player
                    {
                        startFrame = 4;
                        endFrame = 6;
                    }
                    break;
                default: //not flying to player
                    {
                        if (Projectile.velocity.Y > 0) //falling
                        {
                            startFrame = endFrame = 1;
                        }
                        else if (Projectile.velocity.Y < 0) //going upwards
                        {
                            goto case 1;
                        }
                        else //not in air
                        {
                            if (Projectile.velocity.X != 0) //walking
                            {
                                startFrame = 2;
                                endFrame = 3;
                            }
                            //if not moving, frame is 0 as default
                        }
                    }
                    break;
            }
            if (Projectile.frame < startFrame || Projectile.frame > endFrame)
            {
                Projectile.frame = startFrame;
            }
            if (++Projectile.frameCounter > animFrames)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > endFrame)
                    Projectile.frame = startFrame;
            }
        }
        private Asset<Texture2D> EyebrowAsset => ModContent.Request<Texture2D>(Texture + "_Eyebrows");
        private Asset<Texture2D> GlowAsset => ModContent.Request<Texture2D>(Texture + "Glow");
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Vector2 drawOffset = Vector2.UnitY * 10 * Projectile.scale;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);

            if (LumUtils.AnyBosses())
            {
                Main.EntitySpriteDraw((Texture2D)EyebrowAsset, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            }
            Main.EntitySpriteDraw((Texture2D)GlowAsset, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Color.White, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}