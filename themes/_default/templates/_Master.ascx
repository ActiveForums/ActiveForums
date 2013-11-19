<table width="100%">
    <tr>
        <td valign="top" width="150">
        <af:forumgrouprepeater runat="server">
            <headertemplate>                <div style="border:solid 1px #0c6476;background-color:#0d97b3;width:200px;">
                    <div style="margin:3px;color:#fff;font-size:14px;">[RESX:FORUMS]</div>
                    
                </div>
            
            <div class="afpanel">
                <div class="afpanelbtm"></headertemplate>
            <displaytemplate>
            

                    <div style="padding-left:10px;padding-top:5px;padding-bottom:5px;">
            <table border="0" cellpadding="0" cellspacing="0" style="width:95%;">
	            <tr>
	                <td style="text-align:right;">[GROUPCOLLAPSE]</td>
		            <td style="padding-left:10px;" width="100%">[GROUPNAME]</td>
		            
	            </tr>
	            <tr>
		            <td colspan="2">
			            [GROUP]
				            <table cellpadding="4" cellspacing="0" border="0" class="afsubgrid">
			    	            [FORUMS]
					            <tr>
						            <td class="[CSS:ROWCLASS] affoldersmall [CSS:FORUMICON]"><div></div></td>
						            <td class="[CSS:ROWCLASS]" width="100%" colspan="2">[FORUMNAME]<br />[FORUMDESCRIPTION]</td>
						            
					            </tr>
					            [SUBFORUMS]
						            <tr>
							            <td class="afforumrowmid"></td>
							            <td class="afforumrowmid" valign="top">[FORUMICONSM]</td>
							            <td class="afforumrowmid" width="100%" valign="top">[FORUMNAME]<br />[FORUMDESCRIPTION]</td>

						            
						            </tr>
					            [/SUBFORUMS]
					            [/FORUMS]
				            </table>		
			            [/GROUP]
		            </td>
	            </tr>
            </table></div>
            
            </displaytemplate>
            <footertemplate></div>
            </div></footertemplate>
            <noresultstemplate>No Forums Found</noresultstemplate>
        </af:forumgrouprepeater>
        
        </td>
        <td valign="top">
            <div style="height:20px;border-bottom:solid 2px #ffcc00;">
                    <div style="width:15px;height:20px;float:left;"></div>
                   <div class="aftab">
                    <div class="aftab aftablt"></div>
                    <div class="aftab aftabmid"><span class="affont1 afht1" onclick="afCB('test');">Latest</span></div>
                    <div class="aftab aftabrt"></div>
                   </div> 
                   <div style="width:5px;height:20px;float:left;"></div>
                   <div class="aftab">
                    <div class="aftab aftabltsel"></div>
                    <div class="aftab aftabmidsel"><span class="affont1 afht1">Active</span></div>
                    <div class="aftab aftabrtsel"></div>
                   </div>
                   <div style="width:5px;height:20px;float:left;"></div>
                   <div class="aftab">
                    <div class="aftab aftablt"></div>
                    <div class="aftab aftabmid"><span class="affont1 afht1">Not Read</span></div>
                    <div class="aftab aftabrt"></div>
                   </div>  
                   <div style="width:5px;height:20px;float:left;"></div>
                   <div class="aftab">
                    <div class="aftab aftablt"></div>
                    <div class="aftab aftabmid"><span class="affont1 afht1">Unanswered</span></div>
                    <div class="aftab aftabrt"></div>
                   </div>  
            </div>
            
                <af:topicsdisplay runat="server">
                <displaytemplate>
                <table class="afgrid" cellspacing="0" cellpadding="4" width="100%">
						[TOPICS]
						<tr>
							<td class="[ROWCSS]" width="20">[POSTICON]</td>
							<td class="[ROWCSS]" width="100%">
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td rowspan="2" valign="top" width="100%" class="afsubject" title="[BODYTITLE]">[SUBJECTLINK][AF:ICONLINK:LASTREAD]<br />[STARTEDBY][AF:UI:MINIPAGER]</td><td style="text-align:right;">[POSTRATINGDISPLAY]</td><td rowspan="2">[STATUS]</td>
									</tr>
									<tr>
										<td style="white-space:nowrap;">
										    <div class="afmenutoggle" onclick="afMenuShow(this,'[TOPICID]');" id="aft[TOPICID]">
										        <img src="[THEMEPATH]images/configure_24.png" title="[RESX:Actions]" />
										    </div>
										</td>
									</tr>
								</table>
							</td>
							<td class="[ROWCSS]" style="white-space:nowrap;"><div class="af_lastpost">[LASTPOST][RESX:BY]&nbsp;[LASTPOSTDISPLAYNAME][AF:ICONLINK:LASTREPLY]<br />[LASTPOSTDATE][/LASTPOST]</div></td>
						</tr>
						[/TOPICS]
						[PAGER]
				</table>
                </displaytemplate>
            </af:topicsdisplay>
           
        </td>
        <td valign="top" width="200">
        <div style="border:solid 1px #f59701;background-color:#f7b348;width:200px;">
                    <div style="margin:3px;color:#fff;font-size:14px;">Quick Links</div>
                    
                </div>
            <div class="afpanel">
                <div class="afpanelbtm">
                <div style="padding-left:10px;padding-top:10px;padding-bottom:10px;">
            <div class="aflink2">[AF:TB:ModList]</div>
			<div class="aflink2">[AF:TB:MyProfile]</div>
			<div class="aflink2">[AF:TB:MySettings]</div>
			<div class="aflink2">[AF:TB:Search]</div>
			<div class="aflink2">[AF:TB:MemberList]</div>
			<div class="aflink2">[AF:TB:ControlPanel]</div>
			     </div>
			    </div>
		    </div>
        </td>
    </tr>
</table>