import React from "react";
import { useQuery } from "@apollo/react-hooks";
import { gql } from "apollo-boost";
import Loader from "../Loader";
import { IUser } from "../../api/types";

export const UserContext = React.createContext<IUser | undefined>(undefined);

export default ({ children }: any) => {
  const { data, loading, error } = useQuery<IUser>(GET_USER);

  if (error) {
    console.error(error);
  }

  return <React.Fragment>
    <UserContext.Provider value={data}>
      {loading && <Loader />}
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

