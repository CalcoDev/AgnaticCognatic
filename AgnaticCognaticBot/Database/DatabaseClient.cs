using AgnaticCognaticBot.Database.Models;
using NLog;
using Supabase;

namespace AgnaticCognaticBot.Database;

public class DatabaseClient
{
    public Client Client { get; private set; }
    
    public SupabaseTable<Guild> Guilds { get; private set; }
    public SupabaseTable<User> Users { get; private set; }

    private readonly string? _url = Environment.GetEnvironmentVariable("AGNATIC_COGNATIC_SUPABASE_URL");
    private readonly string? _key = Environment.GetEnvironmentVariable("AGNATIC_COGNATIC_SUPABASE_KEY");

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public DatabaseClient()
    {
        if (_url == null)
        {
            _logger.Error("No database url found.");
            throw new Exception("Cannot initialise Supabase DB: no database url found.");
        }
        if (_key == null)
        {
            _logger.Error("No database key found.");
            throw new Exception("Cannot initialise Supabase DB: no database key found.");
        }
    }

    public async Task InitClient()
    {
        await Client.InitializeAsync(_url, _key, new SupabaseOptions
        {
            AutoConnectRealtime = true, 
            ShouldInitializeRealtime = true
        });

        _logger.Info("Succesfully connected to database.");

        try
        {
            Client = Client.Instance;

            Guilds = Client.From<Guild>();
            Users = Client.From<User>();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to connect to database.");
            throw new Exception("Failed to connect to database.");
        }
    }
}