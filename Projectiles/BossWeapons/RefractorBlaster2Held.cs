using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class RefractorBlaster2Held : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/SwarmDrops/RefractorBlaster2";

        private int syncTimer;
        private Vector2 mousePos;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Diffractor Blaster");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 76;
            Projectile.height = 38;
            //Projectile.aiStyle = 136;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 8;
            Projectile.tileCollide = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.netImportant = true;
        }

        public int timer;
        public float lerp = 0.12f;

        public override bool? CanDamage()
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(mousePos.X);
            writer.Write(mousePos.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 buffer;
            buffer.X = reader.ReadSingle();
            buffer.Y = reader.ReadSingle();
            if (Projectile.owner != Main.myPlayer)
            {
                mousePos = buffer;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active)
                Projectile.Kill();

            if (Main.player[Projectile.owner].HeldItem.type == ModContent.ItemType<Items.Weapons.SwarmDrops.RefractorBlaster2>())
            {
                Projectile.damage = Main.player[Projectile.owner].GetWeaponDamage(Main.player[Projectile.owner].HeldItem);
                Projectile.knockBack = Main.player[Projectile.owner].GetWeaponKnockback(Main.player[Projectile.owner].HeldItem, Main.player[Projectile.owner].HeldItem.knockBack);
            }

            Vector2 center = player.MountedCenter;

            Projectile.Center = center;
            Projectile.rotation = Projectile.velocity.ToRotation();

            float extrarotate = ((Projectile.direction * player.gravDir) < 0) ? MathHelper.Pi : 0;
            float itemrotate = Projectile.direction < 0 ? MathHelper.Pi : 0;
            player.itemRotation = Projectile.velocity.ToRotation() + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 10;
            player.itemAnimation = 10;
            Vector2 HoldOffset = new Vector2(Projectile.width / 3, 0).RotatedBy(MathHelper.WrapAngle(Projectile.velocity.ToRotation()));

            Projectile.Center += HoldOffset;
            Projectile.spriteDirection = Projectile.direction * (int)player.gravDir;
            Projectile.rotation -= extrarotate;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                if (Projectile.frame > Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;

                Projectile.frameCounter = 0;
            }

            Projectile.velocity = Vector2.Lerp(Vector2.Normalize(Projectile.velocity),
                Vector2.Normalize(mousePos - player.MountedCenter), lerp); //slowly move towards direction of cursor
            Projectile.velocity.Normalize();

            if (Projectile.owner == Main.myPlayer)
            {
                mousePos = Main.MouseWorld;

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.Center += Projectile.velocity * 20;
                return;
            }

            if (player.channel)
            {
                timer++;
                if (timer % 6 == 0)
                {
                    if (player.inventory[player.selectedItem].UseSound != null)
                        SoundEngine.PlaySound(player.inventory[player.selectedItem].UseSound.Value, Projectile.Center);
                    bool checkmana = player.CheckMana(player.inventory[player.selectedItem].mana, true, false);
                    if (!checkmana)
                        Projectile.Kill();

                }
                if (timer > 60)
                {
                    int type = ModContent.ProjectileType<DarkStarFriendly>();
                    const int max = 10;
                    double spread = MathHelper.PiOver4 / max;
                    for (int i = -max; i <= max; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + HoldOffset * 2, 22f * Projectile.velocity.RotatedBy(spread * i),
                            type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                    SoundEngine.PlaySound(SoundID.Item105 with { Pitch = -0.3f }, Projectile.Center);
                    /*int p = Projectile.NewProjectile(Projectile.Center + HoldOffset * 2, Projectile.velocity * 22, type, Projectile.damage, Projectile.knockBack, player.whoAmI);
					if (p < 1000)
					{
						SplitProj(Main.Projectile[p], 21);
					}*/
                    timer = 0;
                }
                Projectile.timeLeft++;

                if (Projectile.ai[1] == 0)
                {
                    int type = ModContent.ProjectileType<PrimeDeathray>();

                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, Projectile.identity);

                    if (p < Main.maxProjectiles)
                    {
                        SplitProj(Main.projectile[p], 17);
                    }
                    Projectile.ai[1]++;
                }
                else if (player.ownedProjectileCounts[ModContent.ProjectileType<PrimeDeathray>()] < 12)
                {
                    Projectile.Kill();
                }
            }

            Projectile.Center += Projectile.velocity * 20;

            if (!player.channel)
            {
                Projectile.Kill();
            }
        }

        public static void SplitProj(Projectile Projectile, int number)
        {
            //if its odd, we just keep the original 
            if (number % 2 != 0)
            {
                number--;
            }

            double spread = MathHelper.Pi / 2 / number;

            for (int i = 2; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = (j == 0) ? 1 : -1;
                    float ai0 = (Projectile.type == ModContent.ProjectileType<PrimeDeathray>()) ? (i + 1) * factor : 0;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(factor * spread * (i + 1)), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner,
                        ai0, Projectile.ai[1]);
                }
            }

            Projectile.active = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int height = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int width = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Width;
            int frame = height * Projectile.frame;
            SpriteEffects flipdirection = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle Origin = new Rectangle(0, frame, width, height);
            Main.spriteBatch.Draw(texture2D, Projectile.Center - Main.screenPosition, Origin, lightColor, Projectile.rotation, new Vector2(width / 2, height / 2), Projectile.scale, flipdirection, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Items/Weapons/SwarmDrops/RefractorBlaster2_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int height = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int width = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Width;
            int frame = height * Projectile.frame;
            SpriteEffects flipdirection = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle Origin = new Rectangle(0, frame, width, height);
            Main.spriteBatch.Draw(texture2D, Projectile.Center - Main.screenPosition, Origin, Color.White, Projectile.rotation, new Vector2(width / 2, height / 2), Projectile.scale, flipdirection, 0);
        }
    }
}