using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class NekomiAxe : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Items/Weapons/FinalUpgrades/SparklingLove";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sparkling Love");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true; //block tungsten
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 65;
            Projectile.aiStyle = -1;
            Projectile.scale = 4f;
            Projectile.penetrate = -1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().CanSplit = false;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().noInteractionWithNPCImmunityFrames = true;
        }

        public override bool? CanDamage() => Projectile.timeLeft < 5;

        public override void AI()
        {
            //the important part
            int ai1 = (int)Projectile.ai[1];
            int byIdentity = FargoSoulsUtil.GetProjectileByIdentity(Projectile.owner, ai1, ModContent.ProjectileType<NekomiDevi>());
            if (byIdentity != -1)
            {
                Projectile devi = Main.projectile[byIdentity];
                if (Projectile.timeLeft > 15)
                {
                    Vector2 offset = new Vector2(0, -360).RotatedBy(Math.PI / 4 * devi.spriteDirection);
                    Projectile.Center = devi.Center + offset;
                    Projectile.rotation = (float)Math.PI / 4 * devi.spriteDirection - (float)Math.PI / 4;
                }
                else //swinging down
                {
                    if (Projectile.timeLeft == 15) //confirm facing the right direction with right offset
                        Projectile.rotation = (float)Math.PI / 4 * devi.spriteDirection - (float)Math.PI / 4;

                    Projectile.rotation -= (float)Math.PI / 15 * devi.spriteDirection * 0.75f;
                    Vector2 offset = new Vector2(0, -360).RotatedBy(Projectile.rotation + (float)Math.PI / 4);
                    Projectile.Center = devi.Center + offset;
                }

                Projectile.spriteDirection = -devi.spriteDirection;

                Projectile.localAI[1] = devi.velocity.ToRotation();

                if (Projectile.localAI[0] == 0)
                {
                    Projectile.localAI[0] = 1;
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -14);
                    SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

                    MakeDust();
                }
            }
            else if (Projectile.owner == Main.myPlayer && Projectile.timeLeft < 60)
            {
                Projectile.Kill();
                return;
            }
        }

        private void MakeDust()
        {
            const int scaleCounter = 3;

            Vector2 start = Projectile.width * Vector2.UnitX.RotatedBy(Projectile.rotation - (float)Math.PI / 4);
            if (Math.Abs(start.X) > Projectile.width / 2) //bound it so its always inside Projectile's hitbox
                start.X = Projectile.width / 2 * Math.Sign(start.X);
            if (Math.Abs(start.Y) > Projectile.height / 2)
                start.Y = Projectile.height / 2 * Math.Sign(start.Y);
            int length = (int)start.Length();
            start = Vector2.Normalize(start);
            float scaleModifier = scaleCounter / 3f + 0.5f;
            for (int j = -length; j <= length; j += 80)
            {
                Vector2 dustPoint = Projectile.Center + start * j;
                dustPoint.X -= 23;
                dustPoint.Y -= 23;

                /*for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier * 2f);
                    Main.dust[dust].velocity *= 1.4f * scaleModifier;
                }*/

                for (int index1 = 0; index1 < 15; ++index1)
                {
                    int index2 = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier * 2.5f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 16f * scaleModifier;
                    int index3 = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), scaleModifier);
                    Main.dust[index3].velocity *= 8f * scaleModifier;
                    Main.dust[index3].noGravity = true;
                }

                for (int i = 0; i < 5; i++)
                {
                    int d = Dust.NewDust(dustPoint, 46, 46, 86, 0f, 0f, 0, new Color(), Main.rand.NextFloat(1f, 2f) * scaleModifier);
                    Main.dust[d].velocity *= Main.rand.NextFloat(1f, 4f) * scaleModifier;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.timeLeft < 15)
                crit = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Lovestruck, 300);

            if (FargoSoulsUtil.NPCExists(target.realLife) != null)
            {
                foreach (NPC n in Main.npc.Where(n => n.active && (n.realLife == target.realLife || n.whoAmI == target.realLife) && n.whoAmI != target.whoAmI))
                {
                    Projectile.perIDStaticNPCImmunity[Projectile.type][n.whoAmI] = Main.GameUpdateCount + (uint)Projectile.idStaticNPCHitCooldown;
                }
            }
        }

        public override void Kill(int timeleft)
        {
            if (!Main.dedServ && Main.LocalPlayer.active)
                Main.LocalPlayer.GetModPlayer<FargoSoulsPlayer>().Screenshake = 60;

            MakeDust();

            for (int i = -1; i <= 1; i += 2)
            {
                for (int j = 0; j < 50; j++)
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Smoke, i * 3f, 0f, 50, default, 3f);
                    Main.dust[d].noGravity = Main.rand.NextBool();
                    Main.dust[d].velocity *= Main.rand.NextFloat(3f);
                }

                for (int j = 0; j < 15; j++)
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64));
                    Main.gore[gore].velocity.X += j / 3f * i;
                    Main.gore[gore].velocity.Y += Main.rand.NextFloat(2f);
                }
            }

            for (int j = 0; j < 15; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    int d = Dust.NewDust(Projectile.Center, 0, 0, DustID.Smoke, 0, 0f, 50, default, 4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity.Y -= 3f;
                    Main.dust[d].velocity *= Main.rand.NextFloat(3f);
                }

                if (Main.rand.NextBool(3))
                {
                    int gore = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default(Vector2), Main.rand.Next(61, 64), 0.5f);
                    Main.gore[gore].velocity.Y -= 3f;
                    Main.gore[gore].velocity *= Main.rand.NextFloat(2f);
                }
            }

            SoundEngine.PlaySound(SoundID.NPCDeath6 with { Volume = 1.5f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item92 with { Volume = 1.5f }, Projectile.Center);

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0f, Main.myPlayer, -1, -14);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.DD2OgreSmash, 0, 0f, Main.myPlayer);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            float rotationOffset = Projectile.spriteDirection > 0 ? 0 : (float)Math.PI / 2;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Terraria.Graphics.Shaders.ArmorShaderData shader = Terraria.Graphics.Shaders.GameShaders.Armor.GetShaderFromItemId(ItemID.PinkDye);
            shader.Apply(Projectile, new Terraria.DataStructures.DrawData?());

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = new Color(255, 255, 255, 50) * 0.5f;
                if (Projectile.timeLeft > 15)
                    color27 *= 0.5f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i] + rotationOffset;
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation + rotationOffset, origin2, Projectile.scale, effects, 0);
            Texture2D texture2D14 = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Items/Weapons/FinalUpgrades/SparklingLove_glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Main.EntitySpriteDraw(texture2D14, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation + rotationOffset, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}