﻿using System;
using System.Diagnostics;
using System.IO;
using HASS.Agent.Shared.Enums;
using Serilog;

namespace HASS.Agent.Shared.HomeAssistant.Commands.InternalCommands
{
    /// <summary>
    /// Command to perform an action through the configured custom executor
    /// </summary>
    public class CustomExecutorCommand : InternalCommand
    {
        private const string DefaultName = "customexecutor";

        public CustomExecutorCommand(string name = DefaultName, string friendlyName = DefaultName, string command = "", CommandEntityType entityType = CommandEntityType.Switch, string id = default) : base(name ?? DefaultName, friendlyName ?? null, command, entityType, id)
        {
            CommandConfig = command;
            State = "OFF";
        }

        public override void TurnOn()
        {
            State = "ON";

            if (string.IsNullOrWhiteSpace(CommandConfig))
            {
                Log.Warning("[CUSTOMEXECUTOR] [{name}] Unable to launch command, it's configured as action-only", Name, Name);

                State = "OFF";
                return;
            }

            try
            {
                // is there a custom executor provided?
                if (string.IsNullOrEmpty(Variables.CustomExecutorBinary))
                {
                    Log.Warning("[CUSTOMEXECUTOR] [{name}] No custom executor provided, unable to execute", Name);
                    return;
                }

                // does the binary still exist?
                if (!File.Exists(Variables.CustomExecutorBinary))
                {
                    Log.Error("[CUSTOMEXECUTOR] [{name}] Provided custom executor not found: {file}", Name, Variables.CustomExecutorBinary);
                    return;
                }

                // all good, launch
                using var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = Variables.CustomExecutorBinary,
                    Arguments = CommandConfig
                };

                process.StartInfo = startInfo;
                var start = process.Start();

                // check if the start went ok
                if (!start) Log.Error("[CUSTOMEXECUTOR] [{name}] Unable to start executing command: {command}", Name, CommandConfig);

                // yep, done
            }
            catch (Exception ex)
            {
                Log.Error("[CUSTOMEXECUTOR] [{name}] Error while processing: {err}", Name, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }

        public override void TurnOnWithAction(string action)
        {
            State = "ON";

            try
            {
                // is there a custom executor provided?
                if (string.IsNullOrEmpty(Variables.CustomExecutorBinary))
                {
                    Log.Warning("[CUSTOMEXECUTOR] [{name}] No custom executor provided, unable to execute", Name);
                    return;
                }

                // does the binary still exist?
                if (!File.Exists(Variables.CustomExecutorBinary))
                {
                    Log.Error("[CUSTOMEXECUTOR] [{name}] Provided custom executor not found: {file}", Name, Variables.CustomExecutorBinary);
                    return;
                }

                // prepare arguments
                var args = string.IsNullOrWhiteSpace(CommandConfig) ? action : $"{CommandConfig} {action}";

                // all good, launch
                using var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    FileName = Variables.CustomExecutorBinary,
                    Arguments = args
                };

                process.StartInfo = startInfo;
                var start = process.Start();

                // check if the start went ok
                if (!start) Log.Error("[CUSTOMEXECUTOR] [{name}] Unable to start executing command with action '{action}'", CommandConfig, action);

                // yep, done
            }
            catch (Exception ex)
            {
                Log.Error("[CUSTOMEXECUTOR] [{name}] Error while processing custom executor: {err}", Name, ex.Message);
            }
            finally
            {
                State = "OFF";
            }
        }
    }
}
