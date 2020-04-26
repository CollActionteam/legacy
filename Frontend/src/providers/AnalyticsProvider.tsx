import { gql, useMutation } from "@apollo/client";
import React, { useState, useEffect, useContext } from "react";
import { useConsent, Consent } from "./CookieConsentProvider";
import { useSettings } from "./SettingsProvider";
import ReactGA from 'react-ga';
import ReactPixel from 'react-facebook-pixel';

interface UtmTags {
    source: string | null;
    medium: string | null;
    campaign: string | null;
    term: string | null;
    content: string | null;
}

const SEND_EVENT = gql`
  mutation SendUserEvent($eventData: String!, $canTrack: Boolean!) {
    user {
      eventId: ingestUserEvent(eventData: $eventData, canTrack: $canTrack)
    }
  }
`;

// Get cookie by name
const getCookie = (name: string) => {
    var v = document.cookie.match(`(^|;) ?'${name}'=([^;]*)(;|$)`);
    return v ? v[2] : null;
};

// Get local time as ISO string with tz offset at the end.
const getIsoTime = () => {
    const pad = (num: number) => {
        const norm = Math.abs(Math.floor(num));
        return (norm < 10 ? '0' : '') + norm;
    };
    const now = new Date();
    const tzo = -now.getTimezoneOffset();
    const dif = tzo >= 0 ? '+' : '-';
    return now.getFullYear() 
      + '-' + pad(now.getMonth()+1)
      + '-' + pad(now.getDate())
      + 'T' + pad(now.getHours())
      + ':' + pad(now.getMinutes()) 
      + ':' + pad(now.getSeconds())
      + '.' + pad(now.getMilliseconds())
      + dif + pad(tzo / 60) 
      + ':' + pad(tzo % 60);
};

const parseUtm = (): UtmTags | null => {
    const searchParams = new URLSearchParams(window.location.search);
    const utm: UtmTags = {
        source: searchParams.get('utm_source'),
        medium: searchParams.get('utm_medium'),
        campaign: searchParams.get('utm_campaign'),
        term: searchParams.get('utm_term'),
        content: searchParams.get('utm_content')
    };
    if (utm.source === null && utm.medium === null && utm.campaign === null && utm.term === null && utm.content === null) { 
        return null;
    } else {
        return utm;
    }
};

const getEventPayload = (eventCategory: string, eventAction: string, eventLabel: string, eventValue: string | null, referrer: string, hitnumber: number, utm: UtmTags, has_analytics_consent: boolean) => {
    const payload = {
        'page_path'         : window.location.pathname,
        'page_url'          : window.location.href,
        'page_hostname'     : window.location.hostname,
        'referrer'          : document.referrer,
        'event_category'    : eventCategory,
        'event_action'      : eventAction,
        'event_label'       : eventLabel,
        'event_value'       : eventValue,
        'user_agent'        : has_analytics_consent ? navigator.userAgent : null,
        'time'              : getIsoTime(),
        'session_referrer'  : referrer,
        'client_id'         : googleAnalyticsClientId,
        'utm_source'        : utm.source,
        'utm_medium'        : utm.medium,
        'utm_campaign'      : utm.campaign,
        'utm_term'          : utm.term,
        'utm_content'       : utm.content,
        'session_hitnumber' : hitnumber
    };
    return JSON.stringify(payload);
}

const initialUtmStorage = sessionStorage.getItem('utm_tags');
const initialUtm: UtmTags = initialUtmStorage ? JSON.parse(initialUtmStorage) : { source: null, medium: null, campaign: null, term: null, content: null };
const initialHitnumber = parseInt(sessionStorage.getItem('hitnumber') ?? "0");
const initialReferrer = sessionStorage.getItem('referrer') ?? "";
const googleAnalyticsClientId = getCookie('_ga');
const defaultAnalytics = {
    utm: { source: null, medium: null, campaign: null, term: null, content: null} as UtmTags,
    sendUserEvent: async (_needsConsent: boolean, _eventCategory: string, _eventAction: string, _eventLabel: string, _eventValue: string | null) => { },
    checkAndUpdateAnalyticsState: () => { },
    externalAnalyticsInitialized: false
};

