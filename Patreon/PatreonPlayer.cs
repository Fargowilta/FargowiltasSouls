using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls
{
    public class PatreonPlayer : ModPlayer
    {
        public bool Gittle;
        public bool RoombaPet;

        public bool Sasha;
        public bool FishMinion;

        public bool CompOrb;
        public int CompOrbDrainCooldown;

        public bool ManliestDove;
        public bool DovePet;

        public bool Cat;
        public bool KingSlimeMinion;

        public bool WolfDashing;

        public bool PiranhaPlantMode;

        public bool JojoTheGamer;
        public bool PrimeMinion;

        public bool Crimetroid;

        public override TagCompound Save()
        {
            string name = "PatreonSaves" + player.name;
            var PatreonSaves = new List<string>();

            if (PiranhaPlantMode) PatreonSaves.Add("PiranhaPlantMode");

            return new TagCompound {
                    {name, PatreonSaves}
                }; ;
        }

        public override void Load(TagCompound tag)
        {
            string name = "PatreonSaves" + player.name;
            IList<string> PatreonSaves = tag.GetList<string>(name);

            PiranhaPlantMode = PatreonSaves.Contains("PiranhaPlantMode");
        }

        public override void ResetEffects()
        {
            Gittle = false;
            RoombaPet = false;
            Sasha = false;
            FishMinion = false;
            CompOrb = false;
            ManliestDove = false;
            DovePet = false;
            Cat = false;
            KingSlimeMinion = false;
            WolfDashing = false;
            JojoTheGamer = false;
            Crimetroid = false;
            PrimeMinion = false;
        }

        public override void OnEnterWorld(Player player)
        {
            if (Gittle || Sasha || ManliestDove || Cat || JojoTheGamer)
            {
                Main.NewText(Language.GetTextValue("Mods.FargowiltasSouls.PatreonPlayer.Active1") + player.name + Language.GetTextValue("Mods.FargowiltasSouls.PatreonPlayer.Active2"));
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (CompOrbDrainCooldown > 0)
                CompOrbDrainCooldown -= 1;

            if (player.name == "iverhcamer")
            {
                Gittle = true;
                player.pickSpeed -= .15f;
                //shine effect
                Lighting.AddLight(player.Center, 0.8f, 0.8f, 0f);
            }

            if (player.name == "Sasha")
            {
                Sasha = true;

                player.lavaImmune = true;
                player.fireWalk = true;
                player.buffImmune[BuffID.OnFire] = true;
                player.buffImmune[BuffID.CursedInferno] = true;
                player.buffImmune[BuffID.Burning] = true;
            }

            if (player.name == "Dove")
            {
                ManliestDove = true;
            }

            if (player.name == "cat")
            {
                Cat = true;

                if (NPC.downedMoonlord)
                {
                    player.maxMinions += 4;
                }
                else if (Main.hardMode)
                {
                    player.maxMinions += 2;
                }

                player.minionDamage += player.maxMinions * 0.5f;
            }

            if (player.name == "VirtualDefender")
            {
                JojoTheGamer = true;
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            OnHitEither(target, damage, knockback, crit);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            OnHitEither(target, damage, knockback, crit);
        }

        private void OnHitEither(NPC target, int damage, float knockback, bool crit)
        {
            if (Gittle)
            {
                if (Main.rand.NextBool(10))
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (Vector2.Distance(target.Center, npc.Center) < 50)
                        {
                            npc.AddBuff(BuffID.Venom, 300);
                        }
                    }
                }

                if (ModLoader.GetMod("CalamityMod") != null)
                {
                    target.StrikeNPC(target.lifeMax, 0f, 0);
                }
            }

            if (CompOrb && CompOrbDrainCooldown <= 0)
            {
                CompOrbDrainCooldown = 15;
                if (player.CheckMana(10, true, false))
                    player.manaRegenDelay = 300;
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (CompOrb && !item.magic && !item.summon)
            {
                damage = (int)(damage * 1.25f);

                if (player.manaSick)
                    damage = (int)(damage * player.manaSickReduction);

                for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 15, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default(Color), 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, 15, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (CompOrb && !proj.magic && !proj.minion)
            {
                damage = (int)(damage * 1.25f);
                
                if (player.manaSick)
                    damage = (int)(damage * player.manaSickReduction);

                for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 15, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default(Color), 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, 15, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }
            }
        }

        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            OnHitByEither();
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            OnHitByEither();
        }

        private void OnHitByEither()
        {
            if (PiranhaPlantMode)
            {
                int index = Main.rand.Next(Fargowiltas.DebuffIDs.Count);
                player.AddBuff(Fargowiltas.DebuffIDs[index], 180);
            }
        }

        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (WolfDashing) //dont draw player during dash
                while (layers.Count > 0)
                    layers.RemoveAt(0);


            HashSet<int> layersToRemove = new HashSet<int>();

            for (int i = 0; i < layers.Count; i++)
            {
                if (JojoTheGamer && layers[i] == PlayerLayer.Skin)
                {
                    layersToRemove.Add(i);
                }
            }

            foreach (int i in layersToRemove)
            {
                layers.RemoveAt(i);
            }
        }

        public override void FrameEffects()
        {
            if (JojoTheGamer)
            {
                player.legs = mod.GetEquipSlot("BetaLeg", EquipType.Legs);
                player.body = mod.GetEquipSlot("BetaBody", EquipType.Body);
                player.head = mod.GetEquipSlot("BetaHead", EquipType.Head);
            }
        }
    }
}