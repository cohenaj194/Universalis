﻿namespace Universalis.Mogboard.Entities;

public readonly struct UserListId
{
    private readonly Guid _id;

    public UserListId(Guid id)
    {
        _id = id;
    }

    public override string ToString()
    {
        return _id.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is UserListId other && _id.Equals(other._id);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public static UserListId Parse(string id)
    {
        var guid = Guid.Parse(id);
        return new UserListId(guid);
    }

    public static explicit operator UserListId(Guid id) => new(id);

    public static explicit operator Guid(UserListId id) => id._id;
}