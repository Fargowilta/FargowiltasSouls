using FargowiltasSouls.Content.Items.Misc;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Bosses.Champions.Cosmos
{
    public class CosmosForceMoon : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Bosses/Champions/Cosmos/CosmosMoon";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cosmic Moon");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 410;
            Projectile.height = 410;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Generic;

            Projectile.extraUpdates = 0;

            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;

            Projectile.scale = 0f;
            Projectile.Opacity = 0f;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }

        ref float State => ref Projectile.ai[1];
        public override bool? CanHitNPC(NPC target)
        {
            if (State == 0)
                return false;
            return base.CanHitNPC(target);
        }
        public override void AI()
        {
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.1f);
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.2f, 0.1f);
            Projectile.width = 150;
            Projectile.height = 150;

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;

                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

                Projectile.rotation = Main.rand.NextFloat(MathF.Tau);
            }
            if (!Projectile.owner.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[Projectile.owner];
            if (!player.Alive())
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.Distance(player.Center) > 1200)
            {
                Projectile.Kill();
                return;
            }

            if (State == 0)
            {
                Projectile.tileCollide = false;
                if (player.HeldItem != null && player.HeldItem.damage > 0 && player.controlUseItem) //it's being held
                {
                    int distance = 160;
                    float spinFrames = 90;
                    var moons = Main.projectile.Where(p => p.TypeAlive(Type) && p.owner == player.whoAmI && p.ai[1] == 0).ToList();
                    float offset = moons.IndexOf(Projectile) * MathF.Tau / moons.Count;
                    float rotation = MathF.Tau * (Main.GameUpdateCount % spinFrames) / spinFrames;

                    Projectile.Center = player.Center + Vector2.UnitX.RotatedBy(offset + rotation) * distance;
                }
                else // player stops holding it
                {
                    State = 1;
                    if (Main.myPlayer == player.whoAmI)
                    {
                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ThrowShort"), Projectile.Center);
                        Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 2;
                        Projectile.netUpdate = true;
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                    }
                        
                    
                }
            }
            else
            {
                Projectile.extraUpdates = 1;
                Projectile.Opacity = 1;
                Projectile.scale = 0.2f;
                Projectile.tileCollide = true;
                Projectile.velocity *= 1.06f;
                Projectile.velocity = Projectile.velocity.ClampLength(0, 20f);
            }
            

            Projectile.rotation += 0.04f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = (int)(Projectile.width / Math.Sqrt(2));
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnKill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            SoundEngine.PlaySound(SoundID.Item89, Projectile.position);

            if (!Main.dedServ && Main.LocalPlayer.active)
                ScreenShakeSystem.StartShake(4, shakeStrengthDissipationIncrement: 4f / 30);

            if (Projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), Projectile.damage, 0f, Main.myPlayer);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].FargoSouls().CanSplit = false;

                int solar = ModContent.ItemType<SolarBooster>();
                int vortex = ModContent.ItemType<VortexBooster>();
                int nebula = ModContent.ItemType<NebulaBooster>();
                int stardust = ModContent.ItemType<StardustBooster>();

                List<int> possibleBoosters = [solar, vortex, nebula, stardust];
                if (Main.item.Any(i => i.active && i.type == solar) || Main.LocalPlayer.GetModPlayer<BoosterPlayer>().SolarTimer > 0)
                    possibleBoosters.Remove(solar);
                if (Main.item.Any(i => i.active && i.type == vortex) || Main.LocalPlayer.GetModPlayer<BoosterPlayer>().VortexTimer > 0)
                    possibleBoosters.Remove(vortex);
                if (Main.item.Any(i => i.active && i.type == nebula) || Main.LocalPlayer.GetModPlayer<BoosterPlayer>().NebulaTimer > 0)
                    possibleBoosters.Remove(nebula);
                if (Main.item.Any(i => i.active && i.type == stardust) || Main.LocalPlayer.GetModPlayer<BoosterPlayer>().StardustTimer > 0)
                    possibleBoosters.Remove(stardust);
                if (possibleBoosters.Count == 0)
                    possibleBoosters = [solar, vortex, nebula, stardust];
                int itemType = Main.rand.NextFromCollection(possibleBoosters);
                Item.NewItem(Projectile.GetSource_FromThis(), Projectile.Hitbox, itemType, noGrabDelay: true);
            }


            const int num226 = 15;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.UnitX * 40f;
                vector6 = vector6.RotatedBy((num227 - (num226 / 2 - 1)) * 6.28318548f / num226, default) + Projectile.Center;
                Vector2 vector7 = vector6 - Projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, DustID.Torch, 0f, 0f, 0, default, 3f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].velocity = vector7;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color glow = new Color(Main.DiscoR + 210, Main.DiscoG + 210, Main.DiscoB + 210) * Projectile.Opacity;
            Color glow2 = new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50) * Projectile.Opacity;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
    }
}