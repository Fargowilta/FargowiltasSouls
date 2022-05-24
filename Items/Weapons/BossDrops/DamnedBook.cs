//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;

//namespace FargowiltasSouls.Items.Weapons.BossDrops
//{
//    public class DamnedBook : SoulsItem
//    {
//        public override bool Autoload(ref string name)
//        {
//            return false;
//        }

//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("Cultist's Spellbook");
//            Tooltip.SetDefault("");
//            DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "邪教徒的魔法书");
//            Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "");
//
//            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
//        }

//        public override void SetDefaults()
//        {
//            Item.damage = 60;
//            Item.DamageType = DamageClass.Magic;
//            Item.width = 24;
//            Item.height = 28;
//            Item.useTime = 15;
//            Item.useAnimation = 15;
//            Item.useStyle = ItemUseStyleID.Shoot;
//            Item.noMelee = true;
//            Item.knockBack = 2;
//            Item.value = 1000;
//            Item.rare = ItemRarityID.LightPurple;
//            Item.mana = 1;
//            Item.UseSound = SoundID.Item21;
//            Item.autoReuse = true;
//            Item.shoot = ProjectileID.WoodenArrowFriendly;
//            Item.shootSpeed = 8f;
//        }

//        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
//        {
//            int rand = Main.rand.Next(4);
//            int shoot = 0;

//            if (rand == 0)
//                shoot = ModContent.ProjectileType<LunarCultistFireball>();
//            else if (rand == 1)
//                shoot = ModContent.ProjectileType<LunarCultistLightningOrb>();
//            else if (rand == 2)
//                shoot = ModContent.ProjectileType<LunarCultistIceMist>();
//            else
//                shoot = ModContent.ProjectileType<LunarCultistLight>();

//            int p = Projectile.NewProjectile(source, position, velocity, shoot, damage, knockback, player.whoAmI);
//            if (p < 1000)
//            {
//                Main.projectile[p].hostile = false;
//                Main.projectile[p].friendly = true;
//                //Main.projectile[p].playerImmune[player.whoAmI] = 1;
//            }
//            return false;
//        }
//    }
//}