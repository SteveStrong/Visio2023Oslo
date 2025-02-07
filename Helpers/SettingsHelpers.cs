using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.StaticFiles;
using FoundryRulesAndUnits.Models;
using FoundryRulesAndUnits.Extensions;

namespace Foundry.Helpers;

public static class SettingsHelpers
{

    public static T Hydrate<T>(this string target, bool includeFields) where T : class
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        var node = JsonNode.Parse(target);
        node?.WriteTo(writer);
        writer.Flush();

        var options = new JsonSerializerOptions()
        {
            IncludeFields = includeFields,
            IgnoreReadOnlyFields = includeFields,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var result = JsonSerializer.Deserialize<T>(stream.ToArray(), options) as T;

        return result!;
    }

    public static List<T> HydrateList<T>(string target, bool includeFields) where T : class
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        var node = JsonNode.Parse(target);
        node?.WriteTo(writer);
        writer.Flush();

        var options = new JsonSerializerOptions()
        {
            IncludeFields = includeFields,
            IgnoreReadOnlyFields = includeFields,
                        AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var result = JsonSerializer.Deserialize<List<T>>(stream.ToArray(), options) as List<T>;

        return result!;
    }

    public static ContextWrapper<T> HydrateWrapper<T>(string target, bool includeFields) where T : class
    {
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        var node = JsonNode.Parse(target);
        node?.WriteTo(writer);
        writer.Flush();

        var options = new JsonSerializerOptions()
        {
            IncludeFields = includeFields,
            IgnoreReadOnlyFields = includeFields,
                        AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var result = JsonSerializer.Deserialize<ContextWrapper<T>>(stream.ToArray(), options) as ContextWrapper<T>;

        return result!;
    }



    public static string Dehydrate<T>(T target, bool includeFields) where T : class
    {
        var options = new JsonSerializerOptions()
        {
            IncludeFields = includeFields,
            IgnoreReadOnlyFields = includeFields,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var result = JsonSerializer.Serialize(target, options);
        return result;
    }

    public static string DehydrateList<T>(List<T> target, bool includeFields) where T : class
    {
        var options = new JsonSerializerOptions()
        {
            IncludeFields = includeFields,
            WriteIndented = true,
            IgnoreReadOnlyFields = includeFields,
                        AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var result = JsonSerializer.Serialize(target, options);
        return result;
    }



    public static Stream GenerateStream(this string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static bool FileExist(string path)
    {
        var found = File.Exists(path);

        if (found)
        {
            // $"exist {path}".WriteTrace();
        }
        else
        {
            $"DOES NOT exist {path}".WriteTrace();
        }

        return found;
    }

    public static string LocalPath(string directory, string filename)
    {
        string filePath = Path.Combine(directory, filename);
        return filePath;
    }

    public static string FullPath(string directory, string filename)
    {
        string path = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(path, directory, filename);
        return filePath;
    }

    public static string WriteData(string folder, string filename, string data)
    {
        try
        {
            $"WriteData local {folder.ToUpper()}: {filename}".WriteTrace();

            string filePath = FullPath(folder, filename);
            File.WriteAllText(filePath, data);
            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error WriteData {filename}| {ex.Message}");
            throw;
        }
    }
    public static string ReadData(string folder, string filename)
    {
        try
        {
            //$"ReadData local {folder.ToUpper()}: {filename}".WriteTrace();

            string filePath = FullPath(folder, filename);
            string data = File.ReadAllText(filePath);
            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ReadData {filename}| {ex.Message}");
            throw;
        }
    }

    public static List<T> ReadListFromFile<T>(string filename, string directory) where T : class
    {
        try
        {
            string filePath = FullPath(directory, filename);

            string text = File.ReadAllText(filePath);
            var result = HydrateList<T>(text, true);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ReadListFromFile {filename}| {ex.Message}");
            throw;
        }
    }

    public static List<T> WriteListToFile<T>(List<T> data, string filename, string directory) where T : class
    {
        try
        {
            string filePath = FullPath(directory, filename);

            var result = DehydrateList<T>(data, true);
            File.WriteAllText(filePath, result);

            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error WriteListToFile | {ex.Message}");
            throw;
        }
    }

    public static T ReadObjectFromFile<T>(string filename, string directory) where T : class
    {
        try
        {
            string filePath = FullPath(directory, filename);

            string text = File.ReadAllText(filePath);
            var result = Hydrate<T>(text, true);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ReadObjectFromFile | {ex.Message}");
            throw;
        }
    }

    public static void WriteSetting<T>(T value) where T : class
    {
        var key = typeof(T).Name;
        var filename = $"{key}.json";

        try
        {
            string filePath = FullPath("config", filename);

            var result = Dehydrate<T>(value, false);
            File.WriteAllText(filePath, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error WriteSetting | {ex.Message}");
            throw;
        }
    }
}
