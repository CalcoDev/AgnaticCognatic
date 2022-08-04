using Postgrest.Attributes;
using Supabase;

namespace AgnaticCognaticBot.Database.Models;

[Table("users")]
public class User : SupabaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("discord_uid")]
    public decimal DiscordUid { get; set; }

    [Column("rank")]
    public short Rank { get; set; }
    
    [Column("admin_guilds")]
    public decimal[] AdminGuilds { get; set; }
}