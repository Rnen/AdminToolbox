using Smod2;
using Smod2.API;
using Smod2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.API
{
	public class ScheduledCommandCall
	{
		public string command;
		public string[] args;
		public DateTime timeToExecute;
		public bool hasExecuted = false;

		ICommandManager CommandManager => PluginManager.Manager.CommandManager;
		Server Server => PluginManager.Manager.Server;

		public ScheduledCommandCall(string command)
		{
			this.command = command;
			this.args = new string[] { };
			this.timeToExecute = DateTime.Now;
		}
		protected ScheduledCommandCall()
		{
			AdminToolbox.plugin.Info("New ScheduledCommandCall class created");
		}
		~ScheduledCommandCall()
		{
			AdminToolbox.plugin.Info("ScheduledCommandCall class deleted!");
		}
		public ScheduledCommandCall(string command, DateTime dateTime)
		{
			this.command = command;
			this.args = new string[] { };
			this.timeToExecute = dateTime;
		}
		public ScheduledCommandCall(string command, string[] args)
		{
			this.command = command;
			this.args = args;
			this.timeToExecute = DateTime.Now;
		}
		public ScheduledCommandCall(string command, string[] args, DateTime dateTime)
		{
			this.command = command;
			this.args = args;
			this.timeToExecute = dateTime;
		}

		public string[] CallCommand()
		{
			return CommandManager.CallCommand(Server as ICommandSender, this.command, this.args);
		}
	}
}
