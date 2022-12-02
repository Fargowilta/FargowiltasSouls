using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Minions
{
    public class Probe1 : ModProjectile
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
            if (player.whoAmI == Main.myPlayer && player.active && !player.dead && player.GetModPlayer<FargoSoulsPlayer>().Probes)
                Projectile.timeLeft = 2;

            Projectile.ai[0] -= (float)Math.PI / 60f;
            Projectile.Center = player.Center + new Vector2(60, 0).RotatedBy(Projectile.ai[0]);

            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[1] > 0)
                {
                    Projectile.ai[1]--;

                    if (Projectile.ai[1] % 10 == 0)
                    {
                        List<NPC> npcs = Main.npc.Where(n => n.CanBeChasedBy() && Projectile.Distance(n.Center) < 1200 && Collision.CanHitLine(Projectile.Center, 0, 0, n.Center, 0, 0)).ToList();
                        if (npcs.Count > 0)
                        {
                            NPC npc = Main.rand.Next(npcs);
                            Projectile.rotation = Projectile.DirectionTo(npc.Center).ToRotation();
                            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(16f, 0f).RotatedBy(Projectile.rotation).RotatedByRandom(MathHelper.PiOver4 / 2),
                                ModContent.ProjectileType<LightningArc>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.rotation, Main.rand.Next(100));
                            if (p != Main.maxProjectiles)
                                Main.projectile[p].DamageType = Projectile.DamageType;
                            Projectile.rotation += MathHelper.Pi;
                        }
                    }
                }
                else
                {
                    Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();

                    if (--Projectile.localAI[0] < 0f)
                    {
                        if (player.controlUseItem && player.HeldItem.damage > 0 && player.HeldItem.pick == 0 && player.HeldItem.hammer == 0 && player.HeldItem.axe == 0)
                        {
                            Projectile.localAI[0] = player.GetModPlayer<FargoSoulsPlayer>().MasochistSoul ? 15f : 30f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(8f, 0f).RotatedBy(Projectile.rotation),
                                ModContent.ProjectileType<ProbeLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Projectile.netUpdate = true;
                        }
                        else
                        {
                            Projectile.localAI[0] = 0f;
                        }
                    }

                    Projectile.rotation += MathHelper.Pi;
                }

                if (++Projectile.localAI[1] > 20f) //needed for rotation sync in multi
                {
                    Projectile.localAI[1] = 0f;
                    Projectile.netUpdate = true;
                }
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