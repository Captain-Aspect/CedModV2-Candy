﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using InventorySystem.Items;
using MEC;
using Mirror;
#if !EXILED
using NWAPIPermissionSystem;
#else
using Exiled.Permissions.Extensions;
#endif
using PlayerRoles;
using UnityEngine;

namespace CedMod.Addons.AdminSitSystem.Commands.Jail
{
    public class Create : ICommand
    {
        public string Command { get; } = "create";

        public string[] Aliases { get; } = {
            "cr",
            "c"
        };

        public string Description { get; } = "Assigns an available jail location to your player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            if (!sender.CheckPermission("cedmod.jail"))
            {
                response = "no permission";
                return false;
            }

            if (!AdminSitHandler.Singleton.AdminSitLocations.Any(s => !s.InUse))
            {
                response = "There are no locations available.";
                return false;
            }

            var loc = AdminSitHandler.Singleton.AdminSitLocations.FirstOrDefault(s => !s.InUse);
            var plr = CedModPlayer.Get((sender as CommandSender).SenderId);
            
            if (AdminSitHandler.Singleton.Sits.Any(s => s.Players.Any(s => s.UserId == plr.UserId)))
            {
                response = "You are already part of a jail.";
                return false;
            }
            
            AdminToyBase adminToyBase = null;
            foreach (GameObject gameObject in NetworkClient.prefabs.Values)
            {
                AdminToyBase component;
                if (gameObject.TryGetComponent<AdminToyBase>(out component))
                {
                    if (string.Equals("LightSource", component.CommandName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        adminToyBase = UnityEngine.Object.Instantiate<AdminToyBase>(component);
                        adminToyBase.transform.position = loc.SpawnPosition;
                        NetworkServer.Spawn(adminToyBase.gameObject);
                        response = string.Format("Toy \"{0}\" placed! You can remove it by using \"DESTROYTOY {1}\" command.", (object) adminToyBase.CommandName, (object) adminToyBase.netId);
                    }
                }
            }

            var sit = new AdminSit()
            {
                AssociatedReportId = 0,
                InitialDuration = 0,
                InitialReason = "",
                Location = loc,
                SpawnedObjects = new List<AdminToyBase>()
                {
                    adminToyBase,
                },
                Players = new List<AdminSitPlayer>()
            };
            
            AdminSitHandler.Singleton.Sits.Add(sit);

            loc.InUse = true;
            
            JailParentCommand.AddPlr(plr, sit);

            response = "Jail assigned. Use jail add {playerId} to add someone and jail remove {playerId}";
            return false;
        }
    }
}