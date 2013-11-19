<div id="aftopicedit" style="width:500px;height:350px;display:none;" title="[RESX:TopicQuickEdit]">
    <div class="dnnForm">
        <ul class="dnnAdminTabNav">
            <li><a href="#editTopic1">[RESX:Topic]</a></li>
            <li><a href="#editTopic2">[RESX:Categories]</a></li>
            <li><a href="#editTopic3">[RESX:Properties]</a></li>
        </ul>
            <div id="editTopic1">
                <div class="dnnFormItem">
                    <label>[RESX:Subject]:</label>
                    <input type="text" id="aftopicedit-subject" value="" />
                </div>
                <div class="dnnFormItem">
                    <label>[RESX:Tags]:</label>
                    <input type="text" id="aftopicedit-tags" value="" />
                </div>
                <div class="dnnFormItem">
                    <label>[RESX:Priority]:</label>
                    <input type="text" id="aftopicedit-priority" class="am-ui-width100" value="0" />
                </div>
                <div class="dnnFormItem">
                    <label>[RESX:Status]:</label>
                    <select id="aftopicedit-status"><option value="-1">[RESX:NoStatus]</option><option value="0">[RESX:Informative]</option><option value="1">[RESX:NotResolved]</option><option value="3">[RESX:Resolved]</option>
                        </select>
                </div>
                <div class="dnnFormItem">
                    <label>[RESX:Locked]:</label>
                    <input type="checkbox" id="aftopicedit-locked" value="1" />
                </div>
                <div class="dnnFormItem">
                    <label>[RESX:Pinned]:</label>
                    <input type="checkbox" id="aftopicedit-pinned" value="1" />
                </div>
            </div>
            <div id="editTopic2">
                <ul id="catlist">
                    
                </ul>
            </div>
            <div id="editTopic3">
                <ul id="proplist">
                    
                </ul>
            </div>
        
          <ul class="dnnActions dnnClear">
            <li><a href="#" onclick="amaf_saveTopic(); return false;" class="dnnPrimaryAction">[RESX:Save]</a></li>
            <li><a href="#" onclick="am.UI.CloseDiv('aftopicedit'); return false;" class="dnnSecondaryAction">
            [RESX:Cancel]</a></li>
        </ul>
        <input type="hidden" id="aftopicedit-topicid" value="" />
    </div>
</div>
<script type="text/javascript">
    jQuery(function ($) {
        $('#aftopicedit').dnnTabs();
    });
</script>