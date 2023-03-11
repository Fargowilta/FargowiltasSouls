using FargowiltasSouls.Buffs.Boss;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Projectiles.BossWeapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.MutantBoss
{
    public class MutantSpearAim : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Projectiles/MutantBoss/MutantSpear"; //"FargowiltasSouls/Projectiles/BossWeapons/HentaiSpear";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Penetrator");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 0;
            Projectile.timeLeft = 60;
            CooldownSlot = 1;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().TimeFreezeImmune = true;
            Projectile.GetGlobalProjectile<FargoSoulsGlobalProjectile>().DeletionImmuneRank = 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float length = 200;
            float dummy = 0f;
            Vector2 offset = length / 2 * Projectile.scale * (Projectile.rotation - MathHelper.ToRadians(135f)).ToRotationVector2();
            Vector2 end = Projectile.Center - offset;
            Vector2 tip = Projectile.Center + offset;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), end, tip, 8f * Projectile.scale, ref dummy))
                return true;

            return false;
        }

        /* -1: direct, green, 3sec for rapid p2 toss
         * 0: direct, green, 1sec (?)
         * 1: direct, green, 0.5sec for rapid p2 toss
         * 2: predictice, blue, 1sec for p2 destroyer throw
         * 3: predictive, blue, 1.5sec for p1
         * 4: predictive, blue, 1.5sec with no more tracking after a bit for p2 slow dash finisher (maybe red?)
         * 5: predictive, NONE, 1sec for p2 slow dash
         */

        public override void AI()
        {
            //basically: ai1 > 1 means predictive and blue glow, otherwise direct aim and green glow

            NPC mutant = Main.npc[(int)Projectile.ai[0]];
            if (mutant.active && mutant.type == ModContent.NPCType<NPCs.MutantBoss.MutantBoss>())
            {
                Projectile.Center = mutant.Center;

                if (Projectile.localAI[0] == 0) //ensure it faces the right way on tick 1
                    Projectile.rotation = mutant.DirectionTo(Main.player[mutant.target].Center).ToRotation();

                if (Projectile.ai[1] > 1)
                {
                    if (!(Projectile.ai[1] == 4 && Projectile.timeLeft < System.Math.Abs(Projectile.localAI[1]) + 5))
                        Projectile.rotation = Projectile.rotation.AngleLerp(mutant.DirectionTo(Main.player[mutant.target].Center + Main.player[mutant.target].velocity * 30).ToRotation(), 0.2f);
                }
                else
                {
                    Projectile.rotation = mutant.DirectionTo(Main.player[mutant.target].Center).ToRotation();
                }
            }
            else
            {
                Projectile.Kill();
            }

            if (Projectile.localAI[0] == 0) //modifying timeleft for mp sync, localAI1 changed to adjust the rampup on the glow tell
            {
                Projectile.localAI[0] = 1;

                if (Projectile.ai[1] == -1) //extra long startup on p2 direct throw
                {
                    Projectile.timeLeft += 120;
                    Projectile.localAI[1] = -120;
                }
                else if (Projectile.ai[1] == 1) //p2 direct throw rapid fire
                {
                    Projectile.timeLeft -= 30;
                    Projectile.localAI[1] = 30;
                }
                else if (Projectile.ai[1] == 3) //p1 predictive throw
                {
                    Projectile.timeLeft += 30;
                    Projectile.localAI[1] = -30;
                }
                else if (Projectile.ai[1] == 4)
                {
                    Projectile.timeLeft += 20;
                    Projectile.localAI[1] = -20;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Projectile.NewProjectile(Projectile.InheritSource(Projectile), target.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, ModContent.ProjectileType<PhantasmalBlast>(), 0, 0f, Projectile.owner);
            if (FargoSoulsWorld.EternityMode)
            {
                target.GetModPlayer<FargoSoulsPlayer>().MaxLifeReduction += 100;
                target.AddBuff(ModContent.BuffType<OceanicMaul>(), 5400);
                target.AddBuff(ModContent.BuffType<MutantFang>(), 180);
            }
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 600);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            float rotationOffset = MathHelper.ToRadians(135f);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i] + rotationOffset;
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation + rotationOffset, origin2, Projectile.scale, SpriteEffects.None, 0);

            if (Projectile.ai[1] != 5)
            {
                Texture2D glow = FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/MutantBoss/MutantSpearAimGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                float modifier = Projectile.timeLeft / (60f - Projectile.localAI[1]);
                Color glowColor = new Color(51, 255, 191, 210);
                if (Projectile.ai[1] > 1)
                    glowColor = new Color(0, 0, 255, 210);
                //if (Projectile.ai[1] == 4) glowColor = new Color(255, 0, 0, 210);
                glowColor *= 1f - modifier;
                float glowScale = Projectile.scale * 8f * modifier;
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(glow.Bounds), glowColor, 0, glow.Bounds.Size() / 2, glowScale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}