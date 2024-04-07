using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Magmaw
{
    public class MagmawHand : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 1;
        }
        public const int SwordLength = 150;
        public const int ArmWidth = 20;
        public const int DefaultDistance = 220;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.light = 1;
            Projectile.FargoSouls().DeletionImmuneRank = 10;

            Projectile.hide = true;
        }
        public override bool CanHitPlayer(Player target) => HitPlayer;
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!HitPlayer)
                return false;
            //Hilt/hands hitbox
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            //Sword hitbox
            float useless1 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * SwordLength * Projectile.scale, Projectile.width * Projectile.scale, ref useless1))
            {
                return true;
            }
            //"Arm" hitbox
            NPC parent = GetParent();
            float useless2 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, parent.Center, ArmWidth, ref useless2))
            {
                return true;
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
            {
                behindNPCs.Add(index);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //Draw sword
            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * (SwordLength / 2);
            FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, drawPos: pos);

            //Draw arm
            int drawLayers = 1;
            Texture2D armTexture = Terraria.GameContent.TextureAssets.Projectile[ModContent.ProjectileType<GlowLine>()].Value;
            int num156 = armTexture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, armTexture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            NPC parent = GetParent();
            Vector2 dir = Projectile.SafeDirectionTo(parent.Center);
            int length = (int)Projectile.Distance(parent.Center);
            Vector2 offset = dir * length / 2f;
            Vector2 position = Projectile.Center - Main.screenLastPosition + new Vector2(0f, Projectile.gfxOffY) + offset;
            //const float resolutionCompensation = 128f / 24f; //i made the image higher res, this compensates to keep original display size

            Rectangle destination = new((int)position.X, (int)position.Y, length, ArmWidth);//(int)(rectangle.Height * Projectile.scale / resolutionCompensation));

            Color drawColor = Color.Orange * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;

            for (int j = 0; j < drawLayers; j++)
                Main.EntitySpriteDraw(new DrawData(armTexture, destination, new Rectangle?(rectangle), drawColor, dir.ToRotation(), origin2, SpriteEffects.None, 0));
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.localAI[2]);
            writer.Write(HitPlayer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.localAI[2] = reader.ReadSingle();
            HitPlayer = reader.ReadBoolean();
        }
        const int Right = 1;
        const int Left = -1;
        public bool HitPlayer = false;
        public ref float ParentID => ref Projectile.ai[0];
        public ref float Side => ref Projectile.ai[1];
        public override void AI()
        {
            Side = LumUtils.NonZeroSign(Side); //Make sure it's always 1 or -1 (default to 1 if 0)

            NPC parent = GetParent();
            Magmaw magmaw = parent.As<Magmaw>();

            Projectile.timeLeft = 60; //don't despawn
            Projectile.damage = parent.damage;

            magmaw.HandleHandState(this);
        }
        #region Help Methods
        public void RotateTowards(Vector2 vectorToAlignWith, float fraction)
        {
            float dif = FargoSoulsUtil.RotationDifference(Projectile.rotation.ToRotationVector2(), vectorToAlignWith);
            if (Math.Abs(dif) < MathHelper.Pi / 40) //snap to desired if difference is low enough
                fraction = 1;
            Projectile.rotation += (dif * fraction);
        }
        public NPC GetParent()
        {
            int parentID = (int)ParentID;
            if (!parentID.IsWithinBounds(Main.maxNPCs))
            {
                Projectile.Kill();
                return null;
            }
            NPC parent = Main.npc[parentID];
            if (!parent.TypeAlive<Magmaw>())
            {
                Projectile.Kill();
                return null;
            }
            return parent;
        }
        #endregion
    }
}