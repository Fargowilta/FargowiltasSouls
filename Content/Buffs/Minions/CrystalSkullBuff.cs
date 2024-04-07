
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Minions
{
    public class CrystalSkullBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderBuff";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crystal Skull");
            // Description.SetDefault("The pungent eyeball will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.FargoSouls().CrystalSkullMinion = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Minions.CrystalSkull>()] < 1)
                FargoSoulsUtil.NewSummonProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Minions.CrystalSkull>(), 30, 4f, player.whoAmI);
        }
    }
}