using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace FargowiltasSouls.Common
{
    public class EternityWorldIconManager : ModSystem
    {
        private static Asset<Texture2D> EternityIcon;

        private static Asset<Texture2D> MasochistIcon;
        public override void OnModLoad()
        {
            Main.QueueMainThreadAction(() =>
            {
                On_AWorldListItem.GetDifficulty += EternityDifficultyFix;
                On_AWorldListItem.GetIconElement += AddEternityElement;
            });

            LoadIconTextures();
        }
        public override void OnModUnload()
        {
            Main.QueueMainThreadAction(() =>
            {
                On_AWorldListItem.GetDifficulty -= EternityDifficultyFix;
                On_AWorldListItem.GetIconElement -= AddEternityElement;
            });
        }

        private static void LoadIconTextures()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            EternityIcon = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutant", AssetRequestMode.ImmediateLoad);
            MasochistIcon = ModContent.Request<Texture2D>("FargowiltasSouls/Assets/UI/OncomingMutantAura", AssetRequestMode.ImmediateLoad);
        }

        private UIElement AddEternityElement(On_AWorldListItem.orig_GetIconElement orig, AWorldListItem self)
        {
            UIElement worldIcon = orig(self);
            if (self.Data.TryGetHeaderData<EternityWorldIconManager>(out TagCompound tag))
            {
                if (tag.ContainsKey("EternityWorld") && tag.GetBool("EternityWorld"))
                {
                    static UIImage MutantIcon(Asset<Texture2D> texture)
                    {
                        /* top left
                        return new UIImage(texture)
                        {
                            HAlign = 0.5f,
                            VAlign = 0.5f,
                            Top = new StyleDimension(-22, 0f),
                            Left = new StyleDimension(-27, 0f),
                            IgnoresMouseInteraction = true
                        };
                        */
                        return new UIImage(texture)
                        {
                            HAlign = 0.5f,
                            VAlign = 0.5f,
                            Top = new StyleDimension(12, 0f),
                            Left = new StyleDimension(138, 0f),
                            IgnoresMouseInteraction = true
                        };
                    }
                    UIImage emodeIcon = MutantIcon(EternityIcon);
                    worldIcon.Append(emodeIcon);
                    if (tag.ContainsKey("MasochistWorld") && tag.GetBool("MasochistWorld"))
                    {
                        UIImage masoIcon = MutantIcon(MasochistIcon);
                        worldIcon.Append(masoIcon);
                    }
                }

                //return emodeIcon;
            }

            return worldIcon;
        }

        private void EternityDifficultyFix(On_AWorldListItem.orig_GetDifficulty orig, AWorldListItem self, out string expertText, out Color gameModeColor)
        {
            orig.Invoke(self, out string origText, out Color origColor);
            if (origText == "Legendary")
                origText = "Legendary  ";
            expertText = origText;
            gameModeColor = origColor;
        }


        public override void SaveWorldHeader(TagCompound tag)
        {
            tag["EternityWorld"] = WorldSavingSystem.EternityMode;
            tag["MasochistWorld"] = WorldSavingSystem.MasochistModeReal;
        }

    }
}
