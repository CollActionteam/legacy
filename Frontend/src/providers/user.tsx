import React, { useState } from "react";
import { useQuery } from "@apollo/client";
import { gql } from "@apollo/client";
import { IUser } from "../api/types";

export const UserContext = React.createContext({ user: null as (IUser | null), setUser: (_user: IUser | null) => { } });

export default ({ children }: any) => {
  let { data, error } = useQuery(GET_USER);
  const [manualUser, setManualUser] = useState<IUser | null>(null);

  if (error) {
    console.error(error);
  }

  let contextValue = {
    user: (manualUser ?? data?.currentUser ?? null) as IUser | null,
    setUser: setManualUser
  };

  return <UserContext.Provider value={contextValue}>
    {children}
  </UserContext.Provider>;
};

const GET_USER = gql`
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
      projects {
        id
        name
      }
      participates {
        subscribedToProjectEmails
        unsubscribeToken
        project {
          id
          name
        }
      }
    }
  }
`;

