using Smod2;
using Smod2.API;
using Smod2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminToolbox.API
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public class ScheduledCommandCall
	{
		public string command;
		public string[] args;
		public DateTime timeToExecute;
		public bool hasExecuted = false;

		private ICommandManager CommandManager => PluginManager.Manager.CommandManager;

		private Server Server => PluginManager.Manager.Server;

		public ScheduledCommandCall(string command)
		{
			this.command = command;
			this.args = new string[] { };
			this.timeToExecute = DateTime.Now;
		}
		protected ScheduledCommandCall() => AdminToolbox.plugin.Info("New ScheduledCommandCall class created");
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

		public string[] CallCommand() => CommandManager.CallCommand(Server as ICommandSender, this.command, this.args);
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
