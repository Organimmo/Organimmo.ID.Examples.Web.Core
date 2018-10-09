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
using Microsoft.Extensions.Configuration;
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => HostingEnvironment.IsProduction();
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

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
                    // The identity server endpoint
                    // PROD: https://organimmo.eu.auth0.com
                    // DEV: https://dev-organimmo.eu.auth0.com
                    options.Authority = $"https://{Configuration["Identity:Domain"]}";
                    // Your client ID and client secret
                    // Request one by sending an email to support@organimmo.be
                    options.ClientId = Configuration["Identity:ClientID"];
                    options.ClientSecret = Configuration["Identity:ClientSecret"];
                    // Extra audience scopes
                    // These scopes are listed in our API documentation
                    options.Scope.Clear();
                    options.Scope.Add("openid profile " + Configuration["Identity:Scopes"]);
                    // This path must be registered in our identity server
                    // We will post the tokens to this path when the user successfully authenticates
                    options.CallbackPath = new PathString("/signin-oid");
                    options.ClaimsIssuer = "OrganimmoID";
                    // Store tokens in session
                    options.SaveTokens = true;
                    options.ResponseType = "code";

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.SetParameter("audience", $"https://{Configuration["Identity:Audience"]}");
                            return Task.FromResult(0);
                        },
                        // handle the logout redirection 
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            string logoutUri = $"https://{Configuration["Identity:Domain"]}/v2/logout?client_id={Configuration["Identity:ClientID"]}";
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

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }
    }
}