export const AnalyticsContext = React.createContext(defaultAnalytics);

export const useAnalytics = () => useContext(AnalyticsContext);

export default ({ children }: any) => {
    const { consent } = useConsent();
    const [ bufferedEvents, setBufferedEvents ] = useState<string[]>([]);
    const [ externalAnalyticsInitialized, setExternalAnalyticsInitialized ] = useState(false);
    const { googleAnalyticsID, facebookPixelID } = useSettings();
    const pixelConsent = consent.includes(Consent.Analytics) && consent.includes(Consent.Social);

    const [ utm, setUtm ] = useState(initialUtm);
    useEffect(() => {
      sessionStorage.setItem('utm_tags', JSON.stringify(utm));
    }, [ utm ]);

    const [ hitnumber, setHitnumber ] = useState(initialHitnumber);
    useEffect(() => {
      sessionStorage.setItem('hitnumber', hitnumber.toString());
    }, [ hitnumber ]);

    const [ referrer, setReferrer ] = useState(initialReferrer);
    useEffect(() => {
        sessionStorage.setItem('referrer', referrer);
    }, [ referrer ]);

    useEffect(() => {
        if (googleAnalyticsID && facebookPixelID && !externalAnalyticsInitialized) {
            ReactGA.initialize(googleAnalyticsID);
            // Only load pixel when the user originates from facebook/instagram
            if ((utm.source === "facebook" || utm.source === "instagram") && pixelConsent) {
                ReactPixel.init(facebookPixelID);
            }
            setExternalAnalyticsInitialized(true);
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [ googleAnalyticsID, facebookPixelID, pixelConsent ]);

    const checkAndUpdateUtm = () => {
        const checked = parseUtm();
        if (checked !== null && (checked.source !== utm.source || checked.campaign !== utm.campaign || checked.content !== utm.content || checked.medium !== utm.medium || checked.source !== utm.source || checked.term !== utm.term)) {
            setUtm(checked);
        }
    };

    const checkAndUpdateReferrer = () => {
        const referrer = document.referrer;
        if (referrer && referrer !== document.location.hostname) {
            setReferrer(referrer);
        }
    };

    const checkAndUpdateAnalyticsState = () => {
        checkAndUpdateReferrer();
        checkAndUpdateUtm();
    }

    const [ sendEvent ] = useMutation(SEND_EVENT);

    const bufferEvent = (ev: string) => {
        setBufferedEvents(bufferedEvents.concat(ev));
    }

    const flushAnalytics = async () => {
        if (bufferedEvents.length > 0) {
            await Promise.all(
                bufferedEvents.map(ev => 
                    sendEvent({ 
                        variables: { 
                            eventData: ev, 
                            canTrack: consent.includes(Consent.Analytics) 
                        }
                    })
                )
            );
            setBufferedEvents([]);
        }
    }

    const sendUserEvent = async (needsConsent: boolean, eventCategory: string, eventAction: string, eventLabel: string, eventValue: string | null) => {
        await flushAnalytics();
        const payload = getEventPayload(eventCategory, eventAction, eventLabel, eventValue, referrer, hitnumber, utm, consent.includes(Consent.Analytics));
        setHitnumber(hitnumber + 1);
        if (!needsConsent || consent.includes(Consent.Analytics)) {
            await sendEvent({
                variables: {
                    eventData: payload,
                    canTrack: consent.includes(Consent.Analytics)
                }
            });
        } else {
            bufferEvent(payload);
        }
    }

    const contextValue = { utm, sendUserEvent, checkAndUpdateAnalyticsState, externalAnalyticsInitialized };

    return <AnalyticsContext.Provider value={contextValue}>
        { children }
    </AnalyticsContext.Provider>;
};