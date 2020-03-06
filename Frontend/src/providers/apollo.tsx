import React from 'react';
import { ApolloProvider } from '@apollo/react-hooks';
import ApolloClient from 'apollo-boost';

export default ({ children } : any) => {
  const client = new ApolloClient({
    uri: `https://test.collaction.org/graphql`,
  });

  return <ApolloProvider client={client}>{children}</ApolloProvider>;
};
