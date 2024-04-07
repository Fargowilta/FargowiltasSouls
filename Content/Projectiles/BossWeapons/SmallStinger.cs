using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.BossWeapons
{
    public class SmallStinger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Small Stinger");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.HornetStinger);
            AIType = ProjectileID.Bullet;
            Projectile.penetrate = -1;
            Projectile.minion = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 120;
            Projectile.width = 10;
            Projectile.height = 18;
            Projectile.scale *= 1.5f;
            Projectile.height = (int)(Projectile.height * 1.5f);
            Projectile.width = (int)(Projectile.width * 1.5f);
        }

        public override void AI()
        {
            //stuck in enemy
            if (Projectile.ai[0] == 1)
            {
                Projectile.aiStyle = -1;

                Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                int secondsStuck = 15;
                bool kill = false;

                Projectile.localAI[0] += 1f;

                int npcIndex = (int)Projectile.ai[1];
                if (Projectile.localAI[0] >= 60 * secondsStuck)
                {
                    kill = true;
                }
                else if (npcIndex < 0 || npcIndex >= 200)
                {
                    kill = true;
                }
                else if (Main.npc[npcIndex].active && !Main.npc[npcIndex].dontTakeDamage)
                {
                    Projectile.Center = Main.npc[npcIndex].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcIndex].gfxOffY;
                }
                else
                {
                    kill = true;
                }

                if (kill)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.position += Projectile.velocity * 0.5f;

                //dust from stinger
                if (Main.rand.NextBool())
                {
                    int num92 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, 0f, 0f, 0, default, 0.9f);
                    Main.dust[num92].noGravity = true;
                    Main.dust[num92].velocity *= 0.5f;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableCrit();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.type == Projectile.type && p != Projectile && Projectile.Hitbox.Intersects(p.Hitbox))
                {
                    target.SimpleStrikeNPC(Projectile.damage / 2, 0, noPlayerInteraction: true, crit: false);
                    // target.StrikeNPC(damage / 2, 0, 0, true); //normal damage but looks like a crit ech
                    target.AddBuff(BuffID.Poisoned, 600);
                    DustRing(p, 16);
                    p.Kill();
                    SoundEngine.PlaySound(SoundID.Item27 with { Pitch = -0.4f }, Projectile.Center);
                    break;
                }
            }

            Projectile.ai[0] = 1;
            Projectile.ai[1] = target.whoAmI;
            Projectile.velocity = (Main.npc[target.whoAmI].Center - Projectile.Center) * 1f; //distance it sticks out
            Projectile.aiStyle = -1;
            Projectile.damage = 0;
            Projectile.timeLeft = 300;
            Projectile.netUpdate = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int num92 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, Projectile.velocity.X, Projectile.velocity.Y, 0, default, 0.9f);
                Main.dust[num92].noGravity = true;
                Main.dust[num92].velocity *= 0.25f;
                Main.dust[num92].fadeIn = 1.3f;
            }
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }

        private static void DustRing(Projectile proj, int max)
        {
            //dust
            for (int i = 0; i < max; i++)
            {
                Vector2 vector6 = Vector2.UnitY * 5f;
                vector6 = vector6.RotatedBy((i - (max / 2 - 1)) * 6.28318548f / max) + proj.Center;
                Vector2 vector7 = vector6 - proj.Center;
                int d = Dust.NewDust(vector6 + vector7, 0, 0, DustID.CorruptGibs, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = vector7;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color color25 = Lighting.GetColor((int)(Projectile.position.X + Projectile.width * 0.5) / 16, (int)((Projectile.position.Y + Projectile.height * 0.5) / 16.0));
            Texture2D texture2D3 = TextureAssets.Projectile[Projectile.type].Value;
            int num156 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y3 = num156 * Projectile.frame;
            Rectangle rectangle = new(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 7;
            int num159 = 0;
            float num160 = 0f;


            int num161 = num159;
            while (Projectile.ai[0] != 1 && num161 < num157) //doesnt draw trail while stuck in enemy
            {
                Color color26 = color25;
                color26 = Projectile.GetAlpha(color26);
                float num164 = num157 - num161;
                color26 *= num164 / (ProjectileID.Sets.TrailCacheLength[Projectile.type] * 1.5f);
                color26 *= 0.75f;
                Vector2 value4 = Projectile.oldPos[num161];
                float num165 = Projectile.rotation;
                SpriteEffects effects = spriteEffects;
                Main.EntitySpriteDraw(texture2D3, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + Projectile.rotation * num160 * (num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, Projectile.scale * 0.8f, effects, 0);
                num161++;
            }

            Color color29 = Projectile.GetAlpha(color25);
            Main.EntitySpriteDraw(texture2D3, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color29, Projectile.rotation, origin2, Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            //smaller tile hitbox
            width = 8;
            height = 8;
            return true;
        }
    }
}