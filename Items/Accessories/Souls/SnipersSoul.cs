using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    //[AutoloadEquip(EquipType.Neck)]
    public class SnipersSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Sniper's Soul");

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "神枪手之魂");

            string tooltip =
@"30% increased ranged damage
20% chance to not consume ammo
15% increased ranged critical chance
Effects of Sniper Scope
'Ready, aim, fire'";
            Tooltip.SetDefault(tooltip);

            //string tooltip_ch =
//@"增加30%远程伤害
//20%几率不消耗弹药
//增加15%远程暴击率
//拥有狙击镜效果
//'预备，瞄准，开火'";
            //Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, tooltip_ch);
        }



        protected override Color? nameColor => new Color(188, 253, 68);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //reduce ammo consume
            player.GetModPlayer<FargoSoulsPlayer>().RangedSoul = true;
            player.GetDamage(DamageClass.Ranged) += 0.3f;
            player.GetCritChance(DamageClass.Ranged) += 15;

            //add new effects

            if (player.GetToggleValue("Sniper"))
            {
                player.scope = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            /*
hive pack*/
            .AddIngredient(null, "SharpshootersEssence")
            .AddIngredient(ItemID.BoneGlove)
            .AddIngredient(ItemID.MoltenQuiver)
            .AddIngredient(ItemID.StalkersQuiver)
            .AddIngredient(ItemID.ReconScope)

            .AddIngredient(ItemID.DartPistol)
            .AddIngredient(ItemID.Megashark)
            .AddIngredient(ItemID.PulseBow)
            .AddIngredient(ItemID.NailGun)
            .AddIngredient(ItemID.PiranhaGun)
            .AddIngredient(ItemID.SniperRifle)
            .AddIngredient(ItemID.Tsunami)
            .AddIngredient(ItemID.StakeLauncher)
            .AddIngredient(ItemID.EldMelter)
            .AddIngredient(ItemID.Xenopopper)
            .AddIngredient(ItemID.Celeb2)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))
            .Register();

        }
    }
}
