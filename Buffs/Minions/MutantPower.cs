using FargowiltasSouls.Toggler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class MutantPower : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Mutant Power");
            //Description.SetDefault("The power of Mutant is with you");
            //DisplayName.AddTranslation(GameCulture.Chinese, "突变之力");
            //Description.AddTranslation(GameCulture.Chinese, "突变之力与你同在");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderBuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                FargoPlayer fargoPlayer = player.GetModPlayer<FargoPlayer>();

                if (player.GetToggleValue("MasoAbom"))
                {
                    fargoPlayer.Abominationn = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("Abominationn")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("Abominationn"), 0, 10f, player.whoAmI, -1);
                }

                if (player.GetToggleValue("MasoRing"))
                {
                    fargoPlayer.PhantasmalRing = true;
                    if (player.ownedProjectileCounts[mod.ProjectileType("PhantasmalRing")] < 1)
                        Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("PhantasmalRing"), 0, 0f, player.whoAmI);
                }
            }
        }
    }
}