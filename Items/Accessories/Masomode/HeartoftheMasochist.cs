using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face, EquipType.Front, EquipType.Back)]
    public class HeartoftheMasochist : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of the Eternal");
            Tooltip.SetDefault(@"Grants immunity to Living Wasteland, Frozen, Hypothermia, Oozed, Withered Weapon, and Withered Armor
Grants immunity to Feral Bite, Mutant Nibble, Flipped, Unstable, Distorted, and Curse of the Moon
Grants immunity to Wet, Electrified, Oceanic Maul, Moon Leech, Nullification Curse, and water debuffs
Increases damage, critical strike chance, and damage reduction by 5%,
Increases flight time by 100%
You may periodically fire additional attacks depending on weapon type
Your critical strikes inflict Rotting and Betsy's Curse
Press the Fireball Dash key to perform a short invincible dash
Grants effects of Wet debuff while riding Cute Fishron and gravity control
Summons a friendly super Flocko, Mini Saucer, and true eyes of Cthulhu
'Warm, beating, and no body needed'");
            DisplayName.AddTranslation(GameCulture.Chinese, "永恒者之心");
            Tooltip.AddTranslation(GameCulture.Chinese, @"免疫人形废土,冻结,渗入,枯萎武器和枯萎盔甲
免疫野性咬噬,突变啃啄,翻转,不稳定,扭曲和混沌
免疫潮湿,带电,月之血蛭,无效诅咒和由水造成的Debuff
增加10%伤害,暴击率伤害减免
增加100%飞行时间
根据武器类型定期发动额外的攻击
暴击造成贝特希的诅咒
按下火球冲刺按键来进行一次短程的无敌冲刺
骑乘猪鲨坐骑时获得潮湿状态,能够控制重力
召唤一个友善的超级圣诞雪灵,迷你飞碟和真·克苏鲁之眼
'大多数情况下已经不用受苦了'");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 5));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Purple;
            item.value = Item.sellPrice(0, 9);
            item.defense = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.AllDamageUp(.05f);
            fargoPlayer.AllCritUp(5);
            fargoPlayer.MasochistHeart = true;
            player.endurance += 0.05f;

            //pumpking's cape
            player.buffImmune[mod.BuffType("LivingWasteland")] = true;
            fargoPlayer.PumpkingsCape = true;
            fargoPlayer.AdditionalAttacks = true;

            //ice queen's crown
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.Hypothermia>()] = true;
            if (player.GetToggleValue("MasoFlocko"))
                player.AddBuff(mod.BuffType("SuperFlocko"), 2);

            //saucer control console
            player.buffImmune[BuffID.Electrified] = true;
            if (player.GetToggleValue("MasoUfo"))
                player.AddBuff(mod.BuffType("SaucerMinion"), 2);

            //betsy's heart
            player.buffImmune[BuffID.OgreSpit] = true;
            player.buffImmune[BuffID.WitheredWeapon] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            fargoPlayer.BetsysHeart = true;

            //mutant antibodies
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.buffImmune[mod.BuffType("MutantNibble")] = true;
            player.buffImmune[mod.BuffType("OceanicMaul")] = true;
            fargoPlayer.MutantAntibodies = true;
            if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
                player.dripping = true;

            //galactic globe
            player.buffImmune[mod.BuffType("Flipped")] = true;
            player.buffImmune[mod.BuffType("FlippedHallow")] = true;
            player.buffImmune[mod.BuffType("Unstable")] = true;
            player.buffImmune[mod.BuffType("CurseoftheMoon")] = true;
            player.buffImmune[BuffID.VortexDebuff] = true;
            //player.buffImmune[BuffID.ChaosState] = true;
            if (player.GetToggleValue("MasoGrav"))
                player.gravControl = true;
            if (player.GetToggleValue("MasoTrueEye"))
                player.AddBuff(mod.BuffType("TrueEyes"), 2);
            fargoPlayer.GravityGlobeEX = true;
            fargoPlayer.wingTimeModifier += 1f;

            //heart of maso
            player.buffImmune[BuffID.MoonLeech] = true;
            player.buffImmune[mod.BuffType("NullificationCurse")] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("PumpkingsCape"));
            recipe.AddIngredient(mod.ItemType("IceQueensCrown"));
            recipe.AddIngredient(mod.ItemType("SaucerControlConsole"));
            recipe.AddIngredient(mod.ItemType("BetsysHeart"));
            recipe.AddIngredient(mod.ItemType("MutantAntibodies"));
            recipe.AddIngredient(mod.ItemType("GalacticGlobe"));
            recipe.AddIngredient(ItemID.LunarBar, 15);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
