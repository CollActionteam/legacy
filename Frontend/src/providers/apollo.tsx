import React from 'react';
import { ApolloProvider } from '@apollo/react-hooks';
import ApolloClient from 'apollo-boost';

export default ({ children } : any) => {
  const client = new ApolloClient({
    uri: process.env.REACT_APP_BACKEND_URL,
    credentials: 'include'
  });

  return <ApolloProvider client={client}>{children}</ApolloProvider>;
};
