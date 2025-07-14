using System.IO;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Format.Base
{
    public static class FormatHelper
    {
        public static string ThumbnailPathOf(IResultDataObject obj)
        {
            string thumbnailPath = null;
            if (obj != null)
            {
                thumbnailPath = Path.Combine(Path.GetDirectoryName(obj.ResFilePath),
                                                @"LotThumbnail",
                                                $"{Path.GetFileNameWithoutExtension(obj.ResFilePath)}_{obj.ResType.GetExt()}.png");
            }
            return thumbnailPath;
        }

        public static string ThumbnailPathOf(string resFilePath)
        {
            string thumbnailPath = null;
            if (resFilePath != null)
            {
                thumbnailPath = Path.Combine(Path.GetDirectoryName(resFilePath),
                    @"LotThumbnail",
                    $"{Path.GetFileNameWithoutExtension(resFilePath)}_{Path.GetExtension(resFilePath).TrimStart('.')}.png");
            }
            return thumbnailPath;
        }
    }
}
