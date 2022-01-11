using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Weapons.Misc
{
    public class Vineslinger : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Vineslinger");
            //Tooltip.SetDefault("Hitting an enemy fires leaves");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 10;
            item.value = Item.sellPrice(0, 8);
            item.rare = ItemRarityID.LightRed;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 40;
            item.useTime = 40;
            item.knockBack = 5.5f;
            item.damage = 52;
            item.scale = 1.1f;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType("VineslingerBall");
            item.shootSpeed = 30f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.channel = true;
        }
    }
}