using System;

namespace UnitySC.Equipment.Abstractions
{
    public interface IUserInformationProvider
    {
        event EventHandler<UserInformationEventArgs> UserInformationRaised;

        event EventHandler<UserInformationEventArgs> UserWarningRaised;

        event EventHandler<UserInformationEventArgs> UserErrorRaised;
    }
}
