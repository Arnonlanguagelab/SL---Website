/// <reference path="jquery-3.1.1.min.js" />
/// <reference path="common.js" />

/// Constants
//////////////////////////////////
var EXP_URL_PATH        = '/exp/SL.aspx';

var KEY_ID_ENTER        = 13;
var KEY_ID_ENTER_NUMPAD = 13; // same as main 'enter'?
var KEY_ID_SPACE        = 32;
var KEY_ID_1            = 49;
var KEY_ID_2            = 50;
var KEY_ID_1_NUMPAD     = 97;
var KEY_ID_2_NUMPAD     = 98;

var PAUSE_BEFORE_AFTER_QUESTION = 200;

var MAX_RAND_ITERATIONS = 100;

var EXP_PHASE_STARTUP               = 0;
var EXP_PHASE_DEMOGRAPHICS          = 1;
var EXP_PHASE_TRAINING_INSTRUCTIONS = 2;
var EXP_PHASE_TRAINING_TRIALS       = 3;
var EXP_PHASE_TESTING_INSTRUCTIONS  = 4;
var EXP_PHASE_TESTING_TRIALS        = 5;
var EXP_PHASE_FINISH                = 6;

var EXP_TYPE_VSL = 1;
var EXP_TYPE_ASL = 2;
var EXP_TYPE_MSL = 3;
var EXP_TYPE_DSL = 4;

var TRIPLET_TYPE_TRIPLET = 'T';
var TRIPLET_TYPE_FOIL = 'F';

var TRIPLET_ELEMENT_A = 'A';
var TRIPLET_ELEMENT_B = 'B';
var TRIPLET_ELEMENT_C = 'C';


/// Parameters
//////////////////////////////////
var p_ConfigID                          = null;
var p_ConfigName                        = null;
var p_ExpType                           = null;
var p_RandomizationType                 = null;
var p_NumOfTriplets                     = null;
var p_PresetTriplets                    = null;
var p_PresetFoils                       = null;
var p_PauseBetweenStimuli               = null;
var p_StimulusDuration                  = null;
var p_TrainingNumOfTripletRepetitions   = null;
var p_TestingPauseBetweenTriplets       = null;


/// Members
//////////////////////////////////
var m_ExperimentPhase = EXP_PHASE_STARTUP;  // the current experiment phase.
var m_Stimuli = {};                         // a dictionary of {Stimulus ID, Stimulus Object}.
var m_SessionID = null;                     // a unique string for the current session.
var m_Triplets = [];                        // an array of triplets.
var m_TrainingSet = [];                     // an array of triplets and foils to be used in the training trials.
var m_CurrTrainingItemIndex = -1;           // the index of the currently displayed training item.
var m_CurrTrainingItemElement = TRIPLET_ELEMENT_C; // the element type of the currently displayed stimulus.
var m_CurrTrainingStimulus = null;          // the displayed stimulus in the current training trial.
var m_TrainingTrials = [];                  // an array that saves all training trials for later bulk insert to the db.
var m_TestingFoils = [];                    // an array of foils to be used in the testing trials.
var m_TestingQuestions = [];                // an array of questions to be used in the testing trials.
var m_WaitingForAnswer = false;             // set to 'true' when the user should provide an answer in a testing trial.
var m_CurrTestingQuestionIndex = -1;        // the index of the currently displayed testing question.
var m_CurrTestingQuestionStartTime = null;  // used to measure the participant's reaction time.
var m_CurrTestingQuestionElement = null;    // the current element displayed in the testing phase (1-A/B/C or 2-A/B/C).
var m_TestingTrials = [];                   // an array that saves all testing trials for later bulk insert to the db.


