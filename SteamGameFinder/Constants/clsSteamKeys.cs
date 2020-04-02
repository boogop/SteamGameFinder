using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamGameFinder.Constants
{
    public static class clsSteamKeys
    {
        public static string steamSpecialsMoneySection = "col search_price_discount_combined responsive_secondrow";
        public static string steamSpecialsGameName = "col search_name ellipsis";
        public static string steamSpecialsImage = "col search_capsule";
        public static string steamSpecialsDiscount = "col search_discount responsive_secondrow";
        public static string steamSpecialsPrices = "col search_price discounted responsive_secondrow";
        public static string steamSpecialsContainer = "search_result_row ds_collapse_flag";
        public static string steamSpecialsURL = "https://store.steampowered.com/search/?specials=1&os=win";

        public static string steamSearchContainer = "match ds_collapse_flag";
        public static string steamSearchGameName = "match_name";
        public static string steamSearchImage = "match_img";
        public static string steamSearchPrice = "match_price";

        // todo
        public static string steamSearchPage = "https://store.steampowered.com/search/?term=fall";

        /*
         * To use the url produced by regular steam search this block contains one result
         * - read search_result_container
         * - col search_capsule
         * - col search_name ellipsis (will it do spans?)
         * - col search_price discounted responsive_secondrow
         * 
<div id = \"search_result_container\" >\r\n\r\n\t
<div class=\"search_rule\"></div>\r\n\r\n\t\r\n        <!-- Extra empty div to hack around IE7 layout bug -->\r\n        
<div></div>\r\n        <!-- End Extra empty div -->\r\n\r\n\t\t
<div id=\"search_resultsRows\">\r\n\t\t\t\r\n<!-- List Items -->\r\n\t\t
<a href=\"https://store.steampowered.com/app/606880/GreedFall/?snr=1_7_7_151_150_1\"  
data-ds-appid=\"606880\" data-ds-itemkey=\"App_606880\" data-ds-tagids=\"[122,4182,1695,4747,6426,1697,21]\" 
data-ds-descids=\"[2,5]\" data-ds-crtrids=\"[21394]\" onmouseover=\"GameHover( this, event, 'global_hover', 
{&quot;type&quot;:&quot;app&quot;,&quot;id&quot;:606880,&quot;public&quot;:1,&quot;v6&quot;:1} );
\" onmouseout=\"HideGameHover( this, event, 'global_hover' )\" class=\"search_result_row ds_collapse_flag \" 
data-search-page=\"1\" >\r\n            
<div class=\"col search_capsule\"><img src=\"https://steamcdn-a.akamaihd.net/steam/apps/606880/capsule_sm_120.jpg?t=1575302145\" 
srcset=\"https://steamcdn-a.akamaihd.net/steam/apps/606880/capsule_sm_120.jpg?t=1575302145 1x, 
https://steamcdn-a.akamaihd.net/steam/apps/606880/capsule_231x87.jpg?t=1575302145 2x\"></div>\r\n            
<div class=\"responsive_search_name_combined\">\r\n                
<div class=\"col search_name ellipsis\">\r\n                    
<span class=\"title\">GreedFall</span>\r\n                    <p>\r\n                       
<span class=\"platform_img win\"></span>                    </p>\r\n                
</div>\r\n                
<div class=\"col search_released responsive_secondrow\">Sep 9, 2019</div>\r\n               
<div class=\"col search_reviewscore responsive_secondrow\">\r\n                                            
<span class=\"search_review_summary positive\" data-tooltip-html=\"Very Positive&lt;br&gt;80% of the 6,793 user 
reviews for this game are positive.\">\r\n\t\t\t\t\t\t\t\t</span>\r\n                                    </div>\r\n\r\n\r\n                
<div class=\"col search_price_discount_combined responsive_secondrow\" data-price-final=\"2999\">\r\n                   
<div class=\"col search_discount responsive_secondrow\">\r\n                        
<span>-40%</span>\r\n                    </div>\r\n                   
<div class=\"col search_price discounted responsive_secondrow\">\r\n                        
<span style=\"color: #888888;\"><strike>$49.99</strike></span><br>$29.99                    
</div>\r\n                </div>\r\n            </div>\r\n\r\n\r\n            <div style=\"clear: left;\"></div>
r\n        
</a>
         
         */

    }
}
