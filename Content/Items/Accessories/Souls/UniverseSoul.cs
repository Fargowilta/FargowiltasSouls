using Fargowiltas.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Souls
{
    public class UniverseSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();


            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
        public override int NumFrames => 7;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            Item.rare = -12;
            Item.expert = true;
            Item.defense = 4;

            Item.width = 5;
            Item.height = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            DamageClass damageClass = player.ProcessDamageTypeFromHeldItem();
            player.GetDamage(damageClass) += .50f;
            player.GetCritChance(damageClass) += 25;

            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //use speed, velocity, debuffs, crit dmg, mana up, double knockback
            modPlayer.UniverseSoul = modPlayer.UniverseSoulBuffer = true;
            //modPlayer.UniverseCore = true;

            player.AddEffect<UniverseSpeedEffect>(Item);

            player.maxMinions += 2;
            player.maxTurrets += 1;

            player.kbGlove = true;
            player.autoReuseGlove = true;
            player.meleeScaleGlove = true;

            player.counterWeight = 556 + Main.rand.Next(6);
            player.yoyoGlove = true;
            player.yoyoString = true;

            //celestial shell
            player.wolfAcc = true;
            player.accMerman = true;

            if (hideVisual)
            {
                player.hideMerman = true;
                player.hideWolf = true;
            }

            player.lifeRegen += 2;

            player.manaFlower = true;
            player.manaMagnet = true;
            player.magicCuffs = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
            .AddIngredient<BerserkerSoul>()
            .AddIngredient<SnipersSoul>()
            .AddIngredient<ArchWizardsSoul>()
            .AddIngredient<ConjuristsSoul>()
            .AddIngredient<AbomEnergy>(10)
            .AddTile<CrucibleCosmosSheet>();

            recipe.Register();
        }
    }
    
    public class UniverseSpeedEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<UniverseSoul>();
        public override void PostUpdateEquips(Player player)
        {
            float speed = player.FargoSouls().Eternity ? 2.5f : 0.25f;
            player.FargoSouls().AttackSpeed += speed;
        }
    }
    
}