/// Handlers
//////////////////////////////////
$(document).ready(function() {
    hideHomepageLink();
    initExpParameters();
    startExperimentPhase(EXP_PHASE_STARTUP);
    bindEvents();
});
$(document).keypress(function(ev) {
    switch (m_ExperimentPhase) {
        case EXP_PHASE_STARTUP:
            if (ev.which == KEY_ID_SPACE)
                startExperimentPhase(EXP_PHASE_DEMOGRAPHICS);
            break;

        case EXP_PHASE_DEMOGRAPHICS:
            if (ev.which == KEY_ID_ENTER || ev.which == KEY_ID_ENTER_NUMPAD || ev.which == KEY_ID_SPACE)
                return false;
            break;

        case EXP_PHASE_TRAINING_INSTRUCTIONS:
            if (ev.which == KEY_ID_SPACE)
                startExperimentPhase(EXP_PHASE_TRAINING_TRIALS);
            break;

        case EXP_PHASE_TRAINING_TRIALS:
            break;

        case EXP_PHASE_TESTING_INSTRUCTIONS:
            if (ev.which == KEY_ID_SPACE)
                startExperimentPhase(EXP_PHASE_TESTING_TRIALS);
            break;

        case EXP_PHASE_TESTING_TRIALS:
            if (m_WaitingForAnswer && (ev.which == KEY_ID_1 || ev.which == KEY_ID_1_NUMPAD ||
                                       ev.which == KEY_ID_2 || ev.which == KEY_ID_2_NUMPAD)) {
                var rt = new Date().getTime() - m_CurrTestingQuestionStartTime;

                if (ev.which == KEY_ID_1 || ev.which == KEY_ID_1_NUMPAD)
                    saveSessionTestingTrial(1, rt);
                else if (ev.which == KEY_ID_2 || ev.which == KEY_ID_2_NUMPAD)
                    saveSessionTestingTrial(2, rt);

                displayNextQuestion();
            }
            break;

        case EXP_PHASE_FINISH:
            break;
            
        default:
            break;
    }
});
function bindEvents() {
    $('#drp_dem_sex, #drp_dem_age_y, #drp_dem_age_m, #drp_dem_native_lang, #drp_dem_location').unbind('blur').blur(function() {
        validateDemographics(this.id);
    });

    $('#cmd_demographics_ok').unbind('click').click(function () {
        if (!validateDemographics(null))
            return;
        finishDemographics();
    });
}


/// Ajax callbacks
//////////////////////////////////
function loadExperimentParameters(data, status, xhr) {
    p_ConfigID = data.d.Config.ID;
    p_ConfigName = data.d.Config.ConfigName;
    p_ExpType = data.d.Config.ExpType;
    p_RandomizationType = data.d.Config.RandomizationType;
    p_NumOfTriplets = data.d.Config.NumOfTriplets;
    p_PresetTriplets = data.d.Config.PresetTriplets;
    p_PresetFoils = data.d.Config.PresetFoils;
    p_PauseBetweenStimuli = data.d.Config.PauseBetweenStimuli;
    p_StimulusDuration = data.d.Config.StimulusDuration;
    p_TrainingNumOfTripletRepetitions = data.d.Config.TrainingNumOfTripletRepetitions;
    p_TestingPauseBetweenTriplets = data.d.Config.TestingPauseBetweenTriplets;

    getExpStimuli(p_ExpType);
}
function cacheStimuli(data, status, xhr) {
    m_Stimuli = {};

    var i;
    for (i in data.d) {
        // create the stimulus object
        var s = data.d[i];
        var p = '/' + s.Type + '/' + s.Filename;
        var newStimulus = { id: s.ID, expType: s.ExpType, type: s.Type, filename: s.Filename, description: s.Description, path: p };

        // add stimulus to cache
        m_Stimuli[newStimulus.id] = newStimulus;
    }
}
function setNewSession(data, status, xhr) {
    m_SessionID = data.d;
}
function saveDemographicsError(xhr, status, err) {
    // TODO : Handle Error!
}
function saveSessionTripletsError(xhr, status, err) {
    // TODO : Handle Error!
}
function saveSessionTrainingTrialsError(xhr, status, err) {
    // TODO : Handle Error!
}
function saveSessionTestingTrialsError(xhr, status, err) {
    // TODO : Handle Error!
}


