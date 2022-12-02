using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class DestroyerBody : ModProjectile
    {
        public int attackTimer;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Destroyer Body");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
            Projectile.hide = true;
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
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("FargowiltasSouls/Projectiles/Minions/DestroyerBody_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num214 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                color25, Projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                Color.White, Projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if ((int)Main.time % 120 == 0) Projectile.netUpdate = true;

            int num1038 = 30;

            bool flag67 = false;
            Vector2 value67 = Vector2.Zero;
            Vector2 arg_2D865_0 = Vector2.Zero;
            float num1052 = 0f;
            if (Projectile.ai[1] == 1f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            int byIdentity = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, (int)Projectile.ai[0], Projectile.type, ModContent.ProjectileType<DestroyerHead>());
            if (byIdentity >= 0 && Main.projectile[byIdentity].active)
            {
                flag67 = true;
                value67 = Main.projectile[byIdentity].Center;
                Vector2 arg_2D957_0 = Main.projectile[byIdentity].velocity;
                num1052 = Main.projectile[byIdentity].rotation;
                float num1053 = MathHelper.Clamp(Main.projectile[byIdentity].scale, 0f, 50f);
                int arg_2D9AD_0 = Main.projectile[byIdentity].alpha;
                Main.projectile[byIdentity].localAI[0] = Projectile.localAI[0] + 1f;
                if (Main.projectile[byIdentity].type != ModContent.ProjectileType<DestroyerHead>()) Main.projectile[byIdentity].localAI[1] = Projectile.identity;
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

            if (--attackTimer <= 0)
            {
                attackTimer = Main.rand.Next(150) + 150;
            }

            if (attackTimer == 1)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    int selectedTarget = -1; //pick target
                    const float maxRange = 750f;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy(Projectile) && Collision.CanHit(Projectile.Center, 0, 0, Main.npc[i].Center, 0, 0))
                        {
                            if (Projectile.Distance(Main.npc[i].Center) <= maxRange && Main.rand.NextBool()) //random because destroyer
                                selectedTarget = i;
                        }
                    }

                    if (selectedTarget != -1) //shoot
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, 10f * Projectile.DirectionTo(Main.npc[selectedTarget].Center),
                            ProjectileID.MiniRetinaLaser, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        SoundEngine.PlaySound(SoundID.Item12, Projectile.Center);
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 2f;
                dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 60, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f, 100);
                Main.dust[dust].velocity *= 2f;
            }
            if (!Main.dedServ)
            {
                int g = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity / 2, ModContent.Find<ModGore>("FargowiltasSouls/DestroyerBody").Type, Projectile.scale);
                Main.gore[g].timeLeft = 20;
            }
        }
    }
}