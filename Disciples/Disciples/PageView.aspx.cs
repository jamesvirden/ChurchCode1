using System;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CmsData;
using System.IO;
using UtilityExtensions;

namespace Disciples
{
    public partial class PageView : System.Web.UI.Page
    {
        protected PageContent thisPage = new PageContent();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                this.Title = "Unauthorized page";
                Literal1.Text = "You must be registered and logged in to view this content.";
                Admin.Visible = false;
                ToggleEditor(false);
                return;
            }
            Admin.Visible = User.IsInRole("BlogAdministrator");
            string pageUrl = Request.QueryString<string>("p");
            if (pageUrl != "newpage.aspx")
            {
                try
                {
                    thisPage = ContentService.GetPage(pageUrl);
                    this.Title = thisPage.Title;
                    Literal1.Text = thisPage.Body.Replace("\"~/", "\"{0}".Fmt(Util.AppRoot));
                }
                catch
                {
                    lnkEdit.Enabled = false;
                    thisPage.Title = "Oops! Can't find that page...";
                }
                ToggleEditor(false);
            }
            else if (Admin.Visible)
            {
                SetupNewPage();
                this.Title = "Create a New Page";
            }
            EditorScript.Text =
@"
<script type=""text/javascript"">
    $(function() {{
        CKEDITOR.replace('{0}', {{
            filebrowserUploadUrl: '{1}',
            filebrowserImageUploadUrl: '{1}'
        }});
    }});
</script>".Fmt(Body2.UniqueID, Util.ResolveUrl("~/CKUpload.ashx"));

        }
        void SetupNewPage()
        {
            ToggleEditor(true);
            editorTitle.Text = "Create New Page";
            btnDelete.Visible = false;
            lnkEdit.Enabled = false;
        }
        void ToggleEditor(bool showIt)
        {
            pnlEdit.Visible = showIt;
            pnlPublic.Visible = !showIt;
            EditorScript.Visible = showIt;
        }

        protected void lnkEdit_Click(object sender, EventArgs e)
        {
            ToggleEditor(true);
            editorTitle.Text = "Edit " + thisPage.Title;
            lnkEdit.Enabled = false;
            txtTitle.Text = thisPage.Title;
            Body2.Text = thisPage.Body;
            btnDelete.Attributes.Add("onclick", "return CheckDelete();");
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/" + thisPage.PageUrl);
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            thisPage.Title = txtTitle.Text;
            thisPage.Body = Body2.Text;

            bool haveError = false;
            try
            {
                ContentService.SavePage(thisPage);
            }
            catch (Exception x)
            {
                haveError = true;
                ResultMessage1.ShowFail(x.Message);
            }
            if (!haveError)
                Response.Redirect("view/" + thisPage.PageUrl);
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool haveError = false;
            try
            {
                ContentService.DeletePage(thisPage.PageID);
            }
            catch (Exception x)
            {
                ResultMessage1.ShowFail(x.Message);
                haveError = true;
                ToggleEditor(true);
            }
            if (!haveError)
                Response.Redirect("~/admin/cmspagelist.aspx");
        }
    }

}