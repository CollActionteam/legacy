import React, { useContext, useEffect } from "react";
import { useQuery } from "@apollo/client";
import { gql } from "@apollo/client";
import { ISettings } from "../api/types";

export const GET_SETTINGS = gql`
    query GetSettings {
        misc {  
            mailChimpNewsletterListId
            mailChimpUserId
            mailChimpAccount
            mailChimpServer
            disqusSiteId
            stripePublicKey
            externalLoginProviders
            googleAnalyticsID
            facebookPixelID
        }
        categories: __type(name: "Category") {
            enumValues {
                name
            }
        }
        displayPriorities: __type(name: "CrowdactionDisplayPriority") {
            enumValues {
                name
            }
        }
        crowdactionStatusses: __type(name: "CrowdactionStatus") {
            enumValues {
                name
            }
        }
    }
`; 

const defaultSettings: ISettings = {
    mailChimpNewsletterListId: "",
    mailChimpUserId: "",
    mailChimpAccount: "",
    mailChimpServer: "",
    stripePublicKey: "",
    disqusSiteId: "",
    googleAnalyticsID: "",
    facebookPixelID: "",
    externalLoginProviders: [],
    categories: [],
    displayPriorities: [],
    crowdactionStatusses: []
};

export const SettingsContext = React.createContext(defaultSettings);

const mapSettings = (settingsData: any): ISettings => {
    if (!settingsData) {
        return defaultSettings;
    }

    const misc = settingsData.misc;
    return {
        mailChimpNewsletterListId: misc.mailChimpNewsletterListId,
        mailChimpAccount: misc.mailChimpAccount,
        mailChimpServer: misc.mailChimpServer,
        mailChimpUserId: misc.mailChimpUserId,
        stripePublicKey: misc.stripePublicKey,
        disqusSiteId: misc.disqusSiteId,
        externalLoginProviders: misc.externalLoginProviders,
        googleAnalyticsID: misc.googleAnalyticsID,
        facebookPixelID: misc.facebookPixelID,
        categories: settingsData.categories.enumValues.map((v: any) => v.name),
        displayPriorities: settingsData.displayPriorities.enumValues.map((v: any) => v.name),
        crowdactionStatusses: settingsData.crowdactionStatusses.enumValues.map((v: any) => v.name)
    };
};

export default ({ children }: any) => {
    let { data, error } = useQuery(GET_SETTINGS);

    useEffect(() => {
        if (error) {
            console.error(error.message);
        }
    }, [error]);

    return <SettingsContext.Provider value={mapSettings(data)}>
        { children }
    </SettingsContext.Provider>;
};

export const useSettings = () => useContext(SettingsContext);