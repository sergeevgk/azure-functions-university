namespace AzureFunctionsUniversity.Lesson1.Models
{
	record Resume
	{
		public string Name { get; set; }
		public string Website { get; set; }
		public string Country { get; set; }
		public string[] Skills { get; set; }
		public string CurrentRole { get; set; }
	}
}
