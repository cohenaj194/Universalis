﻿using MySqlConnector;
using Universalis.Mogboard.Entities;
using Universalis.Mogboard.Entities.Id;

namespace Universalis.Mogboard;

public class UserSessionsService : IMogboardSessionTable
{
    private readonly string _username;
    private readonly string _password;
    private readonly string _database;
    private readonly int _port;

    public UserSessionsService(string username, string password, string database, int port)
    {
        _username = username;
        _password = password;
        _database = database;
        _port = port;
    }

    public async Task<UserSession?> Get(UserSessionId id, CancellationToken cancellationToken = default)
    {
        await using var db = new MySqlConnection($"User ID={_username};Password={_password};Database={_database};Server=localhost;Port={_port}");
        await db.OpenAsync(cancellationToken);

        await using var command = db.CreateCommand();
        command.CommandText = "select * from dalamud.users_sessions where id=@Id limit 1;";
        command.Parameters.Add("@Id", MySqlDbType.VarChar);
        command.Parameters["@Id"].Value = id.ToString();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? UserSession.FromReader(reader) : null;
    }

    public async Task Create(UserSession entity, CancellationToken cancellationToken = default)
    {
        await using var db = new MySqlConnection($"User ID={_username};Password={_password};Database={_database};Server=localhost;Port={_port}");
        await db.OpenAsync(cancellationToken);

        await using var command = db.CreateCommand();
        entity.IntoCommand(command, "dalamud.users_sessions");

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<UserSession?> Get(string session, CancellationToken cancellationToken = default)
    {
        await using var db = new MySqlConnection($"User ID={_username};Password={_password};Database={_database};Server=localhost;Port={_port}");
        await db.OpenAsync(cancellationToken);

        await using var command = db.CreateCommand();
        command.CommandText = "select * from dalamud.users_sessions where session=@session limit 1;";
        command.Parameters.Add("@session", MySqlDbType.VarChar);
        command.Parameters["@session"].Value = session;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? UserSession.FromReader(reader) : null;
    }
}