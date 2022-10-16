using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class EaterHead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eater Head");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            EModeGlobalProjectile.IgnoreMinionNerf[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.netImportant = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            int num214 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            int y6 = num214 * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture2D13.Width, num214),
                color25, Projectile.rotation, new Vector2(texture2D13.Width / 2f, num214 / 2f), Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            if ((int)Main.time % 120 == 0) Projectile.netUpdate = true;
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            int num1038 = 10;
            if (player.dead) modPlayer.EaterMinion = false;
            if (modPlayer.EaterMinion) Projectile.timeLeft = 2;
            num1038 = 30;

            Vector2 center = player.Center;
            float num1040 = 300f;
            float num1041 = 400f;
            int num1042 = -1;
            if (Projectile.Distance(center) > 1800f)
            {
                Projectile.Center = center;
                Projectile.netUpdate = true;
            }

            bool flag66 = true;
            if (flag66)
            {
                NPC ownerMinionAttackTargetNPC5 = Projectile.OwnerMinionAttackTargetNPC;
                if (ownerMinionAttackTargetNPC5 != null && ownerMinionAttackTargetNPC5.CanBeChasedBy(Projectile, false))
                {
                    float num1043 = Projectile.Distance(ownerMinionAttackTargetNPC5.Center);
                    if (num1043 < num1040 * 3f)
                    {
                        num1042 = ownerMinionAttackTargetNPC5.whoAmI;
                        if (ownerMinionAttackTargetNPC5.boss)
                        {
                            int arg_2D352_0 = ownerMinionAttackTargetNPC5.whoAmI;
                        }
                        else
                        {
                            int arg_2D35E_0 = ownerMinionAttackTargetNPC5.whoAmI;
                        }
                    }
                }

                if (num1042 < 0)
                    for (int num1044 = 0; num1044 < Main.maxNPCs; num1044++)
                    {
                        NPC nPC13 = Main.npc[num1044];
                        if (nPC13.CanBeChasedBy(Projectile, false)
                            && (player.Distance(nPC13.Center) < num1041 || Projectile.Distance(nPC13.Center) < num1041))
                        {
                            float num1045 = Projectile.Distance(nPC13.Center);
                            if (num1045 < num1040)
                            {
                                num1042 = num1044;
                                bool arg_2D3CE_0 = nPC13.boss;
                            }
                        }
                    }
            }

            const int blockTrackDownAllowanceRange = 4;

            bool IsInTile(Vector2 pos)
            {
                Tile tile = Framing.GetTileSafely(pos);
                return tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
            }

            float playerGroundCompareHeight = player.Center.Y;
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = new Vector2(player.Center.X, playerGroundCompareHeight);
                if (IsInTile(pos))
                    break;
                playerGroundCompareHeight += 16;
            }

            bool belowPlayer = Projectile.Center.Y > playerGroundCompareHeight;
            bool canMove = false;
            if (belowPlayer || Projectile.Distance(Main.player[Projectile.owner].Center) > 1200f)
            {
                canMove = true;
            }
            else
            {
                for (int i = 0; i < blockTrackDownAllowanceRange; i++)
                {
                    if (IsInTile(Projectile.Top + Vector2.UnitY * 16 * i))
                    {
                        canMove = true;
                        break;
                    }
                }
            }

            if (!canMove)
            {
                //float fastfallComparePoint = num1042 == -1 ? player.Center.Y : Main.npc[num1042].Center.Y;
                Projectile.velocity.Y += 0.40f; //Projectile.Center.Y > fastfallComparePoint ? 0.25f : 0.50f;
                if (Projectile.velocity.Y > 16f)
                    Projectile.velocity.Y = 16f;

                if (Projectile.velocity.Y > 4f)
                {
                    if (Projectile.velocity.X < 0)
                        Projectile.velocity.X += 0.09f;
                    else
                        Projectile.velocity.X -= 0.09f;
                }
                else if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 16f * 0.4f)
                {
                    if (Projectile.velocity.X < 0)
                        Projectile.velocity.X -= 0.11f;
                    else
                        Projectile.velocity.X += 0.11f;
                }
            }
            else if (num1042 != -1)
            {
                NPC nPC14 = Main.npc[num1042];
                Vector2 vector132 = nPC14.Center - Projectile.Center;
                (vector132.X > 0f).ToDirectionInt();
                (vector132.Y > 0f).ToDirectionInt();
                float scaleFactor15 = 0.4f;
                if (vector132.Length() < 600f) scaleFactor15 = 0.6f;
                if (vector132.Length() < 300f) scaleFactor15 = 0.8f;
                if (vector132.Length() > nPC14.Size.Length() * 0.75f)
                {
                    Projectile.velocity += Vector2.Normalize(vector132) * scaleFactor15 * 1.5f;
                    if (Vector2.Dot(Projectile.velocity, vector132) < 0.25f) Projectile.velocity *= 0.8f;
                }

                float num1046 = 30f;
                if (Projectile.velocity.Length() > num1046) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * num1046;
            }
            else
            {
                center = player.Bottom;

                float num1047 = 0.2f;
                Vector2 vector133 = center - Projectile.Center;
                if (vector133.Length() < 200f) num1047 = 0.12f;
                if (vector133.Length() < 140f) num1047 = 0.06f;
                if (vector133.Length() > 100f || Math.Abs(vector133.Y) > 16 * blockTrackDownAllowanceRange / 2)
                {
                    if (Math.Abs(center.X - Projectile.Center.X) > 20f) Projectile.velocity.X = Projectile.velocity.X + num1047 * Math.Sign(center.X - Projectile.Center.X);
                    if (Math.Abs(center.Y - Projectile.Center.Y) > 10f) Projectile.velocity.Y = Projectile.velocity.Y + num1047 * Math.Sign(center.Y - Projectile.Center.Y);
                }
                else if (Projectile.velocity.Length() > 2f)
                {
                    Projectile.velocity *= 0.96f;
                }

                if (Math.Abs(Projectile.velocity.Y) < 1f) Projectile.velocity.Y = Projectile.velocity.Y - 0.1f;
                float num1048 = 15f;
                if (Projectile.velocity.Length() > num1048) Projectile.velocity = Vector2.Normalize(Projectile.velocity) * num1048;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57079637f;
            int direction = Projectile.direction;
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            if (direction != Projectile.direction) Projectile.netUpdate = true;
            float num1049 = MathHelper.Clamp(Projectile.localAI[0], 0f, 50f);
            Projectile.position = Projectile.Center;
            Projectile.scale = 1f + num1049 * 0.01f;
            Projectile.width = Projectile.height = (int)(num1038 * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 42;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            Projectile.position -= Projectile.velocity / 2;
        }
    }
}