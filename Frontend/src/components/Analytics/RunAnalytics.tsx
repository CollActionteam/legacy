import ReactGA from 'react-ga';
import ReactPixel from 'react-facebook-pixel';
import { useConsent, Consent } from '../../providers/CookieConsentProvider';
import { useLocation } from 'react-router-dom';
import { useAnalytics } from '../../providers/AnalyticsProvider';
import { useEffect } from 'react';

export default () => {
    const { consent } = useConsent();
    const { pathname, search } = useLocation();
    const pixelConsent = consent.includes(Consent.Analytics) && consent.includes(Consent.Social);
    const { sendUserEvent, utm, checkAndUpdateAnalyticsState, externalAnalyticsInitialized } = useAnalytics();

    useEffect(() => {
        checkAndUpdateAnalyticsState();
        sendUserEvent(false, 'pageview', 'navigate', pathname, null);
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [ pathname, search ]);

    useEffect(() => {
        if (externalAnalyticsInitialized) {
            ReactGA.pageview(pathname + search);
        }
    }, [pathname, search, externalAnalyticsInitialized ]);

    useEffect(() => {
        // Only load pixel when the user originates from facebook/instagram
        if (externalAnalyticsInitialized && (utm.source === "facebook" || utm.source === "instagram") && pixelConsent) { 
            ReactPixel.track('PageView', null);
            if (pathname.startsWith("/projects") && pathname.endsWith("/thankyou")) {
                ReactPixel.track('SubmitApplication', null);
            }
            if (pathname.startsWith("/donate/thankyou")) {
                ReactPixel.track('Donate', null);
            }
        }
    }, [ pathname, search, pixelConsent, externalAnalyticsInitialized, utm ]);

    return null;
};