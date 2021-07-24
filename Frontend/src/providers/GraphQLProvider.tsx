import React from 'react';
import { ApolloProvider, ApolloClient, InMemoryCache } from '@apollo/client';

const GraphQLProvider = ({ children } : any) => {
    const client = new ApolloClient({
        uri: `${process.env.REACT_APP_BACKEND_URL}/graphql`,
        credentials: 'include',
        cache: new InMemoryCache()
    });

    return <ApolloProvider client={client}>{children}</ApolloProvider>;
}; 

export default GraphQLProvider;