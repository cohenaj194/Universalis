﻿using MySqlConnector;
using Universalis.Mogboard.Entities;
using Universalis.Mogboard.Entities.Id;

namespace Universalis.Mogboard;

internal class UserCharactersService : IMogboardTable<UserCharacter, UserCharacterId>
{
    private readonly string _username;
    private readonly string _password;
    private readonly string _database;
    private readonly int _port;

    public UserCharactersService(string username, string password, string database, int port)
    {
        _username = username;
        _password = password;
        _database = database;
        _port = port;
    }

    public async Task<UserCharacter?> Get(UserCharacterId id, CancellationToken cancellationToken = default)
    {
        await using var db = new MySqlConnection($"User ID={_username};Password={_password};Database={_database};Server=localhost;Port={_port}");
        await db.OpenAsync(cancellationToken);

        await using var command = db.CreateCommand();
        command.CommandText = "select * from dalamud.users_characters where id=@Id limit 1;";
        command.Parameters.Add("@Id", MySqlDbType.VarChar);
        command.Parameters["@Id"].Value = id.ToString();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? UserCharacter.FromReader(reader) : null;
    }

    public async Task Create(UserCharacter entity, CancellationToken cancellationToken = default)
    {
        await using var db = new MySqlConnection($"User ID={_username};Password={_password};Database={_database};Server=localhost;Port={_port}");
        await db.OpenAsync(cancellationToken);

        await using var command = db.CreateCommand();
        entity.IntoCommand(command, "dalamud.users_characters");

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}