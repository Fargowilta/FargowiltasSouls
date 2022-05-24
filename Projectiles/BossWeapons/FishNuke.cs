using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Projectiles.BossWeapons
{
    public class FishNuke : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fish Nuke");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            //AIType = ProjectileID.Bullet;
            Projectile.scale = 2f;
        }

        public override void AI()
        {
            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < Main.maxNPCs)
            {
                int ai0 = (int)Projectile.ai[0];
                if (Main.npc[ai0].CanBeChasedBy())
                {
                    double num4 = (Main.npc[ai0].Center - Projectile.Center).ToRotation() - Projectile.velocity.ToRotation();
                    if (num4 > Math.PI)
                        num4 -= 2.0 * Math.PI;
                    if (num4 < -1.0 * Math.PI)
                        num4 += 2.0 * Math.PI;
                    Projectile.velocity = Projectile.velocity.RotatedBy(num4 * 0.1f);
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                if (++Projectile.localAI[1] > 12f)
                {
                    Projectile.localAI[1] = 0f;
                    Projectile.ai[0] = FargoSoulsUtil.FindClosestHostileNPC(Projectile.Center, 500, true);
                    Projectile.netUpdate = true;
                }
            }

            if (++Projectile.localAI[0] >= 24f)
            {
                Projectile.localAI[0] = 0f;
                const int max = 18;
                for (int index1 = 0; index1 < max; ++index1)
                {
                    Vector2 vector2 = (Vector2.UnitX * (float)-Projectile.width / 2f + -Vector2.UnitY.RotatedBy((double)index1 * 2 * 3.14159274101257 / max, new Vector2()) * new Vector2(8f, 16f)).RotatedBy((double)Projectile.rotation - 1.57079637050629, new Vector2());
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, 135, 0.0f, 0.0f, 160, new Color(), 1f);
                    Main.dust[index2].scale = 2f;
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].position = Projectile.Center + vector2 * 2f;
                    Main.dust[index2].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[index2].position) * 1.25f;
                    //Main.dust[index2].velocity *= 2f;
                }
            }
            Vector2 vector21 = Vector2.UnitY.RotatedBy(Projectile.rotation, new Vector2()) * 8f * 2;
            int index21 = Dust.NewDust(Projectile.Center, 0, 0, 6, 0.0f, 0.0f, 0, new Color(), 1f);
            Main.dust[index21].position = Projectile.Center + vector21;
            Main.dust[index21].scale = 1.25f;
            Main.dust[index21].noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        /*public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.whoAmI == NPCs.FargoSoulsGlobalNPC.fishBossEX)
            {
                target.life += damage;
                if (target.life > target.lifeMax)
                    target.life = target.lifeMax;
                CombatText.NewText(target.Hitbox, CombatText.HealLife, damage);
                damage = 0;
                crit = false;
            }
        }*/

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            /*if (damage < target.lifeMax / 25)
                damage = target.lifeMax / 25;
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FishNukeExplosion>(),
                    damage, Projectile.knockBack * 2f, Projectile.owner);*/

            /*target.AddBuff(ModContent.BuffType<OceanicMaul>(), 900);
            target.AddBuff(ModContent.BuffType<MutantNibble>(), 900);
            target.AddBuff(ModContent.BuffType<CurseoftheMoon>(), 900);*/
            target.AddBuff(BuffID.Frostburn, 300);
        }

        /*public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FishNukeExplosion>(),
                    Projectile.damage, Projectile.knockBack * 2f, Projectile.owner);
            return true;
        }*/

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
            {
                int modifier = Main.rand.NextBool() ? 1 : -1;
                SpawnRazorbladeRing(6, 17f, 1f * -modifier);
                SpawnRazorbladeRing(6, 17f, 0.5f * modifier);
                /*const int max = 16;
                Vector2 baseVel = Vector2.UnitX.RotatedByRandom(2 * Math.PI);
                for (int i = 0; i < max; i++)
                {
                    float speed = i % 2 == 0 ? 16f : 14f;
                    Projectile.NewProjectile(Projectile.Center, speed * baseVel.RotatedBy(2 * Math.PI / max * i),
                        ModContent.ProjectileType<RazorbladeTyphoonFriendly>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }*/
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FishNukeExplosion>(), Projectile.damage / 2, Projectile.knockBack * 2f, Projectile.owner);
            }
            int num1 = 36;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                Vector2 vector2_1 = (Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f).RotatedBy((double)(index1 - (num1 / 2 - 1)) * 6.28318548202515 / (double)num1, new Vector2()) + Projectile.Center;
                Vector2 vector2_2 = vector2_1 - Projectile.Center;
                int index2 = Dust.NewDust(vector2_1 + vector2_2, 0, 0, 172, vector2_2.X * 2f, vector2_2.Y * 2f, 100, new Color(), 1.4f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = true;
                Main.dust[index2].velocity = vector2_2;
            }
        }

        private void SpawnRazorbladeRing(int max, float speed, float rotationModifier)
        {
            float rotation = 2f * (float)Math.PI / max;
            Vector2 vel = Vector2.UnitX.RotatedByRandom(2 * Math.PI); //Projectile.velocity; vel.Normalize();
            vel *= speed;
            int type = ModContent.ProjectileType<RazorbladeTyphoonFriendly>();
            for (int i = 0; i < max; i++)
            {
                vel = vel.RotatedBy(rotation);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, type, Projectile.damage / 2,
                    Projectile.knockBack, Projectile.owner, rotationModifier, 6f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}