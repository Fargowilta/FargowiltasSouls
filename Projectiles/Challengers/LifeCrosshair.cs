using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.NPCs.Challengers;
using System.IO;
using Terraria.DataStructures;

namespace FargowiltasSouls.Projectiles.Challengers
{

    public class LifeCrosshair : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crosshair");
        }
        public override void SetDefaults()
        {
            Projectile.width = 184;
            Projectile.height = 184;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
        }

        int npc = -1;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && parentNpc.type == ModContent.NPCType<LifeChallenger>())
            {
                npc = parentNpc.whoAmI;
                if (Projectile.ai[1] == 2f)
                {
                    float angleToMe = (Projectile.Center - parentNpc.Center).ToRotation();
                    float angleToPlayer = (Main.player[parentNpc.target].Center - parentNpc.Center).ToRotation();
                    Projectile.localAI[1] = MathHelper.WrapAngle(angleToMe - angleToPlayer);
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(npc);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc = reader.Read7BitEncodedInt();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 0f)
            {
                Projectile.Kill();
            }
            Projectile.ai[0] += 1f;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
                //Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }

            Projectile.alpha -= 12;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;
            //else
            //    Projectile.rotation += MathHelper.TwoPi / 15 * Projectile.localAI[0];

            Projectile.scale = 2f - Projectile.Opacity;

            if (Projectile.ai[1] == 1)
            {
                Projectile.position -= Projectile.velocity;
                Projectile.velocity = Projectile.velocity.RotatedBy(1.75f * MathHelper.Pi / 180f);
            }
            else if (Projectile.ai[1] == 2)
            {
                NPC parent = FargoSoulsUtil.NPCExists(npc);
                if (parent != null)
                {
                    Vector2 offset = Main.player[parent.target].Center - parent.Center;
                    Projectile.Center = parent.Center + offset.RotatedBy(Projectile.localAI[1]);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200) * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
