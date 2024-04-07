using FargowiltasSouls.Content.Items.BossBags;
using FargowiltasSouls.Content.Projectiles.Minions;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Weapons.Challengers
{
    public class KamikazeSquirrelStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Kamikaze Squirrel Staff");
            // Tooltip.SetDefault("Summons friendly squirrels to cuddle your foes\nRight click to make them explode");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神风松鼠杖");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤出友善的松鼠，拥抱你的敌人\n右键点击让松鼠爆炸");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Summon;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 46;
            Item.useAnimation = 46;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 0, 50);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<KamikazeSquirrel>();
            Item.shootSpeed = 1f;
            Item.mana = 10;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback, default, velocity);
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanShoot(Player player) => player.altFunctionUse != 2;

        public override float UseSpeedMultiplier(Player player)
        {
            //if (player.altFunctionUse == 2) return 0.5f;

            return base.UseSpeedMultiplier(player);
        }

        public override bool? UseItem(Player player)
        {
            if (player.ItemTimeIsZero && player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == Item.shoot))
                    p.Kill();
            }

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<TrojanSquirrelBag>(2).AddTile(TileID.Solidifier).Register();
        }
    }
}