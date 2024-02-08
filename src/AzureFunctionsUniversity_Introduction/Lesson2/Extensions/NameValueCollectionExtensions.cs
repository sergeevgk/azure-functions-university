using System.Collections.Specialized;
namespace AzureFunctionsUniversity.Lesson2.Extensions;

public static class NameValueCollectionExtensions
{
	/// <summary>
	///     A NameValueCollection extension method that converts the collection to a dictionary.
	/// </summary>
	/// <param name="this">The collection to act on.</param>
	/// <returns>collection as an IDictionary&lt;string,string&gt;</returns>
	public static IDictionary<string, string> ToDictionary(this NameValueCollection collection)
	{
		var dict = new Dictionary<string, string>();

		foreach (string key in collection.AllKeys)
		{
			dict.Add(key, collection[key]);
		}

		return dict;
	}
}