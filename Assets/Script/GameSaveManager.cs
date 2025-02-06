using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaveManager
{
    private static string GetSavePath(string username, int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"{username}_slot{slot}.bin");
    }

    public static bool SaveGame(UserAccount user, int slot)
    {
        string savePath = GetSavePath(user.username, slot);
        if (CheckFreeSpace() < 1024)
        {
            Debug.LogError("Gagal menyimpan: Ruang penyimpanan tidak mencukupi.");
            return false;
        }

        try
        {
            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(user.username);
                writer.Write(user.password);
                writer.Write(user.xp);
            }
            return true;
        }
        catch (IOException e)
        {
            Debug.LogError("Error saving game: " + e.Message);
            return false;
        }
    }

    public static UserAccount LoadGame(string username, int slot)
    {
        string savePath = GetSavePath(username, slot);
        if (!File.Exists(savePath))
        {
            return null;
        }

        try
        {
            using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                return new UserAccount(reader.ReadString(), reader.ReadString(), reader.ReadInt32());
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error loading game: " + e.Message);
            return null;
        }
    }

    public static long CheckFreeSpace()
    {
        DriveInfo drive = new DriveInfo(Path.GetPathRoot(Application.persistentDataPath));
        return drive.AvailableFreeSpace;
    }
}
