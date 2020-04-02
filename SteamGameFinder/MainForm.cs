using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using gT = SteamGameFinder.Utils.GeneralTools;
using SteamGameFinder.Utils;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using cS = SteamGameFinder.Constants.clsSteamKeys;

namespace SteamGameFinder
{
    #region dox
    /*
     * The Steam store response for the query 'fallout' is below, pseudo-formatted by me. So looking at the structure
       we can see the layout is class->div->class, so we can either do a bunch of string ops or use HtmlAgilityPack to split
       out the match_[blah] nodes.
     
        <a class=\"match ds_collapse_flag \"  data-ds-appid=\"377160\" data-ds-itemkey=\"App_377160\" data-ds-tagids=\"[1695,3835,3834,4182,122,4166,21]\" data-ds-crtrids=\"[33028765,35501442]\" href=\"https://store.steampowered.com/app/377160/Fallout_4/?snr=1_7_15__13\">
        <div class=\"match_name\">Fallout 4</div>
        <div class=\"match_img\"><img src=\"https://steamcdn-a.akamaihd.net/steam/apps/377160/capsule_sm_120.jpg?t=1580240375\"></div>
        <div class=\"match_price\">$29.99</div>
        </a>
        <a class=\"match ds_collapse_flag \"  data-ds-appid=\"22380\" data-ds-itemkey=\"App_22380\" data-ds-tagids=\"[1695,122,3835,4182,1669,3834,3839]\" data-ds-crtrids=\"[5025,33028765,35501442]\" href=\"https://store.steampowered.com/app/22380/Fallout_New_Vegas/?snr=1_7_15__13\">
        <div class=\"match_name\">Fallout: New Vegas</div>
        <div class=\"match_img\"><img src=\"https://steamcdn-a.akamaihd.net/steam/apps/22380/capsule_sm_120.jpg?t=1564071971\"></div>
        <div class=\"match_price\">$9.99</div>
        </a>
        <a class=\"match ds_collapse_flag \"  data-ds-appid=\"588430\" data-ds-itemkey=\"App_588430\" data-ds-tagids=\"[113,1662,7332,3835,4182,599,9]\" data-ds-crtrids=\"[33028765,35501442]\" href=\"https://store.steampowered.com/app/588430/Fallout_Shelter/?snr=1_7_15__13\">
        <div class=\"match_name\">Fallout Shelter</div>
        <div class=\"match_img\"><img src=\"https://steamcdn-a.akamaihd.net/steam/apps/588430/capsule_sm_120.jpg?t=1572360438\"></div>
        <div class=\"match_price\">Free to Play</div>
        </a>
        <a class=\"match ds_collapse_flag \"  data-ds-appid=\"22370\" data-ds-itemkey=\"App_22370\" data-ds-tagids=\"[1695,122,3835,4182,1669,3839,3942]\" data-ds-crtrids=\"[33028765,35501442]\" href=\"https://store.steampowered.com/app/22370/Fallout_3_Game_of_the_Year_Edition/?snr=1_7_15__13\">
        <div class=\"match_name\">Fallout 3: Game of the Year Edition</div>
        <div class=\"match_img\"><img src=\"https://steamcdn-a.akamaihd.net/steam/apps/22370/capsule_sm_120.jpg?t=1564071894\"></div>
        <div class=\"match_price\">$19.99</div>
        </a>
        <a class=\"match ds_collapse_flag \"  data-ds-appid=\"435881\" data-ds-itemkey=\"App_435881\" data-ds-tagids=\"[122,1695,3835,4182,1662,1663,1774]\" data-ds-crtrids=\"[33028765,35501442]\" href=\"https://store.steampowered.com/app/435881/Fallout_4_Far_Harbor/?snr=1_7_15__13\">
        <div class=\"match_name\">Fallout 4 Far Harbor</div>
        <div class=\"match_img\"><img src=\"https://steamcdn-a.akamaihd.net/steam/apps/435881/capsule_sm_120.jpg?t=1533677062\"></div>
        <div class=\"match_price\">$24.99</div>
        </a>
      

        todo:

        specials
        https://store.steampowered.com/search/?specials=1&os=win
      
     */
    #endregion

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            toolTip1.SetToolTip(btnSearch, "Look for stuff");
            toolTip1.SetToolTip(btnSpecials, "Find righteous specials");

        }

        #region buttons

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (chkNull.isNull(txtSearch.Text)) return;

            try
            {
                crossYourFingers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "wat");
            }
            finally
            {
                lblStatus.Text = "Done";
            }
        }

        private void btnSpecials_Click(object sender, EventArgs e)
        {
            try
            {
                tryToGetSpecialsLol();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "wat");
            }
            finally
            {
                lblStatus.Text = "Done";
            }
        }

        #endregion

        #region search methods

        private void crossYourFingers()
        {
            lblStatus.Text = "Trying to talk to Steam";
            Application.DoEvents();

            WebClient client = new WebClient();

            string searchTerm = txtSearch.Text;
            string countryCode = txtCC.Text;
            if (chkNull.isNull(countryCode)) txtCC.Text = "US";

            // this query seems undocumented, at least I can't find information on it. Credit goes
            // to SwiftySpiffy, where I found the query
            // https://github.com/swiftyspiffy/SteamStoreQuery
            string url = "https://store.steampowered.com/search/suggest?term=" +
                searchTerm + "&f=games&cc=" + countryCode + "&lang=english&v=2286217";            

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string response = client.DownloadString(url);

            if (chkNull.isNull(response))
            {
                MessageBox.Show("No results found for " + txtSearch.Text, "Goose Egg");
                return;
            }

            lblStatus.Text = "Reading response";
            Application.DoEvents();

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(response);

            List<results> res = new List<results>();

            lblStatus.Text = "Parsing response";
            Application.DoEvents();
            parseResults(htmlDoc, res);

            lblStatus.Text = "Listing games";
            Application.DoEvents();
            listGames(res);
        }

        private static void parseResults(HtmlAgilityPack.HtmlDocument htmlDoc, List<results> res)
        {
            // use HtmlAgilityPack to loop through the structure of the HTML and try to find the main nodes.
            // seems all nodes are demarcated by whatever 'match ds_collapse_flag' is
            foreach (HtmlNode mdiv in htmlDoc.DocumentNode.SelectNodes("//a[contains(@class,'" + cS.steamSearchContainer + "')]"))
            {
                results r = new results();

                // Load each individual node into a new HtmlDocument
                HtmlAgilityPack.HtmlDocument innerDoc = new HtmlAgilityPack.HtmlDocument();
                innerDoc.LoadHtml(mdiv.InnerHtml);

                // now find the nodes we're looking for
                HtmlNode div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSearchGameName + "')]");
                // if found, HtmlAgilityPack returns what you're looking for in innerhtml
                // --> else {blow up}
                r.theGame = div.InnerHtml;

                div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSearchImage + "')]");
                r.theImage = div.InnerHtml;

                div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSearchPrice + "')]");
                r.thePrice = gT.ReplaceEx(div.InnerHtml, "$", "");

                // add the instance of results() to the res list
                res.Add(r);
            }
        }
       
        #endregion

        #region specials methods

        private void tryToGetSpecialsLol()
        {
            lblStatus.Text = "Trying to talk to Steam";
            Application.DoEvents();

            WebClient client = new WebClient();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string response = client.DownloadString(cS.steamSpecialsURL);
            string foo = response;            
            
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(response);
            List<results> res = new List<results>();

            lblStatus.Text = "Parsing response";
            Application.DoEvents();
            parseResults(htmlDoc, res, cS.steamSpecialsContainer);

            lblStatus.Text = "Listing games";
            Application.DoEvents();
            listGames(res, true);
        }

        private static void parseResults(HtmlAgilityPack.HtmlDocument htmlDoc, List<results> res, string classname)
        {
            // use HtmlAgilityPack to loop through the structure of the HTML and try to find the main nodes.
            foreach (HtmlNode mdiv in htmlDoc.DocumentNode.SelectNodes("//a[contains(@class,'" + classname + "')]"))
            {
                results r = new results();

                // Load each individual node into a new HtmlDocument
                HtmlAgilityPack.HtmlDocument innerDoc = new HtmlAgilityPack.HtmlDocument();
                innerDoc.LoadHtml(mdiv.InnerHtml);

                // now find the nodes we're looking for
                HtmlNode div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSpecialsGameName + "')]");
                // if found, HtmlAgilityPack returns what you're looking for in innerhtml
                // --> else {blow up}
                r.theGame = div.InnerHtml;

                div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSpecialsImage + "')]");
                r.theImage = div.InnerHtml;

                // the discounts and prices are squirrelled away in here, forcing us to do a couple more loads
                div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSpecialsMoneySection + "')]");
                innerDoc.LoadHtml(div.InnerHtml);
                div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSpecialsDiscount + "')]");

                string discount = gT.ReplaceEx(div.InnerText, "\r\n", "").Trim();

                div = innerDoc.DocumentNode.SelectSingleNode("//div[contains(@class,'" + cS.steamSpecialsPrices + "')]");
                string p = gT.ReplaceEx(div.InnerText, "\r\n", "").Trim();
                string[] p1 = p.Split('$');
                string price = "";
                if (p1.Length == 3)
                {
                    price = "Disc " + discount + " $" + p1[1] + " -> $" + p1[2];
                }
                else
                {
                    price = p;
                }

                r.thePrice = price;

                // add the instance of results() to the res list
                res.Add(r);
            }
        }

        #endregion

        #region rando

        private void listGames(List<results> res, bool specials = false)
        {
            // loop over the results, stick the data in controls, group the controls in panels
            // and stuff the whole mess into a flowlayoutpanel
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < res.Count; i++)
            {
                Panel pnl = new Panel();

                PictureBox p = new PictureBox();
                // this part's pretty kludgey
                string img = gT.ReplaceEx(res[i].theImage, "\"", "");
                img = gT.ReplaceEx(img, "<img src=", "");
                img = gT.ReplaceEx(img, ">", "");
                p.ImageLocation = img;
                // without specifying a relative location within panel pnl, everything overlays everything else
                p.Location = new Point(0, 0);
                pnl.Controls.Add(p);

                Label desc = new Label();
                desc.Text = res[i].theGame;
                if (specials)
                    desc.Text = StripHTML(res[i].theGame).Trim();

                desc.AutoSize = true;
                desc.Location = new Point(0, 50);
                desc.ForeColor = Color.Yellow;
                pnl.Controls.Add(desc);

                Label price = new Label();
                price.Text = "$" + chkNull.whenNull(res[i].thePrice);
                price.AutoSize = true;
                price.Location = new Point(0, 70);
                price.ForeColor = Color.Yellow;
                pnl.Controls.Add(price);

                pnl.Parent = flowLayoutPanel1;

            }
        }

        public static string StripHTML(string HTMLText)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            string stripped = reg.Replace(HTMLText, "");
            return stripped;
            // return decode ? HttpUtility.HtmlDecode(stripped) : stripped;
        }
      
        #endregion

    }

    public class results
    {
        // simple class to store three pieces of information about a game
        private string _game;

        public string theGame
        {
            get { return _game; }
            set { _game = value; }
        }

        private string _image;

        public string theImage
        {
            get { return _image; }
            set { _image = value; }
        }

        private string _price;

        public string thePrice
        {
            get { return _price; }
            set { _price = value; }
        }

    }

}
