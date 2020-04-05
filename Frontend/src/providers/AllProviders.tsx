import React from "react";
import Apollo from "./Apollo";
import CookieConsent from "./CookieConsent";
import User from "./User";
import { I18nextProvider } from "react-i18next";
import i18n from "../i18n";

export default ({ children }: any) => {
    return <I18nextProvider i18n={i18n}>
        <Apollo>
            <CookieConsent>
                <User>
                    { children }
                </User>
            </CookieConsent>
        </Apollo>
    </I18nextProvider>;
};