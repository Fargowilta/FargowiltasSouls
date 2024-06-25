using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face, EquipType.Front, EquipType.Back)]
    public class HeartoftheMasochist : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
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
            player.FargoSouls().CanAmmoCycle = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.FargoSouls().CanAmmoCycle = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.FargoSouls().CanAmmoCycle = true;

            FargoSoulsPlayer fargoPlayer = player.FargoSouls();
            player.GetDamage(DamageClass.Generic) += 0.10f;
            player.GetCritChance(DamageClass.Generic) += 10;
            fargoPlayer.MasochistHeart = true;
            player.endurance += 0.05f;

            //pumpking's cape
            player.buffImmune[ModContent.BuffType<LivingWastelandBuff>()] = true;
            player.AddEffect<PumpkingsCapeEffect>(Item);

            //ice queen's crown
            player.buffImmune[ModContent.BuffType<HypothermiaBuff>()] = true;
            IceQueensCrown.AddEffects(player, Item);

            //saucer control console
            player.buffImmune[BuffID.Electrified] = true;
            player.buffImmune[BuffID.VortexDebuff] = true;
            player.AddEffect<UfoMinionEffect>(Item);

            //betsy's heart
            player.buffImmune[BuffID.OgreSpit] = true;
            player.buffImmune[BuffID.WitheredWeapon] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            fargoPlayer.BetsysHeartItem = Item;

            //mutant antibodies
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[BuffID.Rabies] = true;
            player.buffImmune[ModContent.BuffType<MutantNibbleBuff>()] = true;
            player.buffImmune[ModContent.BuffType<OceanicMaulBuff>()] = true;
            fargoPlayer.MutantAntibodies = true;
            if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
                player.dripping = true;

            //galactic globe
            player.buffImmune[ModContent.BuffType<FlippedBuff>()] = true;
            player.buffImmune[ModContent.BuffType<FlippedHallowBuff>()] = true;
            player.buffImmune[ModContent.BuffType<UnstableBuff>()] = true;
            player.buffImmune[ModContent.BuffType<CurseoftheMoonBuff>()] = true;
            //player.buffImmune[BuffID.ChaosState] = true;
            player.AddEffect<MasoGravEffect>(Item);
            player.AddEffect<MasoTrueEyeMinion>(Item);
            fargoPlayer.GravityGlobeEXItem = Item;
            fargoPlayer.WingTimeModifier += 1f;

            //precision seal
            player.buffImmune[ModContent.BuffType<SmiteBuff>()] = true;
            fargoPlayer.PrecisionSeal = true;
            player.AddEffect<PrecisionSealHurtbox>(Item);

            //heart of maso
            player.buffImmune[BuffID.MoonLeech] = true;
            player.buffImmune[ModContent.BuffType<NullificationCurseBuff>()] = true;
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