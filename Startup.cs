// --------------------------------------------------
// Cleanup date: 09/10/2018 11:58
// Cleanup user: Michael Roef
// --------------------------------------------------

#region NAMESPACES

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Organimmo.ID.Examples.Web.Core
{
    public class Startup
    {
        #region METHODS

        #region PUBLIC

        public void ConfigureServices(IServiceCollection services)
        {
            #region AUTHENTICATION

            #region OPTIONS

            // The identity server endpoint
            // PROD: https://organimmo.eu.auth0.com
            // DEV: https://dev-organimmo.eu.auth0.com
            const string authority = "[AUTHORITY]";

            // The API endpoint 
            // Eg: https://ov-api.organimmo.be
            const string audience = "[AUDIENCE]";

            // Your client ID and client secret
            // Request one by sending an email to support@organimmo.be
            const string clientID = "[YOUR CLIENT ID]";
            const string clientSecret = "[YOUR CLIENT SECRET]";

            // Extra audience scopes
            // These scopes are listed in our API documentation
            const string scopes = "";

            // This path must be registered in our identity server
            // We will post the tokens to this path when the user successfully authenticates
            string callBackPath = new PathString("/signin-oid");

            #endregion

            // Add authentication services
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                // Add cookie functionality
                .AddCookie()
                .AddOpenIdConnect("OrganimmoID", options =>
                {
                    options.Authority = authority;
                    options.ClientId = clientID;
                    options.ClientSecret = clientSecret;
                    options.ResponseType = "code";
                    options.Scope.Clear();
                    options.Scope.Add("openid profile " + scopes);
                    options.CallbackPath = callBackPath;
                    options.ClaimsIssuer = "OrganimmoID";

                    // Store tokens in session
                    options.SaveTokens = true;

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.SetParameter("audience", audience);
                            return Task.FromResult(0);
                        },
                        // handle the logout redirection 
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            string logoutUri = $"{authority}/v2/logout?client_id={clientID}";
                            string postLogoutUri = context.Properties.RedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    HttpRequest request = context.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase +
                                                    postLogoutUri;
                                }

                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }

                            context.Response.Redirect(logoutUri);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });

            #endregion

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }

        #endregion

        #endregion
    }
}