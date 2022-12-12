using FargowiltasSouls.Projectiles.Minions;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Weapons.Challengers
{
    public class KamikazePixieStaff : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Expixive Staff");
            Tooltip.SetDefault("Summons friendly pixies that fire shots for 3 seconds, then charge into enemies and explode");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神风松鼠杖");
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤出友善的松鼠，拥抱你的敌人\n右键点击让松鼠爆炸");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Summon;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 46;
            Item.useAnimation = 46;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 25);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<KamikazePixie>();
            Item.shootSpeed = 1f;
            Item.mana = 10;
            Item.noMelee = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback, default, velocity);
            return false;
        }

        //public override bool AltFunctionUse(Player player) => true;

        public override bool CanShoot(Player player) => player.altFunctionUse != 2;

        public override float UseSpeedMultiplier(Player player)
        {
            //if (player.altFunctionUse == 2) return 0.5f;

            return base.UseSpeedMultiplier(player);
        }

        public override bool? UseItem(Player player)
        {
            /*if (player.ItemTimeIsZero && player.altFunctionUse == 2)
            {
                foreach (Projectile p in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == Item.shoot))
                    p.Kill();
            }*/

            return true;
        }
    }
}