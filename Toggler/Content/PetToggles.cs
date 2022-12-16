using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Toggler.Content
{
    public class PetToggles : ToggleCollection
    {
        public override string Mod => "Terraria";
        public override string SortCatagory => "Pets";
        public override int Priority => 3;
        public override bool Active => ModLoader.TryGetMod("FargowiltasSoulsDLC", out Mod mod);

        public int PetHeader = ItemID.ZephyrFish;

        public string PetDino;
        public string PetEater;
        public string PetFaceMonster;
        public string PetGrinch;
        public string PetHornet;
        public string PetImp;
        public string PetPenguin;
        public string PetPanda;
        public string PetDG;
        public string PetSnowman;
        public string PetShroom;
        public string PetWerewolf;
        public string PetBernie;
        public string PetBlackCat;
        //public string PetBlueChicken;
        //public string PetCaveling;
        public string PetChester;
        public string PetCompanionCube;
        public string PetCursedSapling;
        //public string PetDirt;
        public string PetKitten;
        public string PetEstee;
        public string PetEyeSpring;
        public string PetFox;
        public string PetButterfly;
        public string PetGlommer;
        public string PetDragon;
        //public string PetJunimo;
        public string PetHarpy;
        public string PetLizard;
        public string PetMinitaur;
        public string PetParrot;
        public string PetPigman;
        public string PetPlantero;
        public string PetGato;
        public string PetPup;
        public string PetSeed;
        public string PetSpider;
        public string PetMimic;
        public string PetShark;
        //public string PetSpiffo;
        public string PetSquash;
        public string PetGlider;
        public string PetTiki;
        public string PetTurtle;
        public string PetVolt;
        public string PetZephyr;

        //LIGHT PETS
        public string PetOrb;
        public string PetHeart;
        public string PetLantern;
        public string PetNavi;
        public string PetFlicker;
        public string PetWisp;
        public string PetSuspEye;

        //MASTER
        public string PetKS;
        public string PetEoC;
        public string PetEoW;
        public string PetBoC;
        public string PetDeer;
        public string PetQB;
        public string PetSkele;
        public string PetQS;
        public string PetDestroyer;
        public string PetTwins;
        public string PetSkelePrime;
        public string PetOgre;
        public string PetPlantera;
        public string PetPumpking;
        public string PetEverscream;
        public string PetIceQueen;
        public string PetDuke;
        public string PetGolem;
        public string PetEoL;
        public string PetBetsy;
        public string PetMartian;
        public string PetLC;
        public string PetML;


    }
}
