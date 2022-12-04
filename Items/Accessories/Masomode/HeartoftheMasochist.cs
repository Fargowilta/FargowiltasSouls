using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Minions;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face, EquipType.Front, EquipType.Back)]
    public class HeartoftheMasochist : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of the Master");
            Tooltip.SetDefault(@"Grants immunity to Living Wasteland, Hypothermia, Oozed, Withered Weapon, and Withered Armor
Grants immunity to Feral Bite, Mutant Nibble, Flipped, Unstable, Distorted, and Curse of the Moon
Grants immunity to Wet, Electrified, Oceanic Maul, Smite, Moon Leech, Nullification Curse, and water debuffs
Increases damage and critical strike chance by 10% and increases damage reduction by 5%
Increases flight time by 100%
Right Click to guard with your cape
Press the Special Dash key to perform a short invincible fireball dash
Press the Ammo Cycle key to cycle ammos (this effect works passively from inventory)
Grants effects of Wet debuff while riding Cute Fishron and gravity control
Submerging in water refreshes flight time and gives you improved speed and increased max flight time
Graze attacks to gain a bomb that will freeze almost all enemies and projectiles
Reduces your hurtbox size for projectiles
Hold the Precision Seal key to disable dashes and double jumps
Summons a friendly Mini Saucer and true eyes of Cthulhu
'Warm, beating, and no body needed'");
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "永恒者之心");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'大多数情况下已经不用受苦了'
            // 免疫人形废土,冻结,渗入,枯萎武器和枯萎盔甲
            // 免疫野性咬噬,突变啃啄,翻转,不稳定,扭曲和混沌
            // 免疫潮湿,带电,月之血蛭,无效诅咒和由水造成的Debuff
            // 增加10%伤害,暴击率伤害减免
            // 增加100%飞行时间
            // 根据武器类型定期发动额外的攻击
            // 暴击造成贝特希的诅咒
            // 按下火球冲刺按键来进行一次短程的无敌冲刺
            // 骑乘猪鲨坐骑时获得潮湿状态,能够控制重力
            // 召唤一个友善的超级圣诞雪灵,迷你飞碟和真·克苏鲁之眼");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 5));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(0, 9);
            Item.defense = 10;
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().CanAmmoCycle = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().CanAmmoCycle = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<FargoSoulsPlayer>().CanAmmoCycle = true;

            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 10;
            fargoPlayer.MasochistHeart = true;
            player.endurance += 0.05f;

            //pumpking's cape
            player.buffImmune[ModContent.BuffType<LivingWasteland>()] = true;
            if (player.GetToggleValue("MasoPump"))
                fargoPlayer.PumpkingsCapeItem = Item;

            //ice queen's crown
            player.buffImmune[ModContent.BuffType<Hypothermia>()] = true;
            IceQueensCrown.Effects(player, Item);

            //saucer control console
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[BuffID.VortexDebuff] = true;
            if (player.GetToggleValue("MasoUfo"))
                player.AddBuff(ModContent.BuffType<SaucerMinion>(), 2);

            //betsy's heart
            player.buffImmune[BuffID.OgreSpit] = true;
            player.buffImmune[BuffID.WitheredWeapon] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            fargoPlayer.BetsysHeartItem = Item;

            //mutant antibodies
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.buffImmune[ModContent.BuffType<MutantNibble>()] = true;
            player.buffImmune[ModContent.BuffType<OceanicMaul>()] = true;
            fargoPlayer.MutantAntibodies = true;
            if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
                player.dripping = true;

            //galactic globe
            player.buffImmune[ModContent.BuffType<Flipped>()] = true;
            player.buffImmune[ModContent.BuffType<FlippedHallow>()] = true;
            player.buffImmune[ModContent.BuffType<Unstable>()] = true;
            player.buffImmune[ModContent.BuffType<CurseoftheMoon>()] = true;
            //player.buffImmune[BuffID.ChaosState] = true;
            if (player.GetToggleValue("MasoGrav"))
                player.gravControl = true;
            if (player.GetToggleValue("MasoTrueEye"))
                player.AddBuff(ModContent.BuffType<TrueEyes>(), 2);
            fargoPlayer.GravityGlobeEXItem = Item;
            fargoPlayer.WingTimeModifier += 1f;

            //precision seal
            player.buffImmune[ModContent.BuffType<Smite>()] = true;
            fargoPlayer.PrecisionSeal = true;
            if (player.GetToggleValue("PrecisionSealHurtbox", false))
                fargoPlayer.PrecisionSealHurtbox = true;

            //heart of maso
            player.buffImmune[BuffID.MoonLeech] = true;
            player.buffImmune[ModContent.BuffType<NullificationCurse>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<PumpkingsCape>())
            .AddIngredient(ModContent.ItemType<IceQueensCrown>())
            .AddIngredient(ModContent.ItemType<SaucerControlConsole>())
            .AddIngredient(ModContent.ItemType<BetsysHeart>())
            .AddIngredient(ModContent.ItemType<MutantAntibodies>())
            .AddIngredient(ModContent.ItemType<PrecisionSeal>())
            .AddIngredient(ModContent.ItemType<GalacticGlobe>())
            .AddIngredient(ItemID.LunarBar, 15)
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}