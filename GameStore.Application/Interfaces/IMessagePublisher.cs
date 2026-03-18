using System;

namespace GameStore.Application.Interfaces;

public interface IMessagePublisher
{
    // A generic method so we can send ANY type of message (Games, Genres, etc.)
    Task PublishMessageAsync<T>(T message, string queueName);
}
