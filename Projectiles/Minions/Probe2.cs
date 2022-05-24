using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class Probe2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_139";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Probe");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.timeLeft *= 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().Probes)
                Projectile.timeLeft = 2;

            Projectile.ai[0] -= (float)Math.PI / 60f;
            Projectile.Center = player.Center + new Vector2(-60, 0).RotatedBy(Projectile.ai[0]);

            if (Projectile.ai[1] >= 0f && Projectile.ai[1] < Main.maxNPCs)
            {
                NPC npc = Main.npc[(int)Projectile.ai[1]];
                Projectile.rotation = (npc.Center - Projectile.Center).ToRotation();
                if (npc.CanBeChasedBy() && Collision.CanHitLine(npc.Center, 0, 0, Main.player[Projectile.owner].Center, 0, 0))
                {
                    if (--Projectile.localAI[0] < 0f)
                    {
                        Projectile.localAI[0] = player.GetModPlayer<FargoSoulsPlayer>().MasochistSoul ? 15f : 30f;
                        if (Projectile.owner == Main.myPlayer)
                            FargoSoulsUtil.NewSummonProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(8f, 0f).RotatedBy(Projectile.rotation),
                                ModContent.ProjectileType<ProbeLaser>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    Projectile.ai[1] = -1f;
                    Projectile.netUpdate = true;
                }
                Projectile.rotation += (float)Math.PI;
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                    Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation() + (float)Math.PI;
            }

            if (++Projectile.localAI[1] > 20f)
            {
                Projectile.localAI[1] = 0f;
                Projectile.ai[1] = FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 1000f, true);
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}