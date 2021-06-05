using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Shield)]
    public class SupremeDeathbringerFairy : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Deathbringer Fairy");
            Tooltip.SetDefault(@"Grants immunity to Slimed, Berserked, Lethargic, and Infested
Increases damage by 10% and armor penetration by 10
15% increased fall speed
When you land after a jump, slime will fall from the sky over your cursor
While dashing or running quickly you will create a trail of blood scythes
Your attacks inflict Venom and spray honey that increases your life regeneration
Bees and weak Hornets become friendly
Summons 2 Skeletron arms to whack enemies
'Supremacy not necessarily guaranteed'");
            DisplayName.AddTranslation(GameCulture.Chinese, "至高告死精灵");
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫史莱姆、狂暴、昏昏欲睡和感染减益
增加10%伤害和10点护甲穿透
增加15%下落速度
跳跃落地后，一些史莱姆球会从天而降至你的光标处
冲刺或奔跑时会在身后留下一串恶魔镰刀
攻击会造成酸性毒液减益并喷出会增加你的生命恢复速度的蜂蜜
使蜜蜂和弱小的黄蜂变得友好
召唤两条骷髅手臂重击敌人
'至高无上的地位不一定意味着安全无虞'");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Pink;
            item.value = Item.sellPrice(0, 4);
            item.defense = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.SupremeDeathbringerFairy = true;

            //slimy shield
            player.buffImmune[BuffID.Slimed] = true;

            if (player.GetToggleValue("SlimeFalling"))
            {
                player.maxFallSpeed *= 1.5f;
            }

            if (player.GetToggleValue("MasoSlime"))
            {
                player.GetModPlayer<FargoPlayer>().SlimyShield = true;
            }

            //agitating lens
            player.buffImmune[mod.BuffType("Berserked")] = true;
            fargoPlayer.AllDamageUp(.10f);
            fargoPlayer.AgitatingLens = true;

            //queen stinger
            player.buffImmune[mod.BuffType("Infested")] = true;
            //player.honey = true;
            player.armorPenetration += 10;
            player.npcTypeNoAggro[210] = true;
            player.npcTypeNoAggro[211] = true;
            player.npcTypeNoAggro[42] = true;
            player.npcTypeNoAggro[231] = true;
            player.npcTypeNoAggro[232] = true;
            player.npcTypeNoAggro[233] = true;
            player.npcTypeNoAggro[234] = true;
            player.npcTypeNoAggro[235] = true;
            fargoPlayer.QueenStinger = true;

            //necromantic brew
            player.buffImmune[mod.BuffType("Lethargic")] = true;
            fargoPlayer.NecromanticBrew = true;
            if (player.GetToggleValue("MasoSkele"))
                player.AddBuff(mod.BuffType("SkeletronArms"), 2);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("SlimyShield"));
            recipe.AddIngredient(mod.ItemType("AgitatingLens"));
            recipe.AddIngredient(mod.ItemType("QueenStinger"));
            recipe.AddIngredient(mod.ItemType("NecromanticBrew"));
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 5);

            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
