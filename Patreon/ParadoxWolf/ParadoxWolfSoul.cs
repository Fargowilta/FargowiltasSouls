using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasSouls.Patreon.ParadoxWolf
{
    public class ParadoxWolfSoul : PatreonModItem
    {
        private int dashTime = 0;
        private int dashCD = 0;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Paradox Wolf Soul");
            Tooltip.SetDefault(
@"Double tap to dash through and damage enemies
There is a cooldown of 3 seconds between uses");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = 5;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //no dash for you
            if (player.mount.Active)
            {
                return;
            }

            //on cooldown
            if (dashCD > 0)
            {
                dashCD--;

                if (dashCD == 0)
                {
                    double spread = 2 * Math.PI / 36;
                    for (int i = 0; i < 36; i++)
                    {
                        Vector2 velocity = new Vector2(2, 2).RotatedBy(spread * i);

                        int index2 = Dust.NewDust(player.Center, 0, 0, 37, velocity.X, velocity.Y, 100);
                        Main.dust[index2].noGravity = true;
                        Main.dust[index2].noLight = true;
                    }
                }

                return;
            }

            //while dashing
            if (dashTime > 0)
            {
                dashTime--;

                player.position.Y = player.oldPosition.Y;
                player.immune = true;
                player.controlLeft = false;
                player.controlRight = false;
                player.controlJump = false;
                player.controlDown = false;
                player.controlUseItem = false;
                player.controlUseTile = false;
                player.controlHook = false;
                player.controlMount = false;
                player.itemAnimation = 0;
                player.itemTime = 0;

                //dash over
                if (dashTime == 0)
                {
                    player.velocity *= 0.5f;
                    player.dashDelay = 0;
                    dashCD = 180;
                }

                return;
            }

            //checking for direction
            int direction = 0;

            if ((player.controlRight && player.releaseRight))
            {
                if (player.doubleTapCardinalTimer[2] > 0 && player.doubleTapCardinalTimer[2] != 15)
                {
                    direction = 1;
                }
            }
            //left
            else if ((player.controlLeft && player.releaseLeft))
            {
                if (player.doubleTapCardinalTimer[3] > 0 && player.doubleTapCardinalTimer[3] != 15)
                {
                    direction = -1;
                }
            }

            //do the dash
            if (direction != 0)
            {
                player.velocity.X = 25 * (float)direction;
                player.dashDelay = -1;
                dashTime = 20;

                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, new Vector2(player.velocity.X, 0), ModContent.ProjectileType<WolfDashProj>(), (int)(50 * player.GetDamage(DamageClass.Melee).Additive), 0f, player.whoAmI);

                SoundEngine.PlaySound(SoundID.NPCDeath8, player.Center);
            }
        }
    }
}