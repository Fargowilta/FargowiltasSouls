using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class LightningRod : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Lightning Rod");
            //Description.SetDefault("You attract thunderbolts");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "避雷针");
            //Description.AddTranslation(GameCulture.Chinese, "你将会吸引雷电");
        }

        private void SpawnLightning(Entity obj, int type, int damage)
        {
            //tends to spawn in ceilings if the player goes indoors/underground
            Point tileCoordinates = obj.Top.ToTileCoordinates();

            tileCoordinates.X += Main.rand.Next(-25, 25);
            tileCoordinates.Y -= 15 + Main.rand.Next(-5, 5) - ((type == mod.ProjectileType("LightningVortexHostile")) ? 20 : 0);

            for (int index = 0; index < 10 && !WorldGen.SolidTile(tileCoordinates.X, tileCoordinates.Y) && tileCoordinates.Y > 10; ++index) tileCoordinates.Y -= 1;

            Projectile.NewProjectile(tileCoordinates.X * 16 + 8, tileCoordinates.Y * 16 + 17, 0f, 0f, type, damage, 2f, Main.myPlayer,
                0f, obj.whoAmI);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //spawns lightning once per second
            player.GetModPlayer<FargoPlayer>().lightningRodTimer++;
            if (player.GetModPlayer<FargoPlayer>().lightningRodTimer >= 60)
            {
                player.GetModPlayer<FargoPlayer>().lightningRodTimer = 0;
                SpawnLightning(player, mod.ProjectileType("LightningVortexHostile"), 60 / 4);
            }

            //if (Main.rand.Next(60) == 1)
               // SpawnLightning(player, mod.ProjectileType("LightningVortexHostile"), 0);
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            NPCs.FargoSoulsGlobalNPC fargoNPC = npc.GetGlobalNPC<NPCs.FargoSoulsGlobalNPC>();
            fargoNPC.lightningRodTimer++;
            if (fargoNPC.lightningRodTimer >= 60)
            {
                fargoNPC.lightningRodTimer = 0;
                SpawnLightning(npc, mod.ProjectileType("LightningVortex"), 60);
            }
        }
    }
}