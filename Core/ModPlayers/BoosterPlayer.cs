using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasSouls.Core.ModPlayers
{
    public class BoosterPlayer : ModPlayer
    {
        public int SolarTimer = 0;
        public int VortexTimer = 0;
        public int NebulaTimer = 0;
        public int StardustTimer = 0;
        public override void PostUpdateEquips()
        {
            if (SolarTimer > 0)
            {
                SolarTimer--;
                Player.endurance += 0.2f;
            }
            if (VortexTimer > 0)
            {
                VortexTimer--;
                Player.GetCritChance(DamageClass.Generic) += 35;
            }
                
            if (NebulaTimer > 0)
            {
                NebulaTimer--;
            }
            if (StardustTimer > 0)
            {
                StardustTimer--;
                Player.GetDamage(DamageClass.Generic) += 0.35f;
            }
            if (SolarTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Yellow, Language.GetTextValue("Mods.FargowiltasSouls.Items.SolarBooster.Deactivate", 20), true);
            if (VortexTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.LightCyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.VortexBooster.Deactivate", 35), true);
            if (NebulaTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Magenta, Language.GetTextValue("Mods.FargowiltasSouls.Items.NebulaBooster.Deactivate", 5), true);
            if (StardustTimer == 1)
                CombatText.NewText(Player.Hitbox, Color.Cyan, Language.GetTextValue("Mods.FargowiltasSouls.Items.StardustBooster.Deactivate", 35), true);
        }
        public override void UpdateLifeRegen()
        {
            if (NebulaTimer > 0)
                Player.lifeRegen += 10;
        }
    }
}
