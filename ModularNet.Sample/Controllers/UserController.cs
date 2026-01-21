using ModularNet.Core.Attributes;
using ModularNet.Core.Pipes;
using ModularNet.Sample.Interceptors;

namespace ModularNet.Sample.Controllers;

[Controller("users")]
[UseInterceptors(typeof(LoggingInterceptor))]
public class UserController
{
    [Get("{id}")]
    public string GetUser([Pipe(typeof(ParseIntPipe))] int id)
    {
        return $"User with ID: {id}";
    }

    [Get]
    public string GetUsers(
        [Pipe(typeof(ParseIntPipe), 10)] int limit,
        [Pipe(typeof(ParseIntPipe), 0)] int offset)
    {
        return $"Listing users - Limit: {limit}, Offset: {offset}";
    }

    [Post]
    public string CreateUser(string name)
    {
        return $"Created user: {name}";
    }

    [Put("{id}")]
    public string UpdateUser([Pipe(typeof(ParseIntPipe))] int id, string name)
    {
        return $"Updated user {id} with name: {name}";
    }

    [Delete("{id}")]
    public string DeleteUser([Pipe(typeof(ParseIntPipe))] int id)
    {
        return $"Deleted user: {id}";
    }
}
