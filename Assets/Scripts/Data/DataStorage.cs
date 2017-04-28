using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

public enum StorageMethod
{
	XML,
	Binary,
	JSON
}

public static class DataStorage
{
	private static readonly string SavingLocation = Application.persistentDataPath;

	/// <summary>
	/// Generic method used for saving data
	/// </summary>
	/// <param name="dataToStore">Data to store</param>
	/// <param name="method">Storing method</param>
	/// <typeparam name="T">Typeof data to store</typeparam>
	public static void SaveToFile<T>(T dataToStore, StorageMethod method, string name)
	{
		string fileName = SavingLocation + name + "." + method.ToString().ToLower();

        Debug.Log("File: " + fileName);

		try
		{
			switch (method)
			{
			case StorageMethod.Binary:
				BinaryFormatter formatter = new BinaryFormatter();
				using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					formatter.Serialize(stream, dataToStore);
					stream.Close();
					stream.Dispose();
				}
				break;

			case StorageMethod.XML:
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					serializer.Serialize(stream, dataToStore);

					stream.Close();
				}
				break;

			case StorageMethod.JSON:
				string serializedData = JsonUtility.ToJson(dataToStore, true);

                    Debug.Log(serializedData);
				File.WriteAllText(fileName, serializedData);
				break;
			}
		}
		catch(System.Exception ex)
		{
			Debug.LogWarning("File writing error: " + ex.Message);
		}
	}

	/// <summary>
	/// Generic method used for reading save data
	/// </summary>
	/// <returns>Read data</returns>
	/// <param name="method">Storing method</param>
	/// <typeparam name="T">Typeof data to store</typeparam>
	public static T LoadFromFIle<T>(StorageMethod method, string name)
	{
		T storedData = default(T);

		string fileName = SavingLocation + name + "." + method.ToString().ToLower();

        Debug.Log("File: " + fileName);

        try
		{
			switch (method)
			{
			case StorageMethod.Binary:
				using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					BinaryFormatter formatter = new BinaryFormatter();
					storedData = (T)formatter.Deserialize(stream);

					stream.Close();
					stream.Dispose();
				}
				break;

			case StorageMethod.XML:
				using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					storedData = (T)serializer.Deserialize(stream);

					stream.Close();
					stream.Dispose();
				}
				break;

			case StorageMethod.JSON:
				string serializedData = File.ReadAllText(fileName);
				storedData = JsonUtility.FromJson<T>(serializedData);
				break;
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning("File reading error: " + ex.Message);
		}

		return storedData;
	}

	/// <summary>
	/// This method removes all the data
	/// </summary>
	public static void RemoveData(string name)
	{
		for(int i = 0; i < System.Enum.GetNames(typeof(StorageMethod)).Length; i++)
		{
			string fileName = SavingLocation + name + "." + ((StorageMethod)i).ToString().ToLower();

			try
			{
				File.Delete(fileName);
			}
			catch (System.Exception ex)
			{
				Debug.LogWarning(fileName + " deleting error: " + ex.Message);
			}
		}            
	}
}