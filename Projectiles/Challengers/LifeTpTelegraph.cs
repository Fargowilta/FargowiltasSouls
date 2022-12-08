using FargowiltasSouls.NPCs.Challengers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Challengers
{

	public class LifeTpTelegraph : ModProjectile
	{
        public override string Texture => "FargowiltasSouls/Effects/LifeStar";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Teleport Telegraph");
		}
		public override void SetDefaults()
		{
			Projectile.width = 150;
			Projectile.height = 150;
			Projectile.aiStyle = -1;
			Projectile.hostile = false;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.alpha = 0;
		}

        int npc = -1;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && parentNpc.type == ModContent.NPCType<LifeChallenger>())
                npc = parentNpc.whoAmI;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(npc);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc = reader.Read7BitEncodedInt();
        }

        public override void AI()
		{
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = Math.Abs(Projectile.ai[0]);
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 1.5f }, Projectile.Center);
            }

            if (++Projectile.ai[0] > 0f)
            {
                Projectile.Kill();
            }

            float ratio = 1f - Math.Abs(Projectile.ai[0]) / Projectile.localAI[0];
            float rampupVfx = (float)Math.Sin(MathHelper.PiOver2 * ratio);
            Projectile.scale = 0.1f + 1.4f * rampupVfx;
            Projectile.scale *= Main.rand.NextFloat(0.75f, 1f);
            Projectile.rotation = 2f * MathHelper.TwoPi * rampupVfx;

            NPC parent = FargoSoulsUtil.NPCExists(npc);
            if (parent != null)
            {
                Projectile.Center = new Vector2(parent.localAI[0], parent.localAI[1]);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.HotPink;
            color.A = 100;
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}