/// Methods
//////////////////////////////////
function hideHomepageLink() {
    $('#lnk_home').hide();
}
function getExpStimuli(expType) {
    var data = {};
    data._expType = expType;

    sendAjaxRequest(EXP_URL_PATH, 'GetStimuli', data, cacheStimuli);
}
function preloadStimuli(panelID) {
    var i;
    for (i in m_Stimuli) {
        var currStimulus = m_Stimuli[i];
        var newStimulus = null;

        switch (p_ExpType) {
            case EXP_TYPE_VSL:
            case EXP_TYPE_DSL:
                newStimulus = new Image();
                newStimulus.className = 'hidden';
                break;
            case EXP_TYPE_ASL:
            case EXP_TYPE_MSL:
                newStimulus = new Audio();
                break;
        }

        newStimulus.src = currStimulus.path;
        newStimulus.id = i;

        $('#' + panelID).append(newStimulus);
    }
}
function initExpParameters() {
    // Get parameters from the server.
    var getExpParamsData = {};
    getExpParamsData._pd = window.location.search.replace('?pd=','');
    sendAjaxRequest(EXP_URL_PATH, 'LoadExpParameters', getExpParamsData, loadExperimentParameters);
}
function startExperimentPhase(phase) {
    m_ExperimentPhase = phase;
    
    $('#div_startup').hide();
    $('#div_demographics').hide();
    $('#div_training_instructions').hide();
    $('#div_training_trials').hide();
    $('#div_testing_instructions').hide();
    $('#div_testing_trials').hide();
    $('#div_finish').hide();

    switch (phase) {
        case EXP_PHASE_STARTUP:
            $('#div_startup').show();
            break;

        case EXP_PHASE_DEMOGRAPHICS:
            createSession();

            $('#div_demographics').show();
            break;

        case EXP_PHASE_TRAINING_INSTRUCTIONS:
            //createSession();

            // preload the stimuli image files
            preloadStimuli('div_training_panel');

            $('#div_training_instructions').show();
            break;

        case EXP_PHASE_TRAINING_TRIALS:
            generateTriplets();
            generateTrainingTrials();
            displayNextTrainingStimulus();
            startAmbiance();

            $('#div_training_trials').show();
            break;

        case EXP_PHASE_TESTING_INSTRUCTIONS:
            // preload the stimuli image files
            preloadStimuli('div_testing_question_panel');

            $('#div_testing_instructions').show();
            break;

        case EXP_PHASE_TESTING_TRIALS:
            do {
                var ok = generateTestingFoils();
            } while (!ok);

            preloadTripletIndicators();
            generateTestingQuestions();
            m_CurrTestingQuestionElement = 0;
            displayNextQuestion();

            $('#div_testing_trials').show();
            break;

        case EXP_PHASE_FINISH:
            //$('#div_finish_spid_val').text(m_SessionID);
            endSession();

            $('#div_finish').show();
            break;
            
        default:
            break;
    }
}
function createSession() {
    var data = {};
    data._configID = p_ConfigID;
    sendAjaxRequest(EXP_URL_PATH, 'CreateSession', data, setNewSession);
}
function validateDemographics(controlToValidate) {
    var sex_ok = $('#drp_dem_sex').val().length > 0;
    var age_y_ok = $('#drp_dem_age_y').val().length > 0;
    var age_m_ok = $('#drp_dem_age_m').val().length > 0;
    var native_lang_ok = $('#drp_dem_native_lang').val().length > 0;
    var location_ok = $('#drp_dem_location').val().length > 0;

    var reqFieldMsg = 'זהו שדה חובה';

    if (controlToValidate == null || controlToValidate == 'drp_dem_sex')
        $('#lbl_dem_sex_err_msg').text(sex_ok ? ' ' : reqFieldMsg);
    if (controlToValidate == null || controlToValidate == 'drp_dem_age_y')
        $('#lbl_dem_age_y_err_msg').text(age_y_ok ? ' ' : reqFieldMsg);
    if (controlToValidate == null || controlToValidate == 'drp_dem_age_m')
        $('#lbl_dem_age_m_err_msg').text(age_m_ok ? ' ' : reqFieldMsg);
    if (controlToValidate == null || controlToValidate == 'drp_dem_native_lang')
        $('#lbl_dem_native_lang_err_msg').text(native_lang_ok ? ' ' : reqFieldMsg);
    if (controlToValidate == null || controlToValidate == 'drp_dem_location')
        $('#lbl_dem_location_err_msg').text(location_ok ? ' ' : reqFieldMsg);

    return sex_ok && age_y_ok && age_m_ok && native_lang_ok && location_ok;
}
function finishDemographics() {
    $('#cmd_demographics_ok').attr('disabled', 'disabled');

    var data = {};
    data._sessionID = m_SessionID;
    data._subjectID = $('#txt_dem_subject_id').val();
    data._sex = $('#drp_dem_sex').val();
    data._ageY = $('#drp_dem_age_y').val();
    data._ageM = $('#drp_dem_age_m').val();
    data._nativeLang = $('#drp_dem_native_lang').val();
    data._location = $('#drp_dem_location').val();
    data._remarks = $('#txt_dem_remarks').val();

    sendAjaxRequest(EXP_URL_PATH, 'SaveDemographics', data, null, saveDemographicsError)

    startExperimentPhase(EXP_PHASE_TRAINING_INSTRUCTIONS);
}
function generateTriplets() {
    // TODO : Implement case where triplets are preset and not random!

    var copyStimuli = [];

    var i;
    for (i in m_Stimuli)
        copyStimuli.push(i);

    m_Triplets = [];

    var randIndex;
    while (copyStimuli.length > 0) {
        var newTriplet = {};
        
        // set the 'A' element of the triplet
        randIndex = Math.floor(Math.random() * copyStimuli.length);
        newTriplet.a = copyStimuli[randIndex];
        copyStimuli.splice(randIndex, 1);

        // set the 'B' element of the triplet
        randIndex = Math.floor(Math.random() * copyStimuli.length);
        newTriplet.b = copyStimuli[randIndex];
        copyStimuli.splice(randIndex, 1);

        // set the 'C' element of the triplet
        randIndex = Math.floor(Math.random() * copyStimuli.length);
        newTriplet.c = copyStimuli[randIndex];
        copyStimuli.splice(randIndex, 1);

        var tripletID = TRIPLET_TYPE_TRIPLET + (m_Triplets.length + 1);
        newTriplet.tripletID = tripletID;
        newTriplet.type = TRIPLET_TYPE_TRIPLET;

        newTriplet.sessionID = m_SessionID;

        m_Triplets.push(newTriplet);
    }

    saveSessionTriplets(m_Triplets);
}
function saveSessionTriplets(triplets) {
    var data = {};
    data._triplets = triplets;
    sendAjaxRequest(EXP_URL_PATH, 'SaveSessionTriplets', data, null, saveSessionTripletsError);
}
function generateTrainingTrials() {
    m_TrainingSet = [];

    // generate a set of training items.
    var i, j;
    for (i = 0; i < p_TrainingNumOfTripletRepetitions; i++) {
        for (j = 0; j < m_Triplets.length; j++) {
            m_TrainingSet.push($.extend({}, m_Triplets[j])); // $.extend is used to create a copy and not a reference.
        }
    }

    // shuffle the training items.
    shuffle_triplets_no_immediate_repetitions(m_TrainingSet);

    // reset the current training item index.
    m_CurrTrainingItemIndex = -1;
}
function startAmbiance() {
    var media = null;

    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            media = new Audio();
            media.loop = true;
            media.src = '/mp3/background.mp3';
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            media = new Image();
            media.src = '/img/background.jpg';
            break;
    }

    media.id = 'training_ambiance';
    $('#div_training_panel').append(media);

    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            media.play();
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            break;
    }
}
function stopAmbiance() {
    var media = $('#training_ambiance')[0];

    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            media.pause();
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            break;
    }
}
function displayNextTrainingStimulus() {
    var nextStimulus;

    if (m_CurrTrainingItemElement == TRIPLET_ELEMENT_A) {
        m_CurrTrainingItemElement = TRIPLET_ELEMENT_B;
        nextStimulus = m_Stimuli[m_TrainingSet[m_CurrTrainingItemIndex].b];
    }
    else if (m_CurrTrainingItemElement == TRIPLET_ELEMENT_B) {
        m_CurrTrainingItemElement = TRIPLET_ELEMENT_C;
        nextStimulus = m_Stimuli[m_TrainingSet[m_CurrTrainingItemIndex].c];
    }
    else if (m_CurrTrainingItemElement == TRIPLET_ELEMENT_C) {
        m_CurrTrainingItemIndex++;

        if (m_CurrTrainingItemIndex >= m_TrainingSet.length) {
            saveTrainingTrialsToDB(m_TrainingTrials);
            stopAmbiance();
            startExperimentPhase(EXP_PHASE_TESTING_INSTRUCTIONS);
            return;
        }

        m_CurrTrainingItemElement = TRIPLET_ELEMENT_A;
        nextStimulus = m_Stimuli[m_TrainingSet[m_CurrTrainingItemIndex].a];
    }

    // display next training stimulus after due pause time.
    setTimeout(function() {
        displayTrainingStimulus(nextStimulus);
    }, p_PauseBetweenStimuli);
}
function displayTrainingStimulus(stimulus) {
    var stimulusDuration = null;

    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            $('#div_training_panel img').addClass('hidden');
            $('#div_training_panel img#' + stimulus.id).removeClass('hidden');
            stimulusDuration = p_StimulusDuration;
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            var audio = $('#div_training_panel audio#' + stimulus.id)[0];
            audio.currentTime = 0.0;
            audio.play();
            stimulusDuration = audio.duration * 1000;
            break;
    }

    // save current trial and display next training stimulus after the specified duration for training stimuli.
    setTimeout(function() {
        saveCurrTrainingTrial();
        displayNextTrainingStimulus();
    }, stimulusDuration);

    m_CurrTrainingStimulus = stimulus;
}
function saveCurrTrainingTrial() {
    var newTrial = {};

    newTrial.sessionID = m_SessionID;
    newTrial.stimulusID = m_CurrTrainingStimulus.id;
    newTrial.element = m_CurrTrainingItemElement;
    newTrial.tripletID = m_TrainingSet[m_CurrTrainingItemIndex].tripletID;
    newTrial.trialNumber = m_TrainingTrials.length + 1;

    m_TrainingTrials.push(newTrial);
}
function saveTrainingTrialsToDB(trials) {
    var data = {};
    data._trials = trials;
    sendAjaxRequest(EXP_URL_PATH, 'SaveSessionTrainingTrials', data, null, saveSessionTrainingTrialsError)
}
function generateTestingFoils() {
    // init members and variables
    m_TestingFoils = [];
    var aArr = [];
    var bArr = [];
    var cArr = [];
    var i, rai, rbi, rci;
    var ra, rb, rc;

    // init auxiliary arrays with triplets' a/b/c elements
    for (i in m_Triplets) {
        aArr.push(m_Triplets[i].a);
        bArr.push(m_Triplets[i].b);
        cArr.push(m_Triplets[i].c);
    }

    var iter;

    // copy all triplet values to the foils array, and shuffle two of their elements.
    for (i in m_Triplets) {
        // this is used to measure how many iterations of the do..while loop are performed.
        // if this value reaches MAX_RAND_ITERATIONS - it's a dead end and generateTestingFoils should be called again.
        iter = 0;

        // find a random triplet index, different than the current index, 'i'.
        do {
            rai = Math.floor(Math.random() * aArr.length);
            rbi = Math.floor(Math.random() * bArr.length);
            rci = Math.floor(Math.random() * cArr.length);

            ra = aArr[rai];
            rb = bArr[rbi];
            rc = cArr[rci];

            iter++;
        } while ((ra == null || rb == null || rc == null || rai == rbi || rai == rci || rbi == rci) && iter < MAX_RAND_ITERATIONS);

        if (iter >= MAX_RAND_ITERATIONS)
            return false;

        // create a foil from the random elements ra, rb, rc.
        var newFoil = {};
        newFoil.a = ra; //$.extend({}, ra);
        newFoil.b = rb; //$.extend({}, rb);
        newFoil.c = rc; //$.extend({}, rc);
        newFoil.tripletID = TRIPLET_TYPE_FOIL + (m_TestingFoils.length + 1);
        newFoil.type = TRIPLET_TYPE_FOIL;
        newFoil.sessionID = m_SessionID;

        // set the used random elements to null.
        aArr[rai] = null;
        bArr[rbi] = null;
        cArr[rci] = null;

        // add the new foil to the m_TestingFoils member.
        m_TestingFoils.push(newFoil);
    }

    saveSessionTriplets(m_TestingFoils);

    return true;
}
function generateTestingQuestions() {
    // init the questions array.
    m_TestingQuestions = [];

    var questionsBank = generateTestingQuestionsBank();

    for (var i = 0; i < p_NumOfTriplets; i++) {
        var chunk = generateTestingQuestionsChunk(questionsBank, i);
        $.merge(m_TestingQuestions, chunk);
    }
}
function generateTestingQuestionsChunk(questionsBank, m) {
    var chunk = [];

    var r, t;
    var c = 0;
    for (var i = 0; i < p_NumOfTriplets; i++) {
        do {
            r = Math.floor(Math.random() * questionsBank.length);
        } while (questionsBank[r].first.tripletID != m_Triplets[i].tripletID/* && questionsBank[r].second.tripletID != m_Triplets[i].tripletID*/);

        c++;
        t = (p_NumOfTriplets * m) + c;
        var selectedQuestion = questionsBank[r];

        if (t % 2 == 0) {
            var tmp = selectedQuestion.first;
            selectedQuestion.first = selectedQuestion.second;
            selectedQuestion.second = tmp;
        }

        chunk.push(selectedQuestion);
        questionsBank.splice(r, 1);
    }

    shuffle(chunk);

    return chunk;
}
function generateTestingQuestionsBank() {
    var retVal = [];

    // var c = 0;
    for (var i = 0; i < m_Triplets.length; i++) {
        for (var j = 0; j < m_TestingFoils.length; j++) {
            var newQuestion = {};

            newQuestion.first = $.extend({}, m_Triplets[i]);
            newQuestion.second = $.extend({}, m_TestingFoils[j]);

            // c++;
            // if (c % 2 == 0) {
            //     newQuestion.first = $.extend({}, m_Triplets[i]);
            //     newQuestion.second = $.extend({}, m_TestingFoils[j]);
            // }
            // else {
            //     newQuestion.first = $.extend({}, m_TestingFoils[j]);
            //     newQuestion.second = $.extend({}, m_Triplets[i]);
            // }

            // add the new question to the return value.
            retVal.push(newQuestion);
        }
    }

    return retVal;
}
function displayNextQuestion() {
    var nextQuestion;

    m_CurrTestingQuestionIndex++;

    if (m_CurrTestingQuestionIndex >= m_TestingQuestions.length) {
        saveSessionTestingTrials(m_TestingTrials);
        startExperimentPhase(EXP_PHASE_FINISH);
        return;
    }

    nextQuestion = m_TestingQuestions[m_CurrTestingQuestionIndex];

    displayQuestion(nextQuestion);
}
function displayQuestion(question) {
    // show question panel and ignore keys pressed.
    m_WaitingForAnswer = false;
    $('#div_testing_answer_panel').hide();
    $('#div_testing_question_panel').show();

    var sd1a, sd1b, sd1c, sd2a, sd2b, sd2c;

    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            sd1b = p_StimulusDuration;
            sd1a = p_StimulusDuration;
            sd1c = p_StimulusDuration;
            sd2a = p_StimulusDuration;
            sd2b = p_StimulusDuration;
            sd2c = p_StimulusDuration;
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            sd1a = $('#' + question.first.a)[0].duration * 1000;
            sd1b = $('#' + question.first.b)[0].duration * 1000;
            sd1c = $('#' + question.first.c)[0].duration * 1000;
            sd2a = $('#' + question.second.a)[0].duration * 1000;
            sd2b = $('#' + question.second.b)[0].duration * 1000;
            sd2c = $('#' + question.second.c)[0].duration * 1000;
            break;
    }

    // clear stimulus.
    clearTestingStimulus();

    // display first triplet 'A' stimulus.
    setTimeout(function() {
        displayTestingStimulus(m_Stimuli[question.first.a]);
        displayTripletIndicator(1);

        // clear stimulus.
        setTimeout(function() {
            clearTestingStimulus();

            // display first triplet 'B' stimulus.
            setTimeout(function() {
                displayTestingStimulus(m_Stimuli[question.first.b]);

                // clear stimulus.
                setTimeout(function() {
                    clearTestingStimulus();

                    // display first triplet 'C' stimulus.
                    setTimeout(function() {
                        displayTestingStimulus(m_Stimuli[question.first.c]);

                        // clear stimulus.
                        setTimeout(function() {
                            clearTestingStimulus();
                            clearTripletIndicator();

                            // display second triplet 'A' stimulus.
                            setTimeout(function() {
                                displayTestingStimulus(m_Stimuli[question.second.a]);
                                displayTripletIndicator(2);

                                // clear stimulus.
                                setTimeout(function() {
                                    clearTestingStimulus();

                                    // display second triplet 'B' stimulus.
                                    setTimeout(function() {
                                        displayTestingStimulus(m_Stimuli[question.second.b]);

                                        // clear stimulus.
                                        setTimeout(function() {
                                            clearTestingStimulus();

                                            // display second triplet 'C' stimulus.
                                            setTimeout(function() {
                                                displayTestingStimulus(m_Stimuli[question.second.c]);

                                                // clear stimulus.
                                                setTimeout(function() {
                                                    clearTestingStimulus();
                                                    clearTripletIndicator();

                                                    // show answer panel and listen for keys pressed.
                                                    setTimeout(function() {
                                                        clearTestingStimulus();
                                                        m_WaitingForAnswer = true;
                                                        $('#div_testing_question_panel').hide();
                                                        $('#div_testing_answer_panel').show();
                                                        m_CurrTestingQuestionStartTime = new Date().getTime();
                                                    }, PAUSE_BEFORE_AFTER_QUESTION);

                                                }, sd2c);

                                            }, p_PauseBetweenStimuli);

                                        }, sd2b);

                                    }, p_PauseBetweenStimuli);

                                }, sd2a);

                            }, p_TestingPauseBetweenTriplets);

                        }, sd1c);

                    }, p_PauseBetweenStimuli);

                }, sd1b);

            }, p_PauseBetweenStimuli);

        }, sd1a);

    }, PAUSE_BEFORE_AFTER_QUESTION);
}
function displayTestingStimulus(stimulus) {
    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            $('#div_testing_question_panel img').addClass('hidden');
            $('#div_testing_question_panel img#' + stimulus.id).removeClass('hidden');
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            $('#div_testing_question_panel audio#' + stimulus.id)[0].currentTime = 0.0;
            $('#div_testing_question_panel audio#' + stimulus.id)[0].play();
            break;
    }
}
function preloadTripletIndicators() {
    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            var mediaIndicator1 = new Image();
            mediaIndicator1.className = 'hidden';
            mediaIndicator1.src = '/img/one.png';
            mediaIndicator1.id = 'testing_triplet_indicator1';
            var mediaIndicator2 = new Image();
            mediaIndicator2.className = 'hidden';
            mediaIndicator2.src = '/img/two.png';
            mediaIndicator2.id = 'testing_triplet_indicator2';
            $('#div_testing_question_panel').append(mediaIndicator1);
            $('#div_testing_question_panel').append(mediaIndicator2);
            break;
    }
}
function displayTripletIndicator(tripletNumber) {
    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            $('#testing_triplet_indicator' + tripletNumber).removeClass('hidden');
            break;
    }
}
function clearTripletIndicator() {
    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            $('#testing_triplet_indicator1').addClass('hidden');
            $('#testing_triplet_indicator2').addClass('hidden');
            break;
    }
}
function clearTestingStimulus() {
    switch (p_ExpType) {
        case EXP_TYPE_VSL:
        case EXP_TYPE_DSL:
            $('#div_testing_question_panel img').addClass('hidden');
            break;
        case EXP_TYPE_ASL:
        case EXP_TYPE_MSL:
            // stop current audio?
            break;
    }
}
function saveSessionTestingTrial(answer, rt) {
    var newTrial = {};

    newTrial.sessionID = m_SessionID;
    newTrial.stimulus1A = m_TestingQuestions[m_CurrTestingQuestionIndex].first.a;
    newTrial.stimulus1B = m_TestingQuestions[m_CurrTestingQuestionIndex].first.b;
    newTrial.stimulus1C = m_TestingQuestions[m_CurrTestingQuestionIndex].first.c;
    newTrial.stimulus2A = m_TestingQuestions[m_CurrTestingQuestionIndex].second.a;
    newTrial.stimulus2B = m_TestingQuestions[m_CurrTestingQuestionIndex].second.b;
    newTrial.stimulus2C = m_TestingQuestions[m_CurrTestingQuestionIndex].second.c;
    newTrial.answer = answer;
    newTrial.rt = rt;
    newTrial.trialNumber = m_TestingTrials.length + 1;

    m_TestingTrials.push(newTrial);
}
function saveSessionTestingTrials(trials) {
    var data = {};
    data._trials = trials;
    sendAjaxRequest(EXP_URL_PATH, 'SaveSessionTestingTrials', data, null, saveSessionTestingTrialsError)
}
function endSession() {
    var data = {};
    data._sessionID = m_SessionID;
    sendAjaxRequest(EXP_URL_PATH, 'EndSession', data);
}