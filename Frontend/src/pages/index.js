import React from "react";
import Layout from "../components/Layout";
// import { Query } from 'react-apollo';
// import gql from 'graphql-tag';
import { graphql } from "gatsby";

// This query is executed at build time by Gatsby.
export const GatsbyQuery = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
  }
`;

// This query is executed at run time by Apollo.
// const APOLLO_QUERY = gql`
//   query {
//     projects {
//       id,
//       name,
//       url
//     }
//   }
// `;

export default ({ data }) => (
  <Layout>
    <h1>Homepage</h1>
    <p>This will be the homepage.</p>

    <h2>{ data.site.siteMetadata.title }</h2>

    {/* This won't work when building the site (npm run build). 
    It will try to connect to the endpoint to create static html files from the projects 
    which is not the behavior we want. We need a client-side react component that does that. */}
    {/* <Query query={APOLLO_QUERY}>
      {({ data, loading, error }) => {
        if (loading) return <p>Loading...</p>;
        if (error) return <p>Error: ${error.message}</p>;
        return <div>{data.projects}</div>;
      }}
    </Query> */}

  </Layout>
);
