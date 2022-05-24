using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Buffs.Minions;
using FargowiltasSouls.Items.Consumables;
using FargowiltasSouls.Items.Materials;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Masomode
{
    public class BionomicCluster : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bionomic Cluster");
            Tooltip.SetDefault("Grants immunity to Frostburn, Shadowflame, Squeaky Toy, Guilty" +
                "\nGrants immunity to Flames of the Universe, Clipped Wings, Crippled, Webbed, and Purified" +
                "\nGrants immunity to Lovestruck, Stinky, Midas, Hexed, cactus damage, and enemies that steal items" +
                "\nUse to teleport to your last death point" +
                "\nYour attacks can inflict Clipped Wings, spawn Frostfireballs, and produce hearts" +
                "\nAttacks have a chance to squeak and deal 1 damage to you" +
                "\nShadowflame tentacles lash out at nearby enemies and summons a friendly rainbow slime" +
                "\nCertain enemies will drop potions when defeated" +
                "\n[c/00FFFF:Following effects work passively from inventory or vanity slots:]" +
                "\n    Grants immunity to Mighty Wind, Suffocation, and Guilty" +
                "\n    You have autofire, improved night vision, and automatically use mana potions when needed" +
                "\n    You respawn with more life and when no boss is alive, respawn faster" +
                "\n    Right click to zoom and drastically improves reforges" +
                "\n'The amalgamate born of a thousand common enemies'");

            // DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "生态集群");
            // Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "使你免疫霜冻、暗影焰、吱吱作响的玩具、愧疚、强风和窒息减益" +
            //     "\n使你免疫宇宙之火、剪除羽翼、残废、被网住和净化减益" +
            //     "\n使你免热恋、恶臭、迈达斯、邪咒减益，同时免疫仙人掌刺伤和敌人的偷取物品效果" +
            //     "\n攻击会造成剪除羽翼减益并生成霜火球和红心" +
            //     "\n允许所有武器自动挥舞、增强夜视效果并在非Boss战期间加快你的重生速度" +
            //     "\n在需要时自动使用魔力药水\r\n你在受到伤害时有几率发出吱吱声，并使这次受到的伤害降至1点" +
            //     "\n在你受到伤害后会发射暗影焰触手，你在重生时以更多生命重生" +
            //     "\n大多敌人在死亡时会掉落随机的药水，减少50%重铸价格" +
            //     "\n召唤一只彩虹史莱姆" +
            //     "\n使用此饰品后会将你传送至上一次死亡时的地点，右键缩放视域" +
            //     "\n'由上千普通敌人融合而成'");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);
            Item.defense = 6;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item6;
        }

        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<FargoSoulsPlayer>().BionomicPassiveEffect();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer fargoPlayer = player.GetModPlayer<FargoSoulsPlayer>();

            // Concentrated rainbow matter
            player.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
            if (player.GetToggleValue("MasoRainbow"))
                player.AddBuff(ModContent.BuffType<RainbowSlime>(), 2);

            // Dragon fang
            player.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            player.buffImmune[ModContent.BuffType<Crippled>()] = true;
            if (player.GetToggleValue("MasoClipped"))
                fargoPlayer.DragonFang = true;

            // Frigid gemstone
            player.buffImmune[BuffID.Frostburn] = true;
            if (player.GetToggleValue("MasoFrigid"))
            {
                fargoPlayer.FrigidGemstoneItem = Item;
                if (fargoPlayer.FrigidGemstoneCD > 0)
                    fargoPlayer.FrigidGemstoneCD--;
            }

            // Wretched pouch
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[ModContent.BuffType<Shadowflame>()] = true;
            player.GetModPlayer<FargoSoulsPlayer>().WretchedPouchItem = Item;

            // Sands of time
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;

            // Squeaky toy
            player.buffImmune[ModContent.BuffType<Buffs.Masomode.SqueakyToy>()] = true;
            player.buffImmune[ModContent.BuffType<Guilty>()] = true;
            fargoPlayer.SqueakyAcc = true;

            // Tribal charm
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[ModContent.BuffType<Purified>()] = true;
            fargoPlayer.TribalCharm = true;

            // Mystic skull
            player.buffImmune[BuffID.Suffocation] = true;
            if (player.GetToggleValue("ManaFlower", false))
                player.manaFlower = true;

            // Security wallet
            player.buffImmune[ModContent.BuffType<Midas>()] = true;
            fargoPlayer.SecurityWallet = true;

            // Carrot
            player.nightVision = true;
            if (player.GetToggleValue("MasoCarrot", false))
                player.scope = true;

            // Nymph's perfume
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[ModContent.BuffType<Lovestruck>()] = true;
            player.buffImmune[ModContent.BuffType<Hexed>()] = true;
            player.buffImmune[BuffID.Stinky] = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            if (player.GetToggleValue("MasoNymph"))
            {
                fargoPlayer.NymphsPerfume = true;
                if (fargoPlayer.NymphsPerfumeCD > 0)
                    fargoPlayer.NymphsPerfumeCD--;
            }

            // Tim's concoction
            if (player.GetToggleValue("MasoConcoction"))
                player.GetModPlayer<FargoSoulsPlayer>().TimsConcoction = true;
        }

        public override bool CanUseItem(Player player) => player.lastDeathPostion != Vector2.Zero;

        public override bool? UseItem(Player player)
        {
            for (int index = 0; index < 70; ++index)
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 87, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, new Color(), 1.5f);
                Main.dust[d].velocity *= 4f;
                Main.dust[d].noGravity = true;
            }

            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int index = 0; index < Main.maxProjectiles; ++index)
            {
                if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7)
                    Main.projectile[index].Kill();
            }

            if (player.whoAmI == Main.myPlayer)
            {
                player.Teleport(player.lastDeathPostion, 1);
                player.velocity = Vector2.Zero;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, player.lastDeathPostion.X, player.lastDeathPostion.Y, 1);
            }

            for (int index = 0; index < 70; ++index)
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 87, 0.0f, 0.0f, 150, new Color(), 1.5f);
                Main.dust[d].velocity *= 4f;
                Main.dust[d].noGravity = true;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()

            .AddIngredient(ModContent.ItemType<ConcentratedRainbowMatter>())
            .AddIngredient(ModContent.ItemType<WyvernFeather>())
            .AddIngredient(ModContent.ItemType<FrigidGemstone>())
            .AddIngredient(ModContent.ItemType<SandsofTime>())
            .AddIngredient(ModContent.ItemType<SqueakyToy>())
            .AddIngredient(ModContent.ItemType<TribalCharm>())
            .AddIngredient(ModContent.ItemType<MysticSkull>())
            .AddIngredient(ModContent.ItemType<SecurityWallet>())
            .AddIngredient(ModContent.ItemType<OrdinaryCarrot>())
            .AddIngredient(ModContent.ItemType<WretchedPouch>())
            .AddIngredient(ModContent.ItemType<NymphsPerfume>())
            .AddIngredient(ModContent.ItemType<TimsConcoction>())
            //.AddIngredient(ItemID.SoulofLight, 20);
            //.AddIngredient(ItemID.SoulofNight, 20);
            .AddIngredient(ModContent.ItemType<DeviatingEnergy>(), 10)

            .AddTile(TileID.MythrilAnvil)

            .Register();
        }
    }
}
