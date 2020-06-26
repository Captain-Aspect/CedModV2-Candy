﻿using System;
using CommandSystem;
using EXILED.Extensions;
using UnityEngine;

namespace CedMod.Commands.Stuiter
{
    public class StuiterAllCommand : ICommand
    {
        public string Command { get; } = "all";

        public string[] Aliases { get; } = new string[]
        {
            
        };

        public string Description { get; } = "Does fun stuff";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            Cassie.CassieMessage("xmas_bouncyballs", false, false);
                    foreach (GameObject player in PlayerManager.players)
                    {
                        CharacterClassManager component = player.GetComponent<CharacterClassManager>();
                        component.SetClassID(RoleType.Tutorial);
                        component.GetComponent<PlayerStats>().Health = 100;
                        component.GetComponent<Inventory>().items.Clear();
                        component.GetComponent<Inventory>().AddNewItem(ItemType.SCP018);
                        component.GodMode = false;
                    }

            response = "Stuiter time";
            return true;
        }
    }
}