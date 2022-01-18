/// <reference path="jquery-3.1.1.min.js" />
/// <reference path="common.js" />

/// Constants
//////////////////////////////////
var CREATE_CONFIG_URL_PATH  = '/admin/SL_Config_Create.aspx';
var KEY_ID_TAB              = 9;


/// Members
//////////////////////////////////
var m_StimuliCache = [];


/// Handlers
//////////////////////////////////
$(document).ready(function() {
    InitControls();
    BindEvents();
});


/// Ajax callbacks
//////////////////////////////////
function cacheStimuli(data, status, xhr) {
    m_StimuliCache = [];

    var i;
    for (i in data.d) {
        var newStimulus = {};

        newStimulus.value = data.d[i].ID;
        newStimulus.label = data.d[i].ID + '\t(' + data.d[i].Description + ')';

        // add stimulus to cache
        m_StimuliCache.push(newStimulus);
    }
}


/// Methods
//////////////////////////////////
function BindEvents() {
    $('[id$="drpTripletsRandType"]').change(function() {
        SetControlsStatusByRandType(this.selectedIndex)
    });
    $('[id$="drpExpType"]').change(function() {
        GetExpStimuli(this.selectedIndex);

        InitTripletsAutocomplete('txtPresetTriplets');
        InitTripletsAutocomplete('txtPresetFoils');
    });
}
function GetExpStimuli(expType) {
    var data = {};
    data._expType = expType;

    sendAjaxRequest(CREATE_CONFIG_URL_PATH, 'GetStimuli', data, cacheStimuli);
}
function InitControls() {
    SetControlsStatusByRandType($('[id$="drpTripletsRandType"]').val());
}
function SetControlsStatusByRandType(randType) {
    switch (parseInt(randType)) {
        case 1:
            SetControlEnabledStatus('NumberOfTriplets', 'txt', true);
            SetControlEnabledStatus('PresetTriplets', 'txt', false);
            SetControlEnabledStatus('PresetFoils', 'txt', false);
            break;
        case 2:
            SetControlEnabledStatus('NumberOfTriplets', 'txt', false);
            SetControlEnabledStatus('PresetTriplets', 'txt', true);
            SetControlEnabledStatus('PresetFoils', 'txt', true);
            break;
        default:
            SetControlEnabledStatus('NumberOfTriplets', 'txt', false);
            SetControlEnabledStatus('PresetTriplets', 'txt', false);
            SetControlEnabledStatus('PresetFoils', 'txt', false);
            break;
    }
}
function SetControlEnabledStatus(controlID, controlIDPrefix, enabled) {
    if (enabled) {
        $('[id$=' + controlIDPrefix + controlID + ']').removeAttr('disabled'); // enavle control.
        ValidatorEnable($('[id*=val_' + controlID + ']')[0], true); // enable control validators.
    }
    else {
        $('[id$=' + controlIDPrefix + controlID + ']').attr('disabled', 'disabled'); // disable control.
        ValidatorEnable($('[id*=val_' + controlID + ']')[0], false); // disable control validators.
    }
}
function InitTripletsAutocomplete(textboxID) {
    function split(val) {
        return val.split(/[;\[\],]\s*/);
    }
    function splitTriplet(val) {
        return val.split(/[\[\],;]/);
    }
    function extractLast(term) {
        return split(term).pop();
    }
    $('[id$="' + textboxID + '"]').on("keydown", function(event) {
        // don't navigate away from the field on tab when selecting an item
        if (event.keyCode == KEY_ID_TAB && $(this).autocomplete("instance").menu.active ) {
            event.preventDefault();
        }
    })
    $('[id$="' + textboxID + '"]').autocomplete({
        minLength: 0,
        source: function(request, response) {
            // delegate back to autocomplete, but extract the last term
            var t = request.term[request.term.length-1];
            switch (t) {
                case ']':
                    // $('[id$="' + textboxID + '"]')[0].value += ';[';
                    return false;
                case ';':
                    $('[id$="' + textboxID + '"]')[0].value += '[';
                    break;
                case '[':
                case ',':
                    break;
            }

            response($.ui.autocomplete.filter(m_StimuliCache, extractLast(request.term)));
        },
        focus: function() {
            // prevent value inserted on focus
            return false;
        },
        select: function(event, ui) {
            var splitValues = splitTriplet(this.value);
            var lastTyped = splitValues[splitValues.length - 1];
            var preExistingString = this.value.substring(0, this.value.lastIndexOf(lastTyped));
            this.value = preExistingString + ui.item.value;
            // var terms = split(this.value);
            // // remove the current input
            // terms.pop();
            // // add the selected item
            // terms.push(ui.item.value);
            // // add placeholder to get the comma-and-space at the end
            // terms.push("");
            // var nextDelimiter = '];';
            // this.value = this.value + ui.item.value + nextDelimiter; //terms.join(nextDelimiter);
            return false;
        }
    });
}