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
    public class TerrariaSoulMoon : ModProjectile
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
            Projectile.localNPCHitCooldown = -1;

            Projectile.scale = 0f;
            Projectile.Opacity = 0f;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
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

                SoundEngine.PlaySound(SoundID.Item92 with { Volume = 0.5f }, Projectile.Center);

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

            Projectile.extraUpdates = 1;
            Projectile.Opacity = 1;
            Projectile.scale = 0.2f;
            Projectile.tileCollide = true;

            int npcID = (int)Projectile.ai[1];
            if (!npcID.IsWithinBounds(Main.maxNPCs))
                return;
            NPC target = Main.npc[npcID];
            if (!target.Alive())
                return;

            Vector2 idlePosition = target.Center;
            Vector2 toIdlePosition = idlePosition - Projectile.Center;
            float distance = toIdlePosition.Length();
            float speed = 25f;
            float inertia = 15f;
            toIdlePosition.Normalize();
            toIdlePosition *= speed;
            Projectile.velocity = (Projectile.velocity * (inertia - 1f) + toIdlePosition) / inertia;
            if (distance == 0)
                Projectile.velocity = Vector2.Zero;
            if (distance < Projectile.velocity.Length())
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * distance;
            if (Projectile.velocity == Vector2.Zero && distance > 10)
            {
                Projectile.velocity.X = -0.15f;
                Projectile.velocity.Y = -0.05f;
            }
            /*
            Projectile.velocity *= 1.06f;
            Projectile.velocity = Projectile.velocity.ClampLength(0, 20f);
            */

            Projectile.rotation += 0.04f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = (int)(Projectile.width / Math.Sqrt(2));
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override void OnKill(int timeLeft) //vanilla explosion code echhhhhhhhhhh
        {
            SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.5f }, Projectile.position);

            if (!Main.dedServ && Main.LocalPlayer.active)
                ScreenShakeSystem.StartShake(4, shakeStrengthDissipationIncrement: 4f / 30);

            if (Projectile.owner == Main.myPlayer)
            {
                int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(), Projectile.damage, 0f, Main.myPlayer);
                if (p != Main.maxProjectiles)
                    Main.projectile[p].FargoSouls().CanSplit = false;

                int timber = ModContent.ItemType<TimberBooster>();
                int terra = ModContent.ItemType<TerraBooster>();
                int earth = ModContent.ItemType<EarthBooster>();
                int nature = ModContent.ItemType<NatureBooster>();
                int life = ModContent.ItemType<LifeBooster>();
                int death = ModContent.ItemType<DeathBooster>();
                int spirit = ModContent.ItemType<SpiritBooster>();
                int will = ModContent.ItemType<WillBooster>();
                int cosmos = ModContent.ItemType<CosmosBooster>();

                BoosterPlayer boosterPlayer = Main.LocalPlayer.GetModPlayer<BoosterPlayer>();

                List<int> possibleBoosters = [timber, terra, earth, nature, life, death, spirit, will, cosmos];
                if (Main.item.Any(i => i.active && i.type == timber) || boosterPlayer.TimberTimer > 0)
                    possibleBoosters.Remove(timber);
                if (Main.item.Any(i => i.active && i.type == terra) || boosterPlayer.TerraTimer > 0)
                    possibleBoosters.Remove(terra);
                if (Main.item.Any(i => i.active && i.type == earth) || boosterPlayer.EarthTimer > 0)
                    possibleBoosters.Remove(earth);
                if (Main.item.Any(i => i.active && i.type == nature) || boosterPlayer.NatureTimer > 0)
                    possibleBoosters.Remove(nature);
                if (Main.item.Any(i => i.active && i.type == life) || boosterPlayer.LifeTimer > 0)
                    possibleBoosters.Remove(life);
                if (Main.item.Any(i => i.active && i.type == death) || boosterPlayer.DeathTimer > 0)
                    possibleBoosters.Remove(death);
                if (Main.item.Any(i => i.active && i.type == spirit) || boosterPlayer.SpiritTimer > 0)
                    possibleBoosters.Remove(spirit);
                if (Main.item.Any(i => i.active && i.type == will) || boosterPlayer.WillTimer > 0)
                    possibleBoosters.Remove(will);
                if (Main.item.Any(i => i.active && i.type == cosmos) || boosterPlayer.CosmosTimer > 0)
                    possibleBoosters.Remove(cosmos);
                if (possibleBoosters.Count == 0)
                    possibleBoosters = [timber, terra, earth, nature, life, death, spirit, will, cosmos];
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