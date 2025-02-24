using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

public static class AuthenticationHandler
{
    public static AuthenticationState AuthState { get; private set; } = AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> DoAuth(int MaxTries = 5)
    {
        if(AuthState == AuthenticationState.Authenticated)
        {
            return AuthState;
        }

        AuthState = AuthenticationState.Authenticating;
        int tries = 0;
        while (AuthState == AuthenticationState.Authenticating && tries < MaxTries)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            if(AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
            {
                AuthState = AuthenticationState.Authenticated;
                break;
            }

            tries++;
            await Task.Delay(1000);
        }

        return AuthState;
    }
}

public enum AuthenticationState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}
