using FargowiltasSouls.Buffs.Masomode;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.Champions
{
    public class SpiritSpirit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.hostile = true;
            Projectile.scale = 0.8f;
        }

        public override void AI()
        {
            if (--Projectile.ai[1] < 0 && Projectile.ai[1] > -300)
            {
                NPC npc = FargoSoulsUtil.NPCExists(Projectile.ai[0], ModContent.NPCType<NPCs.Champions.SpiritChampion>());
                if (npc != null)
                {
                    Player p = Main.player[npc.target];
                    if (Projectile.Distance(p.Center) > 200 && npc.ai[0] == 3)
                    {
                        for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
                        {
                            Vector2 change = Projectile.DirectionTo(p.Center) * 2.2f;
                            Projectile.velocity = (Projectile.velocity * 29f + change) / 30f;
                        }
                    }
                    else //stop homing when in certain range of player, or npc leaves this mode
                    {
                        Projectile.ai[1] = -300;
                    }
                }
                else
                {
                    Projectile.ai[0] = Player.FindClosest(Projectile.Center, 0, 0);
                }
            }
            else if (Projectile.ai[1] < -300 && Projectile.velocity.Length() < 2.2f)
            {
                Projectile.velocity *= 1.022f;
            }

            for (int i = 0; i < 3; i++) //make up for real spectre bolt having 3 extraUpdates
            {
                Projectile.position += Projectile.velocity;

                /*for (int j = 0; j < 5; ++j)
                {
                    Vector2 vel = Projectile.velocity * 0.2f * j;
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 175, 0f, 0f, 100, default, 1.3f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = Vector2.Zero;
                    Main.dust[d].position -= vel;
                }*/
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (FargoSoulsWorld.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Infested>(), 360);
                target.AddBuff(ModContent.BuffType<ClippedWings>(), 180);
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
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            for (float i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i += 0.2f)
            {
                Player player = Main.player[Projectile.owner];
                Texture2D glow = texture2D13; //FargowiltasSouls.Instance.Assets.Request<Texture2D>("Projectiles/BossWeapons/HentaiSpearSpinGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Color color27 = color26; //Color.Lerp(new Color(255, 255, 0, 210), Color.Transparent, 0.4f);
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                float scale = Projectile.scale;
                scale *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                int max0 = (int)i - 1;//Math.Max((int)i - 1, 0);
                if (max0 < 0)
                    continue;
                Vector2 center = Vector2.Lerp(Projectile.oldPos[(int)i], Projectile.oldPos[max0], 1 - i % 1);
                float smoothtrail = i % 1 * (float)Math.PI / 6.85f;

                center += Projectile.Size / 2;

                Main.EntitySpriteDraw(
                    glow,
                    center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                    null,
                    color27,
                    Projectile.rotation,
                    glow.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0);
            }
            return false;
        }
    }
}