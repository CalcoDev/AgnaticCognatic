using Postgrest.Attributes;
using Supabase;

namespace AgnaticCognaticBot.Database.Models;

[Table("guilds")]
public class Guild : SupabaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("discord_uid")]
    public decimal DiscordUid { get; set; }
}