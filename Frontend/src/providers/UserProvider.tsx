import React, { useContext, useEffect } from "react";
import { useQuery } from "@apollo/client";
import { gql } from "@apollo/client";
import { IUser } from "../api/types";
import { Fragments } from "../api/fragments";

export const UserContext = React.createContext(null as (IUser | null | undefined));

const UserProvider = ({ children }: any) => {
    let { data, error } = useQuery(GET_USER);

    useEffect(() => {
        if (error) {
            console.error(error);
        }
    }, [error]);

    return <UserContext.Provider value={(data?.currentUser) as IUser | null | undefined}>
        { children }
    </UserContext.Provider>;
};

export const useUser = () => useContext(UserContext);

export const GET_USER = gql`
    query GetUser {
        currentUser {  
            id
            firstName
            lastName
            fullName
            email
            isAdmin
            isSubscribedNewsletter
            representsNumberParticipants
            donationSubscriptions {
                id
                startDate
                canceledAt
            }
            crowdactions {
                ${Fragments.crowdactionDetail}
            }
            participates {
                id
                subscribedToCrowdactionEmails
                unsubscribeToken
                crowdaction {
                    ${Fragments.crowdactionDetail}
                }
            }
        }
    }
`;

export default UserProvider;