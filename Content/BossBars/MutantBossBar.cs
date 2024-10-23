﻿using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Drawing;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.BossBars
{
    public class MutantBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;
        public override Asset<Texture2D> GetIconTexture(ref Microsoft.Xna.Framework.Rectangle? iconFrame)
        {   
           if (bossHeadIndex != -1)
           {
              return TextureAssets.NpcHeadBoss[bossHeadIndex];
           }
           return null;
        } 
        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {   
            //get rid of the text during desp, also make the bar shake slightly
            if (npc.ai[0] <= -1 && npc.ai[0] >= -6)
            {
                drawParams.ShowText = false;
                drawParams.BarCenter += Main.rand.NextVector2Circular(0.2f, 0.2f) * 5f;
            }
            if (npc.ai[0] == -7)
            {   
                //get rid of the bar.
                drawParams.BarCenter = Vector2.SmoothStep(Vector2.Zero, Vector2.One, 0.002f);
                drawParams.ShowText = false;
            }
            return true;
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (npc.townNPC || !npc.active)
                return false;

            life = npc.life;
            lifeMax = npc.lifeMax;
                    

            bossHeadIndex = npc.GetBossHeadTextureIndex();
            return true;
        }
    }
}
