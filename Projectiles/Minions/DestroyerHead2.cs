using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class DestroyerHead2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_134";

        public float modifier;
        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Destroyer Head");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
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
            Projectile.alpha = 0;
            Projectile.netImportant = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(modifier);

            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            modifier = reader.ReadSingle();

            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return lightColor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/Minions/DestroyerHead2_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int num214 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y6 = num214 * Projectile.frame;
            Color color25 = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
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

            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                float minionSlotsUsed = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && !Main.projectile[i].hostile && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].minionSlots > 0)
                        minionSlotsUsed += Main.projectile[i].minionSlots;
                }

                float modifier = Main.player[Projectile.owner].maxMinions - minionSlotsUsed;
                if (modifier < 0)
                    modifier = 0;
                if (modifier > 5)
                    modifier = 5;

                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.netUpdate = true;

                    int current = Projectile.whoAmI;
                    for (int i = 0; i <= modifier * 3; i++)
                        current = FargoSoulsUtil.NewSummonProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<DestroyerBody2>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner, Main.projectile[current].identity);
                    int previous = current;
                    current = FargoSoulsUtil.NewSummonProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<DestroyerTail2>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner, Main.projectile[current].identity);
                    Main.projectile[previous].localAI[1] = Main.projectile[current].identity;
                    Main.projectile[previous].netUpdate = true;
                }
            }

            //keep the head looking right
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
            Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            const int aislotHomingCooldown = 0;
            const int homingDelay = 30;
            float desiredFlySpeedInPixelsPerFrame = 40 + modifier * 4;
            float amountOfFramesToLerpBy = 30 + 30 - modifier * 6; // minimum of 1, please keep in full numbers even though it's a float!

            Projectile.ai[aislotHomingCooldown]++;
            if (Projectile.ai[aislotHomingCooldown] > homingDelay)
            {
                Projectile.ai[aislotHomingCooldown] = homingDelay; //cap this value 

                if (Projectile.Distance(mousePos) > 50)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(mousePos) * desiredFlySpeedInPixelsPerFrame;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / amountOfFramesToLerpBy);
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 6;
            target.AddBuff(ModContent.BuffType<LightningRod>(), Main.rand.Next(300, 1200));
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
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);
        }
    }
}