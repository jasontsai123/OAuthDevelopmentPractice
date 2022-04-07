namespace WebSite.Repositories.LineLogin;

public interface ILineLoginApi
{
    /// <summary>
    /// Gets a user's ID, display name, profile image, and status message.
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <returns>A task containing the line profile result</returns>
    Task<LineProfileResult> GetProfileAsync(string accessToken);
}