<script type="text/javascript">
    function amaf_moveTopic() {
        var fid = $('#drpForums').val();
        var tid = $('#aftopicmove-topicid').val();
        amaf_modMove(tid, fid);
    };
</script>
<div id="aftopicmove" style="width:500px;height:350px;display:none;" title="[RESX:MoveTopicTitle]">
    <div class="dnnForm">
        <div class="dnnFormItem">
            <label>[RESX:Subject]:</label>
            <input type="text" id="aftopicmove-subject" value="" disabled="disabled" />
        </div>
        <div class="dnnFormItem">
            <label>[RESX:CurrentLocation]:</label>
            <input type="text" id="aftopicmove-currentforum" value="" disabled="disabled" />
        </div>
        <div class="dnnFormItem">
            <label>[RESX:MoveLocation]:</label>
            <select id="drpForums"></select>
        </div>
        <ul class="dnnActions dnnClear">
            <li><a href="#" onclick="amaf_moveTopic(); return false;" class="dnnPrimaryAction">[RESX:Save]</a></li>
            <li><a href="#" onclick="am.UI.CloseDiv('aftopicmove'); return false;" class="dnnSecondaryAction">
            [RESX:Cancel]</a></li>
        </ul>
        <input type="hidden" id="aftopicmove-topicid" value="" />
    </div>
</div>