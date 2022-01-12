using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Minions
{
    public class CrystalSkull : ModBuff
    {
        public override void SetDefaults()
        {
            ////DisplayName.SetDefault("Crystal Skull");
            ////Description.SetDefault("The pungent eyeball will protect you");
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
            player.GetModPlayer<FargoPlayer>().CrystalSkullMinion = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[mod.ProjectileType("CrystalSkull")] < 1)
                Projectile.NewProjectile(player.Center, Vector2.Zero, mod.ProjectileType("CrystalSkull"), 0, 4f, player.whoAmI);
        }
    }
}