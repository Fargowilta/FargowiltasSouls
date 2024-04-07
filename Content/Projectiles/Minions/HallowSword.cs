using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class HallowSword : ModProjectile
    {
        private Vector2 mousePos;
        private int syncTimer;

        public Vector2 handlePos = Vector2.Zero;
        private int HitsLeft = 0;
        public int SlashCDMax = 60 * 2;
        public const int MaxDistance = 300;
        public bool Reflected = false;
        ref float SlashCD => ref Projectile.ai[1];
        ref float Action => ref Projectile.ai[0];

        ref float SlashRotation => ref Projectile.localAI[0];
        ref float SlashArc => ref Projectile.localAI[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("HallowSword");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            //ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.CloneDefaults(ProjectileID.EmpressBlade);
            AIType = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.timeLeft = 18000;
            //Projectile.minionSlots = 0;
            Projectile.hide = false;

            Projectile.scale = 1;
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

        //actions:
        //0: idle
        //1: slashing
        //2: recovering
        public override void AI()
        {
            if (handlePos == Vector2.Zero)
            {
                handlePos = Projectile.position + Vector2.UnitY * Projectile.height;
            }
            Player player = Main.player[Projectile.owner];

            if (player.whoAmI == Main.myPlayer && (player.dead || !player.HasEffect<AncientHallowMinion>()))
            {
                Projectile.Kill();
                return;
            }

            //why does this exist if it's instantly cleared?????
            //List<int> ai156_blacklistedTargets = new();

            DelegateMethods.v3_1 = Color.Transparent.ToVector3();
            Point point2 = Projectile.Center.ToTileCoordinates();
            DelegateMethods.CastLightOpen(point2.X, point2.Y);

            Position(player);

            if (Action == 1)
            {
                Action = 2;
            }
            if (Action == 2)
            {
                Recover(player);
            }

            if (CheckRightClick(player) && SlashCD <= 0)
            {
                HitsLeft = 10;
                Slash(player);
            }
            //ai156_blacklistedTargets.Clear();

        }
        private void Position(Player player)
        {
            const int offsetX = 50;
            Vector2 offset = Vector2.UnitX * offsetX * Projectile.scale * GetSide(player) + Vector2.UnitY * 0;

            if (player.whoAmI == Main.myPlayer)
            {
                mousePos = MousePos(player);

                if (++syncTimer > 20)
                {
                    syncTimer = 0;
                    Projectile.netUpdate = true;
                }
            }


            Vector2 desiredPos = mousePos + offset;
            handlePos = Vector2.Lerp(handlePos, desiredPos, 0.5f);

            Vector2 desiredCenter = handlePos;// + (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2() * TextureAssets.Projectile[Projectile.type].Value.Width * Projectile.scale / 2;
            Projectile.velocity = (desiredCenter - Projectile.Center) / 3;

            if (Action == 0)
            {
                Projectile.rotation = Wobble();
            }
        }
        private float Wobble()
        {
            const int resist = 200;
            if (Projectile.velocity.ToRotation() > MathHelper.Pi)
            {
                return 0f - MathHelper.Pi * Projectile.velocity.X / resist;
            }
            else
            {
                return 0f + MathHelper.Pi * Projectile.velocity.X / resist;
            }
        }
        private void Slash(Player player)
        {
            Action = 1;
            Projectile.knockBack = 3;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            SlashRotation = Projectile.rotation;
            Projectile.rotation -= GetSide(player) * MathHelper.Pi * 0.9f;
            SlashArc = (Projectile.rotation - SlashRotation + MathHelper.Pi + MathHelper.TwoPi) % MathHelper.TwoPi - MathHelper.Pi;

            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
            Reflected = false;
            if (!player.HasBuff<HallowCooldownBuff>())
            {
                Reflect(Projectile);
            }
            SlashCDMax = Reflected ? 60 * 2 : 60;
            SlashCD = SlashCDMax;
        }
        private void Recover(Player player)
        {
            if (SlashCD <= (SlashCDMax) - 10)
            {
                float desiredRot = Wobble();
                RotateTowards(desiredRot, 2f / ((float)SlashCD / SlashCDMax));
            }
            if (SlashCD > 0)
            {
                SlashCD--;
                if (SlashCD <= 0)
                {
                    Action = 0;
                }
            }
        }
        private void RotateTowards(float rotToAlignWith, float speed)
        {
            Vector2 PV = rotToAlignWith.ToRotationVector2();
            Vector2 LV = Projectile.rotation.ToRotationVector2();
            float anglediff = (float)(Math.Atan2(PV.Y * LV.X - PV.X * LV.Y, LV.X * PV.X + LV.Y * PV.Y)); //real
                                                                                                         //change rotation towards target
            Projectile.rotation = Projectile.rotation.ToRotationVector2().RotatedBy(Math.Sign(anglediff) * Math.Min(Math.Abs(anglediff), speed * MathHelper.Pi / 180)).ToRotation();
        }
        private int GetSide(Player player)
        {
            return -Math.Sign(MousePos(player).X - player.Center.X);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            if (player == null || !player.active || player.dead || HitsLeft <= 0)
            {
                return false;
            }
            int width = TextureAssets.Projectile[Type].Value.Width;
            int height = TextureAssets.Projectile[Type].Value.Height;
            if (Action != 1)
            {
                return false;
            }
            if (projHitbox.ClosestPointInRect(handlePos).Distance(handlePos) > width * Projectile.scale) //optimization, don't do any laser checks if too far away
            {
                return false;
            }
            const int CollisionChecks = 25; //maybe excessive
            for (int i = 0; i < CollisionChecks; i++)
            {
                float frac = (float)i / CollisionChecks;
                float angle = SlashRotation + (SlashArc * frac) - (MathHelper.PiOver2);
                float num6 = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), handlePos, handlePos + angle.ToRotationVector2() * width * Projectile.scale, height * Projectile.scale, ref num6))
                {
                    HitsLeft--;
                    return true;
                }
            }
            return false;
        }
        private void Reflect(Projectile sword)
        {
            Player player = Main.player[sword.owner];
            if (player == null || !player.active)
            {
                return;
            }
            int damageCap = player.FargoSouls().ForceEffect<AncientHallowEnchant>() ? 200 : 150;

            foreach (Projectile p in Main.projectile.Where(p => p.active && p.hostile && p.damage > 0 && FargoSoulsUtil.CanDeleteProjectile(p) && p.damage <= damageCap && sword.Colliding(sword.Hitbox, p.Hitbox)))
            {
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/parrynmuse"), p.Center);
                Reflected = true;
                p.hostile = false;
                p.friendly = true;
                player.AddBuff(ModContent.BuffType<HallowCooldownBuff>(), 60 * 15);
                p.owner = sword.owner;
                p.damage = (int)(sword.damage * 1.5f);
                p.DamageType = sword.DamageType;
                const int speed = 30;
                Vector2 targetVel = Vector2.Normalize(-p.velocity) * speed; //by default, reverse velocity
                NPC targetNPC = p.GetSourceNPC();
                if (!(targetNPC != null && targetNPC.active && !targetNPC.townNPC)) //if cannot find source npc, send towards closest npc instead
                {
                    int target = p.FindTargetWithLineOfSight(800);
                    targetNPC = Main.npc[target];
                }
                if (targetNPC != null && targetNPC.active && !targetNPC.townNPC) //check if found any of the above npc checks, if so, send towards the npc
                {
                    targetVel = Vector2.Normalize(targetNPC.Center - p.Center) * speed;
                }
                p.velocity = targetVel;
                // Don't know if this will help but here it is
                p.netUpdate = true;

            }

        }
        private Vector2 MousePos(Player player)
        {
            return player.Center + (player.Center.SafeDirectionTo(Main.MouseWorld) * Math.Min((Main.MouseWorld - player.Center).Length(), MaxDistance));
        }
        private bool CheckRightClick(Player player)
        {
            return player.controlUseTile && !player.tileInteractionHappened && !player.mouseInterface && !CaptureManager.Instance.Active && !Main.HoveringOverAnNPC
                && !Main.SmartInteractShowingGenuine && PlayerInput.Triggers.Current.MouseRight;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EmpressBladeDrawer empressBladeDrawer = default;
            float num14 = Main.GlobalTimeWrappedHourly % 3f / 3f;
            Player player = Main.player[Projectile.owner];
            float num15 = MathHelper.Max(1f, player.maxMinions);
            float num16 = Projectile.identity % num15 / num15 + num14;
            Color fairyQueenWeaponsColor = Projectile.GetFairyQueenWeaponsColor(0f, 0f, new float?(num16 % 1f));
            Color fairyQueenWeaponsColor2 = Projectile.GetFairyQueenWeaponsColor(0f, 0f, new float?((num16 + 0.5f) % 1f));
            empressBladeDrawer.ColorStart = fairyQueenWeaponsColor;
            empressBladeDrawer.ColorEnd = fairyQueenWeaponsColor2;
            empressBladeDrawer.Draw(Projectile);
            DrawProj_EmpressBlade(Projectile, num16);

            return false;
        }

        private static void DrawProj_EmpressBlade(Projectile proj, float hueOverride)
        {
            Main.CurrentDrawnEntityShader = -1;
            PrepareDrawnEntityDrawing(proj, Main.GetProjectileDesiredShader(proj));
            Vector2 vector = proj.Center - Main.screenPosition;
            proj.GetFairyQueenWeaponsColor(0f, 0f, new float?(hueOverride));
            Color fairyQueenWeaponsColor = proj.GetFairyQueenWeaponsColor(0.5f, 0f, new float?(hueOverride));
            Texture2D value = TextureAssets.Projectile[proj.type].Value;
            vector += (proj.rotation - MathHelper.PiOver2).ToRotationVector2() * value.Width * proj.scale / 2;
            Vector2 origin = value.Frame(1, 1, 0, 0, 0, 0).Size() / 2f;
            //Vector2 origin = Vector2.UnitY * value.Height;
            Color color = Color.White * proj.Opacity;
            color.A = (byte)(color.A * 0.7f);
            fairyQueenWeaponsColor.A /= 2;
            float scale = proj.scale;
            float num = proj.rotation - 1.57079637f;
            float num2 = proj.Opacity * 0.3f;

            float ai0 = 0; //i have no fucking clue what this number does
            if (num2 > 0f)
            {
                float lerpValue = Utils.GetLerpValue(60f, 50f, ai0, true);
                float scale2 = Utils.GetLerpValue(70f, 50f, ai0, true) * Utils.GetLerpValue(40f, 45f, ai0, true);
                for (float num3 = 0f; num3 < 1f; num3 += 0.166666672f)
                {
                    Vector2 value2 = num.ToRotationVector2() * -120f * num3 * lerpValue;
                    Main.EntitySpriteDraw(value, vector + value2, null, fairyQueenWeaponsColor * num2 * (1f - num3) * scale2, num, origin, scale * 1.5f, SpriteEffects.None, 0);
                }
                for (float num4 = 0f; num4 < 1f; num4 += 0.25f)
                {
                    Vector2 value3 = (num4 * 6.28318548f + num).ToRotationVector2() * 4f * scale;
                    Main.EntitySpriteDraw(value, vector + value3, null, fairyQueenWeaponsColor * num2, num, origin, scale, SpriteEffects.None, 0);
                }
            }
            HallowSword sword = proj.ModProjectile != null && proj.ModProjectile is HallowSword ? proj.As<HallowSword>() : null;
            const float slashTime = 5;
            float fade = (float)(sword.SlashCD + slashTime - sword.SlashCDMax) / slashTime;
            Color fadeColor = Color.Lerp(Color.Transparent, Color.LightGoldenrodYellow with { A = 0 }, fade);
            if (sword != null && sword.Action == 2 && sword.SlashCD >= sword.SlashCDMax - slashTime)
            {
                const int SlashImages = 100; //lol
                for (int i = 0; i < SlashImages; i++)
                {
                    float frac = 1 - ((float)i / SlashImages);
                    float angle = sword.SlashRotation + (sword.SlashArc * frac);
                    Vector2 imagePos = proj.Center - Main.screenPosition + (angle - MathHelper.PiOver2).ToRotationVector2() * value.Width * proj.scale / 2;
                    float imageRot = angle - MathHelper.PiOver2;
                    Color imageColor = Color.Lerp(Color.Transparent, fadeColor, frac);
                    Main.EntitySpriteDraw(value, imagePos, null, imageColor, imageRot, origin, scale, SpriteEffects.None, 0);
                    //Main.EntitySpriteDraw(value, imagePos, null, fairyQueenWeaponsColor * num2 * 0.5f * (1f - frac), imageRot, origin, scale, SpriteEffects.None, 0);
                }
            }
            Main.EntitySpriteDraw(value, vector, null, color, num, origin, scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(value, vector, null, fairyQueenWeaponsColor * num2 * 0.5f, num, origin, scale, SpriteEffects.None, 0);
        }

        public static void PrepareDrawnEntityDrawing(Entity entity, int intendedShader)
        {
            Main.CurrentDrawnEntity = entity;
            if (intendedShader != 0)
            {
                if (Main.CurrentDrawnEntityShader == 0 || Main.CurrentDrawnEntityShader == -1)
                {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                }
            }
            else if (Main.CurrentDrawnEntityShader != 0 && Main.CurrentDrawnEntityShader != -1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
            Main.CurrentDrawnEntityShader = intendedShader;
        }
    }
}