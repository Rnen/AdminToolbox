namespace AdminToolbox.API.Webhook
{
	public struct Image
	{
		public string url;
	}
	public struct Author
	{
		public string name;
		public string url;
		public string icon_url;
	}
	public struct Field
	{
		public string name;
		public string value;
		public bool inline;

		public Field(Field field)
		{
			this.name = field.name;
			this.value = field.value;
			this.inline = field.inline;
		}

		public override string ToString() => "Name: " + this.name + " Value: " + this.value + " InLine: " + inline;
	}
	public class EmbedData
	{
		public Author author = new Author();
		public string title = "";
		public string url = "";
		public string description = "";
		public int color = 12395008;
		public Field[] fields = new Field[0];
		public Image image;
		public Image thumbnail;
	}

	public class DiscordWebhook
	{
		public string username = "AdminToolbox";
		public string avatar_url = "https://i.imgur.com/nQpIuUT.png";
		public string content = "";
		public EmbedData[] embeds = new EmbedData[1] { new EmbedData() };
	}
}
