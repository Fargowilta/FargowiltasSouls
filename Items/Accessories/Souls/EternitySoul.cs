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
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip1"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip2"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip3"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip4"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip5"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip6"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip7"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip8"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip9"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip10"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip11"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip12"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip13"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip14"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip15"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip16"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip17"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip18"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip19"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip20"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip21"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip22"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip23"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip24"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip25"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip26"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip27"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip28"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip29"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip30"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip31"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip32"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip33"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip34"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip35"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip36"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip37"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip38"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip39"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip40"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip41"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip42"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip43"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip44"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip45"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip46"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip47"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip48"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip49"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip50"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip51"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip52"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip53"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip54"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip55"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip56"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip57"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip58"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip59"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip60"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip61"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip62"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip63"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip64"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip65"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip66"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip67"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip68"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip69"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip70"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip71"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip72"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip73"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip74"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip75"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip76"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip77"),//debuff immunity expand?? ech
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip78"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip79"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip80"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip81"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip82"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip83"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip84"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip85"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip86"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip87"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip88"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip89"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip90"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip91"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip92"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip93"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip94"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip95"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip96"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip97"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip98"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip99"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip100"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip101"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip102"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip103"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip104"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip105"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip106"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip107"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip108"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip109"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip110"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip111"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip112"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip113"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip114"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip115"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip116"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip117"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip118"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip119"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip120"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip121"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip122"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip123"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip124"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip125"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip126"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip127"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip128"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip129"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip130"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip131"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip132"),
            Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.VanillaTooltip133")
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
            //DisplayName.SetDefault("Soul of Eternity");

            //oh no idk even for translate
            //always believe calamitiki
            
            /*string tooltip_sp = @"'Mortal o Inmortal, todas las cosas reconocen tu reclamación a la divinidad'
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
Efectos de pociones de Brillo, Espeleólogo, Cazador, y Sentido del peligro; Efectos del Modo Constructor, Reliquia del Infinito y atraes objectos desde más lejos";*/

            //DisplayName.AddTranslation(GameCulture.Chinese, "永恒之魂");
            //DisplayName.AddTranslation(GameCulture.Spanish, "Alma de la Eternidad");
            //Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);
            //Tooltip.AddTranslation(GameCulture.Spanish, tooltip_sp);

            /*Tooltip.SetDefault(
@"'Mortal or Immortal, all things acknowledge your claim to divinity'
Crit chance is set to 50%
Crit to increase it by 10%
At 100% every attack gains 10% life steal
You also gain +5% damage and +5 defense
This stacks up to 950 times until you get hit");*/

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 10));
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltipsFull.AddRange(vanillaTooltips);

            string description = Language.GetTextValue("Mods.FargowiltasSouls.EternitySoul.OtherTooltips1");

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
            if (player.GetToggleValue("YoyoBag", false))
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
            player.manaCost -= 0.5f;

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
