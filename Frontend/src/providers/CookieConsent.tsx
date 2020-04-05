import React, { useEffect, useState, useContext } from "react";

export type Consent = "basics" | "analytics" | "stripe" | "disqus" | "social";

const consentKey = "collaction-gdpr-consent";

const initialConsentState = window.localStorage.getItem(consentKey)?.split(";")?.filter(c => c.length !== 0)?.map(c => c as Consent) ?? [];

export const ConsentContext = React.createContext({ consent: initialConsentState, setConsent: (_: Consent[]) => { }, setAllowAllConsent: () => {} });

export const AllowedConsents: Consent[] = [ "basics", "analytics", "stripe", "disqus", "social" ];

export const ConsentDescription = (consent: Consent) => {
    switch (consent) {
        case 'basics':
            return "Essential cookies & anonymous statistics";
        case 'analytics':
            return "Detailed analytics";
        case 'stripe':
            return "Integration with stripe payments for handling our donations";
        case 'disqus':
            return "Integration with disqus comments for handling our project comments";
        case 'social':
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