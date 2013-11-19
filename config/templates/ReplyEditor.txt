<div class="afcrumb">[AF:LINK:FORUMMAIN] > [AF:LINK:FORUMGROUP] > [AF:LINK:FORUMNAME]</div>
<div style="text-align:center;padding-top:10px;">
<div class="afeditor">
	<table cellpadding="10" cellspacing="0" width="100%">
		<tr>
			<td style="border-bottom:solid 1px #cdcdcd;text-align:left;"><span class="aftitle">[RESX:ReplyToTopic]</span></td>
			<td style="border-bottom:solid 1px #cdcdcd;text-align:right;"><span style="font-weight:bold;">[RESX:Topic]:</span> [AF:LINK:TOPICNAME]</td>
		</tr>
		<tr>
			<td colspan="2" align="center">			
				<table cellpadding="0" cellspacing="4" border="0" width="99%">
					[AF:UI:MESSAGEREPLY]
					<tr>
						<td style="text-align:left;">[RESX:ReplyToMessage]:</td>
						<td></td>
					</tr>
					<tr>
						<td colspan="2" style="text-align:left;">[AF:LABEL:BODYREPLY]</td>
					</tr>
					[/AF:UI:MESSAGEREPLY]
					[AF:UI:ANON]
					<tr>
						<td style="text-align:left;">[RESX:Username]:[AF:REQ:USERNAME]</td>
						<td></td>
					</tr>
					<tr>
						<td style="text-align:left;"><div style="width:150px;">[AF:INPUT:USERNAME]</div></td>
						<td></td>
					</tr>
					<tr>
						<td style="text-align:left;">[RESX:SecurityCode]:[AF:REQ:SECURITYCODE]</td>
						<td></td>
					</tr>
					<tr>
						<td style="text-align:left;"><div style="width:150px;">[AF:INPUT:CAPTCHA]</div></td>
						<td></td>
					</tr>
					[/AF:UI:ANON]
					<tr>
						<td style="text-align:left;">[RESX:Subject]:</td>
						<td></td>
					</tr>
					<tr>
						<td style="text-align:left;"><div style="white-space:nowrap;">[AF:LABEL:SUBJECT]</div></td>
						<td></td>
					</tr>
					<tr>
						<td style="text-align:left;">[RESX:Message]:</td>
						<td></td>
					</tr>
					<tr>
						<td width="100%">[AF:INPUT:BODY]</td>
						<td style="width:70px;">[AF:CONTROL:EMOTICONS]</td>
					</tr>
					<tr>
						<td>
							<div class="amtbwrapper" style="text-align:center;">
								<div style="margin-left:0 auto;margin-right:0 auto;min-width:50px;max-width:160px;">
									[AF:BUTTON:SUBMIT][AF:BUTTON:CANCEL][AF:BUTTON:PREVIEW]
								</div>
							</div>
						</td>
						<td></td>
					</tr>
					<tr>
						<td style="text-align:left;">
						    [AF:CONTROL:OPTIONS]
							[AF:UI:SECTION:ATTACH]
								[AF:CONTROL:UPLOAD]
							[/AF:UI:SECTION:ATTACH]
							[AF:UI:SECTION:TOPICREVIEW]
								[AF:CONTROL:TOPICREVIEW]
									<table width="100%" style="background-color:#fff;border-top:solid 1px #cdcdcd;" cellspacing="0" cellpadding="4">
										[TOPIC]
											<tr>
												<td valign="top" class="[POSTINFOCSS]" height="100">[POSTINFO]<br />[SPACER:1:125]</td>
												<td valign="top" class="[POSTREPLYCSS]" width="100%">
													<table cellpadding="4" cellspacing="0" border="0" width="100%">
														<tr>
															<td class="afsubrow"><a name="[POSTID]"></a>[POSTDATE]</td>
															<td class="afsubrow" align=right valign=top>
															
															</td>
														</tr>
														<tr>
															<td colspan="2" class="afpostbody">[AF:CONTROL:POLL][BODY]</td>
														</tr>
														<tr>
															<td colspan="2" class="afpostattach">[ATTACHMENTS]</td>
														</tr>
														<tr>
															<td colspan="2" class="afpostsig">[SIGNATURE]</td>
														</tr>
														<tr>
															<td colspan="2" class="afposteditdate" align="right">[MODEDITDATE]</td>
														</tr>
													</table>				
												</td>
											</tr>
											[/TOPIC]
											[REPLIES]
											<tr>
												<td valign="top" class="[POSTINFOCSS]" height="100">[POSTINFO]<br />[SPACER:1:125]</td>
												<td valign="top" class="[POSTREPLYCSS]" width="100%">
													<table cellpadding="4" cellspacing="0" border="0" width="100%">
														<tr>
															<td class="afsubrow"><a name="[POSTID]"></a>[POSTDATE]</td>
															<td class="afsubrow" align="right">
																
															</td>
														</tr>
														<tr>
															<td colspan="2" class="afpostbody">[BODY]</td>
														</tr>
														<tr>
															<td colspan="2" class="afpostattach">[ATTACHMENTS]</td>
														</tr>
														<tr>
															<td colspan="2" class="afpostsig">[SIGNATURE]</td>
														</tr>
														<tr>
															<td colspan="2" class="afposteditdate" align="right">[MODEDITDATE]</td>
														</tr>
													</table>				
												</td>
											</tr>
										[/REPLIES]
									</table>
								[/AF:CONTROL:TOPICREVIEW]
							[/AF:UI:SECTION:TOPICREVIEW]
						</td>
						<td></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</div>
</div>