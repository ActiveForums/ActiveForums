<table width="100%" cellpadding="0" cellspacing="0">
	<tr>
		<td class="afprlftback">
			[SPACER:150:9]
		</td>
		<td class="afprmidback">
			<table width="100%">
				<tr>
					<td>[AF:PROFILE:AVATAR]</td>
					<td width="100%">
					[AF:PROFILE:DISPLAYNAME][AF:PROFILE:USERSTATUS]<br />
					[AF:PROFILE:USERCAPTION]<br />
					[AF:PROFILE:RANKDISPLAY]<br />
					[AF:PROFILE:RANKNAME]</td>
					<td align="right" style="white-space:nowrap;">
						[RESX:MemberSince]: [AF:PROFILE:DATECREATED]<br />
						[RESX:LastVisit]: [AF:PROFILE:DATELASTACTIVITY]<br />
						[RESX:Posts]: [AF:PROFILE:POSTCOUNT]<br />
						[AF:PROFILE:PMLINK]</td>
				</tr>
			</table>
		</td>
		<td class="afprrgtback">[SPACER:150:8]</td>
	</tr>
	<tr>
		<td class="afprlftborder">
			[SPACER:150:9]
		</td>
		<td valign="top">
		[AM:CONTROLS:TABS]
			[AM:CONTROLS:TAB:AboutMe]
				<table width="100%">
					<tr>
						<td width="50%" valign="top">
							<table>
								<tr>
									<td class="afbold">[RESX:Website]:</td><td class="afnormal">[AF:PROFILE:WEBSITE]</td>
								</tr>
								<tr>
									<td class="afbold">[RESX:Location]:</td><td class="afnormal">[AF:PROFILE:LOCATION]</td>
								</tr>
								<tr>
									<td class="afbold">[RESX:Occupation]:</td><td class="afnormal">[AF:PROFILE:OCCUPATION]</td>
								</tr>
								<tr>
									<td class="afbold">[RESX:Interests]:</td><td class="afnormal">[AF:PROFILE:INTERESTS]</td>
								</tr>
								<tr>
									<td class="afbold" valign="top">[RESX:Avatar]:</td><td>[AF:PROFILE:AVATAR]<br />[AF:CONTROL:AVATAREDIT]</td>
								</tr>
								<tr>
									<td class="afbold" colspan="2">[RESX:Signature]:</td>
								</tr>
								<tr>
									<td colspan="2" class="afnormal">[AF:PROFILE:SIGNATURE]</td>
								</tr>
								
							</table>
						</td>
						<td width="50%" valign="top">
							<table>
								<tr>
									<td class="afnormal" colspan="2">[AF:PROFILE:VIEWUSERPOSTS]</td>
								</tr>
								<tr>
									<td class="afbold">[RESX:Yahoo]:</td><td class="afnormal">[AF:PROFILE:YAHOO]</td>
								</tr>
								<tr>
									<td class="afbold">[RESX:MSN]:</td><td class="afnormal">[AF:PROFILE:MSN]</td>
								</tr>
								<tr>
									<td class="afbold">[RESX:ICQ]:</td><td class="afnormal">[AF:PROFILE:ICQ]</td>
								</tr>
								<tr>
									<td class="afbold">[RESX:AOL]:</td><td class="afnormal">[AF:PROFILE:AOL]</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td colspan="2">
					        <div class="amtbwrapper" style="text-align:center;">
								<div style="margin-left:0 auto;margin-right:0 auto;min-width:50px;max-width:160px;">
									[AF:BUTTON:PROFILEEDIT]
									[AF:BUTTON:PROFILESAVE]
									[AF:BUTTON:PROFILECANCEL]
								</div>
							</div>

						</td>
					</tr>
				</table>
				
			[/AM:CONTROLS:TAB:AboutMe]
			[AM:CONTROLS:TAB:MyPreferences:Private]
				[AM:CONTROLS:ProfileMyPreferences]
			[/AM:CONTROLS:TAB:MyPreferences:Private]
			[AM:CONTROLS:TAB:ForumTracking:Private]
				[AM:CONTROLS:ProfileForumTracker]
			[/AM:CONTROLS:TAB:ForumTracking:Private]
			[AM:CONTROLS:TAB:UserAccount:Admin]
				[AM:CONTROLS:ProfileUserAccount]
			[/AM:CONTROLS:TAB:UserAccount:Admin]
			[AM:CONTROLS:TAB:AdminSettings:Admin]
				[AM:CONTROLS:AdminProfileSettings]
			[/AM:CONTROLS:TAB:AdminSettings:Admin]
			
		[/AM:CONTROLS:TABS]
		</td>
		<td class="afprrgtborder">[SPACER:150:8]</td>
	</tr>
	<tr>
		<td class="afprlftbottom">[SPACER:8:9]</td>
		<td class="afbottomborder">[SPACER:8:100]</td>
		<td class="afprrgtbottom">[SPACER:8:8]</td>
	</tr>
</table>