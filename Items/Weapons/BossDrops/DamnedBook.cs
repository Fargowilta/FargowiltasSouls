using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class DamnedBook : SoulsItem
    {
        public override bool Autoload(ref string name)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Cultist's Spellbook");
            //Tooltip.SetDefault("");
            //DisplayName.AddTranslation(GameCulture.Chinese, "邪教徒的魔法书");
            //Tooltip.AddTranslation(GameCulture.Chinese, "");
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.width = 24;
            item.height = 28;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.value = 1000;
            item.rare = ItemRarityID.LightPurple;
            item.mana = 1;
            item.UseSound = SoundID.Item21;
            item.autoReuse = true;
            item.shoot = ProjectileID.WoodenArrowFriendly;
            item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int rand = Main.rand.Next(4);
            int shoot = 0;

            if (rand == 0)
                shoot = mod.ProjectileType("LunarCultistFireball");
            else if (rand == 1)
                shoot = mod.ProjectileType("LunarCultistLightningOrb");
            else if (rand == 2)
                shoot = mod.ProjectileType("LunarCultistIceMist");
            else
                shoot = mod.ProjectileType("LunarCultistLight");

            int p = Projectile.NewProjectile(position, new Vector2(speedX, speedY), shoot, damage, knockBack, player.whoAmI);
            if (p < 1000)
            {
                Main.projectile[p].hostile = false;
                Main.projectile[p].friendly = true;
                //Main.projectile[p].playerImmune[player.whoAmI] = 1;
            }
            return false;
        }
    }
}