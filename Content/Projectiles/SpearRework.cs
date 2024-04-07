using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Systems;
using System.Collections.Generic;

namespace FargowiltasSouls.Content.Projectiles
{
    public class SpearRework : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int SwingDirection = 1;

        public static List<int> ReworkedSpears = new()
        {
                ProjectileID.Spear,
                ProjectileID.AdamantiteGlaive,
                ProjectileID.CobaltNaginata,
                ProjectileID.MythrilHalberd,
                ProjectileID.OrichalcumHalberd,
                ProjectileID.PalladiumPike,
                ProjectileID.TitaniumTrident,
                ProjectileID.Trident,
                ProjectileID.ObsidianSwordfish,
                ProjectileID.Swordfish,
                ProjectileID.ChlorophytePartisan
            };
        public override void PostAI(Projectile projectile)
        {
            if (WorldSavingSystem.EternityMode)
            {
                if (ReworkedSpears.Contains(projectile.type))
                {
                    ReworkedSpearSwing(projectile);
                }
            }
        }
        public void ReworkedSpearSwing(Projectile projectile)
        {
            Texture2D tex = (Texture2D)TextureAssets.Projectile[projectile.type];
            float HoldoutRangeMax = (float)tex.Size().Length() * projectile.scale; //since sprite is diagonal
            float HoldoutRangeMin = (float)projectile.Size.Length(); //(float)-tex.Size().Length() / 4 * projectile.scale; 
            Player player = Main.player[projectile.owner];

            int duration = (int)(player.itemAnimationMax / 1.5f);
            int WaitTime = player.itemAnimationMax / 5;
            player.heldProj = projectile.whoAmI;
            projectile.spriteDirection = player.direction;
            if (projectile.ai[1] == 0)
                SwingDirection = Main.rand.NextBool(2) ? 1 : -1;
            float Swing = 13; //higher value = less swing
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = player.itemAnimationMax; //only hit once per swing
                                                                      //projectile.ai[1] is time from spawn
                                                                      //projectile.ai[0] is extension, between 0 and 1
            if (projectile.timeLeft > player.itemAnimationMax)
            {
                projectile.timeLeft = player.itemAnimationMax;
            }
            if (projectile.ai[1] <= duration / 2)
            {
                projectile.ai[0] = projectile.ai[1] / (duration / 2);
                projectile.velocity = projectile.velocity.RotatedBy(SwingDirection * projectile.spriteDirection * -Math.PI / (Swing * player.itemAnimationMax));
            }
            else if (projectile.ai[1] <= duration / 2 + WaitTime)
            {
                projectile.ai[0] = 1;
                projectile.velocity = projectile.velocity.RotatedBy(SwingDirection * projectile.spriteDirection * (1.5 * duration / WaitTime) * Math.PI / (Swing * player.itemAnimationMax)); //i know how wacky this looks
            }
            else //backswing
            {
                //projectile.friendly = false; //no hit on backswing
                projectile.ai[0] = (duration + WaitTime - projectile.ai[1]) / (duration / 2);
                projectile.velocity = projectile.velocity.RotatedBy(SwingDirection * projectile.spriteDirection * -Math.PI / (Swing * player.itemAnimationMax));
            }
            //if (projectile.ai[1] == duration / 2)
            //SoundEngine.PlaySound(SoundID.Item1, player.Center);

            projectile.ai[1]++;
            //projectile.velocity = Vector2.Normalize(projectile.velocity); //store direction
            projectile.Center = player.MountedCenter + Vector2.SmoothStep(Vector2.Normalize(projectile.velocity) * HoldoutRangeMin, Vector2.Normalize(projectile.velocity) * HoldoutRangeMax, projectile.ai[0]);
            projectile.position -= projectile.velocity;

            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                projectile.rotation += MathHelper.ToRadians(135f);
            }


            //extra effects
            switch (projectile.type)
            {
                /*
                case ProjectileID.ChlorophytePartisan:
                    {
                        if (projectile.ai[1] == duration / 2 + WaitTime * 2 / 3 && FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity * 5, ProjectileID.SporeCloud, projectile.damage / 3, projectile.knockBack / 3, Main.myPlayer);
                        }
                        break;
                    }
                */
                case ProjectileID.OrichalcumHalberd:
                    {
                        if (projectile.ai[1] == duration / 2 || projectile.ai[1] == duration / 2 + WaitTime && FargoSoulsUtil.HostCheck)
                        {
                            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Normalize(projectile.velocity) * 5, ProjectileID.FlowerPetal, (int)(projectile.damage * 0.75f), projectile.knockBack / 2, Main.myPlayer);
                        }
                        break;
                    }
                    /*
                case ProjectileID.TheRottedFork:
                    {
                        break;
                    }
                    */
            }
        }
    }
}
