import React, { useContext } from "react";
import { useQuery } from "@apollo/client";
import { gql } from "@apollo/client";
import { IUser } from "../api/types";
import { Fragments } from "../api/fragments";

export const UserContext = React.createContext(null as (IUser | null));

export default ({ children }: any) => {
  let { data } = useQuery(GET_USER);

  return <UserContext.Provider value={(data?.currentUser ?? null) as IUser | null}>
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
      projects {
        ${Fragments.projectDetail}
      }
      participates {
        id
        subscribedToProjectEmails
        unsubscribeToken
        project {
          ${Fragments.projectDetail}
        }
      }
    }
  }
`;

