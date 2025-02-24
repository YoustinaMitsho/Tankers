using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationHandler
{
    public static AuthenticationState AuthState { get; private set; } = AuthenticationState.NotAuthenticated;

    public static async Task<AuthenticationState> DoAuth(int MaxRetries = 5)
    {
        if(AuthState == AuthenticationState.Authenticated)
        {
            return AuthState;
        }

        if(AuthState == AuthenticationState.Authenticating)
        {
            Debug.LogWarning("Already Authenticating!");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(MaxRetries);

        return AuthState;
    }

    private static async Task<AuthenticationState> Authenticating()
    {
        while(AuthState == AuthenticationState.Authenticating ||
            AuthState == AuthenticationState.NotAuthenticated)
        {
            await Task.Delay(200);
        }

        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int MaxRetries = 5)
    {
        AuthState = AuthenticationState.Authenticating;
        int retries = 0;
        while (AuthState == AuthenticationState.Authenticating && retries < MaxRetries)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthenticationState.Authenticated;
                    break;
                }
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
                AuthState = AuthenticationState.Error;
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                AuthState = AuthenticationState.Error;
            }

            retries++;
            await Task.Delay(1000);
        }

        if(AuthState != AuthenticationState.Authenticated)
        {
            Debug.LogWarning("Something Went Worng!");
            AuthState = AuthenticationState.TimeOut;
        }
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
