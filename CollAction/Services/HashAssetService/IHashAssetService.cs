namespace CollAction.Services.HashAssetService
{
    /// <summary>
    /// Hashes files to a SHA256 hash, caches the result. We use this service to append the hash of a file to a query string at the end of our static assets, ensures that if the content changes, the browser will reload.
    /// </summary>
    public interface IHashAssetService
    {
        byte[] HashAsset(params string[] location);
        string HashAssetBase64(params string[] location);
    }
}