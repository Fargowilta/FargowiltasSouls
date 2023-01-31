using FargowiltasSouls.Items.Accessories.Masomode;
using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Wings)]
    public class EternitySoul : FlightMasteryWings
    {
        protected override bool HasSupersonicSpeed => true;

        public override bool Eternity => true;
        public override int NumFrames => 10;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Soul of Eternity");

            //oh no idk even for translate
            //put the string array in localization files
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

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "永恒之魂");
            DisplayName.AddTranslation((int)GameCulture.CultureName.Spanish, "Alma de la Eternidad");
            Tooltip.AddTranslation((int)GameCulture.CultureName.Spanish, tooltip_sp);

            Tooltip.SetDefault(
@"'Mortal or Immortal, all things acknowledge your claim to divinity'
Crit chance is set to 50%
Crit to increase it by 10%
At 100% every attack gains 10% life steal
You also gain +5% damage and +5 defense
This stacks up to 950 times until you get hit");

            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.EternitySoul.Vanilla");
            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            const int linesToShow = 7;
            int section = lines.Length / linesToShow;

            string description = Language.GetTextValue("Mods.FargowiltasSouls.ItemExtra.EternitySoul.Additional");
            ulong seed = Main.GameUpdateCount / 5;
            for (int i = 0; i < linesToShow; i++)
            {
                int start = section * i;
                description += "\n" + lines[start + Utils.RandomInt(ref seed, section)];
            }

            tooltips.Add(new TooltipLine(Mod, "tooltip", description));
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                var lineshader = GameShaders.Misc["PulseUpwards"].UseColor(new Color(42, 42, 99)).UseSecondaryColor(FargowiltasSouls.EModeColor());
                lineshader.Apply(null);
                Utils.DrawBorderString(Main.spriteBatch, line.Text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Red;
            Item.value = 200000000;
            Item.shieldSlot = 5;
            Item.defense = 100;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 180;
            Item.useAnimation = 180;
            Item.UseSound = SoundID.Item6;
        }

        public override void UseItemFrame(Player player) => SandsofTime.Use(player);
        public override bool? UseItem(Player player) => true;

        void PassiveEffect(Player player)
        {
            BionomicCluster.PassiveEffect(player, Item);

            player.GetModPlayer<FargoSoulsPlayer>().CanAmmoCycle = true;

            player.GetModPlayer<FargoSoulsPlayer>().WoodEnchantDiscount = true;

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
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player);
        public override void UpdateVanity(Player player) => PassiveEffect(player);
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            PassiveEffect(player);

            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            //auto use, debuffs, mana up
            modPlayer.Eternity = true;

            //UNIVERSE
            modPlayer.UniverseSoul = true;
            modPlayer.UniverseCore = true;
            player.GetDamage(DamageClass.Generic) += 2.5f;
            if (player.GetToggleValue("Universe"))
                modPlayer.AttackSpeed += 2.5f;
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
            modPlayer.ColossusSoul(Item, 0, 0.4f, 15, hideVisual);
            modPlayer.SupersonicSoul(Item, hideVisual);
            modPlayer.FlightMasterySoul();
            modPlayer.TrawlerSoul(Item, hideVisual);
            modPlayer.WorldShaperSoul(hideVisual);

            //TERRARIA
            ModContent.Find<ModItem>(Mod.Name, "TerrariaSoul").UpdateAccessory(player, hideVisual);

            //MASOCHIST
            ModContent.Find<ModItem>(Mod.Name, "MasochistSoul").UpdateAccessory(player, hideVisual);

        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(null, "UniverseSoul")
            .AddIngredient(null, "DimensionSoul")
            .AddIngredient(null, "TerrariaSoul")
            .AddIngredient(null, "MasochistSoul")

            .AddIngredient(null, "EternalEnergy", 30)

            .AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"))

            .Register();
        }
    }
}