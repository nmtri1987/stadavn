using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace RBVH.Stada.Intranet.ConsoleTest
{
    public class TestFunctions
    {
        public static void DeleteGroups()
        {
            string SPSite = "http://tronghieusp";
            using (SPSite objSite = new SPSite(SPSite))
            {
                using (SPWeb oweb = objSite.OpenWeb())
                {
                    List<int> sitegroupids = new List<int>();
                    foreach (SPGroup group in objSite.RootWeb.SiteGroups)
                    {
                        sitegroupids.Add(group.ID);
                    }
                    foreach (int id in sitegroupids) { oweb.SiteGroups.RemoveByID(id); }
                }
            }
        }
        public static void SendMail()
        {

            string SPSite = "http://localhost";
            using (SPSite objSite = new SPSite(SPSite))
            {
                using (SPWeb oweb = objSite.OpenWeb())
                {

                    // SPUtility.SendEmail(oweb, false, false, "email@email.com", "Title test", "this is the body");
                    bool appendHtmlTag = false;
                    bool htmlEncode = false;
                    string toAddress = "tri.ngominh@vn.bosch.com";
                    string subject = "Subject";
                    string message = "Message text TEST CONSOLE";
                    bool result = SPUtility.SendEmail(oweb, appendHtmlTag, htmlEncode, toAddress, subject, message);
                    var abc = 1;
                }
            }

            //   SPWeb web = SPContext.Current.Web;

        }
        private static void TestWorkflow()
        {
            string m_SPSiteUrl = "http://sp-devbox2013";
            using (SPSite m_SPSite = new SPSite(m_SPSiteUrl))
            {
                var m_Test = m_SPSite.RootWeb.WorkflowTemplates;
                foreach (SPWeb m_SPWeb in m_SPSite.AllWebs)
                {
                    Console.WriteLine("Web: " + m_SPWeb.Title);
                    foreach (SPList m_SPList in m_SPWeb.Lists)
                    {
                        Console.WriteLine("List: " + m_SPList.Title);
                        var m_WorkflowAssociations = m_SPList.WorkflowAssociations;
                        foreach (SPWorkflowAssociation m_SPWorkflowAssociation in m_WorkflowAssociations)
                        {
                            Console.WriteLine(m_SPWorkflowAssociation.Name);
                        }
                    }
                }
            }
        }
        private static void UpdateEmployeeShiftTime()
        {
            string m_SPSiteUrl = "http://sp-devbox2013";


            using (SPSite m_SPSite = new SPSite(m_SPSiteUrl))
            {
                var m_SPWeb = m_SPSite.OpenWeb();
                SPList employeeShiftTimeList = m_SPWeb.GetList(string.Format("{0}{1}", m_SPSiteUrl, "/Lists/EmployeeShiftTime"));


                SPListItem employeeShiftTimeListItem = employeeShiftTimeList.AddItem();

                employeeShiftTimeListItem["Employee"] = 1;
                employeeShiftTimeListItem["StadaDate"] = new DateTime(2016, 1, 1).ToString("MM/dd/yyyy");

                employeeShiftTimeListItem["Shift"] = 1;
                employeeShiftTimeListItem["IsValid"] = true;
                employeeShiftTimeListItem.Update();
            }

        }
        private static void InsatllFeature()
        {
            // Get the client context
            using (ClientContext clientContext = new ClientContext("http://sp-devbox2013/"))
            {
                // Load the features
                Site clientSite = clientContext.Site;
                FeatureCollection clientSiteFeatures = clientSite.Features;

                Guid FeatureId = new Guid("2fb92f93-ceba-48f6-90e2-4077f302f67c");
                clientContext.Load(clientSiteFeatures);
                //clientContext.ExecuteQuery();
                // Activate the feature
                clientSiteFeatures.Add(FeatureId, false, FeatureDefinitionScope.Site);
                //clientSiteFeatures.Remove(FeatureId, false);
                clientContext.ExecuteQuery();
            }
        }
        private static void TestFixIssue()
        {
            string m_SPSiteUrl = "http://sp-devbox2013/dev/nguyentran";
            using (SPSite m_SPSite = new SPSite(m_SPSiteUrl))
            {
                var m_SPWeb = m_SPSite.OpenWeb();
                var m_test = m_SPWeb.Lists["EmployeeInfoList"];
                m_SPWeb.Lists.Delete(m_test.ID);
                m_SPWeb.Update();
            }
        }
        private static void FixContentType()
        {
            string m_SiteUrl = "http://sp-devbox2013";

            using (SPSite m_SPSite = new SPSite(m_SiteUrl))
            {
                //foreach (SPWeb m_SPWeb in m_SPSite.AllWebs)
                //{
                //    foreach (SPContentType m_SPContentType in m_SPWeb.ContentTypes)
                //    {
                //        if (m_SPContentType.Name.Contains("Departments"))
                //        {
                //            var m_Usages = SPContentTypeUsage.GetUsages(m_SPContentType);
                //        }
                //    }
                //}
                var m_SPRootWeb = m_SPSite.RootWeb;

            }
        }
        private static void TestLookup()
        {
            string m_SiteUrl = "http://sp-devbox2013";

            using (SPSite m_SPSite = new SPSite(m_SiteUrl))
            {

            }
        }
        private static void TestDateTime(int fromDay, int fromMonth, int fromYear, int toDay, int toMonth, int toYear)
        {
            DateTime fromDate = new DateTime(fromYear, fromMonth, fromDay);
            DateTime toDate = new DateTime(toYear, toMonth, toDay);
            if ((toDate >= fromDate.AddDays(4)))
            {
                Console.WriteLine("Case 1");
                Console.WriteLine("Laeave >= 5 days");
            }
            else
            {
                Console.WriteLine("Case 2");
                Console.WriteLine("Laeave < 5 days");
            }

            // leaveRequestToDate >= leaveRequestFromDate.AddDays(4)
        }

        //private static void TestStringFormat()
        //{
        //    string m_Template = "{0} - {1}";
        //    string m_Text = string.Format(m_Template, 11, 22, 33, 44, 55);
        //}

        private static void TestCAML()
        {
            SPFieldLookupValueCollection values1 = new SPFieldLookupValueCollection();
            SPFieldLookupValueCollection values2 = new SPFieldLookupValueCollection();
            for (int i = 0; i < 2; i++)
            {
                string fieldValue = i + ";#" + i;
                values1.Add(new SPFieldLookupValue(fieldValue));
                // values2.Add(new SPFieldLookupValue(fieldValue));
            }
            string returnString = "<Where>";
            string element = String.Empty;
            for (int i = 0; i < values1.Count; i++)
            {

                if (i < values1.Count)
                {
                    returnString += "<And>";
                }


                element = "<Contains><FieldRef Name='EmployeesShift' /><Value Type='LookupMulti'>" + values1[i].LookupId + "</Value></Contains>";
                returnString += element;
                if (i == values1.Count - 1)
                {
                    returnString += "<Neq><FieldRef Name='IsValid' /><Value Type='Boolean'>0</Value></Neq>";
                }

            }

            string endTag = string.Empty;

            for (int j = 0; j < values1.Count; j++)
            {
                endTag += "</And>";

            }
            returnString += endTag;
            returnString += "</Where>";
        }
        public static void ParseXMLtoDynamic()
        {
            //string xml = @"<?xml version=""1.0"" encoding=""utf - 8""?><entry xml:base=""http://sp-devbox2013/_api/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://schemas.microsoft.com/ado/2007/08/dataservices"" xmlns:m=""http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml""><id>http://sp-devbox2013/_api/Web/GetUserById(79)</id><category term=""SP.User"" scheme=""http://schemas.microsoft.com/ado/2007/08/dataservices/scheme"" /><link rel=""edit"" href=""Web/GetUserById(79)"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/Groups"" type=""application/atom+xml;type=feed"" title=""Groups"" href=""Web/GetUserById(79)/Groups"" /><title /><updated>2016-12-20T03:01:16Z</updated><author><name /></author><content type=""application/xml""><m:properties><d:Id m:type=""Edm.Int32"">79</d:Id><d:IsHiddenInUI m:type=""Edm.Boolean"">false</d:IsHiddenInUI><d:LoginName>i:0#.w|rbvhsp\hm-common</d:LoginName><d:Title>HM ComUser1</d:Title><d:PrincipalType m:type=""Edm.Int32"">1</d:PrincipalType><d:Email>Nguyen.TranNgocHuu@vn.bosch.com</d:Email><d:IsSiteAdmin m:type=""Edm.Boolean"">false</d:IsSiteAdmin><d:UserId m:type=""SP.UserIdInfo""><d:NameId>s-1-5-21-3464779960-3153506264-3595381425-1199</d:NameId><d:NameIdIssuer>urn:office:idp:activedirectory</d:NameIdIssuer></d:UserId></m:properties></content></entry>";
            string xml2 = @"<?xml version=""1.0"" encoding=""utf - 8""?><feed xml:base=""http://sp-devbox2013/_api/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://schemas.microsoft.com/ado/2007/08/dataservices"" xmlns:m=""http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml""><id>b9e52242-fd4c-4eeb-8de0-c8306a0d0d94</id><title /><updated>2016-12-20T02:17:00Z</updated><entry m:etag=""&quot;1&quot;""><id>8d2da3cf-1041-43be-8b7f-0ba122662ed5</id><category term=""SP.Data.EmployeeShiftTimeListItem"" scheme=""http://schemas.microsoft.com/ado/2007/08/dataservices/scheme"" /><link rel=""edit"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/FirstUniqueAncestorSecurableObject"" type=""application/atom+xml;type=entry"" title=""FirstUniqueAncestorSecurableObject"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/FirstUniqueAncestorSecurableObject"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/RoleAssignments"" type=""application/atom+xml;type=feed"" title=""RoleAssignments"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/RoleAssignments"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/AttachmentFiles"" type=""application/atom+xml;type=feed"" title=""AttachmentFiles"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/AttachmentFiles"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/ContentType"" type=""application/atom+xml;type=entry"" title=""ContentType"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/ContentType"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/FieldValuesAsHtml"" type=""application/atom+xml;type=entry"" title=""FieldValuesAsHtml"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/FieldValuesAsHtml"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/FieldValuesAsText"" type=""application/atom+xml;type=entry"" title=""FieldValuesAsText"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/FieldValuesAsText"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/FieldValuesForEdit"" type=""application/atom+xml;type=entry"" title=""FieldValuesForEdit"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/FieldValuesForEdit"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/File"" type=""application/atom+xml;type=entry"" title=""File"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/File"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/Folder"" type=""application/atom+xml;type=entry"" title=""Folder"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/Folder"" /><link rel=""http://schemas.microsoft.com/ado/2007/08/dataservices/related/ParentList"" type=""application/atom+xml;type=entry"" title=""ParentList"" href=""Web/Lists(guid'eb7118bf-17ed-47e7-89c9-e92d0ad8b196')/Items(1)/ParentList"" /><title /><updated>2016-12-20T02:17:00Z</updated><author><name /></author><content type=""application/xml""><m:properties><d:FileSystemObjectType m:type=""Edm.Int32"">0</d:FileSystemObjectType><d:Id m:type=""Edm.Int32"">1</d:Id><d:ContentTypeId>0x010082B4587A9EFA47FCB1ADA5A1D1B8F0950090E4637C7267DB4BA42BD9C8C9CAE567</d:ContentTypeId><d:Title m:null=""true"" /><d:EmployeeId m:type=""Edm.Int32"">1</d:EmployeeId><d:StadaDate m:type=""Edm.DateTime"">2016-11-20T17:00:00Z</d:StadaDate><d:ShiftId m:type=""Edm.Int32"">1</d:ShiftId><d:IsValid m:type=""Edm.Boolean"">true</d:IsValid><d:ID m:type=""Edm.Int32"">1</d:ID><d:Modified m:type=""Edm.DateTime"">2016-12-19T12:11:08Z</d:Modified><d:Created m:type=""Edm.DateTime"">2016-12-19T12:11:08Z</d:Created><d:AuthorId m:type=""Edm.Int32"">53</d:AuthorId><d:EditorId m:type=""Edm.Int32"">53</d:EditorId><d:OData__UIVersionString>1.0</d:OData__UIVersionString><d:Attachments m:type=""Edm.Boolean"">false</d:Attachments><d:GUID m:type=""Edm.Guid"">0a16f7c5-fa5b-41fa-a54f-3883485cf4ef</d:GUID></m:properties></content></entry></feed>";
            //string xml = @"<Students>
            //    <Student ID=""100"">
            //        <Name>Arul</Name>
            //        <Mark>90</Mark>
            //    </Student>
            //    <Student>
            //        <Name>Arul2</Name>
            //        <Mark>80</Mark>
            //    </Student>
            //</Students>";

            dynamic data = DynamicXml.Parse(xml2);

            //var id = students.Student[0].ID;
            //var name1 = students.Student[1].Name;

            //foreach (var std in students.Student)
            //{
            //    Console.WriteLine(std.Mark);
            //}

        }
        public class DynamicXml : DynamicObject
        {
            XElement _root;
            private DynamicXml(XElement root)
            {
                _root = root;
            }

            public static DynamicXml Parse(string xmlString)
            {
                return new DynamicXml(XDocument.Parse(xmlString).Root);
            }

            public static DynamicXml Load(string filename)
            {
                return new DynamicXml(XDocument.Load(filename).Root);
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                var att = _root.Attribute(binder.Name);
                if (att != null)
                {
                    result = att.Value;
                    return true;
                }

                var nodes = _root.Elements(binder.Name);
                if (nodes.Count() > 1)
                {
                    result = nodes.Select(n => n.HasElements ? (object)new DynamicXml(n) : n.Value).ToList();
                    return true;
                }

                var node = _root.Element(binder.Name);
                if (node != null)
                {
                    result = node.HasElements ? (object)new DynamicXml(node) : node.Value;
                    return true;
                }

                return true;
            }
        }
        public static void TestResource()
        {
        }
    }
}
