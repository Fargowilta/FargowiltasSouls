using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    public class ConjuristsSoul : BaseSoul
    {
        public static readonly Color ItemColor = new(0, 255, 255);
        protected override Color? nameColor => ItemColor;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().SummonSoul = true;
            player.GetDamage(DamageClass.Summon) += 0.3f;
            player.maxMinions += 5;
            player.maxTurrets += 2;
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
