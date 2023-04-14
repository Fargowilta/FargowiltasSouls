using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantDestroyerTail : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/NPCs/Resprites/NPC_136";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Destroyer");
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
            Projectile.hide = true;
            CooldownSlot = 1;
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

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num214 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                Projectile.GetAlpha(Color.White), Projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        public override void AI()
        {
            if ((int)Main.time % 120 == 0) Projectile.netUpdate = true;

            int num1038 = 30;

            //dust!
            int dustId = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, 62, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId].noGravity = true;
            int dustId3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 2f), Projectile.width, Projectile.height + 5, 62, Projectile.velocity.X * 0.2f,
                Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
            Main.dust[dustId3].noGravity = true;

            bool flag67 = false;
            Vector2 value67 = Vector2.Zero;
            Vector2 arg_2D865_0 = Vector2.Zero;
            float num1052 = 0f;
            if (Projectile.ai[1] == 1f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            int byIdentity = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)Projectile.ai[0], ModContent.ProjectileType<MutantDestroyerBody>());
            if (byIdentity >= 0 && Main.projectile[byIdentity].active)
            {
                flag67 = true;
                value67 = Main.projectile[byIdentity].Center;
                Vector2 arg_2D957_0 = Main.projectile[byIdentity].velocity;
                num1052 = Main.projectile[byIdentity].rotation;
                float num1053 = MathHelper.Clamp(Main.projectile[byIdentity].scale, 0f, 50f);
                int arg_2D9AD_0 = Main.projectile[byIdentity].alpha;
                Main.projectile[byIdentity].localAI[0] = Projectile.localAI[0] + 1f;
                Projectile.timeLeft = Main.projectile[byIdentity].timeLeft;
            }

            if (!flag67) return;

            Projectile.alpha -= 42;
            if (Projectile.alpha < 0) Projectile.alpha = 0;
            Projectile.velocity = Vector2.Zero;
            Vector2 vector134 = value67 - Projectile.Center;
            if (num1052 != Projectile.rotation)
            {
                float num1056 = MathHelper.WrapAngle(num1052 - Projectile.rotation);
                vector134 = vector134.RotatedBy(num1056 * 0.1f, default(Vector2));
            }

            Projectile.rotation = vector134.ToRotation() + 1.57079637f;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = (int)(num1038 * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (vector134 != Vector2.Zero) Projectile.Center = value67 - Vector2.Normalize(vector134) * 36;
            Projectile.spriteDirection = vector134.X > 0f ? 1 : -1;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 62, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 60, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<LightningRod>(), Main.rand.Next(300, 1200));
            if (FargoSoulsWorld.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
        }
    }
}