using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Weapons.BossDrops;
using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Content.UI.Elements;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class AdamantiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(221, 85, 125);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Pink;
            Item.value = 100000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AdamantiteEffect>(Item);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("FargowiltasSouls:AnyAdamHead")
                .AddIngredient(ItemID.AdamantiteBreastplate)
                .AddIngredient(ItemID.AdamantiteLeggings)
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.QuadBarrelShotgun)
                .AddIngredient(ItemID.DarkLance)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }

    public class AdamantiteEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<EarthHeader>();
        public override int ToggleItemType => ModContent.ItemType<AdamantiteEnchant>();

        public override bool ExtraAttackEffect => true;
        public const float SpreadCap = 17; // spread cap in DEGREES
        public static bool CanBeAffected(Projectile projectile, Player player)
        {
            var modProj = projectile.FargoSouls();
            var modPlayer = player.FargoSouls();
            return 
                projectile.owner == Main.myPlayer && FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, false)
                && modProj.CanSplit && Array.IndexOf(FargoSoulsGlobalProjectile.NoSplit, projectile.type) <= -1
                && projectile.aiStyle != ProjAIStyleID.Spear
                && !(AdamIgnoreItems.Contains(modPlayer.Player.HeldItem.type) || modPlayer.Player.heldProj == projectile.whoAmI || modProj.IsAHeldProj)
                && modProj.ItemSource;
        }
        public static void CalcAdamantiteAttackSpeed(Player player, Item item)
        {
            if (!player.HasEffectEnchant<AdamantiteEffect>())
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (!(item.DamageType != DamageClass.Default && item.pick == 0 && item.axe == 0 && item.hammer == 0 && item.type != ModContent.ItemType<PrismaRegalia>()))
                return;
            if (item.shoot <= ProjectileID.None)
                return;
            if (!modPlayer.HeldItemAdamantiteValid)
                return;
            float maxSpeed = player.ForceEffect<AdamantiteEffect>() ? 0.5f : 0.3f;
            if (ProjectileID.Sets.CultistIsResistantTo[item.shoot])
                maxSpeed /= 2;

            float ratio = Math.Max((float)modPlayer.AdamantiteSpread / SpreadCap, 0);
            modPlayer.AttackSpeed += maxSpeed * ratio;

            if (player.whoAmI == Main.myPlayer)
                CooldownBarManager.Activate("AdamantiteEnchantCharge", ModContent.Request<Texture2D>("FargowiltasSouls/Content/Items/Accessories/Enchantments/AdamantiteEnchant").Value, new(221, 85, 125),
                () => (float)Main.LocalPlayer.FargoSouls().AdamantiteSpread / SpreadCap, activeFunction: player.HasEffectEnchant<AdamantiteEffect>, displayAtFull: true);

        }
        public override void PostUpdateEquips(Player player)
        {
            if (!HasEffectEnchant(player))
                return;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player.HeldItem != null && player.HeldItem.IsWeapon())
            {
                if (modPlayer.AdamantiteItem != player.HeldItem)
                {
                    modPlayer.HeldItemAdamantiteValid = false;
                    modPlayer.AdamantiteSpread = 0;
                }
                modPlayer.AdamantiteItem = player.HeldItem;
            }

            int adaCap = (int)SpreadCap; 

            const float incSeconds = 10;
            const float decSeconds = 1.5f;
            if (modPlayer.WeaponUseTimer > 0)
                modPlayer.AdamantiteSpread += (adaCap / 60f) / incSeconds; //ada spread change per frame, based on total amount of seconds to reach cap
            else
                modPlayer.AdamantiteSpread -= (adaCap / 60f) / decSeconds;

            if (modPlayer.AdamantiteSpread < 0)
                modPlayer.AdamantiteSpread = 0;

            if (modPlayer.AdamantiteSpread > adaCap)
                modPlayer.AdamantiteSpread = adaCap;
        }

        public static int[] AdamIgnoreItems =
        [
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.TerraBlade,
            ItemID.TheHorsemansBlade,
            ModContent.ItemType<DecrepitAirstrikeRemote>()
        ];

        public static void AdamantiteSplit(Projectile projectile, FargoSoulsPlayer modPlayer, int splitDegreeAngle)
        {
            if (!modPlayer.Player.HasEffectEnchant<AdamantiteEffect>())
                return;
            if (AdamIgnoreItems.Contains(modPlayer.Player.HeldItem.type))
                return;
            modPlayer.HeldItemAdamantiteValid = true;
            projectile.velocity = projectile.velocity.RotateRandom(MathHelper.ToRadians(splitDegreeAngle));
            projectile.FargoSouls().Adamantite = true;
        }
    }
}
