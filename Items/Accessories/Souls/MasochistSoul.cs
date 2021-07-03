using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Buffs.Masomode;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(/*EquipType.Head, */EquipType.Front, EquipType.Back, EquipType.Shield)]
    public class MasochistSoul : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of the Siblings");
            Tooltip.SetDefault(
@"Increases wing time by 200%, armor penetration by 50, and movement speed by 20%
Increases max life by 100%, damage by 50%, and damage reduction by 10%
Increases life regen drastically, increases max number of minions and sentries by 10
Grants gravity control, fastfall, and immunity to knockback, almost all Eternity Mode debuffs, and more
Grants autofire to all weapons and you automatically use mana potions when needed
Makes armed and magic skeletons less hostile outside the Dungeon, zoom with right click
Your attacks create additional attacks, hearts, and inflict a cocktail of Eternity Mode debuffs
Press the Fireball Dash key to perform a short invincible dash
Certain enemies will drop potions when defeated, 50% discount on reforges, you respawn with more life
You respawn twice as fast, attacks spawn honey, have improved night vision, and erupt into various attacks when injured
Prevents boss spawns, increases spawn rate, increases loot, and attacks may squeak and deal 1 damage to you
Use to teleport to your last death point
Summons the aid of all Eternity Mode bosses to your side
'Embrace eternity'");

            DisplayName.AddTranslation(GameCulture.Chinese, "万助之魂");
            Tooltip.AddTranslation(GameCulture.Chinese, 
@"延长200%飞行时间，增加50点护甲穿透和20%移动速度
增加100%最大生命值、50%伤害和10%伤害减免
大幅增加生命恢复速度，增加10个最大召唤和哨兵栏位
允许你控制重力，使你免疫击退、摔落伤害和近乎所有的永恒模式减益
允许所有武器自动挥舞且在需要时自动使用魔力药水
在地牢外减少武装和魔法骷髅对你的敌意，右键缩放视野
你的攻击会额外生成心，并造成混合的永恒模式减益
按下“火球冲刺”键后会进行短距离无敌冲刺
大多敌人在死亡时会掉落随机的药水，减少50%重铸价格，你在重生时以更多生命重生
重生速度翻倍，攻击时会释放蜜蜂，增强夜视效果，受到伤害时释放各种攻击
阻止Boss自然生成，增加刷怪率，敌人死亡后会掉落更多战利品，你在受到伤害时有几率发出吱吱声，并使这次受到的伤害降至1点
召唤所有永恒模式Boss的援助到你身边
“拥抱永恒”");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.rare = ItemRarityID.Purple;
            item.value = 5000000;
            item.defense = 30;
            item.useTime = 90;
            item.useAnimation = 90;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useTurn = true;
            item.UseSound = SoundID.Item6;
        }

        public override void SafeModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(Main.DiscoR, 51, 255 - (int)(Main.DiscoR * 0.4));
                }
            }
        }

        public override bool CanUseItem(Player player) => player.lastDeathPostion != Vector2.Zero;

        public override bool UseItem(Player player)
        {
            for (int index = 0; index < 70; ++index)
            {
                int d = Dust.NewDust(player.position, player.width, player.height, 87, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, new Color(), 1.5f);
                Main.dust[d].velocity *= 4f;
                Main.dust[d].noGravity = true;
            }

            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int index = 0; index < 1000; ++index)
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

        public override void UpdateInventory(Player player)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaFlower = true;
            fargoPlayer.SecurityWallet = true;
            fargoPlayer.TribalCharm = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            player.nightVision = true;
            if (player.GetToggleValue("MasoCarrot", false))
                player.scope = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            fargoPlayer.MasochistSoul = true;

            player.AddBuff(mod.BuffType("SouloftheMasochist"), 2);

            //stat modifiers
            fargoPlayer.AllDamageUp(.5f);
            player.endurance += 0.1f;
            player.maxMinions += 10;
            player.maxTurrets += 10;
            player.armorPenetration += 50;
            player.statLifeMax2 += player.statLifeMax;
            player.lifeRegen += 7;
            player.lifeRegenTime += 7;
            player.lifeRegenCount += 7;
            fargoPlayer.wingTimeModifier += 2f;
            player.moveSpeed += 0.2f;

            //slimy shield
            player.buffImmune[BuffID.Slimed] = true;

            if (player.GetToggleValue("SlimeFalling"))
            {
                player.maxFallSpeed *= 1.5f;
            }

            if (player.GetToggleValue("MasoSlime"))
            {
                fargoPlayer.SlimyShield = true;
            }

            //agitating lens
            fargoPlayer.AgitatingLens = true;

            //queen stinger
            //player.honey = true;
            player.npcTypeNoAggro[210] = true;
            player.npcTypeNoAggro[211] = true;
            player.npcTypeNoAggro[42] = true;
            player.npcTypeNoAggro[176] = true;
            player.npcTypeNoAggro[231] = true;
            player.npcTypeNoAggro[232] = true;
            player.npcTypeNoAggro[233] = true;
            player.npcTypeNoAggro[234] = true;
            player.npcTypeNoAggro[235] = true;
            fargoPlayer.QueenStinger = true;

            //necromantic brew
            fargoPlayer.NecromanticBrew = true;

            //supreme deathbringer fairy
            fargoPlayer.SupremeDeathbringerFairy = true;

            //pure heart
            fargoPlayer.PureHeart = true;

            //corrupt heart
            fargoPlayer.CorruptHeart = true;
            if (fargoPlayer.CorruptHeartCD > 0)
                fargoPlayer.CorruptHeartCD -= 2;

            //gutted heart
            fargoPlayer.GuttedHeart = true;
            fargoPlayer.GuttedHeartCD -= 2; //faster spawns

            //mutant antibodies
            player.buffImmune[BuffID.Wet] = true;
            player.buffImmune[BuffID.Rabies] = true;
            fargoPlayer.MutantAntibodies = true;
            if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
                player.dripping = true;

            //lump of flesh
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.Obstructed] = true;
            player.buffImmune[BuffID.Dazed] = true;
            fargoPlayer.SkullCharm = true;
            if (!player.ZoneDungeon)
            {
                player.npcTypeNoAggro[NPCID.SkeletonSniper] = true;
                player.npcTypeNoAggro[NPCID.SkeletonCommando] = true;
                player.npcTypeNoAggro[NPCID.TacticalSkeleton] = true;
                player.npcTypeNoAggro[NPCID.DiabolistRed] = true;
                player.npcTypeNoAggro[NPCID.DiabolistWhite] = true;
                player.npcTypeNoAggro[NPCID.Necromancer] = true;
                player.npcTypeNoAggro[NPCID.NecromancerArmored] = true;
                player.npcTypeNoAggro[NPCID.RaggedCaster] = true;
                player.npcTypeNoAggro[NPCID.RaggedCasterOpenCoat] = true;
            }

            //sinister icon
            if (player.GetToggleValue("MasoIcon"))
                fargoPlayer.SinisterIcon = true;
            if (player.GetToggleValue("MasoIconDrops"))
                fargoPlayer.SinisterIconDrops = true;

            //sparkling adoration
            /*if (SoulConfig.Instance.GetValue(SoulConfig.Instance.Graze, false))
                player.GetModPlayer<FargoPlayer>().Graze = true;

            if (SoulConfig.Instance.GetValue(SoulConfig.Instance.DevianttHearts))
                player.GetModPlayer<FargoPlayer>().DevianttHearts = true;*/

            //dragon fang
            if (player.GetToggleValue("MasoClipped"))
                fargoPlayer.DragonFang = true;

            //frigid gemstone
            player.buffImmune[BuffID.Frostburn] = true;
            if (player.GetToggleValue("MasoFrigid"))
            {
                fargoPlayer.FrigidGemstone = true;
                if (fargoPlayer.FrigidGemstoneCD > 0)
                    fargoPlayer.FrigidGemstoneCD -= 5;
            }

            //wretched pouch
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[mod.BuffType("Shadowflame")] = true;
            player.GetModPlayer<FargoPlayer>().WretchedPouch = true;

            //sands of time
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;

            //mystic skull
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaFlower = true;

            //security wallet
            fargoPlayer.SecurityWallet = true;

            //carrot
            player.nightVision = true;
            if (player.GetToggleValue("MasoCarrot", false))
                player.scope = true;

            //squeaky toy
            fargoPlayer.SqueakyAcc = true;

            //tribal charm
            player.buffImmune[BuffID.Webbed] = true;
            fargoPlayer.TribalCharm = true;

            //nymph's perfume
            player.buffImmune[BuffID.Lovestruck] = true;
            player.buffImmune[BuffID.Stinky] = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
            if (player.GetToggleValue("MasoNymph"))
            {
                fargoPlayer.NymphsPerfume = true;
                if (fargoPlayer.NymphsPerfumeCD > 0)
                    fargoPlayer.NymphsPerfumeCD -= 10;
            }

            //tim's concoction
            if (player.whoAmI == Main.myPlayer && player.GetToggleValue("MasoConcoction"))
                player.GetModPlayer<FargoPlayer>().TimsConcoction = true;

            //dubious circuitry
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            fargoPlayer.FusedLens = true;
            fargoPlayer.GroundStick = true;
            player.noKnockback = true;

            //magical bulb
            player.buffImmune[BuffID.Venom] = true;

            //ice queen's crown
            player.buffImmune[BuffID.Frozen] = true;

            //lihzahrd treasure
            player.buffImmune[BuffID.Burning] = true;
            fargoPlayer.LihzahrdTreasureBox = true;

            //saucer control console
            player.buffImmune[BuffID.Electrified] = true;

            //betsy's heart
            player.buffImmune[BuffID.OgreSpit] = true;
            player.buffImmune[BuffID.WitheredWeapon] = true;
            player.buffImmune[BuffID.WitheredArmor] = true;
            fargoPlayer.BetsysHeart = true;

            //celestial rune/pumpking's cape
            fargoPlayer.CelestialRune = true;
            fargoPlayer.PumpkingsCape = true;
            fargoPlayer.AdditionalAttacks = true;
            if (fargoPlayer.AdditionalAttacksTimer > 0)
                fargoPlayer.AdditionalAttacksTimer -= 2;

            //chalice
            fargoPlayer.MoonChalice = true;

            //galactic globe
            player.buffImmune[BuffID.VortexDebuff] = true;
            //player.buffImmune[BuffID.ChaosState] = true;
            fargoPlayer.GravityGlobeEX = true;
            if (player.GetToggleValue("MasoGrav"))
                player.gravControl = true;

            //heart of maso
            fargoPlayer.MasochistHeart = true;
            player.buffImmune[BuffID.MoonLeech] = true;

            //cyclonic fin
            /*fargoPlayer.CyclonicFin = true;
            if (fargoPlayer.CyclonicFinCD > 0)
                fargoPlayer.CyclonicFinCD -= 2;*/
            /*if (player.mount.Active && player.mount.Type == MountID.CuteFishron)
            {
                if (player.ownedProjectileCounts[mod.ProjectileType("CuteFishronRitual")] < 1 && player.whoAmI == Main.myPlayer)
                    Projectile.NewProjectile(player.MountedCenter, Vector2.Zero, mod.ProjectileType("CuteFishronRitual"), 0, 0f, Main.myPlayer);
                player.MountFishronSpecialCounter = 300;
                player.meleeDamage += 0.15f;
                player.rangedDamage += 0.15f;
                player.magicDamage += 0.15f;
                player.minionDamage += 0.15f;
                player.meleeCrit += 30;
                player.rangedCrit += 30;
                player.magicCrit += 30;
                player.statDefense += 30;
                player.lifeRegen += 3;
                player.lifeRegenCount += 3;
                player.lifeRegenTime += 3;
                if (player.controlLeft == player.controlRight)
                {
                    if (player.velocity.X != 0)
                        player.velocity.X -= player.mount.Acceleration * Math.Sign(player.velocity.X);
                    if (player.velocity.X != 0)
                        player.velocity.X -= player.mount.Acceleration * Math.Sign(player.velocity.X);
                }
                else if (player.controlLeft)
                {
                    player.velocity.X -= player.mount.Acceleration * 4f;
                    if (player.velocity.X < -16f)
                        player.velocity.X = -16f;
                    if (!player.controlUseItem)
                        player.direction = -1;
                }
                else if (player.controlRight)
                {
                    player.velocity.X += player.mount.Acceleration * 4f;
                    if (player.velocity.X > 16f)
                        player.velocity.X = 16f;
                    if (!player.controlUseItem)
                        player.direction = 1;
                }
                if (player.controlUp == player.controlDown)
                {
                    if (player.velocity.Y != 0)
                        player.velocity.Y -= player.mount.Acceleration * Math.Sign(player.velocity.Y);
                    if (player.velocity.Y != 0)
                        player.velocity.Y -= player.mount.Acceleration * Math.Sign(player.velocity.Y);
                }
                else if (player.controlUp)
                {
                    player.velocity.Y -= player.mount.Acceleration * 4f;
                    if (player.velocity.Y < -16f)
                        player.velocity.Y = -16f;
                }
                else if (player.controlDown)
                {
                    player.velocity.Y += player.mount.Acceleration * 4f;
                    if (player.velocity.Y > 16f)
                        player.velocity.Y = 16f;
                }
            }*/

            //sadism
            player.buffImmune[ModContent.BuffType<Antisocial>()] = true;
            player.buffImmune[ModContent.BuffType<Atrophied>()] = true;
            player.buffImmune[ModContent.BuffType<Berserked>()] = true;
            player.buffImmune[ModContent.BuffType<Bloodthirsty>()] = true;
            player.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            player.buffImmune[ModContent.BuffType<Crippled>()] = true;
            player.buffImmune[ModContent.BuffType<CurseoftheMoon>()] = true;
            player.buffImmune[ModContent.BuffType<Defenseless>()] = true;
            player.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
            player.buffImmune[ModContent.BuffType<Flipped>()] = true;
            player.buffImmune[ModContent.BuffType<FlippedHallow>()] = true;
            player.buffImmune[ModContent.BuffType<Fused>()] = true;
            //player.buffImmune[ModContent.BuffType<GodEater>()] = true;
            player.buffImmune[ModContent.BuffType<Guilty>()] = true;
            player.buffImmune[ModContent.BuffType<Hexed>()] = true;
            player.buffImmune[ModContent.BuffType<Hypothermia>()] = true;
            player.buffImmune[ModContent.BuffType<Infested>()] = true;
            player.buffImmune[ModContent.BuffType<IvyVenom>()] = true;
            player.buffImmune[ModContent.BuffType<Jammed>()] = true;
            player.buffImmune[ModContent.BuffType<Lethargic>()] = true;
            player.buffImmune[ModContent.BuffType<LihzahrdCurse>()] = true;
            player.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            player.buffImmune[ModContent.BuffType<LivingWasteland>()] = true;
            player.buffImmune[ModContent.BuffType<Lovestruck>()] = true;
            player.buffImmune[ModContent.BuffType<LowGround>()] = true;
            player.buffImmune[ModContent.BuffType<MarkedforDeath>()] = true;
            player.buffImmune[ModContent.BuffType<Midas>()] = true;
            player.buffImmune[ModContent.BuffType<MutantNibble>()] = true;
            player.buffImmune[ModContent.BuffType<NanoInjection>()] = true;
            player.buffImmune[ModContent.BuffType<NullificationCurse>()] = true;
            player.buffImmune[ModContent.BuffType<Oiled>()] = true;
            player.buffImmune[ModContent.BuffType<OceanicMaul>()] = true;
            player.buffImmune[ModContent.BuffType<Purified>()] = true;
            player.buffImmune[ModContent.BuffType<ReverseManaFlow>()] = true;
            player.buffImmune[ModContent.BuffType<Rotting>()] = true;
            player.buffImmune[ModContent.BuffType<Shadowflame>()] = true;
            player.buffImmune[ModContent.BuffType<SqueakyToy>()] = true;
            player.buffImmune[ModContent.BuffType<Swarming>()] = true;
            player.buffImmune[ModContent.BuffType<Stunned>()] = true;
            player.buffImmune[ModContent.BuffType<Unstable>()] = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(mod.ItemType("SinisterIcon"));
            //recipe.AddIngredient(mod.ItemType("SparklingAdoration"));
            recipe.AddIngredient(mod.ItemType("SupremeDeathbringerFairy"));
            recipe.AddIngredient(mod.ItemType("BionomicCluster"));
            recipe.AddIngredient(mod.ItemType("DubiousCircuitry"));
            recipe.AddIngredient(mod.ItemType("PureHeart"));
            recipe.AddIngredient(mod.ItemType("LumpOfFlesh"));
            recipe.AddIngredient(mod.ItemType("ChaliceoftheMoon"));
            recipe.AddIngredient(mod.ItemType("HeartoftheMasochist"));
            //recipe.AddIngredient(mod.ItemType("CyclonicFin"));
            //recipe.AddIngredient(mod.ItemType("Sadism"), 30);
            recipe.AddIngredient(mod.ItemType("MutantScale"), 15);
            recipe.AddIngredient(mod.ItemType("DeviatingEnergy"), 15);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
