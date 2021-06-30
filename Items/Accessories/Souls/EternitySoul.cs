using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Localization;
using System.Collections.Generic;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class EternitySoul : SoulsItem
    {
        public static int[] tooltipIndex = new int[7];
        public static int Counter = 5;

        private List<String> tooltipsFull = new List<String>();

        private String[] vanillaTooltips = new String[]
        {
    "250% increased damage",
    "250% increased attack speed",
    "100% increased shoot speed",
    "100% increased knockback",
    "Increases armor penetration by 50",
    "Crits deal 10x damage",
    "Drastically increases life regeneration",
    "Increases your maximum mana to 999",
    "Increases your maximum minions by 30",
    "Increases your maximum sentries by 20",
    "Increases your maximum HP by 50%",
    "All attacks inflict Flames of the Universe",
    "All attacks inflict Sadism",
    "All attacks inflict Midas",
    "All attacks reduce enemy immunity frames",
    "Summons fireballs arund you",
    "Summons 2 shadow orbs around you",
    "Summons icicles around you",
    "Summons leaf crystals around you",
    "Summons a hallowed sword and shield",
    "Summons beetles to protect you",
    "Summons a Flameburst minion",
    "Summons a ton of pets",
    "Summons all Eternity Mode bosses to your side ",
    "Attacks may spawn lightning",
    "Attacks may spawn flower petals",
    "Attacks may spawn spectre orbs",
    "Attacks may spawn a Dungeon Guardian",
    "Attacks may spawn snowballs",
    "Attacks may spawn spears",
    "Attacks may spawn hearts",
    "Attacks may spawn a miniture storm",
    "Attacks may spawn buff boosters",
    "Attacks cause increased life regen",
    "Attacks cause shadow dodge",
    "Attacks cause Flameburst shots",
    "Attacks cause Pumpking attacks",
    "Attacks cause Cultist spells",
    "Attacks cause meteor showers",
    "All Projectiles will split",
    "Projectiles may shatter",
    "Projectiles spawn stars",
    "Item and projectile size increased",
    "You leave a trail of fire",
    "Nearby enemies are ignited",
    "Minions occasionally spew scythes",
    "You may spawn temporary minions",
    "Critters have increased defense",
    "Critter's souls may aid you",
    "Enemies explode into needles",
    "Greatly enhances all DD2 sentries",
    "Double-tap down to spawn a palm tree sentry",
    "Double-tap down to call an ancient storm",
    "Double-tap down to call a rain of arrows",
    "Double-tap down to toggle stealth",
    "Double-tap down to spawn a portal",
    "Double-tap down to direct your empowered guardian",
    "Right Click to Guard",
    "Press the Gold Key to encase yourself in gold",
    "Press the Freeze Key to freeze time for 5 seconds",
    "Solar shield allows you to dash",
    "Dashing into solid blocks teleports you through them",
    "Throw a smoke bomb to teleport to it and gain the first strike buff",
    "Jumping will release a spore explosion",
    "Enemies getting too close will trigger all on hit effects",
    "Getting hit reflects damage",
    "Getting hit triggers a blood geyser",
    "Getting hit may squeak",
    "Getting hit causes you to erupt into spiky balls",
    "Getting hit causes you to erupt into Ancient Visions",
    "Grants Crimson regen",
    "Grants immunity to fire",
    "Grants immunity to fall damage",
    "Grants immunity to lava",
    "Grants immunity to knockback",
    "Grants immunity to most debuffs", //expand?? ech
	"Grants doubled herb collection",
    "Grants 50% chance for Mega Bees",
    "15% chance for minion crits",
    "20% chance for bonus loot",
    "Allows Supersonic running and ",
    "Allows infinite flight",
    "Increases fishing skill substantially",
    "All fishing rods will have 10 extra lures",
    "You respawn 10x as fast",
    "Prevents boss spawns",
    "Increases spawn rates",
    "Reduces skeletons hostility outside of the dungeon",
    "Empowers Cute Fishron",
    "Grants autofire",
    "Grants modifier protection",
    "Grants gravity control",
    "Grants fast fall",
    "Enhances grappling hooks",
    "You attract items from further away",
    "Increased block and wall placement speed by 50%",
    "Near infinite block placement",
    "Near infinite mining reach",
    "Mining speed dramatically increased",
    "You reflect all projectiles",
    "When you are hurt, you violently explode to damage nearby enemies",
    "When you die, you revive with full HP",
    "Effects of Fire Gauntlet",
    "Effects of Yoyo Bag",
    "Effects of Sniper Scope",
    "Effects of Celestial Cuffs",
    "Effects of Mana Flower",
    "Effects of Brain of Confusion",
    "Effects of Star Veil",
    "Effects of Sweetheart Necklace",
    "Effects of Bee Cloak",
    "Effects of Spore Sac",
    "Effects of Paladin's Shield",
    "Effects of Frozen Turtle Shell",
    "Effects of Arctic Diving Gear",
    "Effects of Frog Legs",
    "Effects of Flying Carpet",
    "Effects of Lava Waders",
    "Effects of Angler Tackle Bag",
    "Effects of Paint Sprayer",
    "Effects of Presserator",
    "Effects of Cell Phone",
    "Effects of Flower Boots",
    "Effects of Master Ninja Gear",
    "Effects of Greedy Ring",
    "Effects of Celestial Shell",
    "Effects of Shiny Stone",
    "Effects of Spelunker potion",
    "Effects of Dangersense potion",
    "Effects of Hunter potion",
    "Effects of Shine potion",
    "Effects of Builder Mode"
        };

        private String[] thoriumTooltips = new String[]
        {
            "Armor bonuses from Living Wood",
            "Armor bonuses from Life Bloom",
            "Armor bonuses from Yew Wood",
            "Armor bonuses from Tide Hunter",
            "Armor bonuses from Icy",
            "Armor bonuses from Cryo Magus",
            "Armor bonuses from Whispering",
            "Armor bonuses from Sacred",
            "Armor bonuses from Warlock",
            "Armor bonuses from Biotech",
            "Armor bonuses from Cyber Punk",
            "Armor bonuses from Maestro",
            "Armor bonuses from Bronze",
            "Armor bonuses from Darksteel",
            "Armor bonuses from Durasteel",
            "Armor bonuses from Conduit",
            "Armor bonuses from Lodestone",
            "Armor bonuses from Illumite",
            "Armor bonuses from Jester",
            "Armor bonuses from Thorium",
            "Armor bonuses from Terrarium",
            "Armor bonuses from Malignant",
            "Armor bonuses from Folv",
            "Armor bonuses from White Dwarf",
            "Armor bonuses from Celestial",
            "Armor bonuses from Spirit Trapper",
            "Armor bonuses from Dragon",
            "Armor bonuses from Dread",
            "Armor bonuses from Flesh",
            "Armor bonuses from Demon Blood",
            "Armor bonuses from Tide Turner",
            "Armor bonuses from Assassin",
            "Armor bonuses from Pyromancer",
            "Armor bonuses from Dream Weaver",
            "Effects of Flawless Chrysalis",
            "Effects of Bubble Magnet",
            "Effects of Agnor's Bowl",
            "Effects of Ice Bound Strider Hide",
            "Effects of Ring of Unity",
            "Effects of Mix Tape",
            "Effects of Eye of the Storm",
            "Effects of Champion's Rebuttal",
            "Effects of Incandescent Spark",
            "Effects of Greedy Magnet",
            "Effects of Abyssal Shell",
            "Effects of Astro-Beetle Husk",
            "Effects of Eye of the Beholder",
            "Effects of Crietz",
            "Effects of Mana-Charged Rocketeers",
            "Effects of Inner Flame",
            "Effects of Crash Boots",
            "Effects of Vampire Gland",
            "Effects of Demon Blood Badge",
            "Effects of Lich's Gaze",
            "Effects of Plague Lord's Flask",
            "Effects of Phylactery",
            "Effects of Crystal Scorpion",
            "Effects of Guide to Expert Throwing - Volume III",
            "Effects of Mermaid's Canteen",
            "Effects of Deadman's Patch",
            "Effects of Support Sash",
            "Effects of Saving Grace",
            "Effects of Soul Guard",
            "Effects of Archdemon's Curse",
            "Effects of Archangel's Heart",
            "Effects of Medical Bag",
            "Effects of Epic Mouthpiece",
            "Effects of Straight Mute",
            "Effects of Digital Tuner",
            "Effects of Guitar Pick Claw",
            "Effects of Ocean's Retaliation",
            "Effects of Terrarium Defender",
            "Effects of Air Walkers",
            "Effects of Survivalist Boots",
            "Effects of Weighted Winglets"
        };

        private String[] calamityTooltips = new String[]
        {
            "Armor bonuses from Aerospec",
            "Armor bonuses from Statigel",
            "Armor bonuses from Daedalus",
            "Armor bonuses from Bloodflare",
            "Armor bonuses from Victide",
            "Armor bonuses from Xeroc",
            "Armor bonuses from Omega Blue",
            "Armor bonuses from God Slayer",
            "Armor bonuses from Silva",
            "Armor bonuses from Auric Tesla",
            "Armor bonuses from Mollusk",
            "Armor bonuses from Reaver",
            "Armor bonuses from Ataxia",
            "Armor bonuses from Astral",
            "Armor bonuses from Tarragon",
            "Armor bonuses from Demonshade",
            "Effects of Spirit Glyph",
            "Effects of Raider's Talisman",
            "Effects of Trinket of Chi",
            "Effects of Gladiator's Locket",
            "Effects of Unstable Prism",
            "Effects of Counter Scarf",
            "Effects of Fungal Symbiote",
            "Effects of Permafrost's Concoction",
            "Effects of Regenator",
            "Effects of Core of the Blood God",
            "Effects of Affliction",
            "Effects of Deep Dive",
            "Effects of The Transformer",
            "Effects of Luxor's Gift",
            "Effects of The Community",
            "Effects of Abyssal Diving Suit",
            "Effects of Lumenous Amulet",
            "Effects of Aquatic Emblem",
            "Effects of Nebulous Core",
            "Effects of Draedon's Heart",
            "Effects of The Amalgam",
            "Effects of Godly Soul Artifact",
            "Effects of Yharim's Gift",
            "Effects of Heart of the Elements",
            "Effects of The Sponge",
            "Effects of Giant Pearl",
            "Effects of Amidias' Pendant",
            "Effects of Fabled Tortoise Shell",
            "Effects of Plague Hive",
            "Effects of Astral Arcanum",
            "Effects of Hide of Astrum Deus",
            "Effects of Profaned Soul Artifact",
            "Effects of Dark Sun Ring",
            "Effects of Elemental Gauntlet",
            "Effects of Elemental Quiver",
            "Effects of Ethereal Talisman",
            "Effects of Statis' Belt of Curses",
            "Effects of Nanotech",
            "Effects of Asgardian Aegis"
        };

        private String[] dbtTooltips = new String[]
        {
            "Effects of Zenkai Charm",
            "Effects of Aspera Crystallite"
        };

        private String[] soaTooltips = new String[]
        {
            "Armor bonuses from Bismuth",
            "Armor bonuses from Frosthunter",
            "Armor bonuses from Blightbone",
            "Armor bonuses from Dreadfire",
            "Armor bonuses from Space Junk",
            "Armor bonuses from Marstech",
            "Armor bonuses from Blazing Brute",
            "Armor bonuses from Cosmic Commander",
            "Armor bonuses from Nebulous Apprentic",
            "Armor bonuses from Stellar Priest",
            "Armor bonuses from Fallen Prince",
            "Armor bonuses from Void Warden",
            "Armor bonuses from Vulcan Reaper",
            "Armor bonuses from Flarium",
            "Armor bonuses from Asthraltite",
            "Effects of Dreadflame Emblem",
            "Effects of Lapis Pendant",
            "Effects of Frigid Pendant",
            "Effects of Pumpkin Amulet",
            "Effects of Nuba's Blessing",
            "Effects of Novaniel's Resolve",
            "Effects of Celestial Ring",
            "Effects of Ring of the Fallen",
            "Effects of Memento Mori",
            "Effects of Arcanum of the Caster"
        };

        public override bool Eternity => true;
        public override int NumFrames => 10;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul of Eternity");

            //oh no idk even for translate
            string tooltip_ch =
@"“请自行理解它的加成。”
大幅增加生命回复, 最大法力值增至999 ,+30最大召唤栏, +20最大哨兵栏, 增加500%最大生命值 , 50%伤害减免
增加250%所有类型伤害和攻击速度; 增加100%射击速度与击退; 增加50点护甲穿透; 暴击造成10倍伤害, 暴击率设为50%
每次暴击提高10%, 达到100%时所有攻击附带10%的生命偷取, 增加10%伤害, 增加10防御力; 可叠加200,000次, 直至被攻击
所有攻击造成宇宙之火, 施虐狂, 点金术效果, 并削减敌人的击退免疫
召唤冰柱, 叶绿水晶, 神圣剑盾，甲虫, 数个宠物, 山铜火球和所有受虐模式的Boss到你身边
攻击概率产生闪电, 花瓣, 幽灵球, 地牢守卫者, 雪球, 长矛或者增益
攻击造成生命回复增加, 暗影闪避, 焰爆射击和流星雨
抛射物可能会分裂或散开, 物品和抛射物尺寸增加, 攻击造成额外攻击并生成心
身后留下火焰与彩虹路径; 点燃附近敌人; 召唤物偶尔发射镰刀, 有概率生成临时召唤物
大幅增加动物防御力, 它们的灵魂会在死后帮助你; 敌人会爆炸成刺; 极大增强所有地牢守卫者2(联动的塔防内容)的哨兵
双击'下'键生成一个哨兵, 召唤远古风暴, 切换潜行, 生成一个传送门, 指挥你的替身
右键格挡; 按下金身热键, 使自己被包裹在一个黄金壳中; 按下时间冻结热键时停5秒
日耀护盾允许你双击冲刺, 遇到墙壁自动穿透; 扔烟雾弹进行传送, 获得先发制人Buff
受击反弹伤害, 释放包孢子爆炸, 使敌人大出血, 敌人攻击概率无效化, 受伤时爆发各种乱七八糟的玩意
获得血腥套的生命回复效果, 免疫火焰, 坠落伤害和岩浆, 药草收获翻倍, 蜜蜂50%概率变为巨型蜜蜂, 召唤物获得15%暴击率, 20%概率获得额外掉落
免疫击退和诸多Debuff; 允许超音速奔跑和无限飞行; 大幅提升钓鱼能力, 所有钓竿获得额外10个鱼饵
重生速度x10倍；阻止Boss自然生成, 增加刷怪速率, 减少地牢外骷髅的敌意, 增强超可爱猪鲨
武器自动连发, 获得词缀保护, 能够控制重力, 增加掉落速度, 免疫击退和所有受虐模式的Debuff, 增强抓钩以及更多其他效果
增加50%放置物块及墙壁的速度, 近乎无限的放置和采掘距离, 极大提高采掘速度
召唤无可阻挡的死亡之环环绕周围, 反弹所有抛射物; 死亡时爆炸并满血复活
拥有烈火手套, 悠悠球袋, 狙击镜, 星体手铐, 魔力花, 混乱之脑, 星辰项链, 甜心项链和蜜蜂斗篷的效果
拥有孢子囊, 圣骑士护盾, 冰霜龟壳, 北极潜水装备, 蛙腿, 飞毯, 熔岩行走靴和渔具包的效果
拥有油漆喷雾器, 促动安装器, 手机, 重力球, 花之靴, 忍者极意, 贪婪戒指, 天界贝壳和闪耀石的效果
获得发光, 探索者, 猎人和危险感知效果; 获得建造模式权限, 拥有无尽遗物的效果, 可以超远程拾取物品";
            string tooltip_sp = @"'Mortal o Inmortal, todas las cosas reconocen tu reclamación a la divinidad'
Drasticamente incrementa regeneración de vida, incrementa tu mana máximo a 999, súbditos por 30, torretas por 20, vida maxima por 500%, reducción de daño por 50%
250% daño incrementado y velocidad de ataque; 100% velocidad de disparo y retroceso; Incrementa penetración de armadura por 50; Críticos hacen 10x daño y la probabilidad de Crítico se vuelve 50%
Consigue un crítico para incrementarlo por 10%, a 100% cada ataque gana 10% robo de vida y ganas +10% daño y +10 defensa; Esto se apila hasta 200,000 veces hasta que te golpeen
Todos los ataques inflijen Llamas del Universo, Sadismo, Midas y reduce inmunidad de retroceso de los enemigos
Invoca estalactitas, un cristal de hojas, espada y escudo benditos, escarabajos, varias mascotas, bolas de fuego de oricalco y todos los jefes del Modo Masoquista a tu lado
Ataques pueden crear rayos, pétalos, orbes espectrales, un Guardián de la mazmorra, bolas de nieve, lanzas, o potenciadores
Ataques provocan regeneración de vida incrementada, Sombra de Esquivo, explociones de llamas y lluvias de meteoros
Projectiles pueden dividirse o quebrarse, tamaño de objetos y projectiles incrementado, ataques crean ataques adicionales y corazones
Dejas un rastro de fuego y arcoirises; Enemigos cercanos son incinerados y súbditos escupen guadañas ocasionalmente
Animales tienen defensa incrementada y sus almas te ayudarán; Enemigos explotan en agujas; Mejora todas las torretas DD2 grandemente
Tocar dos veces abajo para invocar una torreta, llamar a una tormenta antigua, activar sigilo, invocar un portal, y dirigir tu guardián
Click derecho para Defender; Presiona la Llave dorada para encerrarte en oro; Presiona la Llave congelada para congelar el tiempo por 5 segundos
Escudo de bengala solar te permite embestir, embestir bloques sólidos te teletransporta a través de ellos; Tira una bomba de huma para teletransportarte a ella y obtener el buff de primer golpe
Ser golpeado refleja el daño, suelta una exploción de esporas, inflije super sangrado, puede chillar y causa que erupciones en varias cosas cuando seas dañado
Otorga regeneración carmesí, inmunidad al fuego, daño por caída, y lava, duplica la colección de hierbas
50% probabilidad de Abejas gigantes, 15% probabilidad de críticos de súbditos, 20% probabilidad de botín extra
Otorga inmunidad a la mayoría de estados alterados; Permite velocidad Supersónica y vuelo infinito; Incrementa poder de pesca substancialmente y todas las cañas de pescar tienen 10 señuelos extra
Revives 10x más rapido; Evita invocación de jefes, incrementa generación de enemigos, reduce la hostilidad de esqueletos fuera de la mazmorra y fortalece a Cute Fishron
Otorga ataque continuo, protección de modificadores, control de gravedad, caída rápida, e inmunidad a retroceso, todos los estados alterados del Modo Masoquista, mejora ganchos y más
Incrementa velocidad de colocación de bloques y paredes por 50%, Casi infinito alcance de colocación de bloques y alcance de minar, Velocidad de minería duplicada
Invoca un anillo de la muerte inpenetrable alrededor de tí y tu reflejas todos los projectiles; Cuando mueres, explotas y revives con vida al máximo
Efectos del Guantelete de fuego, Bolsa yoyó, Mira de francotirador, Esposas celestiales, Flor de maná, Cerebro de confusión, Velo estelar, Collar del cariño, y Capa de abejas
Efectos del Saco de esporas, Escudo de paladín, Caparazón de tortuga congelado, Equipo de buceo ártico, Anca de rana, Alfombra voladora, Katiuskas de lava, y Bolsa de aparejos de pescador
Efectos del Spray de pintura, Pulsificador, Móvil, Globo gravitacional, Botas floridas, Equipo de maestro ninja, Anillo codicioso, Caparazón celestial, y Piedra brillante
Efectos de pociones de Brillo, Espeleólogo, Cazador, y Sentido del peligro; Efectos del Modo Constructor, Reliquia del Infinito y atraes objectos desde más lejos";

            DisplayName.AddTranslation(GameCulture.Chinese, "永恒之魂");
            DisplayName.AddTranslation(GameCulture.Spanish, "Alma de la Eternidad");
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
            Tooltip.AddTranslation(GameCulture.Spanish, tooltip_sp);

            Tooltip.SetDefault(
@"'Mortal or Immortal, all things acknowledge your claim to divinity'
Crit chance is set to 50%
Crit to increase it by 10%
At 100% every attack gains 10% life steal
You also gain +5% damage and +5 defense
This stacks up to 950 times until you get hit");

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 10));
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltipsFull.AddRange(vanillaTooltips);

            string description = "Additionally grants:";

            for (int i = 0; i < tooltipIndex.Length; i++)
            {
                description += "\n" + tooltipsFull[tooltipIndex[i]];
            }

            tooltips.Add(new TooltipLine(mod, "tooltip", description));

            Counter--;

            if (Counter <= 0)
            {
                int segment = tooltipsFull.Count / tooltipIndex.Length;
                for (int i = 0; i < tooltipIndex.Length; i++)
                {
                    tooltipIndex[i] = segment * i + Main.rand.Next(segment);
                }

                Counter = 5;
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                var lineshader = GameShaders.Misc["PulseUpwards"].UseColor(new Color(42, 42, 99)).UseSecondaryColor(Fargowiltas.EModeColor());
                lineshader.Apply(null);
                Utils.DrawBorderString(Main.spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Red;
            item.value = 100000000;
            item.shieldSlot = 5;
            item.defense = 100;

            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useTime = 1;
            item.UseSound = SoundID.Item6;
            item.useAnimation = 1;
        }

        public override bool UseItem(Player player)
        {
            player.Spawn();

            for (int num348 = 0; num348 < 70; num348++)
            {
                Dust.NewDust(player.position, player.width, player.height, 15, 0f, 0f, 150, default(Color), 1.5f);
            }

            return base.UseItem(player);
        }

        public override void UpdateInventory(Player player)
        {
            //cell phone
            player.accWatch = 3;
            player.accDepthMeter = 1;
            player.accCompass = 1;
            player.accFishFinder = true;
            player.accDreamCatcher = true;
            player.accOreFinder = true;
            player.accStopwatch = true;
            player.accCritterGuide = true;
            player.accJarOfSouls = true;
            player.accThirdEye = true;
            player.accCalendar = true;
            player.accWeatherRadio = true;
            //bionomic
            FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();
            player.buffImmune[BuffID.WindPushed] = true;
            fargoPlayer.SandsofTime = true;
            player.buffImmune[BuffID.Suffocation] = true;
            player.manaFlower = true;
            fargoPlayer.SecurityWallet = true;
            fargoPlayer.TribalCharm = true;
            fargoPlayer.NymphsPerfumeRespawn = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //auto use, debuffs, mana up
            modPlayer.Eternity = true;

            //UNIVERSE
            modPlayer.UniverseEffect = true;
            modPlayer.AllDamageUp(2.5f);
            if (player.GetToggleValue("Universe"))
            {
                modPlayer.AttackSpeed += 2.5f;
            }
            player.maxMinions += 20;
            player.maxTurrets += 10;
            //accessorys
            if (player.GetToggleValue("YoyoBag"))
            {
                player.counterWeight = 556 + Main.rand.Next(6);
                player.yoyoGlove = true;
                player.yoyoString = true;
            }
            if (player.GetToggleValue("Sniper"))
            {
                player.scope = true;
            }
            player.manaFlower = true;
            player.manaMagnet = true;
            player.magicCuffs = true;

            //DIMENSIONS
            player.statLifeMax2 *= 5;
            player.buffImmune[BuffID.ChaosState] = true;
            modPlayer.ColossusSoul(0, 0.4f, 15, hideVisual);
            modPlayer.SupersonicSoul(hideVisual);
            modPlayer.FlightMasterySoul();
            modPlayer.TrawlerSoul(hideVisual);
            modPlayer.WorldShaperSoul(hideVisual);

            //TERRARIA
            mod.GetItem("TerrariaSoul").UpdateAccessory(player, hideVisual);

            //MASOCHIST
            mod.GetItem("MasochistSoul").UpdateAccessory(player, hideVisual);

            if (ModLoader.GetMod("FargowiltasSoulsDLC") != null)
            {
                Mod fargoDLC = ModLoader.GetMod("FargowiltasSoulsDLC");

                if (ModLoader.GetMod("ThoriumMod") != null)
                {
                    fargoDLC.GetItem("ThoriumSoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.GetMod("CalamityMod") != null)
                {
                    fargoDLC.GetItem("CalamitySoul").UpdateAccessory(player, hideVisual);
                }
                if (ModLoader.GetMod("SacredTools") != null)
                {
                    fargoDLC.GetItem("SoASoul").UpdateAccessory(player, hideVisual);
                }
            }
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            player.wingsLogic = 22;
            ascentWhenFalling = 0.9f; //0.85f
            ascentWhenRising = 0.3f; //0.15f
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.14f; //0.135f
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = player.GetToggleValue("Supersonic") ? 25f : 18f;
            acceleration *= 3.5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UniverseSoul");
            recipe.AddIngredient(null, "DimensionSoul");
            recipe.AddIngredient(null, "TerrariaSoul");
            recipe.AddIngredient(null, "MasochistSoul");

            if (ModLoader.GetMod("FargowiltasSoulsDLC") != null)
            {
                Mod fargoDLC = ModLoader.GetMod("FargowiltasSoulsDLC");

                if (ModLoader.GetMod("ThoriumMod") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("ThoriumSoul"));
                }
                if (ModLoader.GetMod("CalamityMod") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("CalamitySoul"));
                }
                if (ModLoader.GetMod("SacredTools") != null)
                {
                    recipe.AddIngredient(fargoDLC.ItemType("SoASoul"));
                }
            }

            recipe.AddIngredient(null, "Sadism", 30);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
