using FargowiltasSouls.Content.Patreon.ParadoxWolf;
using FargowiltasSouls.Content.Patreon.Potato;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasSouls.Core.ModPlayers
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
        public bool ROB;

        public bool ChibiiRemii;

        public bool Northstrider;

        public bool RazorContainer;

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
            ROB = false;
            PrimeMinion = false;
            ChibiiRemii = false;
            Northstrider = false;
            RazorContainer = false;
        }

        public override void OnEnterWorld()
        {
            if (Gittle || Sasha || ManliestDove || Cat || JojoTheGamer || Northstrider)
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

            if (Player.name == "Northstrider")
            {
                Northstrider = true;

                Player.wingsLogic = 3;
                Player.wings = 3;

                Player.wingTimeMax = 100;
                Player.wingAccRunSpeed = 9f;
                Player.wingRunAccelerationMult = 9f;
            }

            if (CompOrb && Player.itemAnimation > 0)
            {
                Player.manaRegenDelay = Player.maxRegenDelay;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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

                if (ModLoader.TryGetMod("CalamityMod", out Mod _))
                {
                    target.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
                }
            }

            if (CompOrb && CompOrbDrainCooldown <= 0)
            {
                CompOrbDrainCooldown = 15;
                if (Player.CheckMana(10, true, false))
                    Player.manaRegenDelay = Player.maxRegenDelay;
            }
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CompOrb && item.DamageType != DamageClass.Magic && item.DamageType != DamageClass.Summon)
            {
                modifiers.FinalDamage *= 1.25f;

                if (Player.manaSick)
                    modifiers.FinalDamage *= Player.manaSickReduction;

                for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CompOrb && proj.DamageType != DamageClass.Magic && proj.DamageType != DamageClass.Summon)
            {
                modifiers.FinalDamage *= 1.25f;

                if (Player.manaSick)
                    modifiers.FinalDamage *= Player.manaSickReduction;

                for (int num468 = 0; num468 < 20; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100, default, 2f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 2f;
                    num469 = Dust.NewDust(new Vector2(target.Center.X, target.Center.Y), target.width, target.height, DustID.MagicMirror, -target.velocity.X * 0.2f,
                        -target.velocity.Y * 0.2f, 100);
                    Main.dust[num469].velocity *= 2f;
                }
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            OnHitByEither();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
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
                    Item.NewItem(Player.GetSource_Death(), Player.Hitbox, ModContent.ItemType<ParadoxWolfSoul>());
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

        private int razorCD = 0;

        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (RazorContainer && --razorCD <= 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile projectile = Main.projectile[i];

                    if (projectile.type == ModContent.ProjectileType<RazorBlade>() && hitbox.Distance(projectile.Center) < 100)
                    {
                        Player player = Main.player[projectile.owner];

                        Vector2 velocity = Vector2.Normalize((Main.MouseWorld - player.Center)) * 50;

                        projectile.velocity = velocity;
                        projectile.ai[0] = 1;
                        projectile.ai[1] = 0;
                    }
                }

                razorCD = 30;

            }


        }
    }
}