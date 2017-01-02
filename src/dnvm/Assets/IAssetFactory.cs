namespace DotNet.Assets
{
    public interface IAssetFactory
    {
        TAsset Create<TAsset>(string version)
            where TAsset : Asset;
    }
}