using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class SkeletronArmR : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_36";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skeletron Hand");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 52;
            Projectile.height = 52;
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

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead && player.FargoSouls().SkeletronArms)
                Projectile.timeLeft = 2;

            //if (Projectile.damage == 0)
            //{
            //    Projectile.damage = 18;
            //    if (player.FargoSouls().SupremeDeathbringerFairy)
            //        Projectile.damage = 24;
            //    if (player.FargoSouls().MasochistSoul)
            //        Projectile.damage = 48;
            //    Projectile.damage = (int)(Projectile.damage * player.GetDamage(DamageClass.Summon));
            //}

            //tentacle head movement (homing)
            Vector2 playerVel = player.position - player.oldPosition;
            Projectile.position += playerVel;
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 0f)
            {
                Vector2 home = player.Center;
                home.X += 200f;
                home.Y -= 50f;
                Vector2 distance = home - Projectile.Center;
                float range = distance.Length();
                distance.Normalize();
                if (Projectile.ai[0] == 0f)
                {
                    if (range > 15f)
                    {
                        Projectile.ai[0] = -1f; //if in fast mode, stay fast until back in range
                        if (range > 1300f)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    else
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 3f + Main.rand.NextFloat(3f);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    distance /= 8f;
                }
                if (range > 120f) //switch to fast return mode
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
                Projectile.velocity += distance;
                if (range > 30f)
                    Projectile.velocity *= 0.96f;

                if (Projectile.ai[0] > 90f) //attack nearby enemy
                {
                    Projectile.ai[0] = 20f;
                    NPC npc = FargoSoulsUtil.NPCExists(FargoSoulsUtil.FindClosestHostileNPCPrioritizingMinionFocus(Projectile, 400));
                    if (npc != null)
                    {
                        Projectile.velocity = npc.Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 16f;
                        Projectile.velocity += npc.velocity / 2f;
                        Projectile.velocity -= playerVel / 2f;
                        Projectile.ai[0] *= -1f;
                    }
                    Projectile.netUpdate = true;
                }
            }

            Vector2 angle = player.Center - Projectile.Center;
            angle.X += 200f;
            angle.Y += 180f;
            Projectile.rotation = (float)Math.Atan2(angle.Y, angle.X) + (float)Math.PI / 2f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Math.Sign(target.Center.X - Main.player[Projectile.owner].Center.X);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}