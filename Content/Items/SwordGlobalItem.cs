using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.PlayerDrawLayers;
using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items
{
    public class SwordGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void SetDefaults(Item item)
        {
            if (IsBroadsword(item))
            {
                //item.noMelee = false;
                if (!Broadswords.Contains(item.type))
                    Broadswords = Broadswords.Append(item.type).ToArray();

            }
        }
        public bool VanillaShoot = false;
        public SoundStyle? SwingSound = null;
        //phasesabers and shiny swings in by default because vanilla fucks them up
        public static int[] Broadswords = { ItemID.BluePhasesaber, ItemID.GreenPhasesaber, ItemID.PurplePhasesaber, ItemID.YellowPhasesaber, ItemID.OrangePhasesaber, ItemID.RedPhasesaber, ItemID.WhitePhasesaber,
            ItemID.NightsEdge, ItemID.Excalibur, ItemID.TrueExcalibur, ItemID.TrueNightsEdge, ItemID.TheHorsemansBlade, ItemID.TerraBlade};

        public static int[] AllowedModdedSwords = { ModContent.ItemType<TheBaronsTusk>() };
        public static bool IsBroadsword(Item item)
        {
            
            if (item.type == ItemID.StaffofRegrowth || item.type == ItemID.GravediggerShovel)
            {
                return false;
            }
            if (item.type >= ItemID.Count && !AllowedModdedSwords.Contains(item.type))
            {
                return false;
            }
            return (item.CountsAsClass(DamageClass.Melee) && item.IsWeapon() && item.useStyle == ItemUseStyleID.Swing && !item.noMelee && !item.noUseGraphic) || Broadswords.Contains(item.type);
        }
        public override bool? UseItem(Item item, Player player)
        {
            if (Main.myPlayer == player.whoAmI && IsBroadsword(item))
            {
                
            }
            return base.UseItem(item, player);
        }
        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            if (IsBroadsword(item))
            {
                FargoSoulsPlayer mplayer = player.FargoSouls();

                
                mplayer.useDirection = -1;
                if (Main.MouseWorld.X >= player.Center.X)
                {
                    mplayer.useDirection = 1;

                }
                player.direction = mplayer.useDirection;
                mplayer.useRotation = player.Center.AngleTo(Main.MouseWorld);
                
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    //netcode
                }

                float x = 1-  (float)player.itemAnimation / player.itemAnimationMax;
                //ease in out quint
                float lerp = x < 0.5f ? 4 * x * x * x : 1 - (float)Math.Pow(-2 * x + 2, 3) / 2;

                player.itemRotation = mplayer.useRotation + MathHelper.ToRadians(mplayer.useDirection == 1 ? 45 : 135) + MathHelper.ToRadians(MathHelper.Lerp(-140, 110, mplayer.swingDirection == 1 ? lerp : 1 - lerp)* mplayer.useDirection);
                if (player.gravDir == -1f)
                {
                    player.itemRotation = -player.itemRotation;
                }

                bool shooter = player.itemAnimation == (int)(player.itemAnimationMax * 0.6f) && mplayer.shouldShoot && !FargoSoulsGlobalProjectile.FancySwings.Contains(item.shoot);
                if (player.itemAnimation == player.itemAnimationMax && mplayer.shouldShoot && FargoSoulsGlobalProjectile.FancySwings.Contains(item.shoot)) shooter = true;

                if (shooter)
                {
                    mplayer.shouldShoot = false;
                    VanillaShoot = true;
                    MethodInfo PlayerItemCheck_Shoot = typeof(Player).GetMethod("ItemCheck_Shoot", BindingFlags.NonPublic | BindingFlags.Instance);
                    PlayerItemCheck_Shoot.Invoke(player, [player.whoAmI, item, item.damage]);
                    VanillaShoot = false;

                }
                
                if (player.itemAnimation == 1)
                {
                    mplayer.swingDirection *= -1;
                    
                }
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation + MathHelper.ToRadians(-135 * mplayer.useDirection));
                player.itemLocation = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, player.itemRotation + MathHelper.ToRadians(-135 * mplayer.useDirection));
            }
            base.UseStyle(item, player, heldItemFrame);
        }
        
        public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (IsBroadsword(item))
            {
                
                FargoSoulsPlayer mplayer = player.FargoSouls();
                int itemWidth = (int)(TextureAssets.Item[item.type].Width() * (player.GetAdjustedItemScale(item)));
                hitbox = new Rectangle(0, 0, itemWidth, itemWidth);
                hitbox.Location = (player.Center + new Vector2(itemWidth, 0).RotatedBy(player.itemRotation - MathHelper.ToRadians(mplayer.useDirection == 1 ? 40 : 140)) - hitbox.Size()/2).ToPoint();
                
            }
            base.UseItemHitbox(item, player, ref hitbox, ref noHitbox);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (IsBroadsword(item) && !VanillaShoot)
            {
                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
}
