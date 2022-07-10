using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles
{
    public class LightningVortexHostile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vortex");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
            return false;
        }
        Color DrawColor = Color.Cyan;

        bool useVanillaLightning;
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            if (source is EntitySource_Parent parent && parent.Entity is NPC sourceNPC && sourceNPC.type == NPCID.MoonLordCore)
            {
                useVanillaLightning = true;
            }
        }

        public override void AI()
        {
            bool recolor = SoulConfig.Instance.BossRecolors && FargoSoulsWorld.EternityMode;
            if ((NPC.AnyNPCs(NPCID.TheDestroyer) && recolor) || (FargowiltasSouls.Instance.TryFind("MutantBoss", out ModNPC mutant) && NPC.AnyNPCs(mutant.Type)))
                DrawColor = new Color(231, 174, 254);

            int shadertype = (DrawColor == new Color(231, 174, 254)) ? 100 : 0; //if it's recolored, use a shader for all the dusts spawned so they're purple instead of cyan
            Player player = FargoSoulsUtil.PlayerExists(Projectile.localAI[1]);
            if (player == null && Projectile.ai[0] == 0)
                TargetEnemies();

            if (Projectile.localAI[0] < 90)
            {
                if (Projectile.ai[0] != 0)
                {
                    Vector2 rotationVector2 = Projectile.ai[1].ToRotationVector2();
                    rotationVector2.Normalize(); //Projectile.ai[1].ToRotationVector2();
                    Vector2 vector2_1 = rotationVector2.RotatedBy(1.57079637050629, new Vector2()) * (Main.rand.NextBool()).ToDirectionInt() * (float)Main.rand.Next(10, 21);
                    Vector2 vector2_2 = (rotationVector2 * Main.rand.Next(-80, 81) - vector2_1) / 10f;
                    int Type = 229;
                    Dust d = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, Type, 0.0f, 0.0f, 0, new Color(), 1f)];
                    d.noGravity = true;
                    d.position = Projectile.Center + rotationVector2 * vector2_1.Length();
                    d.velocity = rotationVector2 * vector2_2.Length() * 3f;
                    d.scale = 1.5f;
                }
            }

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] <= 50)
            {
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;

                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 4f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
                if (Main.rand.NextBool(4))
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 2f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                }
            }
            else if (Projectile.localAI[0] <= 90)
            {
                Projectile.scale = (Projectile.localAI[0] - 50) / 40;
                Projectile.alpha = 255 - (int)(255 * Projectile.scale);
                Projectile.rotation = Projectile.rotation - 0.1570796f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }

                if (Projectile.localAI[0] == 90 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vector2_3 = 24f * (player != null && Projectile.ai[0] == 0 ? Projectile.DirectionTo(player.Center) : Projectile.ai[1].ToRotationVector2());
                    float ai1New = (Main.rand.NextBool()) ? 1 : -1; //randomize starting direction
                    int type = useVanillaLightning ? ProjectileID.VortexLightning : ModContent.ProjectileType<HostileLightning>();
                    int p = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, vector2_3,
                        type, Projectile.damage, Projectile.knockBack, Projectile.owner,
                        vector2_3.ToRotation(), ai1New * 0.75f);
                    if (p != Main.maxProjectiles && !useVanillaLightning)
                        Main.projectile[p].localAI[1] = shadertype; //change projectile's ai if the recolored vortex portal is being used, so that purple ones always fire purple lightning
                }
            }
            else if (Projectile.localAI[0] <= 120)
            {
                Projectile.scale = 1f;
                Projectile.alpha = 0;
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 60f;
                if (Main.rand.NextBool())
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * Main.rand.Next(10, 21);
                    dust.velocity = spinningpoint.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
                else
                {
                    Vector2 spinningpoint = Vector2.UnitY.RotatedByRandom(6.28318548202515);
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                    dust.noGravity = true;
                    dust.position = Projectile.Center - spinningpoint * 30f;
                    dust.velocity = spinningpoint.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = Projectile.Center;
                }
            }
            else
            {
                Projectile.scale = (float)(1.0 - (Projectile.localAI[0] - 120.0) / 60.0);
                Projectile.alpha = 255 - (int)(255 * Projectile.scale);
                Projectile.rotation = Projectile.rotation - (float)Math.PI / 30f;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                for (int index = 0; index < 2; ++index)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            Vector2 spinningpoint1 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust1 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint1 * 30f, 0, 0, 229, 0.0f, 0.0f, 0, DrawColor, 1f)];
                            dust1.noGravity = true;
                            dust1.position = Projectile.Center - spinningpoint1 * Main.rand.Next(10, 21);
                            dust1.velocity = spinningpoint1.RotatedBy(1.57079637050629, new Vector2()) * 6f;
                            dust1.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                            dust1.scale = 0.5f + Main.rand.NextFloat();
                            dust1.fadeIn = 0.5f;
                            dust1.customData = Projectile.Center;
                            break;
                        case 1:
                            Vector2 spinningpoint2 = Vector2.UnitY.RotatedByRandom(6.28318548202515) * Projectile.scale;
                            Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center - spinningpoint2 * 30f, 0, 0, 240, 0.0f, 0.0f, 0, DrawColor, 1f)];
                            dust2.noGravity = true;
                            dust2.position = Projectile.Center - spinningpoint2 * 30f;
                            dust2.velocity = spinningpoint2.RotatedBy(-1.57079637050629, new Vector2()) * 3f;
                            dust2.shader = GameShaders.Armor.GetSecondaryShader(shadertype, Main.LocalPlayer);
                            dust2.scale = 0.5f + Main.rand.NextFloat();
                            dust2.fadeIn = 0.5f;
                            dust2.customData = Projectile.Center;
                            break;
                    }
                }
            }
        }

        private void TargetEnemies()
        {
            float maxDistance = 1000f;
            int possibleTarget = -1;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && Collision.CanHitLine(Projectile.Center, 0, 0, player.Center, 0, 0))
                {
                    float Distance = Projectile.Distance(player.Center);
                    if (Distance < maxDistance)
                    {
                        maxDistance = Distance;
                        possibleTarget = i;
                    }
                }
            }
            Projectile.localAI[1] = possibleTarget;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(DrawColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}