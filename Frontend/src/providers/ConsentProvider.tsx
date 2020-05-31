import React, { useEffect, useState, useContext } from "react";

export enum Consent { 
    Basics = "Basics",
    Analytics = "Analytics",
    Stripe = "Stripe",
    Social = "Social"
};

const consentKey = 'collaction-gdpr-consent';

const initialConsentState = window.localStorage.getItem(consentKey)?.split(";")?.filter(c => c.length !== 0)?.map(c => c as Consent) ?? [];

export const ConsentContext = React.createContext({ consent: initialConsentState, setConsent: (_: Consent[]) => { }, setAllowAllConsent: () => {} });

export const AllowedConsents: Consent[] = Object.keys(Consent).map(c => c as Consent);

export const ConsentDescription = (consent: Consent) => {
    switch (consent) {
        case Consent.Basics:
            return "Essential cookies & anonymous analytics";
        case Consent.Analytics:
            return "Detailed personalized analytics";
        case Consent.Stripe:
            return "Integration with stripe payments for handling our donations";
        case Consent.Social:
            return "Integration with social media (facebook, twitter, ...)";
    }
}

export const useConsent = () => useContext(ConsentContext);

export default ({ children }: any) => {
    const [ consent, setConsent ] = useState(initialConsentState);
    useEffect(() => {
        localStorage.setItem(consentKey, consent.join(";"));
    }, [ consent ]);
    const setAllowAllConsent = () => setConsent(AllowedConsents);
    const contextValue = {
        consent: consent,
        setConsent: setConsent,
        setAllowAllConsent: setAllowAllConsent
    };

    return <ConsentContext.Provider value={contextValue}>
        { children }
    </ConsentContext.Provider>
};