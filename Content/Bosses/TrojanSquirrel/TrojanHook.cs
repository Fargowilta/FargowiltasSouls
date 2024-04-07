using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.TrojanSquirrel
{
    public class TrojanHook : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_13";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Squirrel Hook");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 8000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        NPC npc;
        Vector2 offset;
        int dir;

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC sourceNPC)
            {
                npc = sourceNPC;
                offset = Projectile.Center - npc.Center;
                dir = sourceNPC.direction;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc is NPC ? npc.whoAmI : -1);
            writer.WritePackedVector2(offset);
            writer.Write((byte)dir);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc = FargoSoulsUtil.NPCExists(reader.ReadInt32());
            offset = reader.ReadPackedVector2();
            dir = reader.ReadByte();
        }

        public override void PostAI()
        {
            if (npc != null && dir != npc.direction)
            {
                dir = npc.direction;
                offset.X *= -1;
            }
        }

        public override void AI()
        {
            if (npc != null)
                npc = FargoSoulsUtil.NPCExists(npc.whoAmI);

            if (npc == null)
            {
                Projectile.Kill();
                return;
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
            }

            Projectile.extraUpdates = WorldSavingSystem.EternityMode ? 1 : 0;

            if (Projectile.ai[0] == 0)
            {
                if (!Collision.SolidTiles(Projectile.Center, 0, 0))
                    Projectile.tileCollide = true;
            }
            else if (Projectile.ai[0] == 1f)
            {
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Zero;

                //if (++Projectile.localAI[1] > 60 || !WorldSavingSystem.EternityMode)
                //{
                Projectile.ai[0] = 2f;
                Projectile.netUpdate = true;
                //}
            }

            if (Projectile.ai[0] == 2f)
            {
                Projectile.extraUpdates++;

                Projectile.tileCollide = false;

                const float speed = 12;
                Projectile.velocity = speed * Projectile.SafeDirectionTo(ChainOrigin);

                Projectile.position += (npc.position - npc.oldPosition) / 2f;

                if (Projectile.Distance(ChainOrigin) < speed)
                    Projectile.Kill();
            }
            else if (Projectile.Distance(ChainOrigin) > 1600f)
            {
                Projectile.ai[0] = 2f;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.DirectionFrom(ChainOrigin).ToRotation() + MathHelper.PiOver2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.ai[0] = 1f;
            return false;
        }

        Vector2 ChainOrigin => npc == null ? Projectile.Center : npc.Center + offset;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (npc == null || !WorldSavingSystem.EternityMode)
                return base.Colliding(projHitbox, targetHitbox);

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), ChainOrigin, Projectile.Center);
        }

        protected virtual bool flashingZapEffect => false; //WorldSavingSystem.EternityMode && Projectile.timeLeft % 10 < 5;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightningTexture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Bosses/TrojanSquirrel/TrojanHookLightning").Value;
            int lightningFrames = 5;

            Rectangle GetRandomLightningFrame()
            {
                int frameHeight = lightningTexture.Height / lightningFrames;
                int frame = Main.rand.Next(lightningFrames);
                return new(0, frameHeight * frame, lightningTexture.Width, frameHeight);
            }
            void DrawLightning(Vector2 position, Color color, float rotation)
            {
                if (!Main.rand.NextBool(4))
                    return;
                Rectangle lightningRect = GetRandomLightningFrame();
                Vector2 lightningOrigin = lightningRect.Size() / 2f;
                Main.EntitySpriteDraw(lightningTexture, position - Main.screenPosition, lightningRect, Color.White, rotation, lightningOrigin, 1f, SpriteEffects.None, 0);
            }
            if (npc != null && TextureAssets.Chain.IsLoaded)
            {
                Texture2D texture = TextureAssets.Chain.Value;
                Vector2 position = Projectile.Center;
                Vector2 mountedCenter = ChainOrigin;
                Rectangle? sourceRectangle = new Rectangle?();
                Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
                float num1 = texture.Height;
                Vector2 vector24 = mountedCenter - position;
                float rotation = (float)Math.Atan2(vector24.Y, vector24.X) - 1.57f;
                bool flag = true;
                if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                    flag = false;
                if (float.IsNaN(vector24.X) && float.IsNaN(vector24.Y))
                    flag = false;
                while (flag)
                    if (vector24.Length() < num1 + 1.0)
                    {
                        flag = false;
                    }
                    else
                    {
                        Vector2 vector21 = vector24;
                        vector21.Normalize();
                        position += vector21 * num1;
                        vector24 = mountedCenter - position;
                        Color color2 = Lighting.GetColor((int)position.X / 16, (int)(position.Y / 16.0));
                        color2 = flashingZapEffect ? Color.White * Projectile.Opacity : Projectile.GetAlpha(color2);
                        Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);

                        bool lightningBehind2 = Main.rand.NextBool();
                        if (lightningBehind2)
                        {
                            DrawLightning(position, lightColor, rotation);
                        }
                        if (flashingZapEffect)
                        {
                            color2.A = 0;
                            Main.EntitySpriteDraw(texture, position - Main.screenPosition, sourceRectangle, color2, rotation, origin, 1f, SpriteEffects.None, 0);
                        }
                        if (!lightningBehind2)
                        {
                            DrawLightning(position, lightColor, rotation);
                        }

                    }
            }

            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;
            Color color = flashingZapEffect ? Color.White * Projectile.Opacity : Projectile.GetAlpha(lightColor);

            bool lightningBehind = Main.rand.NextBool();
            if (lightningBehind)
            {
                DrawLightning(Projectile.Center, lightColor, Projectile.rotation);
            }
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            if (flashingZapEffect)
            {
                color.A = 0;
                Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            }
            if (!lightningBehind)
            {
                DrawLightning(Projectile.Center, lightColor, Projectile.rotation);
            }

            return false;
        }
    }
}