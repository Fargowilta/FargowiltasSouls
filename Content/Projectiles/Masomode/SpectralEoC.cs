using FargowiltasSouls.Core;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Masomode
{
    public class SpectralEoC : ModProjectile
    {
        const string EoCName = "NPC_4";
        public override string Texture => $"FargowiltasSouls/Assets/ExtraTextures/Resprites/{EoCName}";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sky Dragon's Fury");
            Main.projFrames[Projectile.type] = Main.npcFrameCount[NPCID.EyeofCthulhu];

            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.alpha = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60 * 60;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;

            Projectile.extraUpdates = 1;
        }

        public override bool? CanDamage() => false;
        public ref float Timer => ref Projectile.ai[0];

        int alphaCounter = 0;
        bool FinalPhaseBerserkDashesComplete;
        int FinalPhaseDashCD;
        bool FinalPhaseDashHorizSpeedSet;
        int FinalPhaseDashStageDuration;
        int FinalPhaseAttackCounter;

        Vector2 targetCenter = Vector2.Zero;
        public override void AI()
        {
            if (Projectile.frame < 2)
                Projectile.frame = 3;
            if (++Projectile.frameCounter > 4)
            {
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 3;
            }



            const float speedModifier = 0.3f;
            int npcTarget = (int)Projectile.ai[1];
            if (!npcTarget.IsWithinBounds(Main.maxPlayers))
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[npcTarget];
            if (player == null || !player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }
            if (targetCenter == Vector2.Zero)
                targetCenter = player.Center;

            if ((!Main.dayTime || Main.zenithWorld || Main.remixWorld))
            {
                if (Projectile.timeLeft < 300)
                    Projectile.timeLeft = 300;
            }
            else //despawn and retarget
            {
                Projectile.Kill();
            }

            Timer++;
            if (false)//++Timer == 1) //teleport to random position
            {
                /*
                if (FargoSoulsUtil.HostCheck)
                {
                    Projectile.Center = Main.player[Projectile.target].Center;
                    Projectile.position.X += Main.rand.NextBool() ? -600 : 600;
                    Projectile.position.Y += Main.rand.NextBool() ? -400 : 400;
                    Projectile.TargetClosest(false);
                    Projectile.netUpdate = true;
                    NetSync(npc);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SpectralEoC>(), 0, 0, Main.myPlayer, Timer);
                }
                */
            }
            else if (Timer < 90) //fade in, moving into position
            {
                Projectile.alpha -= WorldSavingSystem.MasochistModeReal ? 5 : 4;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }

                const float PI = (float)Math.PI;
                if (Projectile.rotation > PI)
                    Projectile.rotation -= 2 * PI;
                if (Projectile.rotation < -PI)
                    Projectile.rotation += 2 * PI;

                float targetRotation = Projectile.SafeDirectionTo(targetCenter).ToRotation() - PI / 2;
                if (targetRotation > PI)
                    targetRotation -= 2 * PI;
                if (targetRotation < -PI)
                    targetRotation += 2 * PI;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.07f);

                for (int i = 0; i < 3; i++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].noLight = true;
                    Main.dust[d].velocity *= 4f;
                }

                Vector2 target = targetCenter;
                target.X += Projectile.Center.X < target.X ? -600 : 600;
                target.Y += Projectile.Center.Y < target.Y ? -400 : 400;

                /*
                if (Projectile.Center.X < target.X)
                {
                    Projectile.velocity.X += speedModifier;
                    if (Projectile.velocity.X < 0)
                        Projectile.velocity.X += speedModifier * 2;
                }
                else
                {
                    Projectile.velocity.X -= speedModifier;
                    if (Projectile.velocity.X > 0)
                        Projectile.velocity.X -= speedModifier * 2;
                }
                if (Projectile.Center.Y < target.Y)
                {
                    Projectile.velocity.Y += speedModifier;
                    if (Projectile.velocity.Y < 0)
                        Projectile.velocity.Y += speedModifier * 2;
                }
                else
                {
                    Projectile.velocity.Y -= speedModifier;
                    if (Projectile.velocity.Y > 0)
                        Projectile.velocity.Y -= speedModifier * 2;
                }
                if (Math.Abs(Projectile.velocity.X) > 24)
                    Projectile.velocity.X = 24 * Math.Sign(Projectile.velocity.X);
                if (Math.Abs(Projectile.velocity.Y) > 24)
                    Projectile.velocity.Y = 24 * Math.Sign(Projectile.velocity.Y);
                */
                Projectile.velocity = Vector2.Zero;
            }
            else if (!FinalPhaseBerserkDashesComplete) //berserk dashing phase
            {
                Timer = 90;

                const float xSpeed = 18f;
                const float ySpeed = 40f;

                if (++FinalPhaseDashCD == 1)
                {


                    if (!FinalPhaseDashHorizSpeedSet) //only set this on the first dash of each set
                    {
                        FinalPhaseDashHorizSpeedSet = true;
                        Projectile.velocity.X = Projectile.Center.X < targetCenter.X ? xSpeed : -xSpeed;
                    }

                    Projectile.velocity.Y = Projectile.Center.Y < targetCenter.Y ? ySpeed : -ySpeed; //alternate this every dash

                    //ScytheSpawnTimer = 30;
                    //if (WorldSavingSystem.MasochistModeReal)
                    //    SpawnServants();
                    //if (FargoSoulsUtil.HostCheck)
                    //FargoSoulsUtil.XWay(8, Projectile.GetSource_FromThis(), Projectile.Center, ModContent.ProjectileType<BloodScythe>(), 1f, FargoSoulsUtil.ScaledProjectileDamage(Projectile.damage), 0);

                    Projectile.netUpdate = true;
                }
                else if (FinalPhaseDashCD > 20)
                {
                    FinalPhaseDashCD = 0;
                }

                if (FinalPhaseDashStageDuration == 1)
                {
                    SoundEngine.PlaySound(SoundID.ForceRoarPitched with { Volume = 0.2f, Pitch = 0.5f }, targetCenter);
                }

                if (++FinalPhaseDashStageDuration > 600 * 3 / xSpeed + 5) //proceed
                {
                    //ScytheSpawnTimer = 0;
                    FinalPhaseDashStageDuration = 0;
                    FinalPhaseBerserkDashesComplete = true;
                    if (!WorldSavingSystem.MasochistModeReal)
                        FinalPhaseAttackCounter++;
                    Projectile.velocity *= 0.75f;
                    Projectile.netUpdate = true;
                }

                const float PI = (float)Math.PI;
                Projectile.rotation = Projectile.velocity.ToRotation() - PI / 2;
                if (Projectile.rotation > PI)
                    Projectile.rotation -= 2 * PI;
                if (Projectile.rotation < -PI)
                    Projectile.rotation += 2 * PI;
            }
            else
            {
                bool mustRest = FinalPhaseAttackCounter >= 3;

                const int restingTime = 240;

                int threshold = 180;
                if (mustRest)
                    threshold += restingTime;

                if (mustRest && Timer < restingTime + 90)
                {
                    if (Timer == 91)
                        Projectile.velocity = Projectile.SafeDirectionTo(player.Center) * Projectile.velocity.Length() * 0.75f;

                    Projectile.velocity.X *= 0.98f;
                    if (Math.Abs(Projectile.Center.X - player.Center.X) < 300)
                        Projectile.velocity.X *= 0.9f;

                    bool floatUp = Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
                    if (!floatUp && Projectile.Bottom.X > 0 && Projectile.Bottom.X < Main.maxTilesX * 16 && Projectile.Bottom.Y > 0 && Projectile.Bottom.Y < Main.maxTilesY * 16)
                    {
                        Tile tile = Framing.GetTileSafely(Projectile.Bottom);
                        if (tile != null && tile.HasUnactuatedTile)
                            floatUp = Main.tileSolid[tile.TileType];
                    }

                    if (floatUp)
                    {
                        Projectile.velocity.X *= 0.95f;

                        Projectile.velocity.Y -= speedModifier;
                        if (Projectile.velocity.Y > 0)
                            Projectile.velocity.Y = 0;
                        if (Math.Abs(Projectile.velocity.Y) > 24)
                            Projectile.velocity.Y = 24 * Math.Sign(Projectile.velocity.Y);
                    }
                    else
                    {
                        Projectile.velocity.Y += speedModifier;
                        if (Projectile.velocity.Y < 0)
                            Projectile.velocity.Y += speedModifier * 2;
                        if (Projectile.velocity.Y > 15)
                            Projectile.velocity.Y = 15;
                    }
                }
                else
                {
                    alphaCounter += WorldSavingSystem.MasochistModeReal ? 16 : 4;
                    if (alphaCounter > 255)
                    {
                        alphaCounter = 255;
                        if (WorldSavingSystem.MasochistModeReal && Timer < threshold)
                            Timer = threshold;
                    }

                    if (mustRest)
                    {
                        Projectile.velocity.Y -= speedModifier * 0.5f;
                        if (Projectile.velocity.Y > 0)
                            Projectile.velocity.Y = 0;
                        if (Math.Abs(Projectile.velocity.Y) > 24)
                            Projectile.velocity.Y = 24 * Math.Sign(Projectile.velocity.Y);
                    }
                    else
                    {
                        Projectile.velocity *= 0.98f;
                    }
                }

                const float PI = (float)Math.PI;
                float targetRotation = MathHelper.WrapAngle(Projectile.SafeDirectionTo(player.Center).ToRotation() - PI / 2);
                Projectile.rotation = MathHelper.WrapAngle(MathHelper.Lerp(Projectile.rotation, targetRotation, 0.07f));

                if (alphaCounter > 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 0, default, 1.5f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].velocity *= 4f;
                    }
                }

                if (Timer > threshold) //reset
                {
                    Projectile.Kill();
                    /*
                    Timer = 0;
                    FinalPhaseDashCD = 0;
                    FinalPhaseBerserkDashesComplete = false;
                    FinalPhaseDashHorizSpeedSet = false;
                    if (mustRest)
                        FinalPhaseAttackCounter = 0;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                    */
                }
            }
            /*
            if (Projectile.netUpdate)
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, Projectile.whoAmI);
                    NetSync(npc);
                }
                Projectile.netUpdate = false;
            }
            */

            //Projectile.position += player.velocity.Y * Vector2.UnitY;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            bool recolor = SoulConfig.Instance.BossRecolors && WorldSavingSystem.EternityMode;
            Texture2D tex = TextureAssets.Npc[NPCID.EyeofCthulhu].Value;
            int sizeY = tex.Height / Main.projFrames[Type]; //ypos of lower right corner of sprite to draw
            int frameY = Projectile.frame * sizeY;
            Rectangle rectangle = new(0, frameY, tex.Width, sizeY);
            Vector2 origin = rectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color baseColor = recolor ? Color.Cyan : Color.Red;
            Color color = baseColor with { A = 0 } * 0.13f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(tex, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27,
                    num165, origin, Projectile.scale, spriteEffects, 0);
            }

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(color),
                    Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
