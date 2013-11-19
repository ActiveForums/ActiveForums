<%@ Register TagPrefix="am" Namespace="Active.Modules.Forums.Controls" Assembly="Active.Modules.Forums.40" %>
<%@ Register TagPrefix="am" TagName="MarkForumRead" Src="~/DesktopModules/ActiveForums/controls/af_markallread.ascx"%>
<%@ Register TagPrefix="am" TagName="MiniSearch" Src="~/DesktopModules/ActiveForums/controls/af_searchquick.ascx"%>
<div class="afcrumb">[FORUMMAINLINK] > [FORUMGROUPLINK]</div>
<div class="aftitlelg">[FORUMLINK]</div>
<table cellSpacing="0" cellPadding="0" width="100%" border="0">
	<tbody>
		<tr>
			<td colspan=2>
			[SUBFORUMS]
				<table cellpadding="4" cellspacing="0" border="0" class="afsubgrid">
					<tr>
						<td class="afheader">[SPACER:10:1]</td>
						<td class="afheader" width="100%">[RESX:FORUMHEADER]</td>
						<td class="afheader" align="center" style="width:50px;" width="50">[RESX:TOPICSHEADER]</td>
						<td class="afheader" align="center" style="width:50px;" width="50">[RESX:REPLIESHEADER]</td>
						<td class="afheader" align="center" style="width:175px;" width="175" nowrap>[RESX:LASTPOSTHEADER]</td>
					</tr>
					[FORUMS]
					<tr>
						<td class="[CSS:afforumrowtop:afforumrowmid:afforumrowbottom]">[FORUMICON]</td>
						<td class="[CSS:afforumrowtop:afforumrowmid:afforumrowbottom]" width="100%">[FORUMNAME]<br>[FORUMDESCRIPTION]</td>
						<td class="[CSS:afforumrowtop:afforumrowmid:afforumrowbottom]" align="center" style="width:50px;">[TOTALTOPICS]</td>
						<td class="[CSS:afforumrowtop:afforumrowmid:afforumrowbottom]" align="center" style="width:50px;">[TOTALREPLIES]</td>
						<td class="[CSS:afforumrowtop:afforumrowmid:afforumrowbottom]" width="175" style="width:175px;"><nobr>[LASTPOSTSUBJECT:50]</nobr><br><nobr>[RESX:BY]&nbsp;[DISPLAYNAME]</nobr><br><nobr>[LASTPOSTDATE]</nobr><br></td>
					</tr>
					[/FORUMS]
				</table>		
			[/SUBFORUMS]
			</td>
		</tr>
			
		[ANNOUNCEMENTS]
		<tr>
			<td colspan="2">
				 <table class="afgrid" cellspacing="0" cellpadding="4" width="100%">
					<tr>
						<td class="afgrouprow" width="100%" colSpan="2">[RESX:Announcements]</td>
						<td class="afgrouprow" noWrap align="center">[RESX:REPLIES]</td>
						<td class="afgrouprow" noWrap align="center">[RESX:Views]</td>
						<td class="afgrouprow" noWrap align="center">[RESX:LASTPOSTHEADER]</td>
					</tr>
					[ANNOUNCEMENT]
					<tr>
						<td class="[ROWCSS]" width="20">[POSTICON]</td>
						<td class="[ROWCSS]" width="100%">
							<table cellpadding="0" cellspacing="0" border="0">
								<tr>
									<td rowspan="2" valign="top" width="100%" class="afsubject" title="[BODYTITLE]">[SUBJECT][AF:ICONLINK:LASTREAD]<br />[STARTEDBY][AF:UI:MINIPAGER]</td><td align=right>[POSTRATINGDISPLAY]</td><td rowspan="2">[STATUS]</td>
								</tr>
								<tr>
									<td nowrap>
										[ACTIONS:DELETE]
										[ACTIONS:EDIT]
										[ACTIONS:MOVE]
										[ACTIONS:LOCK]
										[ACTIONS:PIN]
									</td>
								</tr>
							</table>
						</td>
						<td class="[ROWCSS]" align="center">[REPLIES]</td>
						<td class="[ROWCSS]" align="center">[VIEWS]</td>
						<td class="[ROWCSS]"><div class="af_lastpost">[LASTPOST]<nobr>[RESX:BY]&nbsp;[LASTPOSTDISPLAYNAME][AF:ICONLINK:LASTREPLY]</nobr><br><nobr>[LASTPOSTDATE]</nobr>[/LASTPOST]</div></td>
					</tr>
					[/ANNOUNCEMENT]
					</table>	
			</td>
		</tr>
		[/ANNOUNCEMENTS]
		<tr>
			<td class="afnormal" style="white-space:nowrap;">[FORUMSUBSCRIBE]</td>
			<td align="right">[MINISEARCH]</td>
		</tr>
		<tr>
			<td class="afnormal" style="padding-bottom:5px;"><div class="afbuttonarea">[ADDTOPIC]</div></td>
			<td align="right">[PAGER1]</td>
		</tr>
		<tr>
			<td colspan="2">
			<div id="afgrid" style="position:relative;">
				<table class="afgrid" cellspacing="0" cellpadding="4" width="100%">
						<tr>
							<td class="afgrouprow" width="100%" colspan="2"><div class="afcontrolheader">[RESX:TOPICSHEADER]</div></td>
							<td class="afgrouprow" align="center"><div class="afcontrolheader">[RESX:REPLIESHEADER]</div></td>
							<td class="afgrouprow" align="center"><div class="afcontrolheader">[RESX:Views]</div></td>
							<td class="afgrouprow" align="center" style="white-space:nowrap;">[RESX:LastPost]</td>
						</tr>
						[TOPICS]
						<tr>
							<td class="[ROWCSS]" width="20">[POSTICON]</td>
							<td class="[ROWCSS]" width="100%">
								<table cellpadding="0" cellspacing="0" border="0">
									<tr>
										<td rowspan="2" valign="top" width="100%" class="afsubject" TITLE="[BODYTITLE]">[SUBJECT][AF:ICONLINK:LASTREAD]<br />[STARTEDBY][AF:UI:MINIPAGER]</td><td align=right>[POSTRATINGDISPLAY]</td><td rowspan="2">[STATUS]</td>
									</tr>
									<tr>
										<td nowrap>
											[ACTIONS:DELETE]
											[ACTIONS:EDIT]
											[ACTIONS:MOVE]
											[ACTIONS:LOCK]
											[ACTIONS:PIN]
										</td>
									</tr>
								</table>
							</td>
							<td class="[ROWCSS]" align="center">[REPLIES]</td>
							<td class="[ROWCSS]" align="center">[VIEWS]</td>
							<td class="[ROWCSS]" style="white-space:nowrap;"><div class="af_lastpost">[LASTPOST]<nobr>[RESX:BY]&nbsp;[LASTPOSTDISPLAYNAME][AF:ICONLINK:LASTREPLY]</nobr><br><nobr>[LASTPOSTDATE]</nobr>[/LASTPOST]</div></td>
						</tr>
						[/TOPICS]
				</table>
			</div>
			</td>
		</tr>
		<tr>
			<td class="afnormal" valign="top"><div class="afbuttonarea">[ADDTOPIC][MARKFORUMREAD]</div>
			<div class="afcrumb">[FORUMMAINLINK] > [FORUMGROUPLINK] > [FORUMLINK]</div>
			</td>
			<td align="right">[PAGER2]<br />[JUMPTO]<br />[RSSLINK]</td>
		</tr>
	</tbody>
</table>