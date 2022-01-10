using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Projectiles.Minions;
using Terraria.ID;

namespace FargowiltasSouls.Items.Weapons.BossDrops
{
    public class DestroyerGun : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Destroyer Gun");
            Tooltip.SetDefault("Becomes longer and faster with up to 3 empty minion slots\n'An old foe beaten into submission..'");
            DisplayName.AddTranslation(GameCulture.Chinese, "毁灭者之枪");
            Tooltip.AddTranslation(GameCulture.Chinese, "'一个被迫屈服的老对手..'");*/
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.mana = 10;
            item.summon = true;
            item.width = 24;
            item.height = 24;
            item.useTime = 70;
            item.useAnimation = 70;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.UseSound = new LegacySoundStyle(4, 13);
            item.value = 50000;
            item.rare = ItemRarityID.Pink;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DestroyerHead>();
            item.shootSpeed = 10f;
        }
    }
}
