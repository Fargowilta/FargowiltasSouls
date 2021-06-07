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
            Tooltip.AddTranslation(GameCulture.Chinese, @"使你免疫人形废土、冰冻、低温、分泌物、枯萎武器和枯萎盔甲减益
使你免疫野性咬噬、突变啃啄、翻转、不稳定、扭曲和月之诅咒减益
使你免疫潮湿、带电、海洋重击、月噬、无效诅咒减益和浸没在水中时获得的减益
增加5%伤害、暴击率和伤害减免
延长100%飞行时间
根据手持武器的类型定期发动额外攻击
攻击会造成腐败和双足翼龙诅咒减益
按下'火球冲刺'键后会进行短距离无敌冲刺
骑着可爱猪龙鱼坐骑或翻转重力时使你获得潮湿减益的效果
召唤一只超级圣诞雪灵、一架迷你飞碟和真·克苏鲁之眼
'一颗温暖的，跳动的心脏，无需躯壳承载'");
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
