using System;

namespace SFA.DAS.EmployerUsers.Application.Services.ValueHashing
{
    public interface IHashingService
    {
        string HashValue(Guid id);
        Guid DecodeValue(string id);
    }
}