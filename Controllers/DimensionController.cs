using Microsoft.AspNetCore.Mvc;
using SnapAPDimensionsIFrame.BusinessLogic;
using SnapAPDimensionsIFrame.Entity;
using SnapAPDimensionsIFrame.Entity.Intacct;
using SnapAPDimensionsIFrame.Models;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security;
using System.Xml;

namespace SnapAPDimensionsIFrame.Controllers
{
    public class DimensionController : Controller
    {
        private readonly ILogger<DimensionController> _logger;
        private readonly IConfiguration _config;
        private readonly DimensionUC _dimensionUC = new DimensionUC();
        private readonly APICredentials apiCredentials = new APICredentials();

        public DimensionController(ILogger<DimensionController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            apiCredentials.SenderID = _config.GetValue<string>("SenderID");
            apiCredentials.SenderPassword = _config.GetValue<string>("SenderPassword");
            apiCredentials.CompanyID = _config.GetValue<string>("CompanyID");
            apiCredentials.UserID = _config.GetValue<string>("UserID");
            apiCredentials.UserPassword = _config.GetValue<string>("UserPassword");
            apiCredentials.LocationID = _config.GetValue<string>("LocationID");
        }

        [HttpGet]
        public IActionResult Index(int? enabled, string? xmlValues)
        {
            List<Dimension> dimensions = new List<Dimension>();
            try
            {
                string connStr = MainModule.GetDBConnectionString(_config);
                SqlConnection sqlConn = MainModule.GetDBConnection(connStr, true);

                if (string.IsNullOrEmpty(xmlValues))
                {
                    dimensions = _dimensionUC.GetDimensions(apiCredentials, sqlConn);
                }
                else
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlValues = "<root>" + xmlValues + "</root>";
                    xmlDoc.LoadXml(xmlValues);
                    XmlNode? childNode = xmlDoc.FirstChild;

                    if (enabled == 1)
                    {
                        dimensions = _dimensionUC.GetDimensions(apiCredentials, sqlConn);

                        foreach (XmlNode dimensionNode in childNode)
                        {
                            string objectName = dimensionNode.Name;
                            if (objectName.EndsWith("ID"))
                            {
                                objectName = objectName.Substring(0, objectName.Length - 2);
                            }
                            Dimension? dimension = dimensions.Where(d => d.ObjectName == objectName).FirstOrDefault();
                            if (dimension != null)
                            {
                                DimensionDetail? dimensionDetail = dimension.DimensionDetails.Where(dd => dd.Value.Split("--")[0] == dimensionNode.InnerText).FirstOrDefault();
                                if (dimensionDetail != null)
                                {
                                    dimensionDetail.Selected = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        List<Mapping> mappings = _dimensionUC.GetMappings(sqlConn);

                        if (childNode != null)
                        {
                            foreach (XmlNode dimensionNode in childNode)
                            {
                                string objectName = dimensionNode.Name;
                                if (objectName.EndsWith("ID"))
                                {
                                    objectName = objectName.Substring(0, objectName.Length - 2);
                                }
                                Mapping? mapping = mappings.Where(m => m.ObjectName == objectName).FirstOrDefault();

                                if (mapping != null)
                                {
                                    Dimension dimension = new Dimension();
                                    dimension.ObjectName = mapping.ObjectName;
                                    dimension.TermLabel = mapping.TermLabel;
                                    dimension.Enabled = false;
                                    dimension.DimensionDetails = new List<DimensionDetail>
                    {
                        new DimensionDetail() { Key = 0, Value = dimensionNode.InnerText, Selected = true, Relations = new Dictionary<string, string>() }
                    };
                                    dimensions.Add(dimension);
                                }
                            }
                        }
                    }
                }

                sqlConn.Close();
            }
            catch (Exception)
            {

            }

            return View(dimensions);
        }

        [HttpPost]
        public IActionResult Index(List<Dimension> dimensions)
        {
            string connStr = MainModule.GetDBConnectionString(_config);
            SqlConnection sqlConn = MainModule.GetDBConnection(connStr, true);

            _dimensionUC.RefreshData(apiCredentials, sqlConn);
            dimensions = _dimensionUC.GetDimensions(apiCredentials, sqlConn);

            sqlConn.Close();

            return View(dimensions);
        }

        [HttpPost]
        public IActionResult GetRestrictions([FromBody()] List<DimensionViewModel> dimensionListViewModel)
        {
            List<DimensionViewModel> restrictions = new List<DimensionViewModel>();
            foreach (DimensionViewModel dimension in dimensionListViewModel)
            {
                if (!string.IsNullOrEmpty(dimension.ObjectName))
                {
                    string connStr = MainModule.GetDBConnectionString(_config);
                    SqlConnection sqlConn = MainModule.GetDBConnection(connStr, true);
                    XmlNode restrictionsXML = _dimensionUC.GetDimensionRestrictedData(apiCredentials, dimension.ObjectName, dimension.ObjectValue);
                    List<Mapping> mappings = _dimensionUC.GetMappings(sqlConn);
                    sqlConn.Close();

                    if (dimension.OneToOneRelations != null)
                    {
                        string[] relationSplit = dimension.OneToOneRelations.Split("|");

                        foreach (string relation in relationSplit)
                        {
                            if (!string.IsNullOrEmpty(relation))
                            {
                                string[] relNameData = relation.Split("-");
                                if (relNameData.Count() >= 2 && !string.IsNullOrEmpty(relNameData[1]))
                                {
                                    restrictions.Add(new DimensionViewModel() { ObjectName = relNameData[0], ObjectValue = relNameData[1] });
                                }
                            }
                        }
                    }

                    if (restrictionsXML != null)
                    {
                        foreach (XmlNode restrictedData in restrictionsXML.SelectNodes("RestrictedData"))
                        {
                            string restrictionValues = "";
                            foreach (XmlNode valueXML in restrictedData.SelectNodes("value"))
                            {
                                restrictionValues += valueXML.InnerText + ",";
                            }
                            restrictionValues = restrictionValues.Substring(0, restrictionValues.Length - 1);
                            restrictions.Add(new DimensionViewModel() { ObjectName = mappings.FirstOrDefault(m => m.ObjectName == restrictedData.SelectSingleNode("dimension").InnerText).ObjectName, ObjectValue = restrictionValues });
                        }
                    }
                }
            }
            return Json(restrictions);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}