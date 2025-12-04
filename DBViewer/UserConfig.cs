using System.Text.Json;

namespace DBViewer;

/// <summary>
/// Manages user-allowed tables configuration with persistence
/// </summary>
public class UserConfig
{
    public List<string> AllowedTables { get; set; } = new();
    
    private const string ConfigFileName = "user_config.json";
    
    public static UserConfig Load()
    {
        try
        {
            if (File.Exists(ConfigFileName))
            {
                string json = File.ReadAllText(ConfigFileName);
                return JsonSerializer.Deserialize<UserConfig>(json) ?? new UserConfig();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load configuration: {ex.Message}", "Warning", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        return new UserConfig();
    }
    
    public void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(ConfigFileName, json);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save configuration: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
