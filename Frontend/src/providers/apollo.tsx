import React from "react";
import fetch from "unfetch";
import { ApolloProvider as Provider } from "react-apollo";
import { InMemoryCache } from "apollo-cache-inmemory";
import { ApolloClient } from "apollo-client";
import { ApolloLink } from "apollo-link";
import { setContext } from "apollo-link-context";
import { onError } from "apollo-link-error";
import { createUploadLink } from "apollo-upload-client";
import { graphql, StaticQuery } from "gatsby";

export default ({ children }) => {
  return <StaticQuery
    query={graphql`
      query {
        site {
          siteMetadata {
            backendUrl
          }
        }
      }
    `}
    render={data => {
      const httpLink = createUploadLink({
        fetch,
        uri: data.site.siteMetadata.backendUrl + "/graphql",
        credentials: "include",
      });

      const errorLink = onError(({ networkError, graphQLErrors }) => {
        if (graphQLErrors) {
          graphQLErrors.map(({ message, locations, path }) =>
            console.log(
              `[GraphQL error]: Message: ${message}, Location: ${locations}, Path: ${path}`
            )
          );
        }
        if (networkError) {
          console.log(`[Network err]: Message:`, networkError);
        }
      });

      const authLink = setContext((_, { headers }) => ({
        headers: {
          ...headers,
        },
      }));

      const link = ApolloLink.from([errorLink, authLink.concat(httpLink)]);

      const client = new ApolloClient({
        cache: new InMemoryCache(),
        link,
      });

      return <Provider client={client}>{children}</Provider>;
    }}
  />;
} 