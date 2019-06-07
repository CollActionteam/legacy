function parseUtmParameters() : string {
    const utmKeys = ["utm_source", "utm_medium", "utm_campaign", "utm_term", "utm_content"];
    const urlParams = new URLSearchParams(window.location.search);
    let result = {};
    urlParams.forEach((value: string, key: string) => {
        if (utmKeys.indexOf(key) !== -1) {
            result[key] = value;
        }
    });
    return JSON.stringify(result);
}

function storeSessionDetails() : void {
    // Check if browser supports Web Storage API.
    if (window.sessionStorage != null) {
        // Check if it"s the start of a new session -- after 30 min (1.8e6 ms) of inactivity.
        let previousTime = parseInt(sessionStorage.getItem("latest_interaction_time"));
        let currentTime = new Date().getTime();
        sessionStorage.setItem("latest_interaction_time", currentTime.toString());

        if (isNaN(previousTime) || (currentTime - previousTime) > 30 * 60 * 1000) {
            // We only read/track this data if people accept cookies, otherwise it'll be auto-removed after the session ends
            sessionStorage.setItem("session_referrer", document.referrer);
            sessionStorage.setItem("utm_tags", parseUtmParameters());
        }
    }
} 

storeSessionDetails();