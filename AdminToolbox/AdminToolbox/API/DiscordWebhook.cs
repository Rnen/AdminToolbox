using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminToolbox.API.Webhook
{
	internal class Image
	{
		internal string url;
	}
	internal class Author
	{
		internal string name = "";
		internal string url = "";
		internal string icon_url = "";
	}
	internal class Field
	{
		internal string name;
		internal string value;
		internal string inline = "false";
		internal Field() { }

		public Field(Field field)
		{
			this.name = field.name;
			this.value = field.value;
			this.inline = field.inline;
		}

		public override string ToString()
		{
#pragma warning disable IDE0022 // Use expression body for methods
			return "Name: " + this.name + " Value: " + this.value + " InLine: " + inline;
#pragma warning restore IDE0022 // Use expression body for methods
		}
	}
	internal class EmbedData
	{
		internal Author author = new Author();
		internal string title = "";
		internal string url = "";
		internal string description = "";
		internal int color = 12395008;
		internal Field[] fields = new Field[] { };
		internal Image image;
		internal Image thumbnail;
	}
	internal class DiscordWebhook
	{
		internal string username = "AdminToolbox";
		internal string avatar_url = "https://puu.sh/D0DRU.png";
		internal string content = "";
		internal EmbedData[] embeds = new EmbedData[] { new EmbedData() };
	}
}
