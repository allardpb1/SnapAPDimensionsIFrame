using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnapAPDimensionsIFrame.Entity;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace SnapAPDimensionsIFrame.Helpers
{
    public static class CustomHtmlHelper
    {
        public static IHtmlContent DropDownListForCustom(this IHtmlHelper helper, Dimension dimension)
        {
            var selectListHtml = "";

            foreach (DimensionDetail dimensionDetail in dimension.DimensionDetails)
            {
                string relations = "";

                if (dimensionDetail.Relations != null)
                {
                    foreach (KeyValuePair<string, string> relation in dimensionDetail.Relations)
                    {
                        relations += relation.Key + "-" + relation.Value + "|";
                    }
                }

                selectListHtml += string.Format(
                    "<option value='{0}' {1} {2}>{3}</option>", dimensionDetail.Key, dimensionDetail.Selected ? "selected" : string.Empty, string.Format("{0}='{1}'", "relation", relations), dimensionDetail.Value);
            }

            return helper.Raw(string.Format("<select id='{0}' name='{1}' userdefined='{2}' class='dimension' style='Width: 200px' {3}>{4}</select>", dimension.ObjectName, dimension.TermLabel,dimension.UserDefinedDimension, dimension.Enabled ? string.Empty : "disabled", selectListHtml));
        }
    }
}
