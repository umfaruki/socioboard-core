﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SocioBoard.Model;
using SocioBoard.Domain;
using System.Collections;
using Facebook;
using GlobusTwitterLib.Authentication;
using SocioBoard.Helper;
using GlobusTwitterLib.App.Core;
using GlobusLinkedinLib.Authentication;
using GlobusLinkedinLib.LinkedIn.Core.SocialStreamMethods;
using Newtonsoft.Json.Linq;
using System.Configuration;
using GlobusTwitterLib.Twitter.Core.TweetMethods;
using GlobusTwitterLib.Twitter.Core.UserMethods;
using GlobusTwitterLib.Twitter.Core.FriendshipMethods;
using System.Data;
using SocioBoard.Feeds;
//using letTalkNew.Message;
using System.IO;
using log4net;

namespace letTalkNew
{
    public partial class AjaxHome : System.Web.UI.Page
    {
        public static int profilelimit = 0;
        ILog logger = LogManager.GetLogger(typeof(AjaxHome));
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["LoggedUser"] == null)
                {
                    Response.Write("logout");
                    return;
                }
                ProcessRequest();
            }
            catch (Exception Err)
            {
                Console.WriteLine(Err.Message);
                logger.Error(Err.Message);
            }
        }
        public void ProcessRequest()
        {
            SocialProfilesRepository socio = new SocialProfilesRepository();
            List<SocialProfile> alstsocioprofiles = new List<SocialProfile>();
            if (!string.IsNullOrEmpty(Request.QueryString["op"]))
            {
                SocioBoard.Domain.User user = (SocioBoard.Domain.User)Session["LoggedUser"];

                if (Request.QueryString["op"] == "social_connectivity")
                {

                    alstsocioprofiles = socio.getAllSocialProfilesOfUser(user.Id);

                    string profiles = string.Empty;
                    //   profiles += "<ul class=\"rsidebar-profile\">";
                    foreach (SocialProfile item in alstsocioprofiles)
                    {
                        try
                        {
                            if (item.ProfileType == "facebook")
                            {
                                try
                                {
                                    FacebookAccountRepository facerepo = new FacebookAccountRepository();
                                    FacebookAccount faceaccount = facerepo.getFacebookAccountDetailsById(item.ProfileId, user.Id);

                                    #region notinuse

                                    //            <li>
                                    //    <div class="userpictiny">
                                    //        <a href="#&quot;" target="_blank">
                                    //            <img alt="" src="Contents/Img/photo.png">                                    
                                    //        </a>
                                    //        <a href="#" title="" class="userurlpic fb_icon"></a>
                                    //    </div>
                                    //</li><img src=\"../Contents/img/fb_icon.png\" width=\"16\" height=\"16\" alt=\"\"> 
                                    #endregion

                                    profiles += "<li id=\"so_" + item.ProfileId + "\"><div id=\"" + item.ProfileId + "\" class=\"userpictiny\" onclick=\"getFacebookProfileId('" + item.ProfileId + "','fb');\"><div class=\"delet_icon\" onclick=\"confirmDel('" + item.ProfileId + "','fb');\"></div><a href=\"http://www.facebook.com/" + faceaccount.FbUserId + "\" target=\"_blank\"><img src=\"http://graph.facebook.com/" + item.ProfileId + "/picture?type=small\" height=\"48\" width=\"48\" alt=\"\" title=\"" + faceaccount.FbUserName + "\" /></a>" +
                                                "<a href=\"#\" class=\"userurlpic fb_icon\" title=\"\"></a></div></li>";
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    logger.Error(ex.Message);
                                }

                            }
                            else if (item.ProfileType == "twitter")
                            {
                                try
                                {
                                    TwitterAccountRepository twtrepo = new TwitterAccountRepository();
                                    SocioBoard.Domain.TwitterAccount twtaccount = twtrepo.getUserInformation(user.Id, item.ProfileId);

                                    profiles += "<li id=\"so_" + item.ProfileId + "\"><div id=\"" + item.ProfileId + "\" class=\"userpictiny\"><div class=\"delet_icon\" onClick=\"confirmDel('" + item.ProfileId + "','twt')\"></div><a href=\"http://twitter.com/" + twtaccount.TwitterScreenName + "\" target=\"_blank\"><img src=\"" + twtaccount.ProfileImageUrl + "\" height=\"48\" width=\"48\" alt=\"\" title=\"" + twtaccount.TwitterScreenName + "\" /></a>" +
                                                "<a href=\"#\" class=\"userurlpic twt_icon\" title=\"\"></a></div></li>";

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    logger.Error(ex.Message);
                                }
                            }
                            else if (item.ProfileType == "linkedin")
                            {
                                try
                                {

                                    LinkedInAccountRepository liRepo = new LinkedInAccountRepository();
                                    string access = string.Empty, tokenSecrate = string.Empty, LdprofileName = string.Empty, LdPreofilePic = string.Empty;
                                    LinkedInAccount liaccount = liRepo.getUserInformation(user.Id, item.ProfileId);

                                    if (liaccount != null)
                                    {
                                        if (!string.IsNullOrEmpty(liaccount.ProfileImageUrl))
                                        {
                                            LdPreofilePic = liaccount.ProfileImageUrl;
                                        }
                                        else
                                        {
                                            LdPreofilePic = "../../Contents/img/blank_user.png";
                                        }


                                        profiles += "<li id=\"so_" + item.ProfileId + "\"><div id=\"" + item.ProfileId + "\" class=\"userpictiny\"><div class=\"delet_icon\" onClick=\"confirmDel('" + item.ProfileId + "','linkedin')\"></div><a href=\"" + liaccount.ProfileUrl + "\" target=\"_blank\"><img src=\"" + LdPreofilePic + "\" height=\"48\" width=\"48\" alt=\"\" title=\"" + liaccount.LinkedinUserName + "\" /></a>" +
                                                   "<a href=\"#\" class=\"userurlpic linkedin_icon\" title=\"\"></a></div></li>";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    logger.Error(ex.Message);
                                }

                            }

                            else if (item.ProfileType == "instagram")
                            {
                                try
                                {
                                    InstagramAccountRepository objInsAccRepo = new InstagramAccountRepository();
                                    InstagramAccount objInsAcc = objInsAccRepo.getInstagramAccountDetailsById(item.ProfileId, user.Id);
                                    string accessToken = string.Empty;

                                    profiles += "<li id=\"so_" + item.ProfileId + "\"><div id=\"" + item.ProfileId + "\" class=\"userpictiny\"><div class=\"delet_icon\" onClick=\"confirmDel('" + item.ProfileId + "','instagram')\"><a href=\"" + objInsAcc.ProfileUrl + "\" target=\"_blank\"></div><img src=\"" + objInsAcc.ProfileUrl + "\" height=\"48\" width=\"48\" alt=\"\" title=\"" + objInsAcc.InsUserName + "\" />" +
                                                "<a href=\"#\" class=\"userurlpic instagram_icon \" title=\"\"></a></div></li>";

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    logger.Error(ex.Message);
                                }
                            }
                            else if (item.ProfileType == "googleplus")
                            {
                                try
                                {

                                    GooglePlusAccountRepository objgpAccRepo = new GooglePlusAccountRepository();
                                    GooglePlusAccount objgpAcc = objgpAccRepo.getGooglePlusAccountDetailsById(item.ProfileId, user.Id);
                                    string accessToken = string.Empty;

                                    profiles += "<li id=\"so_" + item.ProfileId + "\"><div id=\"" + item.ProfileId + "\" class=\"userpictiny\"><div class=\"delet_icon\" onClick=\"confirmDel('" + item.ProfileId + "','googleplus')\"></div><a href=\"http://plus.google.com/" + item.ProfileId + "\" target=\"_blank\"><img src=\"" + objgpAcc.GpProfileImage + "\" height=\"48\" width=\"48\" alt=\"\" title=\"" + objgpAcc.GpUserName + "\" /></a>" +
                                                "<a href=\"#\" class=\"userurlpic gplus_icon\" title=\"\"></a></div></li>";

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    logger.Error(ex.Message);
                                }
                            }
                            else if (item.ProfileType == "googleanalytics")
                            {
                                try
                                {

                                    //GoogleAnalyticsAccountRepository objgaAccRepo = new GoogleAnalyticsAccountRepository();
                                    //GoogleAnalyticsAccount objgaAcc = objgaAccRepo.getGoogelAnalyticsAccountHomeDetailsById(user.Id, item.ProfileId);
                                    //string accessToken = string.Empty;

                                    //profiles += "<li id=\"so_" + item.ProfileId + "\"><div id=\"" + item.ProfileId + "\" class=\"userpictiny\"><div class=\"delet_icon\" onClick=\"confirmDel('" + item.ProfileId + "','googleanalytics')\"></div><a href=\"http://plus.google.com/" + item.ProfileId + "\" target=\"_blank\"><img src=\"../Contents/img/google_analytics.png\" height=\"48\" width=\"48\" alt=\"\" title=\"" + objgaAcc.GaAccountName + "\" /></a>" +
                                    //            "<a href=\"#\" class=\"userurlpic\" title=\"\"></a></div></li>";

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    logger.Error(ex.Message);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);

                        }
                    } //profiles += "</ul>";
                    Response.Write(profiles);
                }
                else if (Request.QueryString["op"] == "woodrafts")
                {
                    string message = string.Empty;
                    try
                    {
                        DraftsRepository draftsRepository = new DraftsRepository();
                        List<Drafts> lstDrafts = draftsRepository.getAllDrafts(user.Id);
                        string profurl = string.Empty;
                        if (string.IsNullOrEmpty(user.ProfileUrl))
                        {
                            profurl = "../Contents/img/blank_img.png";
                        }
                        else
                        {
                            profurl = user.ProfileUrl;
                        }
                        if (lstDrafts.Count != 0)
                        {
                            foreach (Drafts item in lstDrafts)
                            {
                                message += "<section class=\"section\" >" +
                                        "<div class=\"js-task-cont read\"><section class=\"task-owner\"><img width=\"\" height=\"\" border=\"0\" src=\"../Contents/img/task_pin.png\" class=\"avatar\"></section>" +
                                        "<section class=\"task-activity third\"><p>" + user.UserName + "</p><div>" + item.CreatedDate + "</div><p></p></section>" +
                                        "<section class=\"task-message font-13 third\" style=\"margin-right: 6px; height: auto; width: 35%;\"><a id=\"message_" + item.Id + "\" class=\"tip_left\" onclick=\"writemessage(this.innerHTML);\">" + item.Message + "</a></section>" +
                                        "<div style=\"height:70px; margin-top: 0;\" class=\"userpictiny\"><img src=\"" + user.ProfileUrl + "\" alt=\"\" /> </div><section class=\"task-status\" style=\"width: 68px; margin-right: 29px;\">" +
                                           "<span onclick=\"editDraftsMessage('" + item.Id + "');\" class=\"ui-sproutmenu-status\">" +
                                                "<img class=\"edit_button\" src=\"../Contents/img/icon_edit.png\">" +
                                           "</span>" +
                                        "</section>" +
                                    "<a title=\"Delete\" style=\"top: 7px; float: right; margin-top: 9px; margin-right: -46px;\" onclick=\"deleteDraftMessage('" + item.Id + "')\" href=\"#\" class=\"small_remove icon publish_delete\"></a></div></section>";
                            }
                        }
                        else
                        {
                            message += "<section class=\"section\"><div class=\"js-task-cont read\"><section class=\"task-owner\">" +
                                  "<img width=\"32\" height=\"32\" border=\"0\" class=\"avatar\" src=\"" + profurl + "\">" +
                                  "</section><section class=\"task-activity third\"><p>" + user.UserName + "</p><div></div><p></p></section><section style=\"margin-right: 6px; width: 40%; height: auto;\" class=\"task-message font-13 third\">" +
                                  "<a class=\"tip_left\">No Messages in Drafts</a></section><section style=\"width:113px;\" class=\"task-status\"><span class=\"ficon task_active\"></span>" +
                                  "<div class=\"ui_light floating task_status_change\"><a class=\"ui-sproutmenu\" href=\"#nogo\">" +
                                  "<span class=\"ui-sproutmenu-status\"></span></a></div></section></div></section>";
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                    Response.Write(message);
                }
                else if (Request.QueryString["op"] == "savedrafts")
                {
                    try
                    {
                        string message = Request.QueryString["message"];
                        Drafts d = new Drafts();
                        d.CreatedDate = DateTime.Now;
                        d.Message = message;
                        d.ModifiedDate = DateTime.Now;
                        d.UserId = user.Id;
                        d.Id = Guid.NewGuid();
                        DraftsRepository dRepo = new DraftsRepository();
                        if (!dRepo.IsDraftsMessageExist(user.Id, message))
                        {
                            dRepo.AddDrafts(d);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                }
                else if (Request.QueryString["op"] == "midsnaps")
                {
                    try
                    {
                        Random rNum = new Random();
                        string loadtype = Request.QueryString["loadtype"];
                        string midsnaps = string.Empty;
                        if (loadtype == "load")
                            profilelimit = 0;

                        if (profilelimit != -1)
                        {
                            ArrayList alst = socio.getLimitProfilesOfUser(user.Id, profilelimit);

                            if (alst.Count == 0)
                                profilelimit = -1;
                            else if (profilelimit == 0)
                                profilelimit += 2;
                            else
                                profilelimit += 3;

                            midsnaps += "<div class=\"row-fluid\" >";
                            if (loadtype == "load")
                            {
                                AdsRepository objAdsRepo = new AdsRepository();
                                ArrayList lstads = objAdsRepo.getAdsForHome();

                                int i = 0;
                                if (lstads.Count <= 1)
                                {
                                    midsnaps += "<div class=\"span4 rounder recpro\"><button data-dismiss=\"alert\" class=\"close pull-right\" type=\"button\">×</button>" +
                                         "<a href=\"#\"><img src=\"../Contents/img/admin/ads.png\"  alt=\"\" style=\"width:246px;height:331px\"></a></div>";
                                }
                                else
                                {
                                    foreach (var item in lstads)
                                    {
                                        Array temp = (Array)item;
                                        i++;
                                        if (temp != null)
                                        {
                                            if (i == 2)
                                            {
                                                midsnaps += "<div class=\"span4 rounder recpro\"><button data-dismiss=\"alert\" class=\"close pull-right\" type=\"button\">×</button>" +
                                               "<a href=\"#\"><img src=\"" + temp.GetValue(2).ToString() + "\"  alt=\"\" style=\"width:246px;height:331px\"></a></div>";
                                            }
                                        }
                                    }
                                }

                            }
                            foreach (SocialProfile item in alst)
                            {
                                if (item.ProfileType == "facebook")
                                {
                                    try
                                    {
                                        FacebookAccountRepository fbrepo = new FacebookAccountRepository();
                                        FacebookFeedRepository facefeedrepo = new FacebookFeedRepository();
                                        List<FacebookFeed> fbmsgs = facefeedrepo.getAllFacebookUserFeeds(item.ProfileId);
                                        FacebookAccount fbaccount = fbrepo.getFacebookAccountDetailsById(item.ProfileId, user.Id);


                                        midsnaps += "<div id=\"mid_" + item.ProfileId + "\" style=\"height:333px;\" class=\"span4 rounder recpro\"><div class=\"concotop\">" +
                                                   "<div class=\"userpictiny\"><img width=\"56\" height=\"56\" title=\"" + fbaccount.FbUserName + "\" alt=\"\" src=\"http://graph.facebook.com/" + item.ProfileId + "/picture?type=small\"\">" +
                                                   "<a title=\"\" class=\"userurlpic\" href=\"#\"><img alt=\"\" src=\"../Contents/img/fb_icon.png\" width=\"16\" height=\"16\"></a></div>" +
                                                   "<div class=\"useraccname\">" + fbaccount.FbUserName + "</div><div class=\"usercounter\">" +
                                                   "<div class=\"userfoll\">" + fbaccount.Friends;

                                        if (fbaccount.Type == "page")
                                        {
                                            midsnaps += "<span>Fans</span>";
                                        }
                                        else
                                        {
                                            midsnaps += "<span>Friends</span>";

                                        }
                                        midsnaps += "</div>" +
                                                   "<div class=\"userppd\">" + Math.Round(rNum.NextDouble(), 2) + "<span>Avg. Post Per Day</span></div></div></div>" +
                                                   "<div class=\"concoteng\"><h5>recent message</h5> <ul class=\"mess\">";

                                        if (fbmsgs.Count != 0)
                                        {
                                            int msgcount = 0;
                                            foreach (FacebookFeed child in fbmsgs)
                                            {
                                                string mess = string.Empty;
                                                if (msgcount < 2)
                                                {
                                                    if (child.FeedDescription.Length > 40)
                                                    {
                                                        mess = child.FeedDescription.Substring(0, 39);
                                                        mess = mess + "...........";
                                                    }
                                                    else
                                                    {
                                                        mess = child.FeedDescription;
                                                    }

                                                    midsnaps += "<li><div class=\"messpic\"><img title=\"\" alt=\"\" src=\"http://graph.facebook.com/" + child.FromId + "/picture?type=small\"></div>" +
                                                              "<div class=\"messtext\">" + mess + "</div></li>";
                                                    //  midsnaps += "<strong><img src=\"http://graph.facebook.com/" + child.FromId + "/picture?type=small\" />" + mess + "</strong><br/>";
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                                msgcount++;
                                            }
                                        }
                                        else
                                        {
                                            midsnaps += "<strong>No messages were found within the past few days.</strong> \"Messages will be displayed once there is activity in this date range.\"";
                                        }

                                        midsnaps += "</ul></div></div>";
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex.Message);
                                    }
                                }
                                if (item.ProfileType == "googleplus")
                                {
                                    try
                                    {
                                        GooglePlusAccountRepository objgpAccRepo = new GooglePlusAccountRepository();
                                        GooglePlusActivitiesRepository objgpActRepo = new GooglePlusActivitiesRepository();

                                        GooglePlusAccount gaaccount = objgpAccRepo.getGooglePlusAccountDetailsById(item.ProfileId, user.Id);
                                        List<GooglePlusActivities> gpmsgs = objgpActRepo.getAllgoogleplusActivityOfUser(user.Id, item.ProfileId);

                                        midsnaps += "<div id=\"mid_" + item.ProfileId + "\" style=\"height:333px;\" class=\"span4 rounder recpro\"><div class=\"concotop\">" +
                                                   "<div class=\"userpictiny\"><img width=\"56\" height=\"56\" title=\"" + gaaccount.GpUserName + "\" alt=\"\" src='" + gaaccount.GpProfileImage + "'>" +
                                                   "<a title=\"\" class=\"userurlpic\" href=\"#\"><img alt=\"\" src=\"../Contents/img/google_plus.png\" width=\"16\" height=\"16\"></a></div>" +
                                                   "<div class=\"useraccname\">" + gaaccount.GpUserName + "</div><div class=\"usercounter\">" +
                                                   "<div class=\"userfoll\">" + gaaccount.PeopleCount + "<span>Friends</span></div>" +
                                                   "<div class=\"userppd\">" + Math.Round(rNum.NextDouble(), 2) + "<span>Avg. Post Per Day</span></div></div></div>" +
                                                   "<div class=\"concoteng\"><h5>recent message</h5> <ul class=\"mess\">";
                                        if (gpmsgs.Count() != 0)
                                        {
                                            try
                                            {
                                                int msgcount = 0;
                                                foreach (GooglePlusActivities child in gpmsgs)
                                                {
                                                    string mess = string.Empty;
                                                    if (msgcount < 2)
                                                    {
                                                        if (child.Content.Length > 40)
                                                        {
                                                            mess = child.Content.Substring(0, 39);
                                                            mess = mess + "...........";
                                                        }
                                                        else
                                                        {
                                                            mess = child.Content;
                                                        }

                                                        midsnaps += "<li><div class=\"messpic\"><img title=\"\" width=\"24px\" height=\"24px\" alt=\"\" src=\"" + child.FromProfileImage + "\"></div>" +
                                                                  "<div class=\"messtext\">" + mess + "</div></li>";
                                                        //  midsnaps += "<strong><img src=\"http://graph.facebook.com/" + child.FromId + "/picture?type=small\" />" + mess + "</strong><br/>";
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                    msgcount++;
                                                }
                                            }
                                            catch (Exception exx)
                                            {
                                                Console.WriteLine(exx.Message);
                                                logger.Error(exx.Message);
                                            }
                                        }
                                        else
                                        {
                                            midsnaps += "<strong>No messages were found within the past few days.</strong> \"Messages will be displayed once there is activity in this date range.\"";
                                        }

                                        midsnaps += "</ul></div></div>";

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        logger.Error(ex.Message);
                                    }

                                }
                                else if (item.ProfileType == "twitter")
                                {
                                    TwitterAccountRepository twtrepo = new TwitterAccountRepository();
                                    SocioBoard.Domain.TwitterAccount twtaccount = twtrepo.getUserInformation(user.Id, item.ProfileId);
                                    TwitterMessageRepository twtmsgrepo = new TwitterMessageRepository();
                                    List<TwitterMessage> lsttwtmsgs = twtmsgrepo.getAllTwitterMessagesOfProfile(item.ProfileId);
                                    int tweetcount = 0;

                                    midsnaps += "<div id=\"mid_" + item.ProfileId + "\" style=\"height:333px;\" class=\"span4 rounder recpro\"><div class=\"concotop\">" +
                                     "<div class=\"userpictiny\"><img width=\"56\" height=\"56\" title=\"" + twtaccount.TwitterName + "\" alt=\"\" src=\"" + twtaccount.ProfileImageUrl + "\">" +
                                     "<a title=\"\" class=\"userurlpic\" href=\"#\"><img alt=\"\" src=\"../Contents/img/twticon.png\" width=\"16\" height=\"16\"></a></div>" +
                                     "<div class=\"useraccname\">" + twtaccount.TwitterScreenName + "</div><div class=\"usercounter\">" +
                                     "<div class=\"userfoll\">" + twtaccount.FollowersCount + "<span>Followers</span></div>" +
                                     "<div class=\"userppd\">" + Math.Round(rNum.NextDouble(), 2) + "<span>Avg. Post Per Day</span></div></div></div>" +
                                     "<div class=\"concoteng\"><h5>recent message</h5> <ul class=\"mess\">";
                                    try
                                    {
                                        if (lsttwtmsgs.Count == 0)
                                        {
                                            midsnaps += "<strong>No messages were found within the past few days.</strong> \"Messages will be displayed once there is activity in this date range.\"";
                                        }
                                        else
                                        {
                                            foreach (TwitterMessage msg in lsttwtmsgs)
                                            {
                                                if (tweetcount < 2)
                                                {
                                                    try
                                                    {
                                                        string ms = string.Empty;
                                                        if (msg.TwitterMsg.Length > 20)
                                                        {
                                                            ms = msg.TwitterMsg.Substring(0, 20) + "...";

                                                        }
                                                        else
                                                        {
                                                            ms = msg.TwitterMsg;
                                                        }
                                                        midsnaps += "<li><div class=\"messpic\"><img title=\"\" alt=\"\" src=\"" + msg.FromProfileUrl + "\"></div>" +
                                                        "<div class=\"messtext\">" + ms + "</div></li>";
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.Message);

                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                                tweetcount++;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                        Console.WriteLine(ex.Message);
                                        logger.Error(ex.Message);
                                    }
                                    midsnaps += "</ul></div></div>";
                                }
                                else if (item.ProfileType == "linkedin")
                                {
                                    try
                                    {
                                        string access = string.Empty, tokenSecrate = string.Empty, LdprofileName = string.Empty, LdPreofilePic = string.Empty;
                                        LinkedInAccountRepository objLiRepo = new LinkedInAccountRepository();
                                        LinkedInFeedRepository objliFeedRepo = new LinkedInFeedRepository();
                                        LinkedInAccount liAccount = objLiRepo.getUserInformation(user.Id, item.ProfileId);
                                        LinkedInFeedRepository lifeedrepo = new LinkedInFeedRepository();
                                        List<LinkedInFeed> alstliaccount = lifeedrepo.getAllLinkedInFeedsOfProfile(item.ProfileId);
                                        if (liAccount != null)
                                        {
                                            LdprofileName = liAccount.LinkedinUserName;
                                            LdPreofilePic = liAccount.ProfileImageUrl;
                                        }

                                        if (string.IsNullOrEmpty(LdPreofilePic))
                                        {
                                            LdPreofilePic = "../../Contents/img/blank_img.png";
                                        }
                                        int linkedinConcount = 0;
                                        try
                                        {
                                            linkedinConcount = liAccount.Connections;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                        }

                                        midsnaps += "<div id=\"mid_" + item.ProfileId + "\" style=\"height:333px;\" class=\"span4 rounder recpro\"><div class=\"concotop\">" +
                                           "<div class=\"userpictiny\"><img width=\"56\" height=\"56\" title=\"" + LdprofileName + "\" alt=\"\" src=\"" + LdPreofilePic + "\">" +
                                           "<a title=\"\" class=\"userurlpic\" href=\"#\"><img alt=\"\" src=\"../Contents/img/link_icon.png\" width=\"16\" height=\"16\"></a></div>" +
                                           "<div class=\"useraccname\">" + LdprofileName + "</div><div class=\"usercounter\">" +
                                           "<div class=\"userfoll\">" + linkedinConcount + "<span>Connections</span></div>" +
                                           "<div class=\"userppd\">" + Math.Round(rNum.NextDouble(), 2) + "<span>Avg. Post Per Day</span></div></div></div>" +
                                           "<div class=\"concoteng\"><h5>recent message</h5> <ul class=\"mess\">";
                                        int link = 0;
                                        if (alstliaccount.Count == 0)
                                        {
                                            midsnaps += "<strong>No messages were found within the past 14 days.</strong> \"Messages will be displayed once there is activity in this date range.\"";
                                        }
                                        else
                                        {

                                            try
                                            {

                                                foreach (LinkedInFeed liFeed in alstliaccount)
                                                {
                                                    try
                                                    {
                                                        if (link < 2)
                                                        {
                                                            string ms = string.Empty;
                                                            if (liFeed.Feeds.Length > 20)
                                                            {
                                                                ms = liFeed.Feeds.Substring(0, 20) + "...";

                                                            }
                                                            else
                                                            {
                                                                ms = liFeed.Feeds;
                                                            }
                                                            midsnaps += "<li><div class=\"messpic\"><img title=\"\" alt=\"\" src=\"" + liFeed.FromPicUrl + "\"></div>" +
                                                           "<div class=\"messtext\">" + ms + "</div></li>";
                                                            link++;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        logger.Error(ex.Message);
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error(ex.Message);
                                            }

                                        }
                                        midsnaps += "</ul></div></div>";
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        logger.Error(ex.Message);
                                    }
                                }
                                else if (item.ProfileType == "instagram")
                                {
                                    try
                                    {
                                        InstagramAccountRepository objInsAccRepo = new InstagramAccountRepository();
                                        InstagramAccount objInsAcc = objInsAccRepo.getInstagramAccountDetailsById(item.ProfileId, user.Id);

                                        midsnaps += "<div id=\"mid_" + item.ProfileId + "\" style=\"height:333px;\" class=\"span4 rounder recpro\"><div class=\"concotop\">" +
                                               "<div class=\"userpictiny\"><img width=\"56\" height=\"56\" title=\"" + objInsAcc.InsUserName + "\" alt=\"\" src=\"" + objInsAcc.ProfileUrl + "\">" +
                                               "<a title=\"\" class=\"userurlpic\" href=\"#\"><img alt=\"\" src=\"../Contents/img/instagram_24X24.png\" width=\"16\" height=\"16\"></a></div>" +
                                               "<div class=\"useraccname\">" + objInsAcc.InsUserName + "</div></div>" +
                                               "<div class=\"concoteng\"><div class=\"pillow_fade\">" +
                                               " <div class=\"fb_notifications\">" +
                                               "<ul class=\"user-stats\"> " +
                                                    "<li><div class=\"photo_stat\">  photos</div>  <div class=\"number-stat\">" + objInsAcc.TotalImages + "</div></li>" +
                                                    "<li><div class=\"photo_stat\">following</div><div class=\"number-stat\">" + objInsAcc.Followers + "</div></li>" +
                                                    "<li><div class=\"photo_stat\">followers</div><div class=\"number-stat\">" + objInsAcc.FollowedBy + "</div></li>" +
                                                "</ul></div></div></div></div>";
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        logger.Error(ex.Message);
                                    }
                                    //       midsnaps += " <div class=\"col_seven_instagram\">" +
                                    //                "<div class=\"col_seven_instagram_link_my_accounts\">" +
                                    //                  "<div class=\"dt\"><a class=\"img\">" +
                                    //                    "<img src=\"" + objInsAcc.ProfileUrl + "\" width=\"48\" height=\"48\" alt=\"\" /></a>" +
                                    //                  "<span class=\"icon\"></span></div><div class=\"dd\">" +
                                    //                "<h5>" + objInsAcc.InsUserName + "</h5><div class=\"friends_avg\">" +

                                    //        "</div>" +
                                    //    "</div>" +
                                    //"</div>" +
                                    //    "<div class=\"pillow_fade\">" +
                                    //        "<div class=\"fb_notifications\">" +
                                    //            "<ul class=\"user-stats\">" +
                                    //                       "<li>" +
                                    //                          "<div class=\"photo_stat\">  photos" +
                                    //                          "</div>" +
                                    //                          "<div class=\"number-stat\">" + objInsAcc.TotalImages +
                                    //                          "</div>" +
                                    //                       "</li>" +
                                    //                       "<li>" +
                                    //                           "<div class=\"photo_stat\"> followers" +
                                    //                          "</div>" +
                                    //                          "<div class=\"number-stat\">" + objInsAcc.FollowedBy +
                                    //                          "</div>" +
                                    //                       "</li>" +
                                    //                       "<li>" +
                                    //                           "<div class=\"photo_stat\"> following" +
                                    //                          "</div>" +
                                    //                          "<div class=\"number-stat\">" + objInsAcc.Followers +
                                    //                          "</div>" +
                                    //                       "</li>" +
                                    //                    "</ul>" +
                                    //            "</div>" +
                                    //    "</div>" +
                                    //"</div>";
                                }
                                else if (item.ProfileType == "googleanalytics")
                                {
                                    try
                                    {
                                        GoogleAnalyticsAccountRepository objGaAccRepo = new GoogleAnalyticsAccountRepository();
                                        GoogleAnalyticsAccount objGaAcc = objGaAccRepo.getGoogelAnalyticsAccountHomeDetailsById(user.Id, item.ProfileId);
                                        ArrayList arrlist = objGaAccRepo.getGoogelAnalyticsProfilesOfUser(item.ProfileId, user.Id);

                                        midsnaps += "<div class=\"span4 rounder recpro\" style=\"height:333px;\"><div class=\"col_five_ga_my_accounts\"><div class=\"dt\">" +
                                            "<a class=\"img\"><img src=\"../Contents/img/google_analytics.png\" width=\"48\" height=\"48\" alt=\"\" /></a></div>" +
                                        "<div class=\"dd\"><div class=\"useraccname\" style=\"margin-left:0px;\">" + objGaAcc.GaAccountName + "</div><div class=\"friends_avg\"><div class=\"ga_content\">Google Analytics</div></div></div></div>" +
                                    "<div class=\"pillow_fade\">" +
                                      "<div class=\"ga_trafic_number_graph\"><div class=\"ga_trafic\">Email</div><div class=\"gah_eml\">" + objGaAcc.EmailId + "</div><div class=\"ga_graph\"></div></div>" +
                                      "<div class=\"ga_trafic_number_graph\"><div class=\"ga_trafic\">No of Profile</div><div class=\"ga_number\">" + arrlist.Count + "</div><div class=\"ga_graph\"></div></div></div></div>";
                                        //"<div class=\"ga_trafic_number_graph\"><div class=\"ga_trafic\">Day</div><div class=\"ga_number\">" + dayvisit + "</div><div class=\"ga_graph\"></div></div></div></div>";
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        logger.Error(ex.Message);
                                    }
                                }
                            }
                            midsnaps += "</div>";
                            Response.Write(midsnaps);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }


                }
                else if (Request.QueryString["op"] == "accountdelete")
                {
                    SocialProfilesRepository socioprofilerepo = new SocialProfilesRepository();
                    string Profiletype = Request.QueryString["profile"];
                    string profileid = Request.QueryString["profileid"];

                    if (Profiletype == "fb")
                    {
                        try
                        {
                            FacebookAccountRepository fbrepo = new FacebookAccountRepository();
                            int delacc = fbrepo.deleteFacebookUser(profileid, user.Id);
                            if (delacc != 0)
                            {

                                socioprofilerepo.deleteProfile(user.Id, profileid);
                                List<SocialProfile> lstsocioprofile = socioprofilerepo.checkProfileExistsMoreThanOne(profileid);
                                if (lstsocioprofile.Count == 0)
                                {
                                    FacebookMessageRepository fbmsgrepo = new FacebookMessageRepository();
                                    fbmsgrepo.deleteAllMessagesOfUser(profileid, user.Id);
                                    FacebookFeedRepository fbfeedrepo = new FacebookFeedRepository();
                                    fbfeedrepo.deleteAllFeedsOfUser(profileid, user.Id);
                                    FacebookStatsRepository fbstatsrepo = new FacebookStatsRepository();
                                    fbstatsrepo.deleteFacebookStats(profileid, user.Id);
                                }
                            }

                        }
                        catch (Exception exx)
                        {
                            Console.WriteLine(exx.Message);
                            logger.Error(exx.Message);
                        }
                    }
                    else if (Profiletype == "twt")
                    {
                        try
                        {
                            TwitterAccountRepository twtaccountrepo = new TwitterAccountRepository();
                            int deltwtacc = twtaccountrepo.deleteTwitterUser(user.Id, profileid);
                            if (deltwtacc != 0)
                            {
                                socioprofilerepo.deleteProfile(user.Id, profileid);
                                List<SocialProfile> lstsocioprofile = socioprofilerepo.checkProfileExistsMoreThanOne(profileid);
                                if (lstsocioprofile.Count == 0)
                                {
                                    TwitterMessageRepository twtmsgrepo = new TwitterMessageRepository();
                                    int d = twtmsgrepo.deleteTwitterMessage(profileid, user.Id);
                                    TwitterFeedRepository twtfeedrepo = new TwitterFeedRepository();
                                    int s = twtfeedrepo.deleteTwitterFeed(profileid, user.Id);
                                    TwitterStatsRepository twtstatsrepo = new TwitterStatsRepository();
                                    int a = twtstatsrepo.deleteTwitterStats(user.Id, profileid);
                                    TwitterDirectMessageRepository twtdirectmsgrepo = new TwitterDirectMessageRepository();
                                    int f = twtdirectmsgrepo.deleteDirectMessage(user.Id, profileid);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else if (Profiletype == "linkedin")
                    {
                        try
                        {
                            LinkedInAccountRepository linkedaccrepo = new LinkedInAccountRepository();
                            int dellinacc = linkedaccrepo.deleteLinkedinUser(profileid, user.Id);
                            if (dellinacc != 0)
                            {


                                socioprofilerepo.deleteProfile(user.Id, profileid);

                                List<SocialProfile> lstsocioprofile = socioprofilerepo.checkProfileExistsMoreThanOne(profileid);

                                if (lstsocioprofile.Count == 0)
                                {
                                    LinkedInFeedRepository linkedfeedrepo = new LinkedInFeedRepository();
                                    int s = linkedfeedrepo.deleteAllFeedsOfUser(profileid, user.Id);

                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }


                    }
                    else if (Profiletype == "instagram")
                    {
                        try
                        {
                            InstagramAccountRepository insaccrepo = new InstagramAccountRepository();
                            int insacc = insaccrepo.deleteInstagramUser(profileid, user.Id);
                            if (insacc != 0)
                            {


                                socioprofilerepo.deleteProfile(user.Id, profileid);

                                List<SocialProfile> lstsocioprofile = socioprofilerepo.checkProfileExistsMoreThanOne(profileid);

                                if (lstsocioprofile.Count == 0)
                                { }

                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);

                        }
                    }
                    else if (Profiletype == "googleplus")
                    {
                        try
                        {
                            GooglePlusAccountRepository googleplusaccrepo = new GooglePlusAccountRepository();
                            int gplusacc = googleplusaccrepo.deleteGooglePlusUser(profileid, user.Id);
                            if (gplusacc != 0)
                            {

                                socioprofilerepo.deleteProfile(user.Id, profileid);

                                List<SocialProfile> lstsocioprofile = socioprofilerepo.checkProfileExistsMoreThanOne(profileid);

                                if (lstsocioprofile.Count == 0)
                                {
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);

                        }
                    }

                }
                else if (Request.QueryString["op"] == "MasterComposeLetsTalk")
                {
                    string profiles = string.Empty;
                    if (Session["profilesforcomposemessage"] == null)
                    {
                        FacebookAccountRepository fbrepo = new FacebookAccountRepository();
                        ArrayList lstfbaccounts = fbrepo.getAllFacebookAccountsOfUser(user.Id);

                     //   profiles += "<div class=\"drop_top\"></div><div class=\"drop_mid\">";
                        
                        foreach (FacebookAccount item in lstfbaccounts)
                        {
                            try
                            {
                                profiles += "<div class=\"teitter\">" +
                                                                           "<ul>" +
                                                                            "   <li id=\"liid_" + item.FbUserId + "\" onclick=\"composemessage(this.id,'fb')\">" +
                                                                                   "<a >" +
                                                                                       "<img width=\"18\" border=\"none\" src=\"../Contents/img/facebook.png\" alt=\"\" style=\"float:left;\">" +
                                                                                       "<span id=\"composename_"+item.FbUserId +"\" style=\"float:left;margin: 3px 0 0 5px;\">" + item.FbUserName + "</span>" +
                                                                                   "</a>" +
                                                                               "</li>" +
                                                                           "</ul>" +
                                                                           "</div>";
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        TwitterAccountRepository twtaccountrepo = new TwitterAccountRepository();
                        ArrayList alsttwtaccounts = twtaccountrepo.getAllTwitterAccountsOfUser(user.Id);
                        foreach (TwitterAccount item in alsttwtaccounts)
                        {

                            try
                            {
                                profiles += "<div class=\"teitter\">" +
                                    "<ul>" +
                                    "   <li id=\"liid_" + item.TwitterUserId + "\" onclick=\"composemessage(this.id,'twt')\">" +
                                            "<a >" +
                                                "<img width=\"18\" border=\"none\" src=\"../Contents/img/twitter.png\" alt=\"\" style=\"float:left;\">" +
                                                "<span id=\"composename_" + item.TwitterUserId + "\" style=\"float:left;margin: 3px 0 0 5px;\">" + item.TwitterScreenName + "</span>" +
                                            "</a>" +
                                        "</li>" +
                                    "</ul>" +
                                    "</div>";
                            }
                            catch (Exception ex)
                            {

                            }



                        }
                       // profiles += "</div>";
                        LinkedInAccountRepository linkaccountrepo = new LinkedInAccountRepository();
                        ArrayList alstlinkacc = linkaccountrepo.getAllLinkedinAccountsOfUser(user.Id);
                        foreach (LinkedInAccount item in alstlinkacc)
                        {
                            try
                            {
                                profiles += "<div class=\"teitter\">" +
                                    "<ul>" +
                                    "   <li id=\"liid_" + item.LinkedinUserId + "\" onclick=\"composemessage(this.id,'lin')\">" +
                                            "<a >" +
                                                "<img width=\"18\" border=\"none\" src=\"../Contents/img/link.png\" alt=\"\" style=\"float:left;\">" +
                                                "<span id=\"composename_" + item.LinkedinUserId + "\" style=\"float:left;margin: 3px 0 0 5px;\">" + item.LinkedinUserName + "</span>" +
                                            "</a>" +
                                        "</li>" +
                                    "</ul>" +
                                    "</div>";
                            }
                            catch (Exception ex)
                            {

                            }
                           
                        
                        }
                        Session["profilesforcomposemessage"] = profiles;
                    }
                    else
                    {
                        profiles = (string)Session["profilesforcomposemessage"];
                    }
                    Response.Write(profiles);
                }
                else if (Request.QueryString["op"] == "MasterCompose")
                {
                    string profiles = string.Empty;

                    if (Session["profilesforcomposemessage"] == null)
                    {
                        profiles += "<div class=\"drop_top\"></div><div class=\"drop_mid\">";

                        /*facebook users binding*/
                        FacebookAccountRepository fbrepo = new FacebookAccountRepository();
                        ArrayList lstfbaccounts = fbrepo.getAllFacebookAccountsOfUser(user.Id);


                        profiles += "<div class=\"twitte_text\">FACEBOOK</div><div class=\"teitter\"><ul>";

                        if (lstfbaccounts.Count == 0)
                        {
                            profiles += "<li>No Records Found</li>";
                        }
                        else
                        {
                            foreach (FacebookAccount fbacc in lstfbaccounts)
                            {
                                profiles += "<li id=\"liid_" + fbacc.FbUserId + "\"   onclick=\"composemessage(this.id,'fb')\"><a><img id=\"img_" + fbacc.FbUserId + "\" src=\"../Contents/img/facebook.png\" alt=\"" + fbacc.AccessToken + "\" border=\"none\" width=\"18\" style=\"float:left;\" /><span id=\"composename_" + fbacc.FbUserId + "\" style=\"float:left;margin: 3px 0 0 5px;\">" + fbacc.FbUserName + "</span><span id=\"imgurl_" + fbacc.FbUserId + "\" style=\"display:none;\">http://graph.facebook.com/" + fbacc.FbUserId + "/picture?type=small</span></a></li>";
                            }
                        }
                        profiles += "</ul><div>";


                        /*twitter users binding*/
                        TwitterAccountRepository twtaccountrepo = new TwitterAccountRepository();
                        ArrayList alsttwtaccounts = twtaccountrepo.getAllTwitterAccountsOfUser(user.Id);
                        profiles += "<div class=\"twitte_text\">TWITTER</div><div class=\"teitter\"><ul>";

                        if (alsttwtaccounts.Count == 0)
                        {
                            profiles += "<li>No Records Found</li>";
                        }
                        else
                        {
                            foreach (SocioBoard.Domain.TwitterAccount item in alsttwtaccounts)
                            {
                                profiles += "<li id=\"liid_" + item.TwitterUserId + "\"   onclick=\"composemessage(this.id,'twt')\"><a><img id=\"img_" + item.TwitterUserId + "\" src=\"../Contents/img/twitter.png\" alt=\"" + item.OAuthToken + "\" border=\"none\" width=\"18\" style=\"float:left;\" /><span id=\"imgurl_" + item.TwitterUserId + "\" style=\"display:none;\">" + item.ProfileImageUrl + "</span><span id=\"composename_" + item.TwitterUserId + "\" style=\"float:left;margin: 3px 0 0 5px;\">" + item.TwitterScreenName + "</span></a></li>";
                            }
                        }
                        profiles += "</ul> </div>";


                        /*linkedinuserbinding*/
                        LinkedInAccountRepository linkaccountrepo = new LinkedInAccountRepository();
                        ArrayList alstlinkacc = linkaccountrepo.getAllLinkedinAccountsOfUser(user.Id);
                        profiles += "<div class=\"twitte_text\">LINKEDIN</div><div class=\"teitter\"><ul>";

                        if (alstlinkacc.Count == 0)
                        {
                            profiles += "<li>No Records Found</li>";
                        }
                        else
                        {

                            foreach (LinkedInAccount item in alstlinkacc)
                            {
                                string profileurl = string.Empty;

                                if (!string.IsNullOrEmpty(item.ProfileUrl))
                                {
                                    profileurl = item.ProfileUrl;
                                }
                                else
                                {
                                    profileurl = "../../Contents/img/blank_img.png";
                                }
                                profiles += "<li id=\"liid_" + item.LinkedinUserId + "\"   onclick=\"composemessage(this.id,'lin')\"><a><img id=\"img_" + item.LinkedinUserId + "\" src=\"../Contents/img/link.png\" alt=\"" + item.OAuthToken + "\" border=\"none\" width=\"18\" style=\"float:left;\" /><span id=\"imgurl_" + item.LinkedinUserId + "\" style=\"display:none;\">" + profileurl + "</span><span id=\"composename_" + item.LinkedinUserId + "\" style=\"float:left;margin: 3px 0 0 5px;\">" + item.LinkedinUserName + "</span></a></li>";
                            }
                        }
                        profiles += "</ul> </div>";
                        Session["profilesforcomposemessage"] = profiles;
                    }
                    else
                    {
                        profiles = (string)Session["profilesforcomposemessage"];
                    }
                    Response.Write(profiles);
                }
                else if (Request.QueryString["op"] == "sendmessage")
                {

                    string message = Request.QueryString["message"];
                    var userid = Request.QueryString["userid[]"].Split(',');
                    var files = Request.Files.Count;
                    var fi = Request.Files["file"];
                    string file = string.Empty;
                    try
                    {

                        if (Request.Files.Count > 0)
                        {
                            if (fi != null)
                            {
                                var path = Server.MapPath("~/Contents/img/upload");
                                file = path + "/" + fi.FileName;
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                fi.SaveAs(file);
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                        logger.Error(ex.Message);

                    }


                    foreach (var item in userid)
                    {
                        string[] networkingwithid = item.Split('_');
                        if (networkingwithid[0] == "fb")
                        {
                            try
                            {
                                FacebookAccountRepository fbaccountrepo = new FacebookAccountRepository();
                                FacebookAccount fbaccount = fbaccountrepo.getFacebookAccountDetailsById(networkingwithid[1], user.Id);
                                var args = new Dictionary<string, object>();

                                args["message"] = message;

                                if (Request.Files.Count > 0)
                                {
                                    string strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
                                    string strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
                                    string filepath = strUrl + "/Contents/img/upload/" + fi.FileName;
                                    args["picture"] = filepath;
                                }
                                FacebookClient fc = new FacebookClient(fbaccount.AccessToken);

                                if (fbaccount.Type == "page")
                                {
                                    var fbPagepost = fc.Post("/" + fbaccount.FbUserId + "/feed", args);
                                    if (fbPagepost.ToString() != string.Empty)
                                    {
                                        Response.Write("Succesfully posted");
                                    }
                                    else
                                    {
                                        Response.Write("Not posted");
                                    }
                                }
                                else if (fbaccount.Type == "account")
                                {

                                    var facebookpost = fc.Post("/me/feed", args);
                                    if (facebookpost.ToString() != string.Empty)
                                    {
                                        Response.Write("Succesfully posted");
                                    }
                                    else
                                    {
                                        Response.Write("Not posted");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);

                                Console.WriteLine(ex.Message);
                            }

                        }
                        else if (networkingwithid[0] == "twt")
                        {
                            try
                            {
                                TwitterAccountRepository twtaccountrepo = new TwitterAccountRepository();
                                SocioBoard.Domain.TwitterAccount twtaccount = twtaccountrepo.getUserInformation(user.Id, networkingwithid[1]);

                                TwitterHelper twthelper = new TwitterHelper();

                                oAuthTwitter OAuthTwt = new oAuthTwitter();
                                OAuthTwt.AccessToken = twtaccount.OAuthToken;
                                OAuthTwt.AccessTokenSecret = twtaccount.OAuthSecret;
                                OAuthTwt.TwitterScreenName = twtaccount.TwitterScreenName;
                                OAuthTwt.TwitterUserId = twtaccount.TwitterUserId;
                                twthelper.SetCofigDetailsForTwitter(OAuthTwt);
                                Tweet twt = new Tweet();
                                if (Request.Files.Count > 0)
                                {
                                    PhotoUpload ph = new PhotoUpload();
                                    ph.Tweet(file, message, OAuthTwt);
                                }
                                else
                                {
                                    JArray post = twt.Post_Statuses_Update(OAuthTwt, message);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                logger.Error(ex.Message);

                            }
                        }
                        else if (networkingwithid[0] == "lin")
                        {
                            try
                            {

                                string linkedinId = string.Empty;
                                if (networkingwithid.Count() > 2)
                                {
                                    LinkedInAccountRepository linkedinaccrepo = new LinkedInAccountRepository();
                                    LinkedInAccount linkedaccount = linkedinaccrepo.getUserInformation(user.Id, networkingwithid[1]+"_"+networkingwithid[2]);
                                    oAuthLinkedIn Linkedin_oauth = new oAuthLinkedIn();
                                    Linkedin_oauth.Verifier = linkedaccount.OAuthVerifier;
                                    Linkedin_oauth.TokenSecret = linkedaccount.OAuthSecret;
                                    Linkedin_oauth.Token = linkedaccount.OAuthToken;
                                    Linkedin_oauth.Id = linkedaccount.LinkedinUserId;
                                    Linkedin_oauth.FirstName = linkedaccount.LinkedinUserName;
                                    SocialStream sociostream = new SocialStream();
                                    string res = sociostream.SetStatusUpdate(Linkedin_oauth, message);
                                }
                                else
                                {
                                    LinkedInAccountRepository linkedinaccrepo = new LinkedInAccountRepository();
                                    LinkedInAccount linkedaccount = linkedinaccrepo.getUserInformation(user.Id, networkingwithid[1]);
                                    oAuthLinkedIn Linkedin_oauth = new oAuthLinkedIn();
                                    Linkedin_oauth.Verifier = linkedaccount.OAuthVerifier;
                                    Linkedin_oauth.TokenSecret = linkedaccount.OAuthSecret;
                                    Linkedin_oauth.Token = linkedaccount.OAuthToken;
                                    Linkedin_oauth.Id = linkedaccount.LinkedinUserId;
                                    Linkedin_oauth.FirstName = linkedaccount.LinkedinUserName;
                                    SocialStream sociostream = new SocialStream();
                                    string res = sociostream.SetStatusUpdate(Linkedin_oauth, message);
                                
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                logger.Error(ex.Message);

                            }
                        }
                    }
                }
                else if (Request.QueryString["op"] == "wooqueue_messages")
                {
                    ScheduledMessageRepository schmsgrepo = new ScheduledMessageRepository();
                }
                else if (Request.QueryString["op"] == "schedulemessage")
                {

                    var userid = Request.QueryString["users[]"].Split(',');
                    var datearr = Request.QueryString["datearr[]"].Split(',');
                    string message = Request.QueryString["message"];
                    ScheduledMessageRepository schmsgrepo = new ScheduledMessageRepository();
                    string time = Request.QueryString["time"];
                    string clienttime = Request.QueryString["clittime"];


                    foreach (var item in userid)
                    {
                        if (!string.IsNullOrEmpty(item.ToString()))
                        {
                            foreach (var child in datearr)
                            {


                                ScheduledMessage schmessage = new ScheduledMessage();
                                string[] networkingwithid = item.Split('_');

                                if (networkingwithid[0] == "fbscheduler")
                                {
                                    schmessage.CreateTime = DateTime.Now;
                                    schmessage.ProfileType = "facebook";
                                    schmessage.ProfileId = networkingwithid[1];
                                    schmessage.Id = Guid.NewGuid();
                                    schmessage.PicUrl = string.Empty;
                                    schmessage.ClientTime = Convert.ToDateTime(clienttime);
                                    string servertime = this.CompareDateWithServer(clienttime, child + " " + time);
                                    schmessage.ScheduleTime = Convert.ToDateTime(servertime);
                                    schmessage.ShareMessage = message;
                                    schmessage.UserId = user.Id;
                                    schmessage.Status = false;

                                }
                                else if (networkingwithid[0] == "twtscheduler")
                                {

                                    schmessage.CreateTime = DateTime.Now;
                                    schmessage.ProfileType = "twitter";
                                    schmessage.ProfileId = networkingwithid[1];
                                    schmessage.Id = Guid.NewGuid();
                                    schmessage.PicUrl = string.Empty;
                                    schmessage.ClientTime = Convert.ToDateTime(clienttime);
                                    string servertime = this.CompareDateWithServer(clienttime, child + " " + time);
                                    schmessage.ScheduleTime = Convert.ToDateTime(servertime);
                                    schmessage.ShareMessage = message;
                                    schmessage.UserId = user.Id;
                                    schmessage.Status = false;

                                }
                                else if (networkingwithid[0] == "linscheduler")
                                {
                                    schmessage.ClientTime = Convert.ToDateTime(clienttime);
                                    schmessage.CreateTime = DateTime.Now;
                                    schmessage.ProfileType = "linkedin";
                                    schmessage.ProfileId = networkingwithid[1];
                                    schmessage.Id = Guid.NewGuid();
                                    schmessage.PicUrl = string.Empty;
                                    string servertime = this.CompareDateWithServer(clienttime, child + " " + time);
                                    schmessage.ScheduleTime = Convert.ToDateTime(servertime);
                                    schmessage.ShareMessage = message;
                                    schmessage.UserId = user.Id;
                                    schmessage.Status = false;

                                }
                                if (!string.IsNullOrEmpty(message))
                                {
                                    if (!schmsgrepo.checkMessageExistsAtTime(user.Id, schmessage.ShareMessage, schmessage.ScheduleTime, schmessage.ProfileId))
                                    {
                                        schmsgrepo.addNewMessage(schmessage);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (Request.QueryString["op"] == "insight")
                {
                    FacebookAccount objFacebookAccount = (FacebookAccount)Session["fbpagedetail"];
                    FacebookHelper objFbHelper = new FacebookHelper();
                    SocialProfile socioprofile = new SocialProfile();
                    SocialProfilesRepository socioprofilerepo = new SocialProfilesRepository();
                    FacebookAccountRepository fbrepo = new FacebookAccountRepository();
                    FacebookClient fbClient = new FacebookClient(objFacebookAccount.AccessToken);
                    int fancountPage = 0;
                    dynamic fancount = fbClient.Get("fql", new { q = " SELECT fan_count FROM page WHERE page_id =" + Request["id"].ToString() });
                    foreach (var friend in fancount.data)
                    {
                        fancountPage = Convert.ToInt32(friend.fan_count);
                    }
                    objFacebookAccount.Friends = Convert.ToInt32(fancountPage);
                    objFacebookAccount.FbUserId = Request["id"].ToString();
                    objFacebookAccount.FbUserName = Request["name"].ToString();
                    objFacebookAccount.Type = "page";
                    objFacebookAccount.UserId = user.Id;
                    socioprofile.Id = Guid.NewGuid();
                    socioprofile.ProfileDate = DateTime.Now;
                    socioprofile.ProfileId = Request["id"].ToString();
                    socioprofile.ProfileStatus = 1;
                    socioprofile.ProfileType = "facebook";
                    socioprofile.UserId = user.Id;
                    if (!fbrepo.checkFacebookUserExists(objFacebookAccount.FbUserId, user.Id))
                    {
                        fbrepo.addFacebookUser(objFacebookAccount);
                        if (!socioprofilerepo.checkUserProfileExist(socioprofile))
                        {
                            socioprofilerepo.addNewProfileForUser(socioprofile);
                        }
                        else
                        {
                            socioprofilerepo.updateSocialProfile(socioprofile);
                        }
                    }
                    else
                    {
                        fbrepo.updateFacebookUser(objFacebookAccount);
                        if (!socioprofilerepo.checkUserProfileExist(socioprofile))
                        {
                            socioprofilerepo.addNewProfileForUser(socioprofile);
                        }
                        else
                        {
                            socioprofilerepo.updateSocialProfile(socioprofile);
                        }
                    }
                    Session["fbSocial"] = null;
                }
                else if (Request.QueryString["op"] == "countmessages")
                {
                    try
                    {
                        int val = 0;
                        /*facebook*/
                        FacebookAccountRepository fbAccoutsRepo = new FacebookAccountRepository();
                        ArrayList lstfacebookAccounts = fbAccoutsRepo.getAllFacebookAccountsOfUser(user.Id);
                        foreach (FacebookAccount item in lstfacebookAccounts)
                        {
                            try
                            {
                                FacebookClient fb = new FacebookClient(item.AccessToken);
                                dynamic unreadcount = fb.Get("fql", new { q = "SELECT unread_count FROM mailbox_folder WHERE folder_id = 0 AND viewer_id = " + item.FbUserId + "" });
                                foreach (var chile in unreadcount.data)
                                {
                                    var count = chile.unread_count;
                                    int countable = Convert.ToInt32(count.ToString());
                                    val = val + countable;

                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                                Console.WriteLine(ex.Message);
                            }
                        }
                        /*Twitter*/
                        Session["CountMessages"] = val;
                        Response.Write(val);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                else if (Request.QueryString["op"] == "recentfollowers")
                {
                    string recentfollowers = string.Empty;
                    #region RecentFollowers
                    Users twtUsers = new Users();
                    TwitterAccountRepository twtAccRepo = new TwitterAccountRepository();
                    ArrayList lstAccRepo = twtAccRepo.getAllTwitterAccountsOfUser(user.Id);
                    oAuthTwitter oauth = null;
                    foreach (TwitterAccount itemTwt in lstAccRepo)
                    {
                        oauth = new oAuthTwitter();
                        oauth.AccessToken = itemTwt.OAuthToken;
                        oauth.AccessTokenSecret = itemTwt.OAuthSecret;
                        oauth.ConsumerKey = ConfigurationManager.AppSettings["consumerKey"];
                        oauth.ConsumerKeySecret = ConfigurationManager.AppSettings["consumerSecret"];
                        oauth.TwitterScreenName = itemTwt.TwitterScreenName;
                        oauth.TwitterUserId = itemTwt.TwitterUserId;
                        JArray jarresponse = twtUsers.Get_Followers_ById(oauth, itemTwt.TwitterUserId);
                        foreach (var item in jarresponse)
                        {
                            int resposecount = 0;
                            if (item["ids"] != null)
                            {
                                foreach (var child in item["ids"])
                                {
                                    if (resposecount < 2)
                                    {
                                        JArray userprofile = twtUsers.Get_Users_LookUp(oauth, child.ToString());

                                        foreach (var items in userprofile)
                                        {
                                            resposecount++;
                                            try
                                            {
                                                recentfollowers += "<li><a href=\"#\"><img style=\"border:3px solid #FCFCFC;\" title=\"" + items["name"] + "\" width=\"48\" height=\"48\" alt=\"\" src=\"" + items["profile_image_url"] + "\"></a></li>";
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }

                    Response.Write(recentfollowers);

                    #endregion

                }
                else if (Request.QueryString["op"] == "removefollowers")
                {
                    string removeid = Request.QueryString["removeid"];
                    string userid = Request.QueryString["userid"];
                    Friendship friendship = new Friendship();
                    oAuthTwitter oauth = new oAuthTwitter();
                    TwitterAccountRepository twtaccrepo = new TwitterAccountRepository();
                    TwitterAccount twtAccount = twtaccrepo.getUserInformation(user.Id, userid);
                    oauth.TwitterUserId = twtAccount.TwitterUserId;
                    oauth.TwitterScreenName = twtAccount.TwitterScreenName;
                    oauth.ConsumerKey = ConfigurationManager.AppSettings["consumerKey"];
                    oauth.ConsumerKeySecret = ConfigurationManager.AppSettings["consumerSecret"];
                    oauth.AccessToken = twtAccount.OAuthToken;
                    oauth.AccessTokenSecret = twtAccount.OAuthSecret;
                    JArray responseremove = friendship.Post_Friendship_Destroy(oauth, removeid);

                }
                else if (Request.QueryString["op"] == "wooqueuemessages")
                {
                    ScheduledMessageRepository schmsgRepo = new ScheduledMessageRepository();
                    List<ScheduledMessage> lstschMsg = schmsgRepo.getAllMessagesOfUser(user.Id);
                    string schmessages = string.Empty;
                    string profurl = string.Empty;
                    if (string.IsNullOrEmpty(user.ProfileUrl))
                    {
                        profurl = "../Contents/img/blank_img.png";
                    }
                    else
                    {
                        // profurl = "../Contents/img/blank_img.png";
                        profurl = user.ProfileUrl;
                    }
                    if (lstschMsg.Count != 0)
                    {
                        foreach (ScheduledMessage item in lstschMsg)
                        {

                            schmessages += "<section class=\"section\"><div  class=\"js-task-cont read\">" +
                                                     "<section class=\"task-owner\">" +
                                                         "<img width=\"32\" height=\"32\" border=\"0\" src=\"" + profurl + "\" class=\"avatar\">" +
                                                     "</section>" +
                                                     "<section class=\"task-activity third\" style=\"width:19%;\">" +
                                                         "<p>" + user.UserName + "</p>" +
                                                         "<div>" + item.CreateTime + "</div>" +
                                                         "<input type=\"hidden\" value=\"#\" id=\"hdntaskid_1\">" +
                                                         "<p></p>" +
                                                   "</section>" +
                                                   "<section class=\"task-message font-13 third\" style=\"margin-right: 6px; height: auto; width: 23%;\"><a id=\"woomsg_" + item.Id + "\" class=\"tip_left\">" + item.ShareMessage + "</a></section>";

                            if (item.ProfileType == "facebook")
                            {
                                schmessages += "<div style=\"height:70px; margin-top: 0;\" class=\"userpictiny\">" +
                                                    "<img id=\"img_" + item.Id + "\" width=\"48\" height=\"48\" src=\"http://graph.facebook.com/" + item.ProfileId + "/picture?type=small\" alt=\"fb_" + item.ProfileId + "\">" +
                                                    "<a style=\"right: 2px; top: 40px;\" title=\"\" class=\"userurlpic\" href=\"#\">" +
                                                        "<img  alt=\"\" src=\"../Contents/img/facebook.png\" style=\"height: 16px;width: 16x;\"></a></div>";
                            }
                            else if (item.ProfileType == "twitter")
                            {
                                TwitterAccountRepository twtAccRepo = new TwitterAccountRepository();
                                TwitterAccount twtAccount = twtAccRepo.getUserInformation(user.Id, item.ProfileId);
                                schmessages += "<div style=\"height:70px; margin-top: 0;\" class=\"userpictiny\">" +
                                                "<img id=\"img_" + item.Id + "\" width=\"48\" height=\"48\" src=\"" + twtAccount.ProfileImageUrl + "\" alt=\"twt_" + item.ProfileId + "\">" +
                                                "<a style=\"right: 2px; top: 40px;\" title=\"\" class=\"userurlpic\" href=\"#\">" +
                                                    "<img  alt=\"\" src=\"../Contents/img/twitter.png\" style=\"height: 16px;width: 16x;\"></a></div>";
                            }
                            else if (item.ProfileType == "linkedin")
                            {
                                LinkedInAccountRepository linkAccRepo = new LinkedInAccountRepository();
                                LinkedInAccount linkedAccount = linkAccRepo.getUserInformation(user.Id, item.ProfileId);
                                schmessages += "<div style=\"height:70px; margin-top: 0;\" class=\"userpictiny\">" +
                                                    "<img id=\"img_" + item.Id + "\" width=\"48\" height=\"48\" src=\"" + linkedAccount.ProfileImageUrl + "\" alt=\"lin_" + item.ProfileId + "\">" +
                                                    "<a style=\"right: 2px; top: 40px;\" title=\"\" class=\"userurlpic\" href=\"#\">" +
                                                        "<img  alt=\"\" src=\"../Contents/img/link.png\" style=\"height: 16px;width: 16x;\"></a></div>";

                            }
                            schmessages += "<section class=\"task-status\" style=\"width: 113px; float: left; margin-top: 15px; margin-left: 14.4%;\">" +
                                //           "<span class=\"ficon task_active\" id=\"taskcomment\">" +
                                //// "<img width=\"14\" height=\"17\" alt=\"\" src=\"../Contents/img/task/task_pin.png\" onclick=\"getmemberdata('7fd5773f-c5b0-4624-bba1-b8a6c0fbd56d');\">" +
                                //          "</span>" +
                                          "<div class=\"ui_light floating task_status_change\">" +
                                               "<a href=\"#nogo\" class=\"ui-sproutmenu\">" +
                                                   "<span class=\"ui-sproutmenu-status\">" + item.Status +
                                // "<img title=\"Edit Status\" onclick=\"PerformClick(this.id)\" src=\"../Contents/img/icon_edit.png\" class=\"edit_button\" id=\"img_7fd5773f-c5b0-4624-bba1-b8a6c0fbd56d_True\">
                                                      "</span>" +
                                              "</a>" +
                                          "</div>" +
                                      "</section>" +
                                      "<span class=\"ui-sproutmenu-status\" onclick=\"editWooQueue('" + item.Id + "');\" style=\"margin-left:8px;\"><img title=\"Edit Status\" src=\"../Contents/img/icon_edit.png\" class=\"edit_button\"></span>" +
                                      "<a class=\"small_remove icon publish_delete\" title=\"Delete\" style=\"margin-left: 8px; top: 7px;\" onclick=\"deleteWooQueueMessage('" + item.Id + "')\" href=\"#\" ></a>" +
                                   "</div></section>";
                        }
                    }
                    else
                    {
                        schmessages = "<section class=\"section\"><div class=\"js-task-cont read\"><section class=\"task-owner\">" +
                          "<img width=\"32\" height=\"32\" border=\"0\" class=\"avatar\" src=\"" + profurl + "\">" +
                          "</section><section class=\"task-activity third\"><p>" + user.UserName + "</p><div></div><p></p></section><section style=\"margin-right: 6px; width: 40%; height: auto;\" class=\"task-message font-13 third\">" +
                          "<a class=\"tip_left\">No Schduled Messages</a></section><section style=\"width:113px;\" class=\"task-status\"><span class=\"ficon task_active\"></span>" +
                          "<div class=\"ui_light floating task_status_change\"><a class=\"ui-sproutmenu\" href=\"#nogo\">" +
                          "<span class=\"ui-sproutmenu-status\"></span></a></div></section></div></section>";
                    }
                    Response.Write(schmessages);

                }
                else if (Request.QueryString["op"] == "drafts_messages")
                {

                }
                else if (Request.QueryString["op"] == "getFbUsersForHome")
                {

                    string midsnaps = string.Empty;
                    FacebookAccountRepository facerepo = new FacebookAccountRepository();
                    ArrayList alst = facerepo.getAllFacebookAccountsOfUser(user.Id);
                    foreach (FacebookAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.FbUserId + "','fb')\" id=\"fb_" + item.FbUserId + "\"><img src=\"../Contents/img/fbicon_new.png\" alt=\"\" /><a>" + item.FbUserName + "</a></li>";
                    }
                    Response.Write(midsnaps);

                }
                else if (Request.QueryString["op"] == "getTwtUsersForHome")
                {
                    string midsnaps = string.Empty;
                    TwitterAccountRepository twtAccrepo = new TwitterAccountRepository();
                    ArrayList alst = twtAccrepo.getAllTwitterAccountsOfUser(user.Id);
                    foreach (TwitterAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.TwitterUserId + "','twt')\" id=\"twt_" + item.TwitterUserId + "\"><img src=\"../Contents/img/twittericon_new.png\" alt=\"\" /><a>" + item.TwitterScreenName + "</a></li>";
                    }
                    Response.Write(midsnaps);

                }
                else if (Request.QueryString["op"] == "getLinUsersForHome")
                {
                    string midsnaps = string.Empty;
                    LinkedInAccountRepository linkAccountRepo = new LinkedInAccountRepository();
                    ArrayList alst = linkAccountRepo.getAllLinkedinAccountsOfUser(user.Id);
                    foreach (LinkedInAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.LinkedinUserId + "','lin')\" id=\"lin," + item.LinkedinUserId + "\"><img style=\"height:18px;width:18px;\" src=\"../Contents/img/linked_25X24.png\" alt=\"\" /><a>" + item.LinkedinUserName + "</a></li>";
                    }
                    Response.Write(midsnaps);
                }
                else if (Request.QueryString["op"] == "getGplusUsersForHome")
                {

                    string midsnaps = string.Empty;
                    GooglePlusAccountRepository linkAccountRepo = new GooglePlusAccountRepository();
                    ArrayList alst = linkAccountRepo.getAllGooglePlusAccountsOfUser(user.Id);
                    foreach (GooglePlusAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.GpUserId + "','gplus')\" id=\"gplus_" + item.GpUserId + "\"><img style=\"height:18px;width:18px;\" src=\"../Contents/img/g+new.png\" alt=\"\" /><a>" + item.GpUserName + "</a></li>";
                    }
                    Response.Write(midsnaps);

                }


                else if (Request.QueryString["op"] == "getUserDetails")
                {
                    string messages = string.Empty;
                    string network = Request.QueryString["network"];
                    string userid = Request.QueryString["userid"];
                    messages += "<h3>Recent messages</h3>";
                    if (network == "fb")
                    {
                        FacebookAccountRepository fbRepo = new FacebookAccountRepository();
                        FacebookAccount fbAccount = fbRepo.getFacebookAccountDetailsById(userid, user.Id);
                        FacebookFeedRepository lstfbfeed = new FacebookFeedRepository();
                        List<FacebookFeed> lstfbmsg = lstfbfeed.getAllFacebookUserFeeds(userid);
                        messages += "<div style=\"display:none;\"><div id=\"fb_fans\">" + fbAccount.Friends + "</div></div>  ";
                        foreach (FacebookFeed item in lstfbmsg)
                        {
                            messages += "<span>" +
                                "<abbr>" + item.FeedDate + "</abbr>" +
                                "<i><img src=\"http://graph.facebook.com/" + item.FromId + "/picture?type=small\"></i>" +
                                "<h4><b>@" + item.FromName + "</b></h4>" +
                                "<p>" + item.FeedDescription + "</p>" +

                           " </span>";
                        }

                        // messages += "<ul id=\"fb_pagination\"></ul>";

                    }
                    else if (network == "twt")
                    {
                        TwitterAccountRepository twtAccRepo = new TwitterAccountRepository();
                        TwitterAccount TwtAccount = twtAccRepo.getUserInformation(user.Id, userid);
                        TwitterMessageRepository twtMsgRepo = new TwitterMessageRepository();
                        List<TwitterMessage> twtMsg = twtMsgRepo.getAllTwitterMessagesOfProfile(userid);
                        messages += "<div style=\"display:none;\"><div id=\"twt_fans\">" + TwtAccount.FollowersCount + "</div><div id=\"twt_imgurl\">" + TwtAccount.ProfileImageUrl + "</div></div> ";

                        foreach (TwitterMessage item in twtMsg)
                        {
                            messages += "<span>" +
                              "<abbr>" + item.MessageDate + "</abbr>" +
                              "<i><img src=\"" + item.FromProfileUrl + "\"></i>" +
                              "<h4><b>@" + item.FromScreenName + "</b></h4>" +
                              "<p>" + item.TwitterMsg + "</p>" +

                         " </span>";
                        }

                    }
                    else if (network == "lin")
                    {
                        LinkedInAccountRepository linkedin = new LinkedInAccountRepository();
                        LinkedInAccount linkAccount = linkedin.getUserInformation(user.Id, userid);

                        LinkedInFeedRepository linked = new LinkedInFeedRepository();
                        List<LinkedInFeed> lstLinked = linked.getAllLinkedInFeedsOfProfile(userid);
                        string profimg = string.Empty;
                        if (string.IsNullOrEmpty(linkAccount.ProfileImageUrl))
                        {
                            profimg = "../Contents/img/blank_user.png";
                        }
                        else
                        {
                            profimg = linkAccount.ProfileImageUrl;
                        }
                        messages += "<div style=\"display:none;\"><div id=\"lin_fans\">" + linkAccount.Connections + "</div><div id=\"twt_imgurl\">" + profimg + "</div></div> ";
                        foreach (LinkedInFeed item in lstLinked)
                        {
                            messages += "<span>" +
                               "<abbr>" + item.FeedsDate + "</abbr>" +
                               "<i><img style=\"height:58px;\" src=\"" + item.FromPicUrl + "\"></i>" +
                               "<h4><b>@" + item.FromName + "</b></h4>" +
                               "<p>" + item.Feeds + "</p>" +

                          " </span>";
                        }
                    }
                    else if (network == "gplus")
                    {

                        string midsnaps = string.Empty;
                        GooglePlusActivitiesRepository objActRepo = new GooglePlusActivitiesRepository();
                        List<GooglePlusActivities> lstmsggauser = objActRepo.getAllgoogleplusActivityOfUser(userid);
                       
                        foreach (GooglePlusActivities item in lstmsggauser)
                        {
                            messages += "<span>" +
                                "<abbr>" + item.PublishedDate + "</abbr>" +
                                "<i><img src=\"" + item.FromProfileImage + "\"></i>" +
                                "<h4><b>@" + item.FromUserName + "</b></h4>" +
                                "<p>" + item.Content + "</p>" +

                           " </span>";
                        }
                    }
                    Response.Write(messages);
                }
                else if (Request.QueryString["op"] == "getFbUsersForHome")
                {

                    string midsnaps = string.Empty;
                    FacebookAccountRepository facerepo = new FacebookAccountRepository();
                    ArrayList alst = facerepo.getAllFacebookAccountsOfUser(user.Id);
                    foreach (FacebookAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.FbUserId + "','fb')\" id=\"fb_" + item.FbUserId + "\"><img src=\"../Contents/img/fbicon_new.png\" alt=\"\" /><a>" + item.FbUserName + "</a></li>";
                    }
                    Response.Write(midsnaps);

                }
                else if (Request.QueryString["op"] == "getTwtUsersForHome")
                {
                    string midsnaps = string.Empty;
                    TwitterAccountRepository twtAccrepo = new TwitterAccountRepository();
                    ArrayList alst = twtAccrepo.getAllTwitterAccountsOfUser(user.Id);
                    foreach (TwitterAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.TwitterUserId + "','twt')\" id=\"twt_" + item.TwitterUserId + "\"><img src=\"../Contents/img/twittericon_new.png\" alt=\"\" /><a>" + item.TwitterScreenName + "</a></li>";
                    }
                    Response.Write(midsnaps);

                }
                else if (Request.QueryString["op"] == "getLinUsersForHome")
                {
                    string midsnaps = string.Empty;
                    LinkedInAccountRepository linkAccountRepo = new LinkedInAccountRepository();
                    ArrayList alst = linkAccountRepo.getAllLinkedinAccountsOfUser(user.Id);
                    foreach (LinkedInAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.LinkedinUserId + "','lin')\" id=\"lin," + item.LinkedinUserId + "\"><img  src=\"../Contents/img/linked_25X24.png\" alt=\"\" /><a>" + item.LinkedinUserName + "</a></li>";
                    }
                    Response.Write(midsnaps);
                }
                else if (Request.QueryString["op"] == "getGplusUsersForHome")
                {

                    string midsnaps = string.Empty;
                    GooglePlusAccountRepository linkAccountRepo = new GooglePlusAccountRepository();
                    ArrayList alst = linkAccountRepo.getAllGooglePlusAccountsOfUser(user.Id);
                    foreach (GooglePlusAccount item in alst)
                    {
                        midsnaps += "<li onclick=\"getUserDetailsforHome('" + item.GpUserId + "','gplus')\" id=\"gplus_" + item.GpUserId + "\"><img src=\"../Contents/img/google_plus.png\" alt=\"\" /><a>" + item.GpUserName + "</a></li>";
                    }
                    Response.Write(midsnaps);

                }


                else if (Request.QueryString["op"] == "getUserDetails")
                {
                    string messages = string.Empty;
                    string network = Request.QueryString["network"];
                    string userid = Request.QueryString["userid"];
                    messages += "<h3>Recent messages</h3>";
                    if (network == "fb")
                    {
                        FacebookAccountRepository fbRepo = new FacebookAccountRepository();
                        FacebookAccount fbAccount = fbRepo.getFacebookAccountDetailsById(userid, user.Id);
                        FacebookFeedRepository lstfbfeed = new FacebookFeedRepository();
                        List<FacebookFeed> lstfbmsg = lstfbfeed.getAllFacebookUserFeeds(userid);
                        messages += "<div style=\"display:none;\"><div id=\"fb_fans\">" + fbAccount.Friends + "</div></div>  ";
                        foreach (FacebookFeed item in lstfbmsg)
                        {
                            messages += "<span>" +
                                "<abbr>" + item.FeedDate + "</abbr>" +
                                "<i><img src=\"http://graph.facebook.com/" + item.FromId + "/picture?type=small\"></i>" +
                                "<h4><b>@" + item.FromName + "</b></h4>" +
                                "<p>" + item.FeedDescription + "</p>" +

                           " </span>";
                        }

                        // messages += "<ul id=\"fb_pagination\"></ul>";

                    }
                    else if (network == "twt")
                    {
                        TwitterAccountRepository twtAccRepo = new TwitterAccountRepository();
                        TwitterAccount TwtAccount = twtAccRepo.getUserInformation(user.Id, userid);
                        TwitterMessageRepository twtMsgRepo = new TwitterMessageRepository();
                        List<TwitterMessage> twtMsg = twtMsgRepo.getAllTwitterMessagesOfProfile(userid);
                        messages += "<div style=\"display:none;\"><div id=\"twt_fans\">" + TwtAccount.FollowersCount + "</div><div id=\"twt_imgurl\">" + TwtAccount.ProfileImageUrl + "</div></div> ";

                        foreach (TwitterMessage item in twtMsg)
                        {
                            messages += "<span>" +
                              "<abbr>" + item.MessageDate + "</abbr>" +
                              "<i><img src=\"" + item.FromProfileUrl + "\"></i>" +
                              "<h4><b>@" + item.FromScreenName + "</b></h4>" +
                              "<p>" + item.TwitterMsg + "</p>" +

                         " </span>";
                        }

                    }
                    else if (network == "lin")
                    {
                        LinkedInAccountRepository linkedin = new LinkedInAccountRepository();
                        LinkedInAccount linkAccount = linkedin.getUserInformation(user.Id, userid);

                        LinkedInFeedRepository linked = new LinkedInFeedRepository();
                        List<LinkedInFeed> lstLinked = linked.getAllLinkedInFeedsOfProfile(userid);
                        string profimg = string.Empty;
                        if (string.IsNullOrEmpty(linkAccount.ProfileImageUrl))
                        {
                            profimg = "../Contents/img/blank_user.png";
                        }
                        else
                        {
                            profimg = linkAccount.ProfileImageUrl;
                        }
                        messages += "<div style=\"display:none;\"><div id=\"lin_fans\">" + linkAccount.Connections + "</div><div id=\"twt_imgurl\">" + profimg + "</div></div> ";
                        foreach (LinkedInFeed item in lstLinked)
                        {
                            messages += "<span>" +
                               "<abbr>" + item.FeedsDate + "</abbr>" +
                               "<i><img style=\"height:58px;\" src=\"" + item.FromPicUrl + "\"></i>" +
                               "<h4><b>@" + item.FromName + "</b></h4>" +
                               "<p>" + item.Feeds + "</p>" +

                          " </span>";
                        }
                    }
                    else if (network == "gplus")
                    {

                        string midsnaps = string.Empty;
                        GooglePlusActivitiesRepository objActRepo = new GooglePlusActivitiesRepository();
                        List<GooglePlusActivities> lstmsggauser = objActRepo.getAllgoogleplusActivityOfUser(userid);
                       
                        foreach (GooglePlusActivities item in lstmsggauser)
                        {
                            messages += "<span>" +
                                "<abbr>" + item.PublishedDate + "</abbr>" +
                                "<i><img src=\"" + item.FromProfileImage + "\"></i>" +
                                "<h4><b>@" + item.FromUserName + "</b></h4>" +
                                "<p>" + item.Content + "</p>" +

                           " </span>";
                        }
                    }
                    Response.Write(messages);
                }
                else if (Request.QueryString["op"] == "messagechk")
                {
                    //SocioBoard.Domain.Messages mstable = new SocioBoard.Domain.Messages();
                    //string[] types = Request.QueryString["type[]"].Split(',');
                    //DataSet ds = (DataSet)Session["MessageDataTable"];
                    //DataSet dss = DataTableGenerator.CreateDataSetForTable(mstable);
                    //DataTable dtt = dss.Tables[0];
                    //DataView dv = new DataView(dtt);
                    //AjaxMessage ajxfed = new AjaxMessage();
                    //string message = string.Empty;
                    //foreach (var item in types)
                    //{
                    //    try
                    //    {
                    //        DataRow[] foundRows = ds.Tables[0].Select("Type = '" + item + "'");
                    //        foreach (var child in foundRows)
                    //        {
                    //            dtt.ImportRow(child);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        logger.Error(ex.Message);
                    //        Console.WriteLine(ex.Message);
                    //    }
                    //}
                    //message = ajxfed.BindData(dtt);
                    //Response.Write(message);
                }
            }
        }
        public string CompareDateWithServer(string clientdate, string scheduletime)
        {
            DateTime client = Convert.ToDateTime(clientdate);
            string strTimeNow = String.Format("{0:s}", client).Replace('T', ' ');

            DateTime server = DateTime.Now;
            DateTime schedule = Convert.ToDateTime(scheduletime);
            if (DateTime.Compare(client, server) > 0)
            {

                double minutes = (server - client).TotalMinutes;
                schedule = schedule.AddMinutes(minutes);

            }
            else if (DateTime.Compare(client, server) == 0)
            {

            }
            else if (DateTime.Compare(client, server) < 0)
            {
                double minutes = (server - client).TotalMinutes;
                schedule = schedule.AddMinutes(minutes);
            }
            return schedule.ToString();
        }


    }
}