[AF:CONTROL:ALPHABAR]
[AF:CONTROL:LIST]
	[AF:CONTROL:LIST:HEADER]
		<div id="afgrid" style="position:relative;">
		<table class="afgrid" cellspacing="0" cellpadding="4" width="100%">
			<tr>
				<td class="afgrouprow" style="padding-left:10px;">[RESX:Name]</td>
				<td class="afgrouprow" style="white-space:nowrap;" align="center">[RESX:Location]</td>
				<td class="afgrouprow" style="white-space:nowrap;" align="center">[RESX:Website]</td>
				<td class="afgrouprow" style="white-space:nowrap;" align="center">[RESX:Joined]</td>
				<td class="afgrouprow" style="white-space:nowrap;" align="center">[RESX:LastActive]</td>
				<td class="afgrouprow" style="white-space:nowrap;" align="center">[RESX:Posts]</td>
			</tr>
	[/AF:CONTROL:LIST:HEADER]
	[AF:CONTROL:LIST:ITEM]
			<tr>
				<td class="aftopicrow">[AF:PROFILE:DISPLAYNAME]</td>
				<td class="aftopicrow">[AF:PROFILE:LOCATION]</td>
				<td class="aftopicrow">[AF:PROFILE:WEBSITE]</td>
				<td class="aftopicrow" align="center" style="white-space:nowrap;">[AF:PROFILE:DATECREATED]</td>
				<td class="aftopicrow" align="center" style="white-space:nowrap;">[AF:PROFILE:DATELASTACTIVITY]</td>
				<td class="aftopicrow" align="center">[AF:PROFILE:POSTCOUNT]</td>
			</tr>
	[/AF:CONTROL:LIST:ITEM]
	[AF:CONTROL:LIST:ALTITEM]
			<tr>
				<td class="aftopicrowalt">[AF:PROFILE:DISPLAYNAME]</td>
				<td class="aftopicrowalt">[AF:PROFILE:LOCATION]</td>
				<td class="aftopicrowalt">[AF:PROFILE:WEBSITE]</td>
				<td class="aftopicrowalt" align="center" style="white-space:nowrap;">[AF:PROFILE:DATECREATED]</td>
				<td class="aftopicrowalt" align="center" style="white-space:nowrap;">[AF:PROFILE:DATELASTACTIVITY]</td>
				<td class="aftopicrowalt" align="center">[AF:PROFILE:POSTCOUNT]</td>
			</tr>
	[/AF:CONTROL:LIST:ALTITEM]
	[AF:CONTROL:LIST:FOOTER]
		</table></div>
	[/AF:CONTROL:LIST:FOOTER]
[/AF:CONTROL:LIST]
<div align="right">[AF:CONTROL:PAGER]</div>