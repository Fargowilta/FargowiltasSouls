using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Minions
{
    public class OpticLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_100";

        public int targetID = -1;
        public int searchTimer = 3;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Death Laser");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetID);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetID = reader.ReadInt32();
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DeathLaser);
            AIType = ProjectileID.DeathLaser;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120 * (Projectile.extraUpdates + 1);

            /*Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;*/
        }

        /*public override void AI()
        {
            if (Projectile.timeLeft < 60 * (Projectile.extraUpdates + 1)) //stop homing
                return;

            if (targetID == -1) //no target atm
            {
                if (searchTimer == 0) //search every few ticks
                {
                    searchTimer = 6;

                    int possibleTarget = -1;
                    float closestDistance = 500f;

                    for (int i = 0; i < 200; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (npc.active && npc.chaseable && npc.lifeMax > 5 && !npc.dontTakeDamage && !npc.friendly && !npc.immortal)
                        {
                            float distance = Vector2.Distance(Projectile.Center, npc.Center);

                            if (closestDistance > distance)
                            {
                                closestDistance = distance;
                                possibleTarget = i;
                            }
                        }
                    }

                    if (possibleTarget != -1)
                    {
                        targetID = possibleTarget;
                        Projectile.netUpdate = true;
                    }
                }
                searchTimer--;
            }
            else //currently have target
            {
                NPC npc = Main.npc[targetID];

                if (npc.active && npc.chaseable && !npc.dontTakeDamage && npc.immune[Projectile.owner] == 0) //target is still valid
                {
                    Vector2 distance = npc.Center - Projectile.Center;
                    double angle = distance.ToRotation() - Projectile.velocity.ToRotation();
                    if (angle > Math.PI)
                        angle -= 2.0 * Math.PI;
                    if (angle < -Math.PI)
                        angle += 2.0 * Math.PI;

                    if (Projectile.ai[0] == -1)
                    {
                        if (Math.Abs(angle) > Math.PI * 0.75)
                        {
                            Projectile.velocity = Projectile.velocity.RotatedBy(angle * 0.07);
                        }
                        else
                        {
                            float range = distance.Length();
                            float difference = 12.7f / range;
                            distance *= difference;
                            distance /= 7f;
                            Projectile.velocity += distance;
                            if (range > 70f)
                            {
                                Projectile.velocity *= 0.977f;
                            }
                        }
                    }
                    else
                    {
                        Projectile.velocity = Projectile.velocity.RotatedBy(angle * 0.1);
                    }
                }
                else //target lost, reset
                {
                    targetID = -1;
                    searchTimer = 0;
                    Projectile.netUpdate = true;
                }
            }
        }*/

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 8;
            target.AddBuff(BuffID.Ichor, 600);
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
            Rectangle rectangle = new(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            return false;
        }
    }
}