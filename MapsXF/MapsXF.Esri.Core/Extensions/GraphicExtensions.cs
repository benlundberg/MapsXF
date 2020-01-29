using Esri.ArcGISRuntime.UI;

namespace Esri.Core.Extensions
{
    public static class GraphicExtensions
    {
        public static string GetId(this Graphic graphic)
        {
            if (graphic.Attributes.TryGetValue(Constants.GeometryId, out object id))
            {
                return id.ToString();
            }

            return default;
        }

        public static void SetId(this Graphic graphic, string id)
        {
            graphic.Attributes.Add(Constants.GeometryId, id);
        }
    }
}
