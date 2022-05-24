using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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

        public bool ChibiiRemii;

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);

            string name = "PatreonSaves" + Player.name;
            var PatreonSaves = new List<string>();

            if (PiranhaPlantMode) PatreonSaves.Add("PiranhaPlantMode");

            tag.Add(name, PatreonSaves);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            string name = "PatreonSaves" + Player.name;
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
            ChibiiRemii = false;
        }

        public override void OnEnterWorld(Player player)
        {
            if (Gittle || Sasha || ManliestDove || Cat || JojoTheGamer)
            {
                string text = Language.GetTextValue($"Mods.{Mod.Name}.Message.PatreonNameEffect");
                Main.NewText($"{text}, {Player.name}!");
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (CompOrbDrainCooldown > 0)
                CompOrbDrainCooldown -= 1;

            if (Player.name == "iverhcamer")
            {
                Gittle = true;
                Player.pickSpeed -= .15f;
                //shine effect
                Lighting.AddLight(Player.Center, 0.8f, 0.8f, 0);
            }

            if (Player.name == "Sasha")
            {
                Sasha = true;

                Player.lavaImmune = true;
                Player.fireWalk = true;
                Player.buffImmune[BuffID.OnFire] = true;
                Player.buffImmune[BuffID.CursedInferno] = true;
                Player.buffImmune[BuffID.Burning] = true;
            }

            if (Player.name == "Dove")
            {
                ManliestDove = true;
            }

            if (Player.name == "cat")
            {
                Cat = true;

                if (NPC.downedMoonlord)
                {
                    Player.maxMinions += 4;
                }
                else if (Main.hardMode)
                {
                    Player.maxMinions += 2;
                }

                Player.GetDamage(DamageClass.Summon) += Player.maxMinions * 0.5f;
            }

            if (Player.name == "VirtualDefender")
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

                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    target.StrikeNPC(target.lifeMax, 0f, 0);
                }
            }

            if (CompOrb && CompOrbDrainCooldown <= 0)
            {
                CompOrbDrainCooldown = 15;
                if (Player.CheckMana(10, true, false))
                    Player.manaRegenDelay = 300;
            }
        }

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (CompOrb && item.DamageType != DamageClass.Magic && item.DamageType != DamageClass.Summon)
            {
                damage = (int)(damage * 1.25f);

                if (Player.manaSick)
                    damage = (int)(damage * Player.manaSickReduction);

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
            if (CompOrb && proj.DamageType != DamageClass.Magic && proj.DamageType != DamageClass.Summon)
            {
                damage = (int)(damage * 1.25f);

                if (Player.manaSick)
                    damage = (int)(damage * Player.manaSickReduction);

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
                int index = Main.rand.Next(FargowiltasSouls.DebuffIDs.Count);
                Player.AddBuff(FargowiltasSouls.DebuffIDs[index], 180);
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (SoulConfig.Instance.PatreonWolf)
            {
                if (damageSource.TryGetCausingEntity(out Entity entity) && entity is NPC npc && npc.active && npc.type == NPCID.Wolf)
                {
                    Item.NewItem(Player.GetSource_Death(), Player.Hitbox, ModContent.ItemType<Patreon.ParadoxWolf.ParadoxWolfSoul>());
                }
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            base.HideDrawLayers(drawInfo);

            if (WolfDashing) //dont draw player during dash
                drawInfo.DrawDataCache.Clear();

            //HashSet<int> layersToRemove = new HashSet<int>();
            //for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
            //{
            //    if (JojoTheGamer && drawInfo.DrawDataCache[i] == PlayerLayer.Skin)
            //    {
            //        layersToRemove.Add(i);
            //    }
            //}
            //foreach (int i in layersToRemove)
            //{
            //    drawInfo.DrawDataCache.RemoveAt(i);
            //}

            //alternative for jojo changes? idk
            //Terraria.DataStructures.PlayerDrawLayers.Skin.Hide();
        }

        public override void FrameEffects()
        {
            //if (JojoTheGamer)
            //{
            //    Player.legs = Mod.GetEquipSlot("BetaLeg", EquipType.Legs);
            //    Player.body = Mod.GetEquipSlot("BetaBody", EquipType.Body);
            //    Player.head = Mod.GetEquipSlot("BetaHead", EquipType.Head);
            //}
        }
    }
}