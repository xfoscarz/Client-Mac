using System;
using System.IO;
using Godot;
using LiteDB;

public sealed class DatabaseService
{
    public static DatabaseService Instance { get; } = new DatabaseService();

    public LiteDatabase DB { get; private set; }

    private DatabaseService()
    {
        DB = new LiteDatabase($"{Constants.USER_FOLDER}/data.db");
    }

    public void Dispose()
    {
        DB.Dispose();
    }
}
