using Fargowiltas.Utilities.Extensions;
using FargowiltasSouls.Content.Projectiles.ChallengerItems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class SisypheanFist : SoulsItem
    {
        private int delay = 0;
        private bool LastMouse = false;
        public static readonly SoundStyle ThrowSFX = new("FargowiltasSouls/Assets/Sounds/Throw");

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.channel = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.shoot = ModContent.ProjectileType<SisypheanBoulder>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.knockBack = 10f;
        }

        public override void HoldItem(Player player)
        {
            if (LastMouse && !Main.mouseLeft && delay == 0 && CanUseItem(player))
            {
                delay = 60;
            }
            if (delay > 0)
            {
                delay--;
            }
            LastMouse = Main.mouseLeft;
            base.HoldItem(player);
        }

        public override bool CanUseItem(Player player)
        {
            bool grounded = player.velocity.Y == 0 && !player.mount.Active && player.gravDir > 0 && player.grapCount == 0;

            Tile tile = Framing.GetTileSafely(player.Bottom);
            bool notPlatforms = !isPlatform(tile.TileType);

            return grounded && delay <= 0 && notPlatforms && base.CanUseItem(player);

            static bool isPlatform(int tileType)
            {
                return tileType == TileID.Platforms || tileType == TileID.PlanterBox;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int length = 40;
            for (int i = 0; i < 20; i++) {
                int dustID = Dust.NewDust(player.direction == 1 ? player.Bottom : player.Bottom + new Vector2(-length, 0f), length, 0, DustID.Stone);
                Dust dust = Main.dust[dustID];
                dust.velocity.X = 0;
                dust.velocity.Y = -2 * Main.rand.NextFloat();
            }
            ScreenShakeSystem.StartShake(5);
            SoundEngine.PlaySound(ThrowSFX, player.Center);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
