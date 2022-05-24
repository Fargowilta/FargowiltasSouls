using FargowiltasSouls.Toggler;
using Terraria;
using Terraria.ID;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    public class VoidSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            DisplayName.SetDefault("Soul of the Void");

            string tooltip =
@"Summons SOMETHING";
            Tooltip.SetDefault(tooltip);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = -12;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.GetModPlayer<FargoSoulsPlayer>();
            modPlayer.VoidSoul = true;
            modPlayer.AddPet(player.GetToggleValue("PetHornet"), hideVisual, BuffID.BabyHornet, ProjectileID.BabyHornet);
            modPlayer.AddPet(player.GetToggleValue("PetSeed"), hideVisual, BuffID.PetSapling, ProjectileID.Sapling);
            modPlayer.AddPet(player.GetToggleValue("PetFaceMonster"), hideVisual, BuffID.BabyFaceMonster, ProjectileID.BabyFaceMonster);
            modPlayer.AddPet(player.GetToggleValue("PetHeart"), hideVisual, BuffID.CrimsonHeart, ProjectileID.CrimsonHeart);
            modPlayer.AddPet(player.GetToggleValue("PetFlicker"), hideVisual, BuffID.PetDD2Ghost, ProjectileID.DD2PetGhost);
            modPlayer.AddPet(player.GetToggleValue("PetDino"), hideVisual, BuffID.BabyDinosaur, ProjectileID.BabyDino);
            modPlayer.AddPet(player.GetToggleValue("PetSnowman"), hideVisual, BuffID.BabySnowman, ProjectileID.BabySnowman);
            modPlayer.AddPet(player.GetToggleValue("PetGrinch"), hideVisual, BuffID.BabyGrinch, ProjectileID.BabyGrinch);
            modPlayer.AddPet(player.GetToggleValue("PetMinitaur"), hideVisual, BuffID.MiniMinotaur, ProjectileID.MiniMinotaur);
            modPlayer.AddPet(player.GetToggleValue("PetParrot"), hideVisual, BuffID.PetParrot, ProjectileID.Parrot);
            modPlayer.AddPet(player.GetToggleValue("PetNavi"), hideVisual, BuffID.FairyBlue, ProjectileID.BlueFairy);
            modPlayer.AddPet(player.GetToggleValue("PetLantern"), hideVisual, BuffID.MagicLantern, ProjectileID.MagicLantern);
            //modPlayer.AddPet(SoulConfig.Instance.DGPet, hideVisual, BuffID.BabySkeletronHead, ProjectileID.BabySkeletronHead);
            modPlayer.AddPet(player.GetToggleValue("PetSquash"), hideVisual, BuffID.Squashling, ProjectileID.Squashling);
            modPlayer.AddPet(player.GetToggleValue("PetPup"), hideVisual, BuffID.Puppy, ProjectileID.Puppy);
            modPlayer.AddPet(player.GetToggleValue("PetEater"), hideVisual, BuffID.BabyEater, ProjectileID.BabyEater);
            modPlayer.AddPet(player.GetToggleValue("PetOrb"), hideVisual, BuffID.ShadowOrb, ProjectileID.ShadowOrb);
            modPlayer.AddPet(player.GetToggleValue("PetGato"), hideVisual, BuffID.PetDD2Gato, ProjectileID.DD2PetGato);
            modPlayer.AddPet(player.GetToggleValue("PetShroom"), hideVisual, BuffID.BabyTruffle, ProjectileID.Truffle);
            modPlayer.AddPet(player.GetToggleValue("PetWisp"), hideVisual, BuffID.Wisp, ProjectileID.Wisp);
            modPlayer.AddPet(player.GetToggleValue("PetCursedSapling"), hideVisual, BuffID.CursedSapling, ProjectileID.CursedSapling);
            modPlayer.AddPet(player.GetToggleValue("PetEyeSpring"), hideVisual, BuffID.EyeballSpring, ProjectileID.EyeSpring);
            modPlayer.AddPet(player.GetToggleValue("PetTiki"), hideVisual, BuffID.TikiSpirit, ProjectileID.TikiSpirit);
            modPlayer.AddPet(player.GetToggleValue("PetTurtle"), hideVisual, BuffID.PetTurtle, ProjectileID.Turtle);
            modPlayer.AddPet(player.GetToggleValue("PetLizard"), hideVisual, BuffID.PetLizard, ProjectileID.PetLizard);
            modPlayer.AddPet(player.GetToggleValue("PetDino"), hideVisual, BuffID.PetDD2Dragon, ProjectileID.DD2PetDragon);
            modPlayer.AddPet(player.GetToggleValue("PetCompanionCube"), hideVisual, BuffID.CompanionCube, ProjectileID.CompanionCube);
            modPlayer.AddPet(player.GetToggleValue("PetPenguin"), hideVisual, BuffID.BabyPenguin, ProjectileID.Penguin);
            modPlayer.AddPet(player.GetToggleValue("PetZephyr"), hideVisual, BuffID.ZephyrFish, ProjectileID.ZephyrFish);
            modPlayer.AddPet(player.GetToggleValue("PetSpider"), hideVisual, BuffID.PetSpider, ProjectileID.Spider);
        }
    }
}
