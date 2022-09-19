using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class ConjuristsSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Conjurist's Soul");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤之魂");
            string tooltip =
@"30% increased summon damage
Increases your max number of minions by 3
Increases your max number of sentries by 3
Increased minion knockback
'An army at your disposal'";
            Tooltip.SetDefault(tooltip);

            //string tooltip_ch =
//@"增加30%召唤伤害
//+4最大召唤栏
//+2最大哨兵栏
//增加召唤物击退
//'一支听命于您的军队'";
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);


        }

        protected override Color? nameColor => new Color(0, 255, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.3f;
            player.maxMinions += 3;
            player.maxTurrets += 3;
            player.GetKnockback(DamageClass.Summon) += 3f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "OccultistsEssence")
            .AddIngredient(ItemID.MonkBelt)
            .AddIngredient(ItemID.SquireShield)
            .AddIngredient(ItemID.HuntressBuckler)
            .AddIngredient(ItemID.ApprenticeScarf)
            .AddIngredient(ItemID.PygmyNecklace)
            .AddIngredient(ItemID.PapyrusScarab)


            .AddIngredient(ItemID.Smolstar) //blade staff
            .AddIngredient(ItemID.PirateStaff)
            .AddIngredient(ItemID.OpticStaff)
            .AddIngredient(ItemID.DeadlySphereStaff)
            .AddIngredient(ItemID.StormTigerStaff)
            .AddIngredient(ItemID.StaffoftheFrostHydra)
            //mourningstar?
            //.AddIngredient(ItemID.DD2BallistraTowerT3Popper);
            //.AddIngredient(ItemID.DD2ExplosiveTrapT3Popper);
            //.AddIngredient(ItemID.DD2FlameburstTowerT3Popper);
            //.AddIngredient(ItemID.DD2LightningAuraT3Popper);
            .AddIngredient(ItemID.TempestStaff)
            .AddIngredient(ItemID.RavenStaff)
            .AddIngredient(ItemID.XenoStaff)
            .AddIngredient(ItemID.EmpressBlade) //terraprisma

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();


        }
    }
}
