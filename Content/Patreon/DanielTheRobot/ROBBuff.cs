using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Patreon.DanielTheRobot
{
    public class ROBBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<PatreonPlayer>().ROB = true;
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<ROB>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<ROB>(), 0, 0f, player.whoAmI);
            }
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (Main.LocalPlayer.name.Contains("Daniel"))
            {
                tip = Language.GetTextValue("Mods.FargowiltasSouls.Buffs.ROBBuff.DescPatreon");
            }
            else
            {
                tip = Language.GetTextValue("Mods.FargowiltasSouls.Buffs.ROBBuff.DescNonPatreon");
            }
        }
    }
}