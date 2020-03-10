import React from "react";
import { useQuery } from "@apollo/react-hooks";
import { gql } from "apollo-boost";
import { IUser } from "../../api/types";

export const UserContext = React.createContext({ user: undefined as (IUser | undefined), setUser: (_user: IUser) => { } });

export default ({ children }: any) => {
  let { data, error } = useQuery(GET_USER);

  if (error) {
    console.error(error);
  }

  let contextValue = {
    user: data?.currentUser as IUser | undefined,
    setUser: (newUser: IUser) => { contextValue.user = newUser; }
  };

  return <React.Fragment>
    <UserContext.Provider value={contextValue}>
      {children}
    </UserContext.Provider>
  </React.Fragment>
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

