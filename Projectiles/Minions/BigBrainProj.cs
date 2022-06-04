using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class BigBrainProj : ModProjectile
    {
        public const int MaxMinionSlots = 11;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Brain");
            Main.projFrames[Projectile.type] = 12;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            EModeGlobalProjectile.IgnoreMinionNerf[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
            /*Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;*/
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            if (player.dead) modPlayer.BigBrainMinion = false;
            if (modPlayer.BigBrainMinion) Projectile.timeLeft = 2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 12;
            }

            float slotsModifier = MaxMinionSlots / 7f * Math.Min(Projectile.minionSlots / MaxMinionSlots, 1f);

            Projectile.ai[0] += 0.1f + 0.3f * slotsModifier;
            Projectile.alpha = (int)(Math.Cos(Projectile.ai[0] / 0.4f * MathHelper.TwoPi / 180) * 60) + 60;

            float oldScale = Projectile.scale;
            Projectile.scale = 0.75f + 0.5f * slotsModifier;

            Projectile.position = Projectile.Center;
            Projectile.width = (int)(Projectile.width * Projectile.scale / oldScale);
            Projectile.height = (int)(Projectile.height * Projectile.scale / oldScale);
            Projectile.Center = Projectile.position;

            NPC targetnpc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000, center: Main.player[Projectile.owner].MountedCenter));
            bool targetting = targetnpc != null; //targetting code, prioritize targetted npcs, then look for closest if none is found

            if (targetting)
            {
                if (++Projectile.localAI[0] > 5)
                {
                    Projectile.localAI[0] = 0;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        const float speed = 18f;
                        int damage = (int)(Projectile.originalDamage * Projectile.scale); //damage directly proportional to Projectile scale, change later???
                        int type = ModContent.ProjectileType<BigBrainIllusion>();

                        //Vector2 spawnpos = targetnpc.Center + Main.rand.NextVector2CircularEdge(150, 150);
                        //Projectile.NewProjectile(spawnpos, speed * Vector2.Normalize(targetnpc.Center - spawnpos), type, damage, Projectile.knockBack, Projectile.owner, Projectile.scale);

                        Vector2 spawnFromMe = Main.player[Projectile.owner].Center + (Projectile.Center - Main.player[Projectile.owner].Center).RotatedBy(MathHelper.TwoPi / 4 * Main.rand.Next(4));
                        Vector2 vel = speed * Vector2.Normalize(targetnpc.Center + targetnpc.velocity * 15 - spawnFromMe);
                        FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), spawnFromMe, vel, type, damage, Projectile.knockBack, Projectile.owner, Projectile.scale);
                    }
                }
            }

            Projectile.Center = player.Center + new Vector2(0, (200 + Projectile.alpha) * Projectile.scale).RotatedBy(Projectile.ai[1] + Projectile.ai[0] / MathHelper.TwoPi);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * Projectile.scale);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 6;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i <= 3; i++) //simulate collision of 4 Projectiles orbiting because i didnt want to make orbiting illusions seperate Projectiles, also makes collision with scale changes better
            {
                Player player = Main.player[Projectile.owner];
                Vector2 newCenter = player.Center + new Vector2(0, (200 + Projectile.alpha) * Projectile.scale).RotatedBy((i * MathHelper.PiOver2) + Projectile.ai[1] + Projectile.ai[0] / MathHelper.TwoPi);
                int width = (int)(Projectile.scale * Projectile.width);
                int height = (int)(Projectile.scale * Projectile.height);
                Rectangle newprojhitbox = new Rectangle((int)newCenter.X - width / 2, (int)newCenter.Y - height / 2, width, height);
                if (newprojhitbox.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameheight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle rectangle = new Rectangle(0, Projectile.frame * frameheight, texture.Width, frameheight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, rectangle.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            for (int i = 1; i <= 3; i++)
            {
                Player player = Main.player[Projectile.owner];
                Vector2 newCenter = player.Center + new Vector2(0, (200 + Projectile.alpha) * Projectile.scale).RotatedBy((i * MathHelper.PiOver2) + Projectile.ai[1] + Projectile.ai[0] / MathHelper.TwoPi);
                Color newcolor = lightColor * Projectile.Opacity * Projectile.Opacity * Projectile.Opacity * (Main.mouseTextColor / 255f);

                Main.EntitySpriteDraw(texture, newCenter - Main.screenPosition, new Rectangle?(rectangle), newcolor, Projectile.rotation, rectangle.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}