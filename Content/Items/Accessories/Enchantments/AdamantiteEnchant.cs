using FargowiltasSouls.Content.Items.Weapons.Challengers;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls.Core.Toggler.Content;

using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
	public class AdamantiteEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            // DisplayName.SetDefault("Adamantite Enchantment");
            /* Tooltip.SetDefault("Every weapon shot will split into 2" +
                "\nAll weapon shots deal 50% damage" +
                "\nThey hit twice as fast and gain armor penetration equal to 50% damage" +
                "\n'Chaos'"); */
        }

        protected override Color nameColor => new(221, 85, 125);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
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
        public override bool HasToggle => true;
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            //player.DisplayToggle("Adamantite");
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //modPlayer.AdamantiteEnchantItem = item;

            int adaCap = 60; //ada cap in DEGREES

            AdamantiteFields fields = player.GetEffectFields<AdamantiteFields>();

            const float incSeconds = 10;
            const float decSeconds = 1.5f;
            if (modPlayer.WeaponUseTimer > 0)
                fields.AdamantiteSpread += (adaCap / 60f) / incSeconds; //ada spread change per frame, based on total amount of seconds to reach cap
            else
                fields.AdamantiteSpread -= (adaCap / 60f) / decSeconds;

            if (fields.AdamantiteSpread < 0)
                fields.AdamantiteSpread = 0;

            if (fields.AdamantiteSpread > adaCap)
                fields.AdamantiteSpread = adaCap;
        }

        public static int[] AdamIgnoreItems = new int[]
        {
            ItemID.NightsEdge,
            ItemID.TrueNightsEdge,
            ItemID.Excalibur,
            ItemID.TrueExcalibur,
            ItemID.TerraBlade,
            ModContent.ItemType<DecrepitAirstrikeRemote>()
        };

        public static void AdamantiteSplit(Projectile projectile, FargoSoulsPlayer modPlayer, int splitDegreeAngle)
        {
            bool adaForce = modPlayer.ForceEffect(modPlayer.Player.EffectItem<AdamantiteEffect>().ModItem);
            bool isProjHoming = ProjectileID.Sets.CultistIsResistantTo[projectile.type];

            if (AdamIgnoreItems.Contains(modPlayer.Player.HeldItem.type))
            {
                return;
            }

            float adaDamageRatio = isProjHoming ? (adaForce ? 0.375f : 0.6f) : (adaForce ? 0.5f : 0.7f);
            // if its homing, damage is 0.6x2/0.4x3 (+20%)
            // if its not homing, damage is 0.7x2/0.5x3 (+40/50%)

            foreach (Projectile p in FargoSoulsGlobalProjectile.SplitProj(projectile, 3, MathHelper.ToRadians(splitDegreeAngle), adaDamageRatio))
            {
                if (p.Alive())
                {
                    p.FargoSouls().HuntressProj = projectile.FargoSouls().HuntressProj;
                }
            }

            if (!adaForce)
            {
                projectile.type = ProjectileID.None;
                projectile.timeLeft = 0;
                projectile.active = false;
            }
            else
            {
                projectile.damage = (int)(projectile.damage * adaDamageRatio);
            }
        }
    }

    public class AdamantiteFields : EffectFields
    {
        public double AdamantiteSpread;
    }
}
