using System.IO;

namespace YooAsset.Editor
{

    [DisplayName("忽略指定类型资源")]
    public class CollectAllIgnoreOthers : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            string extension = Path.GetExtension(data.AssetPath);
            string[] ignoreExNames = data.UserData.Split(",");

            if (ignoreExNames.Length <= 0)
            {
                return true;
            }

            for (int i = 0; i < ignoreExNames.Length; i++)
            {
                if (extension == ignoreExNames[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
