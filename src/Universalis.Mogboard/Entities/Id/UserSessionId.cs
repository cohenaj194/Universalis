﻿namespace Universalis.Mogboard.Entities.Id;

public readonly struct UserSessionId
{
    private readonly System.Guid _id;

    public UserSessionId(System.Guid id)
    {
        _id = id;
    }

    public override string ToString()
    {
        return _id.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is UserSessionId other && _id.Equals(other._id);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public static UserSessionId Parse(string id)
    {
        var guid = System.Guid.Parse(id);
        return new UserSessionId(guid);
    }

    public static explicit operator UserSessionId(System.Guid id) => new(id);

    public static explicit operator System.Guid(UserSessionId id) => id._id;

    public static bool operator ==(UserSessionId left, UserSessionId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UserSessionId left, UserSessionId right)
    {
        return !(left == right);
    }
}