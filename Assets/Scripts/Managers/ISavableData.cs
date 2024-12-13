public interface ISavableData
{
    /// <summary>
    /// Saves data to JSON
    /// </summary>
    /// <returns></returns>
    public string ToJson();

    /// <summary>
    /// Loads the data from the JSON
    /// </summary>
    /// <param name="json"></param>
    public void FromJson(string json);
}
