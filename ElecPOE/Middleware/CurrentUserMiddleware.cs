
using ForekOnline.Application.Common.Utility;

namespace ElecPOE.Middleware
{
    /// <summary>
    /// Middleware for setting the current user in the session.
    /// </summary>
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware logic to set the current user in the session.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        public async Task Invoke(HttpContext context)
        {
            var user = GetUser();

            if (string.IsNullOrEmpty(user))
            {
                user = "No User";
            }
            else
            {
                user = GetUser();
            }

            context.Session.SetString("CurrentUser", user);

            await _next(context);
        }

        /// <summary>
        /// Retrieves the currently logged-in user from the helper class.
        /// </summary>
        /// <returns>The username of the logged-in user.</returns>
        private string GetUser()
        {
            return Helper.loggedInUser;
        }
    }

}
